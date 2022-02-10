using Microsoft.EntityFrameworkCore;
using PaperTrading.Data;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class TradeService : ITradeService
    {
        private readonly PaperContext _context;

        public TradeService(PaperContext paperContext)
        {
            _context = paperContext;
        }

        public async Task AddTrade(Trade trade)
        {
            await _context.AddAsync(trade);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Trade>> GetUserTrades(int userId) =>
            await _context.Trades.Where(u => u.UserId == userId).Include(l => l.Coin).ToListAsync();

        public async Task<IEnumerable<Trade>> GetCoinTradesBuy(int coinId, decimal price) =>
            await _context.Trades.Where(u => u.CoinId == coinId && u.Price >= price && u.Status == TradeStatus.Open && u.Side == TradeSide.Buy).ToListAsync();

        public async Task<IEnumerable<Trade>> GetCoinTradesSell(int coinId, decimal price) =>
            await _context.Trades.Where(u => u.CoinId == coinId && u.Price <= price && u.Status == TradeStatus.Open && u.Side == TradeSide.Sell).ToListAsync();

        public async Task<Trade> GetTrade(int tradeId) =>
            await _context.Trades.Where(u => u.TradeId == tradeId).FirstOrDefaultAsync();


        public async Task Update(Trade trade)
        {
            _context.Update(trade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<Wallet> wallet)
        {
            _context.UpdateRange(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
