﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoGiaDung.Models;

namespace WebsiteBanDoGiaDung.Controllers
{
    public class AccountController : Controller
    {
        private WebsiteBanDoGiaDungDbContext db = new WebsiteBanDoGiaDungDbContext();
        //public AccountController()
        //{
        //    if (System.Web.HttpContext.Current.Session["User_Name"] == null)
        //    {
        //        System.Web.HttpContext.Current.Response.Redirect("~/");
        //    }
        //}

        [HttpPost]
        public JsonResult UserLogin(String User, String Password)
        {
            int count_username = db.Users.Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User ) && m.Access == 0).Count();
            if (count_username == 0)
            {

                return Json(new { s = 1 });
            }
            else
            {
                Password = XString.ToMD5(Password);
                //Password = Password;
                var user_acount = db.Users.Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User) && m.Password == Password);
                if (user_acount.Count() == 0)
                {
                    return Json(new { s = 2 });
                }
                else
                {
                    var user = user_acount.First();
                    Session["User_Name"] = user.Fullname;
                    Session["User_ID"] = user.ID;
                }
            }
            return Json(new { s = 0 });
        }

        public ActionResult UserLogout(String url)
        {
            Session["User_Name"] = null;
            Session["User_ID"] = null;
            return Redirect("~/" + url);
        }
        public ActionResult ProFile()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult Notification()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult Order()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
                {
                       System.Web.HttpContext.Current.Response.Redirect("~/");
                }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var list = db.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.CreateDate).ToList();
            ViewBag.itemOrder = db.Orderdetails.ToList();
            ViewBag.productOrder = db.Products.ToList();
            return View(list);
        }
        public ActionResult ActionOrder()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            var list = db.Orders.ToList();
            return View("_ActionOrder", list);
        }
        public ActionResult OrderDetails(int id)
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var checkO = db.Orders.Where(m => m.UserID == userid && m.ID == id);
            if (checkO.Count() == 0)
            {
                return this.NotFound();
            }

            var id_order = db.Orders.Where(m => m.UserID == userid && m.ID == id).FirstOrDefault();
            ViewBag.Order = id_order;
            var itemOrder = db.Orderdetails.Where(m => m.OrderID == id_order.ID).ToList();
            ViewBag.productOrder = db.Products.ToList();
            return View(itemOrder);
        }
        public ActionResult NotFound()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        [HttpPost]
        public JsonResult Register(MUser user)
        {
            try
            {
                var checkPM = db.Users.Any(m => m.Phone == user.Phone || m.Email.ToLower().Equals(user.Email.ToLower()));
                if (checkPM)
                {
                    return Json(new { Code = 1, Message = "Số điện thoại hoặc Email đã được sử dụng." });
                }
                user.Gender = 1;
                user.Image = "";
                user.Access = 0;
                user.Status = 1;
                user.Password = XString.ToMD5(user.Password);
                user.Created_at = DateTime.Now;
                user.Created_by = 1;
                user.Updated_at = DateTime.Now;
                user.Updated_by = 1;

                db.Users.Add(user);
                db.SaveChanges();

                return Json(new { Code = 0, Message = "Đăng ký thành công!" });
            }
            catch (Exception e)
            {
                return Json(new { Code = 1, Message = "Đăng ký thất bại!" });
                throw e;
            }
        }
    }
}