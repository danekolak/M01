using System;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCodeCamp.Controllers
{
    [Route("api/camps")]
    public class CampsController : Controller
    {
        private readonly ICampRepository _repo;

        public CampsController(ICampRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(camps);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var camp = _repo.GetCamp(id);
                if (camp == null) return NotFound();
                return Ok(camp);
            }
            catch
            {
            }
            return BadRequest();
        }
    }
}
