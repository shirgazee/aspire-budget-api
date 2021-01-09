using System;
using System.ComponentModel.DataAnnotations;

namespace TestWebApi.Models
{
    public class SaveAccountTransferRequest
    {
        public DateTime Date { get; set; }
        public double Sum { get; set; }
        
        [Required]
        public string AccountFrom { get; set; }
        
        [Required]
        public string AccountTo { get; set; }
        
        public string Memo { get; set; }
        public string Cleared { get; set; }
    }
}