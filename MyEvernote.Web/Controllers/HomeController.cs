using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLAyer;
using MyEvernote.BusinessLAyer.Result;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjets;
using MyEvernote.Web.Filters;
using MyEvernote.Web.Models;
using MyEvernote.Web.ViewModels;


namespace MyEvernote.Web.Controllers
{
    [Ex]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private EverNoteUserManager everNoteUserManager = new EverNoteUserManager();

        // GET: Home
        public ActionResult Index()
        {
            //categoryController üzerinden gelen view data
            //if (TempData["mm"] != null)
            //{
            //    return View(TempData["mm"] as List<Note>);
            //}

            return View(noteManager.ListQueryable().Where(x=>x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(nm.GetAllNoteQueryable().OrderByDescending(x => x.ModifiedOn).ToList()); //sql den sıralama yaptırmak
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Category cat = categoryManager.Find(x=>x.Id==id.Value);

            //if (cat == null)
            //{
            //    return HttpNotFound();
            //}

            //List<Note> notes = cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList();

            List <Note> notes = noteManager.ListQueryable().Where(x => x.IsDraft == false && x.CategoryId == id)
                .OrderByDescending(x => x.ModifiedOn).ToList();

            return View("Index",notes /*cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList()*/);
        }

        public ActionResult MostLiked()
        {
            NoteManager nm = new NoteManager();

            return View("Index", nm.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            //EvernoteUser currenUser = Session["login"] as EvernoteUser;

            
            BusinessLayerResult<EvernoteUser> res = everNoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Erros.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Erros
                };
                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {

            //EvernoteUser currenUser = Session["login"] as EvernoteUser;

            BusinessLayerResult<EvernoteUser> res = everNoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Erros.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Erros
                };
                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
          
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername"); // kontrolden kaldırıyor

            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" ||
                                             ProfileImage.ContentType == "image/jpg" ||
                                             ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfileImageFilename = filename;
                }

                BusinessLayerResult<EvernoteUser> res = everNoteUserManager.UpdateProfile(model);

                if (res.Erros.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Erros,
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "/Home/EditProfile"
                    };
                    return View("Error", errorNotifyObj);
                }

                //Session["login"] = res.Result;

                CurrentSession.Set<EvernoteUser>("login",res.Result);

                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            //EvernoteUser currenUser = Session["login"] as EvernoteUser;

          
            BusinessLayerResult<EvernoteUser> res = everNoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Erros.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Profil Silinemedi.",
                    Items = res.Erros,
                    RedirectingUrl = "/Home/ShowProfile"
                };
                return View("Error", errorNotifyObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }


        public ActionResult TestNotify()
        {
            OkViewModel model = new OkViewModel()
            {
                Header = "Yönlendirme..",
                Title = "Ok test",
                RedirectingTimeout = 3000,
                Items = new List<string>() { "test başarılı 1", "test başarılı 2" }
            };
            return View("Ok", model);
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                BusinessLayerResult<EvernoteUser> res = everNoteUserManager.LoginUser(model);

                if (res.Erros.Count > 0)
                {
                    if (res.Erros.Find(x => x.Code == ErrorMessage.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "http://Home/Activate/1234-4567-78980";
                    }
                    res.Erros.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                //Session["login"] = res.Result;

                CurrentSession.Set<EvernoteUser>("login",res.Result);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                BusinessLayerResult<EvernoteUser> res = everNoteUserManager.RegisterUser(model);

                //Hata dönüyorsa
                if (res.Erros.Count > 0)
                {
                    res.Erros.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",

                };
                notifyObj.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon link'ine tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız.");


                return View("Ok", notifyObj);
            }

            return View(model);
        }



        public ActionResult UserActivate(Guid id)
        {
          
            BusinessLayerResult<EvernoteUser> res = everNoteUserManager.ActivateUser(id);

            if (res.Erros.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Erros
                };

                return View("Error", errorNotifyObj);
            }

            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi...",
                RedirectingUrl = "/Home/Login"
            };
            okNotifyObj.Items.Add("Hesap Aktifleştirildi.Artık not paylaşabilir ve beğenme yapabilirsiniz.");

            return View("Ok",okNotifyObj);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HassError()
        {
            return View();
        }

        public ActionResult AutError()
        {
            return View();
        }

    }
}