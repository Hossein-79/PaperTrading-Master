using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }

        public int UserId { get; set; }

        public int CoinId { get; set; }

        public decimal Balance { get; set; }

        public User User { get; set; }

        public Coin Coin { get; set; }
    }
}
