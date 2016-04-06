using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class ApplicationDbContextSeedData
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ApplicationDbContextSeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task EnsureSeedDataAsync()
        {
            if (await _userManager.FindByEmailAsync("mikey@starfantasygames.com") == null)
            {
                return;
            }

            if(!_context.Connections.Any())
            {
                var family = _context.Families.Add(new Family { Name = "Smiths" }).Entity;
                

                _context.Connections.Add(
                    new Connection { User = _context.Users.FirstOrDefault(u => u.Email == "mikey@starfantasygames.com"), Family = family, ConnectionType = ConnectionType.Caretaker });

                _context.SaveChanges();

                //var familyPhoto = _context.FilePaths.Add(new FamilyPhoto(family)).Entity;
                //family.FamilyPhoto = familyPhoto;

                //_context.Families.Update(family);
                //_context.SaveChanges();
            }
        }
    }
}
