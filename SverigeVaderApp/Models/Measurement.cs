using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SverigeVaderApp.Models
{
    public class Measurement
    {
        public Guid Id { get; set; }

        [DisplayName("Plats")]
        public string City { get; set; }
        
        [DisplayName("Datum")]
        public DateTime Date { get; set; }
       
        public Record Record { get; set; }
        
    }
}
