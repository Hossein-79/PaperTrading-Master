using Microsoft.EntityFrameworkCore;
using PaperTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperTrading.Data
{
    public class PaperContext : DbContext
    {
        public PaperContext(DbContextOptions<PaperContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Coin> Coins { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Trade> Trades { get; set; }

        public DbSet<PNL> PNLs { get; set; }
    }
}
