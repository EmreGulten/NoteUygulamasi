﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.Common;
using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T>:RepositoryBase,IDataAccess<T> where T : class
    {
        //private DatabaseContext db;
        private DbSet<T> _objectSet;

        public Repository()
        {
            //db = RepositoryBase.CreateContext();
            _objectSet = db.Set<T>();
        }

        public List<T> List()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable<T>();
        }

        public List<T> List(Expression<Func<T, bool>> where)
        {
            return _objectSet.Where(where).ToList();
        }

        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }

        public int Insert(T obj)
        {
            _objectSet.Add(obj);

            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now;

                o.CreatedOn = now;
                o.ModifiedOn =now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();
            }
            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
              
                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();
            }

            return Save();
        }


        public int Delete(T obj)
        {
            //if (obj is MyEntityBase)
            //{
            //    MyEntityBase o = obj as MyEntityBase;

            //    o.ModifiedOn = DateTime.Now;
            //    o.ModifiedUsername = "system"; // TODO : işlem yapan kullanıcı
            //}

            _objectSet.Remove(obj);
            return Save();
        }


        public int Save()
        {
            return db.SaveChanges();
        }
    }

}
