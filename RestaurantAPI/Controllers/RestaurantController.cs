using System.Security.Claims;
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
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService service;

        public RestaurantController(IRestaurantService service)
        {
            this.service = service;
        }

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
        [Authorize(Policy = "AtLeast20")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = service.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }
    }
}
