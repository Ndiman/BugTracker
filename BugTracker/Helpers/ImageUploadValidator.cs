using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class ImageUploadValidator
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null) return false;
            try
            {
                var allowedTypes = db.AttachmentTypes.Select(a => a.Type).ToList();
                var fileExt = Path.GetExtension(file.FileName).Split('.')[1];
                return allowedTypes.Contains(fileExt);
            }
            catch
            {
                return false;
            }
        }
    }

    public class IconHelper
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetIcon(string filePath)
        {
            var type = Path.GetExtension(filePath).Split('.')[1];
            return db.AttachmentTypes.FirstOrDefault(t => t.Type == type).MediaUrl;
        }
    }
}