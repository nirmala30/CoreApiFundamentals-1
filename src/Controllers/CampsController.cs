using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ICampRepository _repository;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Get all camps
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCampsAsync(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);

                CampModel[] model = _mapper.Map<CampModel[]>(results);

                return Ok(model);
            } catch
            {
                return this.StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetCampAsync(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                var Model = _mapper.Map<CampModel>(result);

                return Ok(Model);
            } catch
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> GetAllCampsByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);

                if(!results.Any())
                {
                    return NotFound();
                }

                return _mapper.Map<CampModel[]>(results);
            }
            catch
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

 
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {
            try
            {
                //var exists = _repository.GetCampAsync(campModel.Moniker);

                //if(exists != null)
                //{
                //    BadRequest("Moniker is in use");
                //}
                var location = _linkGenerator.GetPathByAction("GetCampAsync", "Camps", new {campModel.Moniker });

                if(string.IsNullOrEmpty(location))
                {
                    BadRequest("Could not use the current moniker");
                }


                var camp = _mapper.Map<Camp>(campModel);

                _repository.Add(camp);

                if(await _repository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<CampModel>(camp));
                }
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
