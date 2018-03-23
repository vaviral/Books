using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleLogin.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

namespace GoogleLogin.Controllers
{
    public class AccountController : Controller
    {
        private string FilePath = @"c:\users\hp\documents\visual studio 2015\Projects\GoogleLogin\GoogleLogin\App_Data\UserRegistrations.txt";
        public AccountController()
        {
        }
        private List<T> Read<T>()
        {
            try
            {
                var serializer = new JsonSerializer();
                List<T> List = new List<T>();
                using (var file = System.IO.File.OpenText(FilePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        List = serializer.Deserialize<List<T>>(reader);
                    }
                }
                return List;
            }
            catch
            {
                throw;
            }
        }
        private int Write<T>(List<T> List)
        {
            try
            {
                var serializer = new JsonSerializer();
                using (StreamWriter file = System.IO.File.CreateText(FilePath))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        serializer.Serialize(writer, List);
                    }
                }
                return 1;
            }
            catch
            {
                throw;
            }
        }
        private bool IsRegistered(List<RegisterViewModel> List,string Email)
        {
            try
            {
                if (List.Exists(e => e.Email == Email))
                    return true;
                else return false;
            }
            catch
            {
                throw;
            }
        }
        private int RegisterNewUser(List<RegisterViewModel> list, RegisterViewModel model)
        {
            try
            {
                if (list == null)
                    list = new List<RegisterViewModel>();
                else
                    list.Add(model);
                list.Add(model);
                return Write(list);
            }
            catch
            {
                throw;
            }
        }
        private bool IsLoginValid(List<RegisterViewModel> List, string Email, string Password)
        {
            try
            {
                if (List != null)
                {
                    if (List.Exists(e => e.Email.Equals(Email) && e.Password.Equals(Password)))
                        return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }
        private Result GenerateResponse(int Output)
        {
            Result response = new Result();
            switch (Output)
            {
                case 0:
                    response.Status = 0;
                    response.Message = "Failed";
                    break;
                case 1:
                    response.Status = 1;
                    response.Message = "Succeeded";
                    break;
            }
            return response;
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            try
            {
                List<RegisterViewModel> List = Read<RegisterViewModel>();
                Result response = new Result();
                if (IsRegistered(List, Email))
                {
                    if (IsLoginValid(List, Email, Password))
                    {
                        Session["Username"] = Email;
                        response.Status = 1;
                        response.Message = "Succeeded";
                        return Json(response);
                    }
                }
                response.Status = 0;
                response.Message = "Failed";
                response.ErrorMessage = "Wrong Credentials";
                return Json(response);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            try
            {
                List<RegisterViewModel> List = Read<RegisterViewModel>();
                if (List == null)
                {
                    return Json(GenerateResponse(RegisterNewUser(null, model)));
                }
                else if (!IsRegistered(List, model.Email))
                {
                    return Json(GenerateResponse(RegisterNewUser(List, model)));
                }
                Result response = new Result();
                response.Status = 0;
                response.Message = "Failed";
                response.ErrorMessage = "Already Registered";
                return Json(response);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                Session["Username"] = loginInfo.Email;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}