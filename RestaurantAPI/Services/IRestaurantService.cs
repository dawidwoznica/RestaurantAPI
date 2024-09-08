using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        public void Update(int id, UpdateRestaurantDto dto);
        public void Delete(int id);
        public RestaurantDto GetById(int id);

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        public int Create(CreateRestaurantDto dto);
    }
}
