using System.Web;
using System.Collections.Generic;

namespace FamilyMember.Models
{
    public class UserModels
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsVerified { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int? RoleId { get; set; }
        public string FilePath { get; set; }
        public string RoleIds { get; set; }
        public int? AgencyMasterId { get; set; }
        public string AgencyName { get; set; }
        public List<AgencyMasterModel> AgencyMasterList { get; set; }
        public string AgencyMasterIdString { get; set; }
        public int UserCategoryMasterId { get; set; }
        public string UserCategoryName { get; set; }
        public List<UserCategoryMasterModel> UserCategoryMasterList { get; set; }
        public string UserCategoryMasterIdString { get; set; }
       

    }
}