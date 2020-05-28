using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.BusinessLAyer.Abstract;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;

namespace MyEvernote.BusinessLAyer
{
    public class CategoryManager : ManagerBase<Category>
    {
        public override int Delete(Category category)
        {
            NoteManager noteManager = new NoteManager();
            LikedManager likedManager = new LikedManager();
            CommentManager commentManager = new CommentManager();

            foreach (var note in category.Notes.ToList())
            {
                foreach (var liked in note.Likeds.ToList())
                {
                    likedManager.Delete(liked);
                }

                foreach (var comment in note.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }

                noteManager.Delete(note);
            }

            return base.Delete(category);
        }
    }
}
