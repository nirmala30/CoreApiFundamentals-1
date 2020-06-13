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
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }


        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCampsAsync(bool includeTalks = false)
        {
            try
            {
                var results = await repository.GetAllCampsAsync(includeTalks);
                var model = mapper.Map<CampModel[]>(results);
                return model;
            }
            catch(Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }

        [HttpGet("moniker")]
        public async Task<ActionResult<CampModel>> GetCampAsync(string moniker)
        {
            try
            {
                var result = await repository.GetCampAsync(moniker);
                var model = mapper.Map<CampModel>(result);
                if(model == null) return NotFound();
                return model;
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
          
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime dateTime, bool includeTalks = false)
        {
            try
            {
                var results = await repository.GetAllCampsByEventDate(dateTime, includeTalks);
                if (!results.Any()) return NotFound();
                var models = mapper.Map<CampModel[]>(results);
                return models;
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

       
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var link = linkGenerator.GetPathByAction("GetCampAsync", "camps", new { moniker = model.Moniker });
                if (string.IsNullOrWhiteSpace(link))
                    return BadRequest();
                var camp = mapper.Map<Camp>(model);
                repository.Add(camp);
                if(await repository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{camp.Moniker}", mapper.Map<CampModel>(camp));
                }
                return BadRequest();

            }catch(Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

    }
}
