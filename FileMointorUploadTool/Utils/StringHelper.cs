using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Utils
{
    public class StringHelper
    {
        public static string genFileName(string path)
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss") + new Random().Next().ToString()+"."+getSuffix(path);
        }

        public static string getSuffix(string path)
        {
            string[] s = path.Split('.');
            if (s.Length > 1)
            {
                return s[s.Length - 1];
            }
            return "";
        }

        public static FileMointorUploadTool.Entity.Article parase(string filePath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath); ;
            string[] s = fileName.Split('_');
            string[] spstring = {"t","c","ta","ty" };
            int defaultChannelId = 43;
            FileMointorUploadTool.Entity.Article article = new Entity.Article();
            article.ChannelId = defaultChannelId;
            if (s.Length == 1)
            {
                article.Title = fileName;
            }
            for(int i = 0; i < s.Length; ++i)
            {
                if (s[i] == "t" && i != s.Length -1)
                {
                    article.Title = s[i + 1];
                }
                if (s[i] == "c" && i != s.Length - 1)
                { 
                    article.ChannelId = Convert.ToInt32(s[i+1]);
                }
                if (s[i] == "ta" && i != s.Length - 1)
                {
                    article.TagStr = s[i + 1];
                }
                if (s[i] == "ty" && i != s.Length - 1)
                {
                    article.TypeId = Convert.ToInt32(s[i+1]);
                }
            }
            return article;
        }

        public enum EARTICLETYPE
        { 
            EIMAGE = 0,
            ETXT   = 1,
            EUNKOWN = 2,
        }

        public static EARTICLETYPE getArticleEnum(string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            string suffix = getSuffix(fileName);
            if (suffix == "gif"
                || suffix == "jpg"
                || suffix == "png")
            {
                return EARTICLETYPE.EIMAGE;
            }
            if (suffix == "txt")
            {
                return EARTICLETYPE.ETXT;
            }
            return EARTICLETYPE.EUNKOWN;
        }
    }
}
