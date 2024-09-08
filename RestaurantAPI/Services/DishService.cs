using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public class DishService : IDishService
    {
        private readonly RestaurantDbContext dbContext;
        private readonly IMapper mapper;

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = mapper.Map<Dish>(dto);

            dish.RestaurantId = restaurantId;

            dbContext.Dishes.Add(dish);
            dbContext.SaveChanges();

            return dish.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);

            var dishDto = mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishesDtos = mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishesDtos;
        }

        public void Delete(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);

            dbContext.Dishes.Remove(dish);
            dbContext.SaveChanges();
        }

        public void DeleteAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            dbContext.Dishes.RemoveRange(restaurant.Dishes);
            dbContext.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = dbContext.Restaurants.Include(r => r.Dishes).FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }

        private Dish GetDishById(int restaurantId, int dishId)
        {
            var dish = dbContext.Dishes.FirstOrDefault(d => d.Id == dishId && d.RestaurantId == restaurantId);

            if (dish == null)
                throw new NotFoundException("Dish not found");

            return dish;
        }
    }
}
