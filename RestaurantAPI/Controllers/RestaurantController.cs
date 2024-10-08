﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RestaurantController(IRestaurantService service) : ControllerBase
    {
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateRestaurantDto dto)
        {
            service.Update(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete([FromRoute] int id)
        {
            service.Delete(id);

            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        //[Authorize(Policy = "AtLeast20")]
        //[Authorize(Policy = "CreatedAtLeast2Restaurants")]
        public ActionResult<PagedResult<RestaurantDto>> GetAll([FromQuery] RestaurantQuery query)
        {
            var pagedResult = service.GetAll(query);

            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurantDto = service.GetById(id);

            return Ok(restaurantDto);
        }


        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var id = service.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }
    }
}
