using System;
using System.Data;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tips.Models
{
    public class BlogRepository
    {
        private Tips_Entities db = new Tips_Entities();

        public void Save()
        {
            db.SaveChanges();
        }

        #region Blog

        public IQueryable<BlogEntry> GetAllBlogEntries()
        {
            return db.BlogEntry.OrderByDescending(b => b.ID);
        }

        public BlogEntry GetBlogEntry(int ID)
        {
            return db.BlogEntry.SingleOrDefault(b => b.ID == ID);
        }

        public BlogEntry GetLatestBlogEntry()
        {
            DateTime maxDate = (from b in db.BlogEntry
                                select b.PostedDate).Max();

            return GetAllBlogEntries().SingleOrDefault(b => b.PostedDate == maxDate);
        }


        public void Add(BlogEntry b)
        {
            db.BlogEntry.Add(b);
        }

        public void Delete(BlogEntry b)
        {
            db.BlogEntry.Remove(b);
        }

        #endregion


        #region Comments

        public IQueryable<Comment> GetAllComments()
        {
            return db.Comment.OrderByDescending(c => c.PostedDate);
        }

        public Comment GetComment(int commentID)
        {
            return db.Comment.SingleOrDefault(c => c.ID == commentID);
        }
        
        public void Add(Comment c)
        {
            db.Comment.Add(c);
        }

        public void Delete(Comment c)
        {
            db.Comment.Remove(c);
        }

        #endregion
    }
}
