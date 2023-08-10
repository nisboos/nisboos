using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using project.Models;
using System.Data.Entity;
using System.IO;
using System.Web.Razor.Generator;
using project.Models.ViewModel;
using Newtonsoft.Json;
using System.Data.Entity.SqlServer;



namespace project.Controllers
{
    public class HomeController : Controller
    {
        projectEntities db = new projectEntities();
        public ActionResult Index()
        {
            var products = db.tProduct.ToList();
            if (Session["Admin"] == null)
            {
                if (Session["Member"] == null)
                {
                    return View("Index", "_Layout", products);
                }
                return View("Index", "_LayoutMember", products);
                
            }
            else 
            {
                return RedirectToAction("Index","Admin");
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            var member = db.tMember
                .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                .FirstOrDefault();
            
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤, 登入失敗";
                return View();
            }
            if (!member.IsVerified)
            {
                ViewBag.Message = "您的帳號尚未驗證，請先驗證後再進行登入";
                return View();
            }
            if (fUserId=="admin" && fPwd=="password")
            {                
                Session["Admin"] = member;                
            }
            Session["Welcome"] = member.fName + "歡迎光臨";
            Session["Member"] = member;
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(tMember member)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            // 對 fEmail 進行驗證
            if (!IsValidEmail(member.fEmail))
            {
                ModelState.AddModelError("fEmail", "請輸入有效的電子郵件地址");
                return View();
            }

            var pmember = db.tMember
                .Where(m => m.fUserId == member.fUserId)
                .FirstOrDefault();
            var existingEmailMember = db.tMember
                .FirstOrDefault(m => m.fEmail == member.fEmail);
            if (pmember != null)
            {
                ViewBag.Message = member.fUserId + "已有人使用，請重新註冊";
                return View();
            }
            if (existingEmailMember != null)
            {
                ViewBag.Message = "該電子郵件地址已被使用，請使用其他電子郵件地址";
                return View();
            }
            if (pmember == null)
            {
                // 生成驗證碼或令牌
                string verificationCode = GenerateVerificationCode();

                // 將驗證碼與註冊資料一起存儲在資料庫中
                member.VerificationCode = verificationCode;                
                try
                {
                    SendVerificationEmail(member.fEmail, verificationCode);
                    TempData["SuccessMessage"] = "驗證信已成功寄出，請查收您的郵件並完成驗證。";                
                }
                catch
                {
                    ViewBag.Message = "郵件發送失敗，請確認您輸入的電子郵件地址是否正確。";
                    return View();
                }
                db.tMember.Add(member);
                db.SaveChanges();
            }
            var products = db.tProduct.ToList();
            return View("Index", "_Layout", products);           
        }
        public ActionResult VerifyEmail(string code)
        {
                // 根據驗證碼進行處理
                var member = db.tMember.FirstOrDefault(m => m.VerificationCode == code);
                if (member != null)
                {
                    member.IsVerified = true;
                    db.SaveChanges();
                    return RedirectToAction("RegistrationSuccess");
                }
                // 驗證失敗，導向相應的處理邏輯或視圖
                return RedirectToAction("VerificationFailed");
        }
        public ActionResult RegistrationSuccess()//驗證成功(註冊成功)
        {
            return View();
        }
        public ActionResult VerificationFailed()//驗證失敗
        {
            ViewBag.Message = "驗證失敗！無效的驗證碼，可能為EMAIL輸入錯誤，請重新註冊。";
            return View();
        }

        private void SendVerificationEmail(string email, string verificationCode)//寄送驗證信
        {

            string subject = "註冊驗證郵件";
            string body = $"請點擊以下連結完成註冊驗證：<a href='{Url.Action("VerifyEmail", "Home", new { code = verificationCode }, Request.Url.Scheme)}'>點擊這裡</a>";

            // 使用您的電子郵件服務發送郵件
            // 這裡是使用 SmtpClient 的例子
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("A9226616@ulive.pccu.edu.tw", "asdfzxcv40202");
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("A9226616@ulive.pccu.edu.tw");
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
            
        }

        private string GenerateVerificationCode()//生成驗證碼
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return verificationCode;
        }
        private bool IsValidEmail(string email)//驗證輸入的EMAIL格式是否正確
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            // 根據電子郵件地址查找相應的用戶
            var user = db.tMember.FirstOrDefault(m => m.fEmail == email);
            if (user != null)
            {
                // 生成重設密碼的令牌
                string resetToken = GenerateVerificationCode();

                // 將重設密碼的令牌與用戶關聯並存儲在資料庫中
                user.VerificationCode = resetToken;
                db.SaveChanges();

                // 發送重設密碼的郵件
                SendPasswordResetEmail(user.fEmail, resetToken);

                // 重定向到密碼重設成功的頁面
                return RedirectToAction("PasswordResetEmailSent");
            }

            // 如果用戶不存在，顯示錯誤消息
            ViewBag.Message = "此用戶不存在";
            return View();
        }
        public ActionResult PasswordResetEmailSent()//寄出更改密碼的畫面
        {
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            // 根據重設密碼的令牌查找相應的用戶
            var user = db.tMember.FirstOrDefault(m => m.VerificationCode == token);
            if (user != null)
            {
                // 將令牌傳遞到視圖中，以便在提交表單時進行驗證
                ViewBag.Token = token;
                return View();
            }

            // 如果令牌無效，重定向到密碼重設失敗的頁面
            return RedirectToAction("PasswordResetFailed");
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, string fPwd, string confirmPwd)
        {
            // 根据重设密码的令牌查找相应的用户
            var member = db.tMember.FirstOrDefault(m => m.VerificationCode == token);

            if (member != null  )
            {
                if(fPwd == confirmPwd)
                { 
                // 更新用户的密码
                member.fPwd = fPwd;
                member.VerificationCode = null; // 清除重设密码的令牌
                db.SaveChanges();
                return View("PasswordResetSuccess");
                }
                else
                {
                    ViewBag.Message = "輸入的密碼不相符";
                    return View();
                }
            }
            else 
            { 
            // 如果令牌无效或密码不匹配，重定向到密码重设失败的页面
            return View("PasswordResetFailed");
            }
        }

        public ActionResult PasswordResetSuccess()
        {
            ViewBag.Message = "變更密碼成功";
            return View();
        }

        public ActionResult PasswordResetFailed()
        {
            ViewBag.Message = "變更密碼失敗";
            return View();
        }

        private void SendPasswordResetEmail(string email, string token)//寄送驗證信
        {
            string subject = "變更密碼郵件";
            string body = $"請點擊以下連結完成變更密碼：<a href='{Url.Action("ResetPassword", "Home", new { token = token }, Request.Url.Scheme)}'>點擊這裡</a>";

            // 使用您的電子郵件服務發送郵件
            // 這裡是使用 SmtpClient 的例子
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("A9226616@ulive.pccu.edu.tw", "asdfzxcv40202");
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("A9226616@ulive.pccu.edu.tw");
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

        }
        public ActionResult ShoppingCar()
        {
            Session["OrderGuid"] = Guid.NewGuid().ToString();
            string fuserid = (Session["Member"] as tMember).fUserId;
            //var shoppingCar = db.tShoppingCar
            //    .Where(s => s.fUserId == fuserid && s.fIsApproved == "否")
            //    //.Join(db.tProduct, p => p.fPId, s.fPId, (p, s) => )
            //    .ToList();

            var shoppingCar = (from s in db.tShoppingCar
                               where s.fUserId == fuserid && s.fIsApproved == "否"
                               join p in db.tProduct on s.fPid equals p.fPId
                               select new ShoppingCarItem
                               {
                                   UserId = fuserid,
                                   Fid = s.fid,
                                   Pid = s.fPid,
                                   Name = p.fName,
                                   Qty = s.fQty,
                                   Price = p.fPrice,
                                   IsApproved = s.fIsApproved
                               }).ToList();
            return View("ShoppingCar", "_LayoutMember", shoppingCar);
        }
        public ActionResult AddCar(string fPId)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            //商業邏輯層
            var currentCar = db.tShoppingCar
                .Where(p => p.fPid == fPId && p.fIsApproved == "否" && p.fUserId == fUserId)
                .FirstOrDefault();
            if (currentCar == null)
            {
                var product = db.tProduct
                    .Where(p => p.fPId == fPId)
                    .FirstOrDefault();
                tShoppingCar shoppingCar = new tShoppingCar();
                shoppingCar.fUserId = fUserId;
                //TODO 刪除tShoppingCar欄位
                shoppingCar.fPid = fPId;
                shoppingCar.fQty = 1;
                shoppingCar.fIsApproved = "否";
                db.tShoppingCar.Add(shoppingCar);
            }
            else
            {
                currentCar.fQty++;
            }
            db.SaveChanges();
            //
            return RedirectToAction("ShoppingCar");
        }
        [HttpPost]
        public ActionResult ShoppingCar(FormCollection formCollection, string fReceiver, string fEMail, string fAddress)
        {
            // 获取用户勾选的商品数量
            string quantityJson = formCollection["quantity"];
            Dictionary<string, int> itemQuantities = JsonConvert.DeserializeObject<Dictionary<string, int>>(quantityJson);

            // 获取当前用户的ID
            string fUserId = ((tMember)Session["Member"]).fUserId;

            // 创建订单
            string guid = Session["OrderGuid"].ToString();
            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEMail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            order.fPaid = "未付款";

            // 保存订单
            db.tOrder.Add(order);

            // 遍历itemQuantities字典，处理每个商品
            foreach (var item in itemQuantities)
            {
                string fPId = item.Key;
                int quantity = item.Value;

                // 根据商品ID找到对应的商品
                var product = db.tProduct.FirstOrDefault(p => p.fPId == fPId);

                if (product != null)
                {
                    // 假设tOrderList是您要新增的数据表
                    tOrderDetail orderDetail = new tOrderDetail();
                    orderDetail.fOrderGuid = guid;
                    orderDetail.fUserId = fUserId;
                    orderDetail.fPId = fPId;
                    orderDetail.fName = product.fName;
                    orderDetail.fPrice = product.fPrice;
                    orderDetail.fQty = quantity;
                    orderDetail.fIsApproved = "是";
                    // 保存订单详情
                    db.tOrderDetail.Add(orderDetail);
                    // 找到对应的购物车记录并删除
                    var shoppingCarItem = db.tShoppingCar.FirstOrDefault(p => p.fPid == fPId && p.fIsApproved == "否" && p.fUserId == fUserId);
                    if (shoppingCarItem != null)
                    {
                        db.tShoppingCar.Remove(shoppingCarItem);
                    }
                }
            }
            // 提交数据库更改
            db.SaveChanges();
            return RedirectToAction("Payment");

        }

        public ActionResult OrderList(string searchOrder)
        {     
                      
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var orderList = db.tOrder
                .Where(m => m.fUserId == fUserId)
                .OrderByDescending(m => m.fDate)
                .ToList();
            if (!string.IsNullOrEmpty(searchOrder))
            {
                // 按空格分隔搜索条件
                var searchTerms = searchOrder.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


                foreach (var term in searchTerms)
                {
                    int parsedInt;
                    bool isInt = int.TryParse(term, out parsedInt);

                    orderList = orderList.Where(c =>
                        c.fPaid.Contains(term) ||
                        (isInt && c.fId == parsedInt)
                    ).ToList();

                }

            }
            return View("OrderList", "_LayoutMember", orderList);
        }
        public ActionResult Payment()
        {

            return View("Payment", "_LayoutMember");
        }
        [HttpPost]
        public ActionResult ProcessPayment(int orderId)
        {
            var order = db.tOrder.Find(orderId);

            if (order != null)
            {
                // 更新訂單狀態為已完成付款
                order.fPaid = "銀行付款審核中";
                // 生成西元年月日部分
                string currentDatePart = DateTime.Now.ToString("yyyyMMdd");

                // 生成11位隨機數字部分
                Random random = new Random();
                string randomDigits = random.Next(100000000, 999999999).ToString();
                string random11 = random.Next(10, 99).ToString();

                // 組合成最終的交易ID
                order.fTrancsationId = currentDatePart + randomDigits + random11;
                // 提交更改到資料庫
                db.SaveChanges();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }

        public ActionResult bank()
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var order = db.tOrder
         .Where(m => m.fUserId == fUserId && m.fPaid == "未付款")
         .OrderByDescending(m => m.fDate)
         .ToList();
            ViewBag.OrderList = order;
            return View("bank", "_LayoutMember");
        }
        public ActionResult Delete(int fId)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var deletecar = db.tOrder
                .Where(p => p.fId == fId && p.fUserId == fUserId)
                .FirstOrDefault();
            if (deletecar != null)
            {
                db.tOrder.Remove(deletecar);
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }
        public ActionResult Deletecar(int fId)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var deletecar = db.tShoppingCar
                .Where(p => p.fid == fId && p.fUserId == fUserId)
                .FirstOrDefault();
            if (deletecar != null)
            {
                db.tShoppingCar.Remove(deletecar);
            }
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }


        [HttpGet]
            public ActionResult Membercenter(bool edit = false)
            {
                // 获取当前登录用户的会员信息
                tMember member = (tMember)Session["Member"];
                if (member == null)
                {
                    return RedirectToAction("Login"); // 如果用户未登录，重定向到登录页面
                }
            if (edit)
            {
                ViewBag.EditMode = true;
                return View(member);
            }

            ViewBag.EditMode = false;
            var updatedMemberData = TempData["UpdatedMemberData"] as tMember; // 从TempData中获取更新后的会员信息
            if (updatedMemberData != null)
            {
                return View("Membercenter",  updatedMemberData);
            }
            return View(member); // 将会员信息传递给编辑视图
            }

            [HttpPost]
            public ActionResult Membercenter(tMember updatedMember)
            {
                if (ModelState.IsValid)
                {
                    string fUserId = updatedMember.fUserId;
                    var member = db.tMember.FirstOrDefault(m => m.fUserId == fUserId);
                    if (member != null)
                    {
                        member.fPwd = updatedMember.fPwd;
                        member.fName = updatedMember.fName;
                        member.fEmail = updatedMember.fEmail;
                        member.fBirthday = updatedMember.fBirthday;
                        member.fClass = updatedMember.fClass;
                        db.SaveChanges();
                        ViewBag.Message = "保存成功";
                        Session["Member"] = member;

                    var updatedMemberData = db.tMember.FirstOrDefault(m => m.fUserId == fUserId);

                    // 使用TempData传递更新后的会员信息
                    TempData["UpdatedMemberData"] = updatedMemberData;
                    Session["Welcome"] = updatedMemberData.fName + "歡迎光臨";
                    // 重定向到Membercenter动作方法，并传递edit参数为false
                    return RedirectToAction("Membercenter", new { edit = false });
                }
                    else
                    {
                        ViewBag.Message = "找不到对应的成员对象";
                    }
                }
                return View(updatedMember);
            }

      

        [HttpGet]
        public ActionResult Coachcenter(bool edit = false)
        {
            // 获取当前登录用户的会员信息
            tMember member = (tMember)Session["Member"];
            if (member == null)
            {
                return RedirectToAction("Login"); // 如果用户未登录，重定向到登录页面
            }           
            tCoach coach = db.tCoach.FirstOrDefault(c => c.fUserId ==member.fUserId);
            if (coach == null)
            {
                // 若教練資料不存在，您可以視情況進行相應處理，例如建立新的教練資料
                coach = new tCoach
                {                  
                    fUserId = member.fUserId,
                    // ... 設定其他教練資料的初始值 ...
                };

                // 將新建立的教練資料保存至資料庫
                db.tCoach.Add(coach);
                db.SaveChanges();
            }
            if (edit)
            {
                ViewBag.EditMode = true;
                return View(coach);
            }

            ViewBag.EditMode = false;
            var updatedCoachData = TempData["updatedCoachData"] as tCoach; // 从TempData中获取更新后的会员信息
            if (updatedCoachData != null)
            {
                return View("Coachcenter", updatedCoachData);
            }
            return View(coach); // 将会员信息传递给编辑视图
        }
        private string GetUniqueFileName(string fileName)
        {
            // 假設您使用的是Guid（全球唯一識別碼）來生成唯一的名稱
            string guid = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(fileName);
            return $"{guid}{extension}";
        }
        [HttpPost]
        public ActionResult Coachcenter(tCoach updatedCoach, HttpPostedFileBase profileImage)
        {
            if (ModelState.IsValid)
            {
                string fUserId = updatedCoach.fUserId;
                var coach = db.tCoach.FirstOrDefault(m => m.fUserId == fUserId);

                if (coach != null)
                {                    
                    coach.cName = updatedCoach.cName;
                    coach.cGender= updatedCoach.cGender;
                    coach.cStar= updatedCoach.cStar;
                    coach.cInterest= updatedCoach.cInterest;
                    coach.cItem= updatedCoach.cItem;
                    coach.cYears= updatedCoach.cYears;
                    coach.cLineid = updatedCoach.cLineid;
                    coach.cPhone= updatedCoach.cPhone;
                    coach.cEmail = updatedCoach.cEmail;

                    if (profileImage != null && profileImage.ContentLength > 0)
                    {
                        // 獲取檔案名稱
                        string fileName = Path.GetFileName(profileImage.FileName);
                        
                        // 生成一個唯一的新檔案名稱，避免覆蓋
                        string uniqueFileName = GetUniqueFileName(fileName);

                        // 設置圖片儲存路徑（您可以根據需求自行修改）
                        string imagePath = Path.Combine(Server.MapPath("~/Uploads/"), uniqueFileName);


                        // 儲存圖片到伺服器
                        profileImage.SaveAs(imagePath);

                        // 將圖片路徑保存到資料庫中
                        coach.cImg = "~/Uploads/" + uniqueFileName;

                    }
                    //coach.cImg= updatedCoach.cImg;
                    db.SaveChanges();
                    ViewBag.Message = "保存成功";
                    Session["Coach"] = coach;

                    var updatedCoachData = db.tCoach.FirstOrDefault(m => m.fUserId == fUserId);

                    // 使用TempData传递更新后的会员信息
                    TempData["UpdatedMemberData"] = updatedCoachData;
                    Session["Welcome"] = updatedCoachData.cName + "歡迎光臨";
                    // 重定向到Membercenter动作方法，并传递edit参数为false
                    return RedirectToAction("Coachcenter", new { edit = false });
                }
                else
                {
                    ViewBag.Message = "找不到对应的成员对象";
                }

            }
            return View(updatedCoach);
        }
        // ...
        public ActionResult Coachmanage()
        {
            string fuserid = (Session["Member"] as tMember).fUserId;
            var orderDetail = db.tOrderDetail
                .Where(p => p.fUserId == fuserid && p.fIsApproved == "否")
                .ToList();
            return View("Coachmanage", "_LayoutMember", orderDetail);
        }

        public ActionResult Querycoach(string searchString)
        {
            // 获取所有教练
            var coaches = db.tCoach.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                // 按空格分隔搜索条件
                var searchTerms = searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var term in searchTerms)
                {
                    // 按照每个搜索条件进行筛选
                    coaches = coaches.Where(c =>
                        c.cItem.Contains(term) ||
                        c.cId.ToString().Contains(term) ||
                        c.cName.Contains(term) ||
                        c.cGender.Contains(term)
                    ).ToList();
                }
            }

            if (Session["Member"] == null)
            {
                return View("Querycoach", "_Layout", coaches);
            }
            return View("Querycoach", "_LayoutMember", coaches);
        }
        public ActionResult Contactcoach()
        {
            var Contactcoach = db.tCoach.ToList();
            if (Session["Member"] == null)
            {
                return View("Contactcoach", "_Layout", Contactcoach);
            }
            return View("Contactcoach", "_LayoutMember", Contactcoach);
        }
        public ActionResult Bookingclass()
        {
            var products = db.tProduct.ToList();
            if (Session["Member"] == null)
            {
                return View("Bookingclass", "_Layout", products);
            }
            return View("Bookingclass", "_LayoutMember", products);
        }
        public ActionResult Myfavourite()
        {
            string fUserId = (Session["Member"] as tMember).fUserId;
            var Myfavourite = db.tmyf
                .Where(m => m.cUserId == fUserId)
                .ToList();
            return View("Myfavourite", "_LayoutMember", Myfavourite);
        }
        public ActionResult AddMyfavourite(int cId, string cImg, string cName, string cStar, string cGender, string cInterest, string cItem, string cYears)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var Mylove = db.tmyf
                .Where(p => p.cId == cId && p.cUserId == fUserId)
                .FirstOrDefault();
            if (Mylove == null)
            {
                var coach = db.tCoach
                    .Where(p => p.cId == cId)
                    .FirstOrDefault();
                tmyf myf = new tmyf();
                myf.cId = cId;
                myf.cImg = cImg;
                myf.cName = cName;
                myf.cStar = cStar;
                myf.cGender = cGender;
                myf.cInterest = cInterest;
                myf.cItem = cItem;
                myf.cYears = cYears;
                myf.cUserId = fUserId;  // 添加這一行以設定 cUserId
                db.tmyf.Add(myf);
            }
            db.SaveChanges();  // 將 SaveChanges 移至這裡以確保無論 Mylove 是否為 null，都會儲存更改

            return RedirectToAction("Myfavourite");
        }
        public ActionResult DeleteMyf(int cId)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            var Mylove = db.tmyf
                .Where(p => p.cId == cId && p.cUserId == fUserId)
                .FirstOrDefault();
            if (Mylove != null)
            {
                db.tmyf.Remove(Mylove);
            }
            db.SaveChanges();
            return RedirectToAction("Myfavourite");
        }
       
        public ActionResult BookCoach(int cId)
        {
            string fUserId = (Session["Member"] as tMember).fUserId;
            var coach = db.tmyf
                .Where(p => p.cId == cId && p.cUserId == fUserId)
                .ToList();
            return View("BookCoach", "_LayoutMember", coach);
        }
        [HttpPost]
        public ActionResult AddBookCoach(int cId, string cName, DateTime cDate, TimeSpan cTime)
        {
            string fUserId = ((tMember)Session["Member"]).fUserId;
            BookCoach BkC = new BookCoach();
            BkC.fUserId = fUserId;
            BkC.cId = cId;
            BkC.cName = cName;
            BkC.cDate = cDate;
            BkC.cTime = cTime;
            var BookCList = db.tmyf
                .Where(p => p.cUserId == fUserId)
                .ToList();
            db.BookCoach.Add(BkC);
            db.SaveChanges();
            return RedirectToAction("BookCoachList");
        }

        public ActionResult BookCoachList()
        {
            string fUserId = (Session["Member"] as tMember).fUserId;
            var BookCoachList = db.BookCoach
                .Where(m => m.fUserId == fUserId)
                .OrderByDescending(p => p.cDate)
                .ToList();
            return View("BookCoachList", "_LayoutMember", BookCoachList);
        }


    }
}