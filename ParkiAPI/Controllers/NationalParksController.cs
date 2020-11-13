using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkiAPI.Models;
using ParkiAPI.Models.Dtos;
using ParkyAPI.Respository.IRespository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;

        }

        /// <summary>
        /// Get list of all National parks.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200,Type =typeof(List<NationalParkDto>))]
        
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();

            var objDto = new List<NationalParkDto>();

            foreach(var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual national park.
        /// </summary>
        /// <param name="nationalParkId"> The Id of the national park </param>
        /// <returns></returns>

        [HttpGet("{nationalParkId:int}", Name ="GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]

        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        /// <summary>
        /// Create a National Park.
        /// </summary>
        /// <param name="nationalParkDto"> The object of the park being created </param>
        /// <returns></returns>

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }


            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(nationalParkObj)){
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { NationalParkId=nationalParkObj.Id}, nationalParkObj);


        }

        /// <summary>
        /// Update a National Parks details
        /// </summary>
        /// <param name="nationalParkId"> The Id of the national park to update </param>
        /// <param name="nationalParkDto"> The updated national park object </param>
        /// <returns></returns>

        [HttpPatch("{nationalParkId:int}", Name ="UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null|| nationalParkId!=nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record { nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Delete a National Park
        /// </summary>
        /// <param name="nationalParkId"> The Id Of the National Park to delete </param>
        /// <returns></returns>

        [HttpDelete("{nationalParkId:int}", Name="DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId)){
                return NotFound();

            }

            var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);

            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record { nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

    }

}
