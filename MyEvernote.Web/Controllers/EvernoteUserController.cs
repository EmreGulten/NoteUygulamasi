using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLAyer;
using MyEvernote.BusinessLAyer.Result;
using MyEvernote.Entities;
using MyEvernote.Web.Filters;

namespace MyEvernote.Web.Controllers
{
    [Auth]
    [AuthAdmin]
    [Ex]
    public class EvernoteUserController : Controller
    {
       private EverNoteUserManager everNoteUserManager = new EverNoteUserManager();

        // GET: EvernoteUser
        public ActionResult Index()
        {
            return View(everNoteUserManager.List());
        }

        // GET: EvernoteUser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvernoteUser evernoteUser = everNoteUserManager.Find(x=>x.Id == id.Value);

            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        // GET: EvernoteUser/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                // TODO:düzelt,lecek
                
                BusinessLayerResult<EvernoteUser> res = everNoteUserManager.Insert(evernoteUser);

                if (res.Erros.Count > 0)
                {
                    res.Erros.ForEach(x=>ModelState.AddModelError("",x.Message));
                    return View(evernoteUser);
                }

                return RedirectToAction("Index");
            }

            return View(evernoteUser);
        }

        // GET: EvernoteUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                //TODO:düzenlenecek
                BusinessLayerResult<EvernoteUser> res = everNoteUserManager.Update(evernoteUser);

                if (res.Erros.Count > 0)
                {
                    res.Erros.ForEach(x=>ModelState.AddModelError("",x.Message));
                    return View(evernoteUser);
                }

                return RedirectToAction("Index");
            }
            return View(evernoteUser);
        }

        // GET: EvernoteUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        // POST: EvernoteUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id);
            everNoteUserManager.Delete(evernoteUser);
            return RedirectToAction("Index");
        }

    }
}
