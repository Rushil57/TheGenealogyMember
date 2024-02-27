using FamilyMember.Entities;
using FamilyMember.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FamilyMember.Controllers
{
    public class MemberController : Controller
    {
        private FamilyMemberEntities db = new FamilyMemberEntities();
        // GET: Member
        public ActionResult Index()
        {
            return PartialView();
        }
     
        public async Task<ActionResult> GetMember(int id)
        {
            //return View();
            var data = await db.Members.FindAsync(id);

            var member = new MemberModels
            {
                MemberId = data.MemberId,
                TheFamilyMemberId = data.TheFamilyMemberId,              
                Name = data.Name,
                DOB = data.DOB.ToString("dd/MM/yyyy"),
                BloodGroup = data.BloodGroup,
                Address = data.Address,
                EmergencyContactName_No = data.EmergencyContactName_No,
                MedicalAllergy = data.MedicalAllergy,            
                QRImage = "../Content/images/QR Codes/QRImage_" + id + ".jpg"
            };

            return View("ViewFamilyMember", member);
        }
    }
}