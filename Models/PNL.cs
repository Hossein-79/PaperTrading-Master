using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Models
{
    public class PNL
    {
        public int PNLId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public decimal DayBalance { get; set; }

        public User User { get; set; }
    }
}
