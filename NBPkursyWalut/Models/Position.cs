using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace NBPkursyWalut.Models
{
   
    public class Position
    {
        [Key]
        [XmlIgnore]
        public int ID { get; set; }



        [XmlIgnore]
        public DateTime Date { get; set; }



        [XmlElement("nazwa_waluty")]
        public String CurrencyName { get; set; }
      
        

        [XmlElement("przelicznik")]
        public String CurrencyConversion { get; set; }

        [XmlElement("kod_waluty")]
        public String CurrencyCode { get; set; }


        [XmlIgnore]
        public Double Average { get; set; }

        [NotMapped]
        [XmlElement("kurs_sredni")]
        public String AverageRate 
        {
            get 
            {
                return Average.ToString();
            }
            set 
            {
                Average = Convert.ToDouble(value);
               
            }
         }

       







    }
}