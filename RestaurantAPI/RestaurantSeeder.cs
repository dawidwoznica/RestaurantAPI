using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder(RestaurantDbContext dbContext)
    {
        public void Seed()
        {
            if (dbContext.Database.CanConnect())
            {
                if (dbContext.Database.IsRelational())
                {
                    var pendingMigrations = dbContext.Database.GetPendingMigrations();

                    if (pendingMigrations != null && pendingMigrations.Any())
                        dbContext.Database.Migrate();
                }

                if (!dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    dbContext.Roles.AddRange(roles);
                    dbContext.SaveChanges();
                }

                if (!dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    dbContext.Restaurants.AddRange(restaurants);
                    dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },

                new Role()
                {
                    Name = "Manager"
                },

                new Role()
                {
                    Name = "Admin"
                }
            };

            return roles;
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "KFC Corporation, doing business as KFC (also commonly referred to by its historical name Kentucky Fried Chicken), is an American fast food restaurant chain that specializes in fried chicken. Headquartered in Louisville, Kentucky, it is the world's second-largest restaurant chain (as measured by sales) after McDonald's, with over 30,000 locations globally in 150 countries as of April 2024. The chain is a subsidiary of Yum! Brands, a restaurant company that also owns the Pizza Hut and Taco Bell chains.",
                    ContactEmail = "contact@kfc.com",
                    ContactNumber = "444555666",
                    HasDelivery = true,
                    Dishes = new List<Dish>() {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Price = 10.30M
                        },
                        new Dish()
                        {
                            Name = "Chicken Nuggets",
                            Price = 5.30M
                        }
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001"
                    }
                },
                new Restaurant {
                    Name = "McDonalds",
                    Category = "Fast Food",
                    Description = "McDonald's Corporation, doing business as McDonald's, is an American multinational fast food chain, founded in 1940 as a restaurant operated by Richard and Maurice McDonald, in San Bernardino, California, United States.",
                    ContactEmail = "contact@mcdonalds.com",
                    ContactNumber = "456456456",
                    HasDelivery = true,
                    Dishes = new List<Dish>() {
                        new Dish()
                        {
                            Name = "Big Mac",
                            Price = 11.50M
                        },
                        new Dish()
                        {
                            Name = "Fries",
                            Price = 6.60M
                        }
                    },
                    Address = new Address()
                    {
                        City = "Poznań",
                        Street = "Budziszyńska 20",
                        PostalCode = "61-765"
                    }
                }
            };

            return restaurants;
        }
    }
}
