using Microsoft.EntityFrameworkCore;
using PaperTrading.Data;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class UserService : IUserService
    {
        private readonly PaperContext _context;

        public UserService(PaperContext paperContext)
        {
            _context = paperContext;
        }

        public async Task AddUser(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUser(string name) =>
            await _context.Users.Where(u => u.Name == name).FirstOrDefaultAsync();

        public async Task<User> GetUser(int useId) =>
            await _context.Users.Where(u => u.UserId == useId).FirstOrDefaultAsync();
    }
}
