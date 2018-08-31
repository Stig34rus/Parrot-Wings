using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace PW.Models
{

    public class WalletContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }

    public class Wallet
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public decimal Balance { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string When { get; set; }
        public decimal Value { get; set; }
        
    }

    public class WalletHistory
    {
        public string From { get; set; }
        public string To { get; set; }
        public string When { get; set; }
        public decimal Value { get; set; }

    }
    
}