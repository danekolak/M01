using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using AutoMapper;
using MyCodeCamp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCodeCamp.Controllers
{
    [Route("api/camps")]
    public class CampsController : BaseController
    {
        private readonly ILogger<CampsController> _logger;
        private readonly ICampRepository _repo;
        private IMapper _mapper;

        public CampsController(ICampRepository repo, ILogger<CampsController> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

       

        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{moniker}", Name = "CampGet")]
        public IActionResult Get(string moniker, bool includeSpeakers = false)
        {
            try
            {                  
                
                var camp = includeSpeakers == false ? _repo.GetCampByMonikerWithSpeakers(moniker) : _repo.GetCampByMoniker(moniker);
                if (camp == null) return NotFound($"Camp {moniker} was not found");

                return Ok(_mapper.Map<CampModel>(camp, opt => opt.Items["UrlHelper"]=this.Url));
            }
            catch
            {
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Camp model)
        {
            try
            {
                _logger.LogInformation("Creating a new Code Camp");


                _repo.Add(model);
                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { id = model.Id });
                    return Created(newUri, model);
                }
                _logger.LogWarning("Could not save Camp to the database");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Camp model)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null)
                    return NotFound($"Could not find a camp with an ID of {id}");

                oldCamp.Name = model.Name ?? oldCamp.Name;
                oldCamp.Description = model.Description ?? oldCamp.Description;
                oldCamp.Location = model.Location ?? oldCamp.Location;
                oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;
                oldCamp.EventDate = model.EventDate != DateTime.MinValue ? model.EventDate : oldCamp.EventDate;

                if (await _repo.SaveAllAsync())
                    return Ok(oldCamp);
            }
            catch (Exception)
            {
            }
            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null)  return NotFound($"Could not find Camp with ID of {id}");

                    _repo.Delete(oldCamp);
                if(await _repo.SaveAllAsync())
                {
                    return Ok();
                }

            }
            catch (Exception)
            {
            }
            return BadRequest("Could not delete Camp");
        }

    }
}
