using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public IEnumerable<Wallet> Wallets { get; set; }

        public IEnumerable<Trade> Trades { get; set; }

        public IEnumerable<PNL> PLNs { get; set; }
    }
}
