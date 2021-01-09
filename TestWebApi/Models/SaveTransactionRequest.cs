using System;
using System.ComponentModel.DataAnnotations;

namespace TestWebApi.Models
{
    public class SaveTransactionRequest
    {
        public DateTime Date { get; set; }
        
        public double Outflow { get; set; }
        
        public double Inflow { get; set; }
        
        [Required]
        public string Category { get; set; }
        
        [Required]
        public string Account { get; set; }
        
        public string Memo { get; set; }
        
        public string Cleared { get; set; }
    }
}