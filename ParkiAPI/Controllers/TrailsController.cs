using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of all Trials.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]

        public IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();
            var objDto = new List<TrailDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual Trial.
        /// </summary>
        /// <param name="trailId">The id of the trial</param>
        /// <returns></returns>

        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);
            if (obj == null)
            {
                return NotFound();

            }

            var objDto = _mapper.Map<TrailDto>(obj);

            return Ok(objDto);
        }


        /// <summary>
        /// Create a new Trail
        /// </summary>
        /// <param name="trailDto">The object of the Trail being created</param>
        /// <returns></returns>

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail already exists");
                return StatusCode(404, ModelState);
            }
            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when creating the Trail {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { trailId = trailObj.Id }, trailObj);

        }

        /// <summary>
        /// Update a existing Trail
        /// </summary>
        /// <param name="trailId"> The Id of the trail you want to update </param>
        /// <param name="trailDto"> The updated trail object</param>
        /// <returns></returns>

        [HttpPatch("{trailId:int}", Name ="Update Trail")]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]


        public IActionResult UpdateTrail(int trailId,[FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto==null||trailDto.Id!=trailId)
            {
                return BadRequest(ModelState);
            }

            var obj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(obj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record { obj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a Trail
        /// </summary>
        /// <param name="trailId"> The Id of the Trail you want to delete </param>
        /// <returns></returns>

        [HttpDelete("{trailId:int}", Name ="Delete Trail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound();
            }

            var trailObj = _trailRepo.GetTrail(trailId);

            if (!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record { trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
