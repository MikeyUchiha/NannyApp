using AutoMapper;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using NannyApp.Models;
using NannyApp.Models.Interfaces;
using NannyApp.ViewModels.API.Families;
using System;
using System.Collections.Generic;
using System.Net;

namespace NannyApp.Controllers.API
{
    [Route("api/[controller]")]
    public class FamiliesController : Controller
    {
        private ILogger<FamiliesController> _logger;
        private INannyAppRepository _repository;

        public FamiliesController(INannyAppRepository repository, ILogger<FamiliesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult GetAll()
        {
            try
            {
                IEnumerable<Family> results = _repository.GetAllFamilies();

                if (results == null)
                {
                    return Json(null);
                }
                _logger.LogInformation("Getting all familes.");
                return Json(Mapper.Map<IEnumerable<FamilyViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get familes", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occured finding Families");
            }
        }

        [HttpGet("{id:int}")]
        public JsonResult GetById(int id)
        {
            try
            {
                Family results = _repository.GetFamily(id);

                if (results == null)
                {
                    return Json(null);
                }
                _logger.LogInformation("Get by Id");
                return Json(Mapper.Map<FamilyViewModel>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get family", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occured finding Family");
            }
        }

        [HttpGet("{name}")]
        public JsonResult Get(string name)
        {
            try
            {
                IEnumerable<Family> results = _repository.GetFamiliesByUserName(name);

                if (results == null)
                {
                    return Json(null);
                }
                _logger.LogInformation("Get by username");
                return Json(Mapper.Map<IEnumerable<FamilyViewModel>>(results));
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to get familes for user {name}", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occured finding Username");
            }
        }
    }
}
