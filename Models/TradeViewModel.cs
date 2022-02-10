using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{
    public class TradeViewModel
    {
        public Coin Usdt { get; set; }

        public Coin Token { get; set; }

        public Trade Trade { get; set; }

        public IEnumerable<Trade> OpenTrades { get; set; }

        public IEnumerable<Trade> SuccessTrade { get; set; }
    }
}
