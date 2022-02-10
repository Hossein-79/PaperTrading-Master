using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{

    public enum TradeSide : byte
    {
        Buy = 1,
        Sell = 2,
    }

    public enum TradeStatus : byte
    {
        Open = 1,
        Success = 2,
        Cancel = 3,
    }

    public class Trade
    {
        public int TradeId { get; set; }

        public int UserId { get; set; }

        public int CoinId { get; set; }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public TradeSide Side { get; set; }

        public TradeStatus Status { get; set; }

        public Coin Coin { get; set; }

        public User User { get; set; }
    }
}
