using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyMember.Models
{
    public class QRCodeModels
    {
        public string MASTER_ID { get; set; }
        public string PRE_GATEIN_HOUSE_ID { get; set; }
        public List<DimensionDetailsModels> DimensionDetailsList { get; set; }
    }
    public class DimensionDetailsModels
    {
        public int LENGTH { get; set; }
        public int WIDTH { get; set; }
        public int HEIGHT { get; set; }
        public int PIECES { get; set; }
        public string WEIGHT { get; set; }
    }
}