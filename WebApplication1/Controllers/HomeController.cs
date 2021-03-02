using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form )
        {
            var client = new RestClient("https://demo-ipg.ctdev.comtrust.ae:2443/");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n\"Registration\": {\r\n \"Currency\": \"AED\",\r\n \"ReturnPath\": \"https://localhost/callbackURL\",\r\n \"TransactionHint\": \"CPT:Y;VCC:Y;\",\r\n \"OrderID\": \"7210055701315345\",\r\n \"Store\": \"0000\",\r\n \"Terminal\": \"0000\",\r\n \"Channel\": \"Web\",\r\n \"Amount\": \"2.00\",\r\n \"Customer\": \"Demo Merchant\",\r\n \"OrderName\": \"Paybill\",\r\n \"UserName\":\"Demo_fY9c\",\r\n \"Password\":\"Comtrust@20182018\"\r\n }\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            var reg = JsonConvert.DeserializeObject<PaymentRegistration.Root>(response.Content);
            if (reg.Transaction.ResponseCode != "0")
            {
                ViewBag.error = reg.Transaction.ResponseDescription;
                ViewBag.Success = string.Empty;
                return View();
            }


            client = new RestClient("https://demo-ipg.ctdev.comtrust.ae:2443");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n \"Finalization\": {\r\n \"TransactionID\": \"" + reg.Transaction.TransactionID + "\",\r\n \"Customer\": \"Demo Merchant\",\r\n \"UserName\":\"Demo_fY9c\",\r\n \"Password\":\"Comtrust@20182018\"\r\n }\r\n}", ParameterType.RequestBody);
            response = client.Execute(request);
            Console.WriteLine(response.Content);

            client = new RestClient("https://demo-ipg.ctdev.comtrust.ae:2443/");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n \"Authorization\": {\r\n\"Customer\": \"Demo Merchant\",\r\n\"Language\": \"en\",\r\n\"Currency\": \"AED\",\r\n \"OrderName\": \"Pinger-NBAD\",\r\n\"OrderID\": \"990000227113719\",\r\n \"Channel\": \"W\",\r\n\"Amount\": \"100\",\r\n\"TransactionHint\": \"CPT:Y;\",\r\n\"CardNumber\": \""+form["cardNumber"]+"\",\r\n\"ExpiryMonth\": \""+ form["expiryMonth"] + "\",\r\n \"ExpiryYear\": \""+ form["expiryYear"] + "\",\r\n \"VerifyCode\": \""+ form["cvCode"] + "\",\r\n\"UserName\":\"Demo_fY9c\",\r\n \"Password\":\"Comtrust@20182018\"\r\n }\r\n}\r\n", ParameterType.RequestBody);
            response = client.Execute(request);
            Console.WriteLine(response.Content);
            var Authorise = JsonConvert.DeserializeObject<Authorisation.Root>(response.Content);
            if (Authorise.Transaction.ResponseCode != "0")
            {
                ViewBag.error = Authorise.Transaction.ResponseDescription;
                ViewBag.Success = string.Empty;
                return View();
            }
            else
            {
                ViewBag.Success = Authorise.Transaction.ResponseDescription;
                ViewBag.error = string.Empty;
            }
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}