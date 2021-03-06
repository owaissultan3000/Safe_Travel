using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using carpool.Models;

namespace carpool.Services.UserServices
{
    public interface IUserService
    {
        
        public Task<User> GetUser(string email);
        public Task<string> CreateUser(UserModel userModel);
        public Task<string> UpdateUser(UserModel userModel);
        public Task<string> DeleteUser(string email);
        public Task<List<Ride>> ViewAvailableRides();
        public Task<string> BookRide(BookRide bookRide);
    }
}