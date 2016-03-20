using System.Collections.Generic;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public interface INannyAppRepository
    {
        ApplicationUser GetUserWithProfilePhotoByUserName(string username);
        void AddOrUpdateProfilePhotoByUser(FilePath photo, ApplicationUser user);
        IEnumerable<Family> GetAllFamilies();
        IEnumerable<Family> GetAllFamiliesWithChildren();
        IEnumerable<ApplicationUser> GetAllUsers();
        IEnumerable<ApplicationUser> GetAllUsersWithProfilePhotos();
        void RemoveProfilePhotoByUser(ApplicationUser user);
        bool SaveAll();
    }
}