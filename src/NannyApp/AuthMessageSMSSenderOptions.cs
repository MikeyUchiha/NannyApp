namespace NannyApp.Services
{
    public class AuthMessageSMSSenderOptions
    {
        public string AuthToken { get; internal set; }
        public string SendNumber { get; internal set; }
        public string SID { get; internal set; }
    }
}