using BarCode.Models;
using FamilyMember.Entities;
using FamilyMember.Models;
using FamilyMember.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using Newtonsoft.Json;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using ZXing.Common;
using System.Security.Cryptography;

namespace FamilyMember.ApiControllers
{
    public class MemberController : ApiController
    {
        private FamilyMemberEntities db = new FamilyMemberEntities();

        // GET: api/User
        public async Task<List<MemberModels>> GetMembers()
        {

            var result = await db.Members.ToListAsync();
            var member = result.Select(a => new MemberModels
            {
                MemberId = a.MemberId,
                TheFamilyMemberId = a.TheFamilyMemberId,
                Name = a.Name,
                DOB = a.DOB.ToString("dd/MM/yyyy"),
                BloodGroup = a.BloodGroup,
                Address = a.Address,
                EmergencyContactName_No = a.EmergencyContactName_No,
                MedicalAllergy = a.MedicalAllergy,            
                IsQRCodeGenerated = a.IsQRCodeGenerated
            }).ToList();

            return member;
        }

        // GET: api/Member/5
        [ResponseType(typeof(MemberModels))]
        public async Task<IHttpActionResult> GetMember(int id)
        {
            var data = await db.Members.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            var member = new MemberModels
            {
                MemberId = data.MemberId,
                TheFamilyMemberId = data.TheFamilyMemberId,
                Name = data.Name,
                DOB = data.DOB.ToString("dd/MM/yyyy"),
                BloodGroup = data.BloodGroup,
                Address = data.Address,
                EmergencyContactName_No = data.EmergencyContactName_No,
                MedicalAllergy = data.MedicalAllergy
                          
            };

            return Ok(member);
        }

      
        // DELETE: api/Member/5
        //public async Task<IHttpActionResult> DeleteMember(int id)
        //{
        //    var member = await db.Members.FindAsync(id);
        //    member.IsDeleted = true;         

        //    db.Entry(member).State = EntityState.Modified;
        //    await db.SaveChangesAsync();

        //    //return new TextResult("hello", Request);
        //    return Ok();
        //}

        // POST: api/Member
        //[System.Web.Http.HttpPost]
        //[System.Web.Http.ActionName("SaveMember")]
        public async Task<HttpResponseMessage> SaveMember(MemberModels model)
        {
            bool result = true;
            string message = "";

            var sessionMember = (MemberModels)(HttpContext.Current.Session["Member"]);

            try
            {
                if (model.MemberId == 0)
                {                  
                        var member = new Member();
                        member.TheFamilyMemberId  = model.TheFamilyMemberId;
                        member.Name = model.Name;
                        member.DOB = Convert.ToDateTime(model.DOB);
                        member.BloodGroup = model.BloodGroup;
                        member.Address = model.Address;
                        member.EmergencyContactName_No = model.EmergencyContactName_No;
                        member.MedicalAllergy = model.MedicalAllergy;
                       
                        db.Members.Add(member);
                        message = "Data saved successfully.";                  
                }
                else
                {

                    var member = await db.Members.FindAsync(model.MemberId);
                    member.TheFamilyMemberId = model.TheFamilyMemberId;
                    member.Name = model.Name;
                    member.DOB = Convert.ToDateTime(model.DOB);
                    member.BloodGroup = model.BloodGroup;
                    member.Address = model.Address;
                    member.EmergencyContactName_No = model.EmergencyContactName_No;
                    member.MedicalAllergy = model.MedicalAllergy;
                           
                    db.Entry(member).State = EntityState.Modified;
                    message = "Data saved successfully.";

                }

                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                message = "Some error has occured.";
                result = false;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
        //[System.Web.Http.HttpGet]
        //public async Task<HttpResponseMessage> GenerateBarcode(int id)
        //{
        //    bool result = true;
        //    string message = "";
        //    barcodecs objbar = new barcodecs();
        //    var member = await db.Members.FindAsync(id);
        //    member.Barcode = objbar.generateBarcode();
        //    member.BarCodeImage = objbar.getBarcodeImage(objbar.generateBarcode(), id.ToString());


        //    db.Entry(member).State = EntityState.Modified;
        //    message = "Barcode Generated successfully.";
        //    await db.SaveChangesAsync();
        //    //context.Products.InsertOnSubmit(objprod);
        //    //context.SubmitChanges();
        //    return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        //}
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GenerateQRBarcode(int id)
        {
            //var memberData = await db.Members.FindAsync(id);
            string message = "";
            string EncryptionKey = "123456789";
            var dd = new List<DimensionDetailsModels>();
            dd.Add(new DimensionDetailsModels { HEIGHT = 100, LENGTH = 1, PIECES = 10, WIDTH = 100, WEIGHT = "1000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 101, LENGTH = 2, PIECES = 11, WIDTH = 101, WEIGHT = "2000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 102, LENGTH = 3, PIECES = 12, WIDTH = 102, WEIGHT = "3000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 103, LENGTH = 4, PIECES = 13, WIDTH = 103, WEIGHT = "4000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 104, LENGTH = 5, PIECES = 14, WIDTH = 104, WEIGHT = "5000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 105, LENGTH = 6, PIECES = 15, WIDTH = 105, WEIGHT = "6000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 106, LENGTH = 7, PIECES = 16, WIDTH = 106, WEIGHT = "7000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 107, LENGTH = 8, PIECES = 17, WIDTH = 107, WEIGHT = "8000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 108, LENGTH = 9, PIECES = 18, WIDTH = 108, WEIGHT = "9000" });
            dd.Add(new DimensionDetailsModels { HEIGHT = 109, LENGTH = 10, PIECES = 19, WIDTH = 109, WEIGHT = "10000" });

            var member = new QRCodeModels
            {
                MASTER_ID = "ABC",
                PRE_GATEIN_HOUSE_ID = "XYZ",
                DimensionDetailsList = dd
                
                //MemberId = memberData.MemberId,
                //TheFamilyMemberId = memberData.TheFamilyMemberId,
                //Name = memberData.Name,
                //DOB = memberData.DOB.ToString("dd/MM/yyyy"),
                //BloodGroup = memberData.BloodGroup,
                //Address = memberData.Address,
                //EmergencyContactName_No = memberData.EmergencyContactName_No,
                //MedicalAllergy = memberData.MedicalAllergy,
                //IsQRCodeGenerated = true,
                //QRImage = "present"


            };
            var memberJSONString = JsonConvert.SerializeObject(member);
            var writer = new BarcodeWriter();
            writer.Renderer = new BitmapRenderer();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new QrCodeEncodingOptions
            {
                Height = 300,
                Width = 300
                
            };


            writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M);
            

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(EncryptionKey));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(memberJSONString);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            Bitmap bm = writer.Write(Convert.ToBase64String(Results));
            var decrypt = DecryptString(Convert.ToBase64String(Results), EncryptionKey);
            Bitmap overlay = new Bitmap(@"E:/TheFamilyMember/FamilyMember/Content/maccs_logo.jpg");

            int deltaHeigth = bm.Height - overlay.Height;
            int deltaWidth = bm.Width - overlay.Width;

            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));


           // var result = writer.Write(memberJSONString);
            string path = HttpContext.Current.Server.MapPath("~/Content/images/QR Codes/QRImage_" + member.MASTER_ID + ".jpg");
            var barcodeBitmap = new Bitmap(bm);



            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            //memberData.IsQRCodeGenerated = true;
          //  db.Entry(memberData).State = EntityState.Modified;
            await db.SaveChangesAsync();
            message = "QR Code generated successfully.";

            return Request.CreateResponse(HttpStatusCode.OK, new { result = bm, message = message });
        }
        public string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}