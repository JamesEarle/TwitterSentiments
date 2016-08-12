using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace TwitterSentiments.Models
{
    public class Request
    {
        // Primary key
        public int ID { get; set; }

        // Form inputs
        [Display(Name = "Twitter Handle")]
        public string TwitterHandle { get; set; }

        
        public int Count { get; set; }
        public double Result { get; set; }

        // Auto timestamp
        [Display(Name = "Date Created")]
        public DateTime Timestamp { get; } = DateTime.Now;
    }

    public class RequestDbContext : DbContext
    {
        public DbSet<Request> Requests { get; set; }
    }
}