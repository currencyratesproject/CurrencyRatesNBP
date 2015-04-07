using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBPkursyWalut.Models
{
    public class Dir
    {
        [Key]
        public int ID { get; set; }

        public String DirNumber { get; set; }
        
    }
}