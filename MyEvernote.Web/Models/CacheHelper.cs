using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using MyEvernote.BusinessLAyer;
using MyEvernote.Entities;

namespace MyEvernote.Web.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("category-cache"); // cache katagori listesi

            if (result == null)
            {
                CategoryManager categoryManager = new CategoryManager();

                result = categoryManager.List();

                WebCache.Set("category-cache",result,20,true);
            }

            return result;
        }

        public static void RemoveCategoriesFromCache()
        {
            RemoveCache("category-cache");
        }

        public static void RemoveCache(string key)
        {
            WebCache.Remove(key);
        }
    }
}