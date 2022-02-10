using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{
    public class Coin
    {
        public int CoinId { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public decimal Price { get; set; }

        [NotMapped]
        public decimal UserBalance { get; set; }

        public IEnumerable<Wallet> Wallets { get; set; }

        public IEnumerable<Trade> Trades { get; set; }
    }
}
