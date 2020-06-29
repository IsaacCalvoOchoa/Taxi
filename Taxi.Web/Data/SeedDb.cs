using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Isaac", "Calvo", "isaaccalvoochoa@gmail.com", "6861881069", "Deveza 1241 Barcelona", UserType.Admin);
            var driver = await CheckUserAsync("2020", "Isaac", "Calvo", "ochoacalvoisaac@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Driver);
            var user1 = await CheckUserAsync("3030", "Isaac", "Calvo", "ofisnoviembre2016@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            var user2 = await CheckUserAsync("4040", "Isaac", "Calvo", "josuecerv15@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            await CheckTaxisAsync(driver, user1, user2);

        } 
        private async Task<UserEntity> CheckUserAsync(
           string document,
           string firstName,
           string lastName,
           string email,
           string phone,
           string address,
           UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());

        }

        private async Task CheckTaxisAsync(
                    UserEntity driver,
                    UserEntity user1,
                    UserEntity user2)
        {
            if (!_dataContext.Taxis.Any())
            {
                _dataContext.Taxis.Add(new TaxiEntity
                {
                    User = driver,
                    Plaque = "TPQ123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus consectetur mi viverra velit dapibus, vel ultricies tortor dictum. Aenean malesuada elementum feugiat. Praesent dapibus congue luctus. Nam fringilla luctus ex et mattis. In pellentesque ornare nibh, et auctor leo varius in. Cras iaculis ultrices massa, sed tincidunt diam pharetra ac. Proin quis libero eros. Praesent vel viverra purus, id imperdiet diam. Suspendisse dapibus consectetur sagittis.",
                            User = user1
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Vestibulum blandit egestas dui, sed maximus augue sollicitudin nec. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Ut a faucibus diam. Morbi ac ornare ligula. Nullam mollis lacus et ante aliquet, eu consectetur risus ornare. Mauris eget orci aliquam, interdum diam non, ultricies ipsum. Ut eu fermentum quam, a hendrerit leo.",
                            User = user1
                        }
                    }
                });

                _dataContext.Taxis.Add(new TaxiEntity
                {
                    Plaque = "THW321",
                    User = driver,
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user2
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user2
                        }
                    }
                });

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
