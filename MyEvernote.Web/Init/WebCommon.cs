using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyEvernote.Common;
using MyEvernote.Entities;
using MyEvernote.Web.Models;

namespace MyEvernote.Web.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            //if (HttpContext.Current.Session["login"] != null)
            //{
            //    EvernoteUser user = HttpContext.Current.Session["login"] as EvernoteUser;
            //    return user.Username;
            //}

            EvernoteUser user = CurrentSession.User;

            if (user != null)
            {
                return user.Username;
            }
            else
            {
                return "system";
            }

      
        }
    }
}