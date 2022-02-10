using Microsoft.EntityFrameworkCore;
using PaperTrading.Data;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class CoinService : ICoinService
    {
        private readonly PaperContext _context;

        public CoinService(PaperContext paperContext)
        {
            _context = paperContext;
        }

        public async Task<Coin> GetUsdt() =>
            await _context.Coins.Where(u => u.Symbol == "USDT").FirstOrDefaultAsync();

        public async Task<Coin> GetCoin(string symbol) =>
            await _context.Coins.Where(u => u.Symbol == symbol).FirstOrDefaultAsync();

        public async Task<IEnumerable<Coin>> GetCoins() =>
            await _context.Coins.ToListAsync();

        public async Task Update(Coin coin)
        {
            _context.Update(coin);
            await _context.SaveChangesAsync();
        }
    }
}
