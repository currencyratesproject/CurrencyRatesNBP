using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NBPkursyWalut.Models
{
    [XmlRoot("tabela_kursow")]
    public class RatesTableDay
    {
        [XmlElement("data_publikacji")]
        public String PublicationDate { get; set; }

        [XmlElement("pozycja")]
        public List<Position> Positions { get; set; }

    }
}