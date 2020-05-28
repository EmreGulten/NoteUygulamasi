using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.MySql
{
   public class RepositoryBase
   {
       protected static object context;
       protected static object _lockSyn = new object();

       protected RepositoryBase()
       {
           CreateContext();
       }

       private static void CreateContext()
       {
           if (context == null)
           {
               lock (_lockSyn)
               {
                   if (context == null)
                   {
                       context = new object();
                   }
               }
           }
       }
   }
}
