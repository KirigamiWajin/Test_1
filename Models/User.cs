using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
    }
}