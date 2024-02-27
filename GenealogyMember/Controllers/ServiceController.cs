using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Security;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Net;
using System.IO;
using FamilyMember.Models;

namespace FamilyMember.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Services
        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpPost]
        public void SubmitPayment(FormCollection form)
        {

            //string firstName = model.FirstName;
            //string amount = model.Amount;
            //string productInfo = model.ProductInformation;
            //string email = model.Email;
            //string phone = model.PhoneNumber;
            //string surl = model.SuccessURL;
            //string furl = model.FailureURL;

            string firstName = form["FirstName"].ToString();
            string amount = form["Amount"].ToString();
            string productInfo = form["ProductInformation"].ToString();
            string email = form["Email"].ToString();
            string phone = form["PhoneNumber"].ToString();
            string surl = form["SuccessURL"].ToString();
            string furl = form["FailureURL"].ToString();


            RemotePost myremotepost = new RemotePost();
            string key = "PXpCFiGP";
            string salt = "tSk4WBzzOD";

            //posting all the parameters required for integration.

            myremotepost.Url = "https://secure.payu.in/_payment";
            myremotepost.Add("key", "PXpCFiGP");
            string txnid = Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", amount);
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            //myremotepost.Add("surl", "http://thefamilymember.com/Return/Return");
            //myremotepost.Add("furl", "http://thefamilymember.com/Return/Return");
            myremotepost.Add("surl", "http://localhost:3094/Return/Return");//Change the success url here depending upon the port number of your local system.
            myremotepost.Add("furl", "http://localhost:3094/Return/Return");//Change the failure url here depending upon the port number of your local system.


            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;
            //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);

            myremotepost.Post();

        }

        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();


            public string Url = "";
            public string Method = "post";
            public string FormName = "myForm1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
        }

        //Hash generation Algorithm

        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }


        public string Generatetxnid()
        {

            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);

            return txnid1;
        }

    }
}