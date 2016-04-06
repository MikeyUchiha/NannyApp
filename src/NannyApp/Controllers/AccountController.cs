using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using NannyApp.Models;
using NannyApp.Services;
using NannyApp.ViewModels.Account;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.AspNet.Hosting;
using NannyApp.Models.Interfaces;

namespace NannyApp.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private static IApplicationEnvironment _hostingEnvironment;
        private static IHostingEnvironment _environment;

        public static string EConfUser { get; set; }
        public static string connection = null;
        public static string command = null;
        public static string parameterName = null;
        public static string methodName = null;
        string codeType = null;
        private INannyAppRepository _repository;

        public static string OEmail { get; set; }
        public static string OBirthday { get; set; }
        public static string OFname { get; set; }
        public static string OLname { get; set; }

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IApplicationEnvironment hostingEnvironment,
            IHostingEnvironment environment,
            INannyAppRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _hostingEnvironment = hostingEnvironment;
            _environment = environment;
            _repository = repository;
            connection = GetConnectionString("DefaultConnection");
        }

        #region Login
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await _userManager.FindByNameAsync(model.LoginUsername);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        EConfUser = model.LoginUsername;
                        return RedirectToAction(nameof(AccountController.EmailConfirmationFailed), "Account");
                    }
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.LoginUsername, model.LoginPassword, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    UpdateLastLoginDate(model.LoginUsername);
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion Login

        #region Register
        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var custEmail = FindEmail(model.RegisterEmail);
                var custUserName = FindUserName(model.RegisterUsername);
                var user = new ApplicationUser
                {
                    UserName = model.RegisterUsername,
                    Email = model.RegisterEmail,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Country = model.Country,
                    BirthDate = model.BirthDate,
                    JoinDate = DateTime.Now,
                    EmailLinkDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                };
                if (custEmail == null && custUserName == null)
                {
                    var result = await _userManager.CreateAsync(user, model.RegisterPassword);
                    if (result.Succeeded)
                    {
                        //For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        //Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, date = DateTime.Now, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.RegisterEmail, "Confirm your account",
                        //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                        //await _signInManager.SignInAsync(user, isPersistent: false);

                        // Send an email with this link
                        codeType = "EmailConfirmation";
                        await SendEmail("ConfirmEmail", "Account", user, model.RegisterEmail, "WelcomeEmail", "Confirm your account");

                        _logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToAction(nameof(AccountController.ConfirmationEmailSent), "Account");
                    }
                    AddErrors(result);
                }
                else
                {
                    if(custEmail != null)
                    {
                        ModelState.AddModelError("", "Email is already registered.");
                    }
                    if(custUserName != null)
                    {
                        ModelState.AddModelError("", "Username " + model.RegisterUsername + " is already taken.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion Register

        #region SendEmail

        public async Task SendEmail(string actionName, string controllerName, ApplicationUser user, string email, string emailTemplate, string emailSubject)
        {
            string code = null;
            if (codeType == "EmailConfirmation")
            {
                code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }
            else if (codeType == "ResetPassword")
            {
                code = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            var callbackUrl = Url.Action(actionName, controllerName, new { userId = user.Id, date = DateTime.Now, code = code }, protocol: HttpContext.Request.Scheme);
            var message = await EMailTemplate(emailTemplate);
            message = message.Replace("@ViewBag.Name", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName));
            message = message.Replace("@ViewBag.Link", callbackUrl);

            await _emailSender.SendEmailAsync(email, emailSubject, message);
        }

        #endregion SendEmail

        #region ConfirmEmail
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, DateTime date, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(AccountController.ConfirmationLinkExpired), "Account");
            }
            var emailConf = EmailConfirmationById(userId);
            if (emailConf == true)
            {
                return RedirectToAction(nameof(AccountController.ConfirmationLinkUsed), "Account");
            }
            if (date != null)
            {
                if (date.AddHours(1) < DateTime.Now)
                {
                    return RedirectToAction(nameof(AccountController.ConfirmationLinkExpired), "Account");
                }
                else
                {
                    var result = await _userManager.ConfirmEmailAsync(user, code);
                    return View(result.Succeeded ? "ConfirmEmail" : "Error");
                }
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendConfirmationMail()
        {
            string res = null;
            string connection = GetConnectionString("DefaultConnection");
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand("SELECT Email AS Email FROM dbo.AspNetUsers WHERE UserName = @UserName", myConnection))
            {
                cmd.Parameters.AddWithValue("@UserName", EConfUser);
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        if (reader.Read())
                        {
                            // To avoid unexpected bugs access columns by name.
                            res = reader["Email"].ToString();
                            var user = await _userManager.FindByEmailAsync(res);
                            UpdateEmailLinkDate(EConfUser);
                            codeType = "EmailConfirmation";
                            await SendEmail("ConfirmEmail", "Account", user, res, "WelcomeEmail", "Confirm your account");
                        }
                        myConnection.Close();
                    }
                }
            }
            return RedirectToAction(nameof(AccountController.ConfirmationEmailSent), "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailConfirmationFailed()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmationEmailSent()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmationLinkExpired()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmationLinkUsed()
        {
            return View();
        }
        #endregion ConfirmEmail

        #region LogOff
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion LogOff

        #region ExternalLogin
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            string userid = null;
            bool custEmailConf = false;
            string custUserName = null;

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            string userprokey = info.ProviderKey;
            userid = FindUserId(userprokey);
            if(userid != null)
            {
                custEmailConf = EmailConfirmationById(userid);
                custUserName = FindUserNameById(userid);
            }

            if (custEmailConf == false && custUserName != null)
            {
                EConfUser = custUserName;
                return RedirectToAction(nameof(AccountController.EmailConfirmationFailed), "Account");
            }
            else
            {
                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData["LoginProvider"] = info.LoginProvider;

                    if(info.LoginProvider == "Facebook")
                    {
                        OEmail = info.ExternalPrincipal.FindFirst(c => c.Type == "urn:facebook:email").Value;
                        OBirthday = info.ExternalPrincipal.FindFirst(c => c.Type == "urn:facebook:birthday").Value;
                        OFname = info.ExternalPrincipal.FindFirst(c => c.Type == "urn:facebook:first_name").Value;
                        OLname = info.ExternalPrincipal.FindFirst(c => c.Type == "urn:facebook:last_name").Value;
                    }
                    else if(info.LoginProvider == "Google")
                    {
                        OEmail = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                        OFname = info.ExternalPrincipal.FindFirstValue(ClaimTypes.GivenName);
                        OLname = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Surname);
                    }
                    else
                    {
                        OEmail = null;
                        OBirthday = null;
                        OFname = null;
                        OLname = null;
                    }

                    return View("ExternalLoginConfirmation");
                }
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var custEmail = FindEmail(model.Email);
                var custUserName = FindUserName(model.ExtUsername);
                var user = new ApplicationUser
                {
                    UserName = model.ExtUsername,
                    Email = model.Email,
                    FirstName = model.ExtFirstName,
                    LastName = model.ExtLastName,
                    Country = model.ExtCountry,
                    BirthDate = model.ExtBirthDate,
                    JoinDate = DateTime.Now,
                    EmailLinkDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                };

                if (custEmail == null && custUserName == null)
                {
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            codeType = "EmailConfirmation";
                            await SendEmail("ConfirmEmail", "Account", user, model.Email, "WelcomeEmail", "Confirm your account");

                            _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                            return RedirectToAction(nameof(AccountController.ConfirmationEmailSent), "Account");
                        }
                    }
                    AddErrors(result);
                }
                else
                {
                    if (custEmail != null)
                    {
                        ModelState.AddModelError("", "Email is already registered.");
                    }
                    if (custUserName != null)
                    {
                        ModelState.AddModelError("", "Username " + model.ExtUsername + " is already taken.");
                    }
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("ExternalLoginConfirmation");
        }
        #endregion ExternalLogin

        #region ForgotPassword
        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion ForgotPassword

        #region ResetPassword
        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion ResetPassword

        #region SendCode
        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }
        #endregion SendCode

        #region VerifyCode
        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }
        }
        #endregion VerifyCode

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public async Task<string> EMailTemplate(string template)
        {
            var templateFilePath = Path.Combine(_environment.WebRootPath, @"Templates\\" + template + ".cshtml");
            StreamReader objstreamreaderfile = new StreamReader(templateFilePath);
            var body = await objstreamreaderfile.ReadToEndAsync();
            objstreamreaderfile.Close();
            return body;
        }

        public string GetConnectionString(string connection)
        {
            var configurationPath = Path.Combine(_hostingEnvironment.ApplicationBasePath, "appsettings.json");
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(configurationPath)
                .AddEnvironmentVariables();

            var configuration = configBuilder.Build();
            string conOut = configuration["Data:"+ connection + ":ConnectionString"];
            return conOut;
        }

        public static string ReturnString(string str)
        {
            string strOut = null;
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(command, myConnection))
            {
                cmd.Parameters.AddWithValue(parameterName, str);
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            if (methodName == "FindEmail")
                            {
                                strOut = reader["Email"].ToString();
                            }
                            else if (methodName == "FindUserName" || methodName == "FindUserNameById")
                            {
                                strOut = reader["UserName"].ToString();
                            }
                            else if (methodName == "FindUserId")
                            {
                                strOut = reader["UserId"].ToString();
                            }

                        }
                        myConnection.Close();
                    }
                    return strOut;
                }
            }
        }
        public static string FindEmail(string email)
        {
            command = "SELECT Email AS Email FROM AspNetUsers WHERE Email = @Email";
            parameterName = "@Email";
            methodName = "FindEmail";
            return ReturnString(email);
        }
        public string FindUserName(string username)
        {
            command = "SELECT UserName AS UserName FROM AspNetUsers WHERE UserName = @UserName";
            parameterName = "@UserName";
            methodName = "FindUserName";
            return ReturnString(username);
        }
        public string FindUserNameById(string userid)
        {
            command = "SELECT UserName AS UserName FROM AspNetUsers WHERE Id = @Id";
            parameterName = "@Id";
            methodName = "FindUserNameById";
            return ReturnString(userid);
        }
        public string FindUserId(string userprokey)
        {
            command = "SELECT UserId AS UserId FROM AspNetUserLogins WHERE ProviderKey = @ProviderKey";
            parameterName = "@ProviderKey";
            methodName = "FindUserId";
            return ReturnString(userprokey);
        }

        public bool ReturnBool(string str)
        {
            bool econfOut = false;
            string res = null;
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(command, myConnection))
            {
                cmd.Parameters.AddWithValue(parameterName, str);
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            res = reader["EmailConfirmed"].ToString();
                            if (res == "False")
                            {
                                econfOut = false;
                            }
                            else
                            {
                                econfOut = true;
                            }
                        }
                        myConnection.Close();
                    }
                    return econfOut;
                }
            }
        }
        public bool EmailConfirmation(string username)
        {
            command = "SELECT EmailConfirmed AS EmailConfirmed FROM AspNetUsers WHERE UserName = @UserName";
            parameterName = "@UserName";
            return ReturnBool(username);
        }
        public bool EmailConfirmationById(string userid)
        {
            command = "SELECT EmailConfirmed AS EmailConfirmed FROM AspNetUsers WHERE Id = @Id";
            parameterName = "@Id";
            return ReturnBool(userid);
        }

        public int UpdateDatabase(string username)
        {
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(command, myConnection))
            {
                cmd.Parameters.AddWithValue(parameterName, username);
                myConnection.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateEmailLinkDate(string username)
        {
            command = "UPDATE AspNetUsers SET EmailLinkDate = '" + DateTime.Now + "' WHERE UserName = @UserName";
            parameterName = "@UserName";
            return UpdateDatabase(username);
        }
        public int UpdateLastLoginDate(string username)
        {
            command = "UPDATE AspNetUsers SET LastLoginDate = '" + DateTime.Now + "' WHERE UserName = @UserName";
            parameterName = "@UserName";
            return UpdateDatabase(username);
        }


        public static IEnumerable<SelectListItem> GetCountries()
        {
            RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
            List<SelectListItem> countryNames = new List<SelectListItem>();
            string cult = CultureInfo.CurrentCulture.EnglishName;
            string count = cult.Substring(cult.IndexOf('(') + 1,
                             cult.LastIndexOf(')') - cult.IndexOf('(') - 1);
            //To get the Country Names from the CultureInfo installed in windows
            foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                countryNames.Add(new SelectListItem()
                {
                    Text = country.DisplayName,
                    Value = country.DisplayName,
                    Selected = count == country.EnglishName
                });
            }
            //Assigning all Country names to IEnumerable
            IEnumerable<SelectListItem> nameAdded =
                countryNames.GroupBy(x => x.Text).Select(
                    x => x.FirstOrDefault()).ToList<SelectListItem>()
                    .OrderBy(x => x.Text);
            return nameAdded;
        }
        #endregion
    }
}
