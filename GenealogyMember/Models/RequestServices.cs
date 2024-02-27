using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyMember.Models
{
    public class RequestServices
    {
        public int ServiceId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ServiceDuration { get; set; }
        public int AssignedTo { get; set; }
        public int ServiceMasterId { get; set; }
        public string ServiceName { get; set; }
        public List<ServiceMasterModel> ServiceMasterList { get; set; }
        public string ServiceMasterIdString { get; set; }
        public List<UserModels> UserMasterList { get; set; }
        //public int UserMasterId { get; set; }
        public string AssignedToString { get; set; }
    }
}