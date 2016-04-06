using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using NannyApp.ViewModels.Users;
using AutoMapper;
using NannyApp.Models.Interfaces;

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

        public ApplicationUser GetUserByUserName(string username)
        {
            try
            {
                return _context.Users
                    .Include(u => u.ProfilePhoto)
                    .Include(u => u.Connections)
                    .ThenInclude(f => f.Family)
                    .Include(u => u.Groups)
                    .Where(x => x.UserName == username)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get users with profile pictures from the database", ex);
                return null;
            }
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            try
            {
                return _context.Users
                    .Include(u => u.ProfilePhoto)
                    .Include(u => u.Connections)
                    .Include(u => u.Groups)
                    .OrderBy(u => u.UserName)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get users from the database", ex);
                return null;
            }
        }

        public void AddOrUpdateProfilePhotoByUser(ProfilePhoto photo, ApplicationUser user)
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

        public void RemoveProfilePhotoByUser(ApplicationUser user)
        {
            try
            {
                var userProfilePhoto = user.ProfilePhoto;
                if (userProfilePhoto != null)
                {
                    _context.Remove(user.ProfilePhoto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not add profile photo by username to the database", ex);
                return;
            }
        }

        public Family GetFamily(int id)
        {
            try
            {
                return _context.Families
                    .Include(f => f.Children)
                    .Include(f => f.Connections)
                    .ThenInclude(c => c.User)
                    .Where(f => f.Id == id)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get family from the database", ex);
                return null;
            }
        }

        public IEnumerable<Family> GetFamiliesByUserName(string userName)
        {
            try
            {
                return _context.Families
                    .Include(f => f.Children)
                    .Include(f => f.Connections)
                    .ThenInclude(c => c.User)
                    .Where(c => c.Connections.Any(u => u.User.UserName == userName))
                    .OrderBy(f => f.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get families by username from the database", ex);
                return null;
            }
        }

        public IEnumerable<Family> GetAllFamilies()
        {
            try
            {
                return _context.Families
                    .Include(f => f.Children)
                    .Include(f => f.Connections)
                    .ThenInclude(c => c.User)
                    .OrderBy(f => f.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get families from the database", ex);
                return null;
            }
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
