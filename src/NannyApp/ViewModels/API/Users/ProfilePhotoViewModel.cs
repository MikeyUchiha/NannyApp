using NannyApp.Models;

namespace NannyApp.ViewModels.API.Users
{
    public class ProfilePhotoViewModel
    {
        private ProfilePhoto _profilephoto;

        public string Url { get; internal set; }

        public ProfilePhotoViewModel(ProfilePhoto profilephoto)
        {
            _profilephoto = profilephoto;
            GetUrlFromProfilePhoto();
        }

        private void GetUrlFromProfilePhoto()
        {
            var profilePhoto = _profilephoto;
            if(profilePhoto != null)
            {
                Url = profilePhoto.FileUrl;
            }
        }
    }
}
