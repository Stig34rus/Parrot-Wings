using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PW.Models;

namespace PW.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private ApplicationUserManager _userManager;
        private Service.WalletService _walletService;
        private string walletOwner;

        public WalletController()
        {
            _walletService = new Service.WalletService();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index()
        {
            walletOwner = HttpContext.User.Identity.GetUserId();
            Wallet wallet = _walletService.GetWalletInfo(walletOwner);
            ViewBag.Wallet = wallet;
            List<WalletHistory> history = _walletService.GetHistory(walletOwner);
            ViewBag.History = history;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Transaction model)
        {
            List<KeyValuePair<string, string>> ErrorList = new List<KeyValuePair<string, string>>();
            walletOwner = HttpContext.User.Identity.GetUserId();
            model.From = walletOwner;
            model.To = _walletService.GetIdByEmail(model.To);
            model.When = DateTime.Now.ToString();
            _walletService.CreateTransaction(model, ref ErrorList);
            Wallet wallet = _walletService.GetWalletInfo(walletOwner);
            ViewBag.Wallet = wallet;
            List<WalletHistory> history = _walletService.GetHistory(walletOwner);
            ViewBag.History = history;
            foreach (var Error in ErrorList)
                ModelState.AddModelError(Error.Key, Error.Value);
            return View(model);
        }
        [HttpPost]
        public string GetWallets(string val)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = _walletService.GetWalletsList(val);
            
            string json = serializer.Serialize(result);
            return json;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _walletService != null)
            {
                _walletService.Dispose();
                _walletService = null;
            }

            base.Dispose(disposing);
        }

#region Вспомогательные приложения
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        

#endregion
    }
}