using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    using System.Linq.Expressions;

    public class RestaurantService(
        RestaurantDbContext dbContext,
        IMapper mapper,
        ILogger<RestaurantService> logger,
        IAuthorizationService authorizationService,
        IUserContextService userContextService)
        : IRestaurantService
    {
        public RestaurantDto GetById(int id)
        {
            var restaurant = dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var restaurantDto = mapper.Map<RestaurantDto>(restaurant);
            return restaurantDto;
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null || r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) ||
                            r.Description.ToLower().Contains(query.SearchPhrase.ToLower()));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>()
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description },
                    { nameof(Restaurant.Category), r => r.Category },
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.Descending
                    ? baseQuery.OrderByDescending(selectedColumn)
                    : baseQuery.OrderBy(selectedColumn);
            }


            var restaurant = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var restaurantDtos = mapper.Map<List<RestaurantDto>>(restaurant);

            var result = new PagedResult<RestaurantDto>(restaurantDtos, baseQuery.Count(), query.PageSize, query.PageNumber);

            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = mapper.Map<Restaurant>(dto);

            restaurant.CreatedById = userContextService.GetUserId;

            dbContext.Restaurants.Add(restaurant);
            dbContext.SaveChanges();
            return restaurant.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var operationResult = authorizationService.AuthorizeAsync(userContextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!operationResult.Succeeded)
                throw new ForbidException("Authorization failed");


            dbContext.Restaurants.Remove(restaurant);
            dbContext.SaveChanges();
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var operationResult = authorizationService.AuthorizeAsync(userContextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!operationResult.Succeeded)
                throw new ForbidException("Authorization failed");

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            dbContext.SaveChanges();
        }
    }
}
