using NannyApp.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NannyApp.Models.Interfaces
{
    public interface INannyAppRepository
    {
        ApplicationUser GetUserByUserName(string username);
        IEnumerable<ApplicationUser> GetAllUsers();
        void AddOrUpdateProfilePhotoByUser(ProfilePhoto photo, ApplicationUser user);
        void RemoveProfilePhotoByUser(ApplicationUser user);
        Family GetFamily(int id);
        IEnumerable<Family> GetFamiliesByUserName(string userName);
        IEnumerable<Family> GetAllFamilies();
        bool SaveAll();
    }
}