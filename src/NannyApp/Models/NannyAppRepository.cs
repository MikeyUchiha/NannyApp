using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;

namespace NannyApp.Models
{
    public class NannyAppRepository : INannyAppRepository
    {
        private ApplicationDbContext _context;
        private ILogger _logger;

        public NannyAppRepository(ApplicationDbContext context, ILogger<NannyAppRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            try
            {
                return _context.Users.OrderBy(u => u.UserName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get users from the database", ex);
                return null;
            }
        }

        public IEnumerable<ApplicationUser> GetAllUsersWithProfilePhotos()
        {
            try
            {
                return _context.Users
                    .Include(u => u.ProfilePhoto)
                    .OrderBy(u => u.UserName)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get users with profile pictures from the database", ex);
                return null;
            }
            
        }

        public ApplicationUser GetUserWithProfilePhotoByUserName(string username)
        {
            try
            {
                return _context.Users
                    .Include(u => u.ProfilePhoto)
                    .Where(x => x.UserName == username)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get users with profile pictures from the database", ex);
                return null;
            }
        }

        public void AddOrUpdateProfilePhotoByUser(FilePath photo, ApplicationUser user)
        {
            try
            {
                var userProfilePhoto = user.ProfilePhoto;
                photo.User = user;
                if (userProfilePhoto != null)
                {
                    user.ProfilePhoto.FileName = photo.FileName;
                    user.ProfilePhoto.FileType = photo.FileType;
                    user.ProfilePhoto.FileUrl = photo.FileUrl;
                    _context.Update(user);
                }
                else
                {
                    _context.Add(photo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not add profile photo by username to the database", ex);
                return;
            }
        }

        public IEnumerable<Family> GetAllFamilies()
        {
            try
            {
                return _context.Families.OrderBy(f => f.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get families from the database", ex);
                return null;
            }
        }

        public IEnumerable<Family> GetAllFamiliesWithChildren()
        {
            try
            {
                return _context.Families
                    .Include(f => f.Children)
                    .OrderBy(f => f.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get families with children from the database", ex);
                return null;
            }
        }

        public void RemoveProfilePhotoByUser(ApplicationUser user)
        {
            try
            {
                var userProfilePhoto = user.ProfilePhoto;
                if (userProfilePhoto != null)
                {
                    _context.FilePaths.Remove(user.ProfilePhoto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not add profile photo by username to the database", ex);
                return;
            }
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
