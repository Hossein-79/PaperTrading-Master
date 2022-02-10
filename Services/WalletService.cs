using Microsoft.EntityFrameworkCore;
using PaperTrading.Data;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class WalletService : IWalletService
    {
        private readonly PaperContext _context;

        public WalletService(PaperContext paperContext)
        {
            _context = paperContext;
        }

        public async Task AddWallet(Wallet wallet)
        {
            await _context.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Wallet>> GetUserWallets(int userId) =>
            await _context.Wallets.Where(u => u.UserId == userId).Include(l => l.Coin).ToListAsync();

        public async Task<Wallet> GetUserWallet(int userId, int coinId) =>
            await _context.Wallets.Where(u => u.UserId == userId && u.CoinId == coinId).FirstOrDefaultAsync();

        public async Task Update(Wallet wallet)
        {
            _context.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<Wallet> wallet)
        {
            _context.UpdateRange(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
