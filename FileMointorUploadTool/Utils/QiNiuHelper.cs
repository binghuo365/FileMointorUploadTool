using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.RS;
using Qiniu.RPC;
using Qiniu.IO;

namespace FileMointorUploadTool.Utils
{
    public class QiNiuHelper
    {
        static string BUCKET = System.Configuration.ConfigurationManager.AppSettings["BUCKET"];

        /// <summary>
        /// 上传文件测试
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fname"></param>
        public static bool PutFile(string key, string fname)
        {
            try
            {
                Qiniu.Conf.Config.Init();
                var policy = new PutPolicy(BUCKET);
                string upToken = policy.Token();
                PutExtra extra = new PutExtra();
                IOClient client = new IOClient();
                client.PutFile(upToken, key, fname, extra);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static string uploadFile(string fname)
        {
            try
            {
                string key = FileMointorUploadTool.Utils.StringHelper.genFileName(fname);
                PutFile(key, fname);
                return key;
            }
            catch(Exception ex)
            {
                return "error";
            }
        }
    }
}
