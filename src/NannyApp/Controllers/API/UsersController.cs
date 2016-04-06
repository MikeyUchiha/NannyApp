using AutoMapper;
using Microsoft.AspNet.Mvc;
using NannyApp.Models;
using NannyApp.Models.Interfaces;
using NannyApp.ViewModels.Users;
using System.Collections.Generic;

namespace NannyApp.Controllers.API
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private INannyAppRepository _repository;

        public UsersController(INannyAppRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var results = _repository.GetAllUsers();

            if (results == null)
            {
                return Json(null);
            }

            return Json(Mapper.Map<IEnumerable<UserViewModel>>(results));
        }

        [HttpGet("{name}")]
        public JsonResult Get(string name)
        {
            var results = _repository.GetUserByUserName(name);

            if(results == null)
            {
                return Json(null);
            }

            return Json(Mapper.Map<UserViewModel>(results));
        }
    }
}
