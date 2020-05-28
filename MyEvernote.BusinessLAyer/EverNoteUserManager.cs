using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.BusinessLAyer.Abstract;
using MyEvernote.BusinessLAyer.Result;
using MyEvernote.Common.Helpers;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjets;

namespace MyEvernote.BusinessLAyer
{
   public class EverNoteUserManager : ManagerBase<EvernoteUser>
    {
        
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            var user = Find(x => x.Username == data.Username || x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            //kullanıcı varsa
            if (user !=null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessage.UsernameAlreadyExists, "Kullanıcı adı Kayıtlı");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessage.EmailAlreadyExists, "E-posta Kayıtlı.");
                   
                }
            }
            else
            {
                //kullanıcı yoksa yeni oluştur
                int dbResult = base.Insert(new EvernoteUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFilename = "user_boy.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false
                    
                });

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    // TODO : Aktivasyon maili atılacak
                    //layerResult.Result.ActivateGuid
                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body =
                        $"Merhaba {res.Result.Username};<br><br> Hesabınızı Aktifleştirmek için <a href='{activateUri}' target='_blank'>Tıklayınız</a>.";

                    MailHelper.SendMail(body,res.Result.Email,"Hesap Aktifleştirme");
                }
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            //Giriş Kontrolü
            //Hesap aktive edilmiş mi

          
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessage.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessage.CheckYourEmail, "Lütfen e-posta adresnizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessage.UsernameOrPassWrong, "Kullanıcı adı yada şifre uyuşmuyor.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result =Find(x => x.ActivateGuid == activateId);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessage.UserAlreadyActive,"Kullanıcı zaten aktif.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessage.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);

            if (res.Result == null)
            {
                res.AddError(ErrorMessage.UserNotFound,"Kullanıcı Bulunamadı");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            var db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessage.UsernameAlreadyExists ,"Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessage.EmailAlreadyExists,"E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false) // resim kontrol ediliyor
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessage.ProfileCouldNotUpdated,"Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
           BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
           EvernoteUser user = Find(x => x.Id == id);

           if (user != null)
           {
               if (Delete(user) == 0)
               {
                   res.AddError(ErrorMessage.UserCouldNotRemove,"Kullanıcı silinemedi.");
                   return res;
               }
           }
           else
           {
               res.AddError(ErrorMessage.UserCouldNotFind,"Kullanıcı bulunamadı.");
           }

           return res;
        }

        //Method hidding
        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)
        {
            var user = Find(x => x.Username == data.Username || x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = data;

            //kullanıcı varsa
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessage.UsernameAlreadyExists, "Kullanıcı adı Kayıtlı");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessage.EmailAlreadyExists, "E-posta Kayıtlı.");

                }
            }
            else
            {
                res.Result.ProfileImageFilename = "user_boy.png ";
                res.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessage.UserCouldNotInserted, "Kullanıcı Eklenemedi.");
                }
            }

            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            var db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessage.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessage.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

          

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessage.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }
    }
}
