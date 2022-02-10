using Microsoft.EntityFrameworkCore;
using PaperTrading.Data;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class PNLService : IPNLService
    {
        private readonly PaperContext _context;

        public PNLService(PaperContext paperContext)
        {
            _context = paperContext;
        }

        public async Task AddPNL(PNL pNL)
        {
            await _context.AddAsync(pNL);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PNL>> GetUserPNLs(int userId, int take = 20) =>
            await _context.PNLs.Where(u => u.UserId == userId).OrderByDescending(u => u.PNLId).Take(take).ToListAsync();

    }
}
