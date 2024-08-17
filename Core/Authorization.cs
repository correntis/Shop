using Shop.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.MVVM.Model;
using System.Security.Cryptography.Xml;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel.UserActivities;
using System.IO;

namespace Shop.Core
{
    public static class Authorization
    {
        public static async Task<User> FindUserAsync(string login)
        {
            using ShopContext context = new();

            var user = await context.FindUserAsync(login);

            return user;
        }

        public static async Task<Admin> FindAdminAsync(string login)
        {
            using ShopContext context = new();

            var admin = await context.FindAdminAsync(login);

            return admin;
        }

        public static async Task RegisterUserAsync(string email, string password)
        {
            using ShopContext context = new();

            var userAccount = new UserAccount
            {
                Login = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };
            var user = new User
            {
                UserAccount = userAccount,
                DeliveryInfo = new(),
                Email = userAccount.Login,
                Name = "Неизвестно",
                Avatar = File.ReadAllBytes("C:\\Users\\Максим\\Desktop\\КП\\Shop\\Images\\user_default.jpg")
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();
        }
    }
}
