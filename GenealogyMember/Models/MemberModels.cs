using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyMember.Models
{
    public class MemberModels
    {
        public int MemberId { get; set; }
        public string TheFamilyMemberId { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName_No { get; set; }
        public string MedicalAllergy { get; set; }
        public bool IsQRCodeGenerated { get; set; }
        public string QRImage { get; set; }
    }
}