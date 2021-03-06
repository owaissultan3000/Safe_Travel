using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using carpool.Models;
using Microsoft.EntityFrameworkCore;

namespace carpool.Services.UserServices
{
    public class UserService : IUserService
    {
        ApiDbContext db;

        public UserService(ApiDbContext _db)
        {
            db = _db;
        }
        public async Task<List<User>> AllUsers()
        {
            if (db != null)
            {
                return await db.Users.ToListAsync();
            }

            return null;
        }


        public async Task<string> CreateUser(UserModel userModel)
        {
            if (db != null)
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userModel.Email);
                if (user == null)
                {
                    userModel.UserId = Guid.NewGuid();
                    userModel.Password = userModel.ConfirmPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
                    User userDB = new User{
                    UserId = userModel.UserId.ToString(),
                    UserName = userModel.UserName.ToLower(),
                    PhoneNumber = userModel.PhoneNumber,
                    Gender = userModel.Gender.ToLower(),
                    Email = userModel.Email.ToLower(),
                    Passwords = userModel.Password,
                    CreateionDate = DateTime.Now,
                    Role = userModel.Role.ToString().ToLower()
                };
                await db.Users.AddAsync(userDB);
                await db.SaveChangesAsync();
                return "User Created Successfully";
                }
                else
                {
                    return "User already exist with email " + userModel.Email;
                }
                
            }

            return "Something went wrong try again";
        }

        public async Task<string> DeleteUser(string email)
        {
            int result = 0;

            if (db != null)
            {
                //Find the post for specific post id
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    //Delete that user
                    db.Users.Remove(user);

                    //Commit the transaction
                    result = await db.SaveChangesAsync();
                }
                return email+" Deleted Successfully";
            }

            return "Something went wrong try again";
        }

        public async Task<User> GetUser(string email)
        {
            if (db != null)
            {
               User user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
               return user;
            }

            return null;
        }

        public async Task<string> UpdateUser(UserModel userModel)
        {
            if (db != null)
            {
                    User user = await db.Users.FirstOrDefaultAsync(u => u.Email == userModel.Email);
                    User userDB = new User{
                    UserId = user.UserId.ToString(),
                    UserName = userModel.UserName,
                    PhoneNumber = userModel.PhoneNumber,
                    Gender = userModel.Gender,
                    Email = user.Email,
                    Passwords = BCrypt.Net.BCrypt.HashPassword(userModel.Password),
                    // ConfirmPassword = BCrypt.Net.BCrypt.HashPassword(userModel.ConfirmPassword),
                    Role = user.Role.ToString()
                };
                //Update that user
                db.Users.Update(userDB);

                //Commit the transaction
                await db.SaveChangesAsync();

                return "User Updated Successfully";
            }
            return "Something went wrong try again";
        }

        public async Task<List<Ride>> ViewAvailableRides()
        {
            if (db != null)
            {
                return await db.Rides.ToListAsync();
            }

            return null;
        }

        public async Task<string> BookRide(BookRide bookRide)
        {
            if (db != null)
            {
                Booking ride = await db.Bookings.FirstOrDefaultAsync(b => b.PassengerId == bookRide.PassengerId.ToString() );
                if (ride == null)
                {
                    bookRide.BookingId = Guid.NewGuid();
                    Booking booking = new Booking{
                        BookingId = bookRide.BookingId.ToString(),
                        CaptainId = bookRide.CaptainId.ToString(),
                        PassengerId = bookRide.PassengerId.ToString(),
                        PassengerName = bookRide.PassengerName,
                        PassengerPhoneNumber = bookRide.PassengerPhoneNumber,
                        PassengerDestination = bookRide.PassengerDestination
                };
                   await db.Bookings.AddAsync(booking);
                   await db.SaveChangesAsync();
                   return "Ride Booked Successfully";
                }
            return "You already booked a ride";

            }
        return "Something went wrong try again";

        }
    }
}