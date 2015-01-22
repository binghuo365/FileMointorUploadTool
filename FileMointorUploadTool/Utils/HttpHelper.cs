using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace FileMointorUploadTool.Utils
{
    public static class HttpHelper
    {
        public static string LoginUrl = "http://www.behake.com/jeeadmin/jeecms/login.do?returnUrl=/jeeadmin/jeecms/index.do";
        public static CookieContainer _cookieContainer;
        public static string _cookiesString;
        public static string UrlCap = "http://www.behake.com/captcha.svl";
        public static string publishUrl = "http://www.behake.com/jeeadmin/jeecms/content/o_save.do";

        public static string doPost(string Url, byte[] postData, CookieContainer bCookie, String encodingFormat, String referer)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                myRequest.CookieContainer = bCookie;
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                myRequest.Method = "POST";
                myRequest.Timeout = 30000;
                myRequest.KeepAlive = true;
                if (referer != "")
                    myRequest.Referer = referer;
                myRequest.Headers["Cache-control"] = "no-cache";
                myRequest.Headers["Accept-Language"] = "zh-cn";
                myRequest.Headers.Add("Cookie", _cookiesString);
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Trident/4.0; GTB7.4; GTB7.1; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; InfoPath.2)";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.Accept = "*/*";
                myRequest.ContentLength = postData.Length;
                Stream newStream = myRequest.GetRequestStream();
                newStream.Write(postData, 0, postData.Length);
                newStream.Close();
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                _cookiesString = myRequest.CookieContainer.GetCookieHeader(myRequest.RequestUri);
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(encodingFormat));
                string outdata = reader.ReadToEnd();
                reader.Close();
                if (!outdata.Contains("基础连接已经关闭: 连接被意外关闭") && !outdata.Contains("无法连接到远程服务器") && !outdata.Contains("基础连接已经关闭: 接收时发生错误。"))
                    return outdata;
                else
                    return "基础连接已经关闭: 连接被意外关闭";
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("基础连接已经关闭: 连接被意外关闭") && !ex.Message.Contains("无法连接到远程服务器") && !ex.Message.Contains("基础连接已经关闭: 接收时发生错误。"))
                    return ex.Message;
                else
                    return "基础连接已经关闭: 连接被意外关闭";
            }

        }

        public static Image doGetImg(string Url, CookieContainer cookieContainer)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                myRequest.ServicePoint.Expect100Continue = true;
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                if (cookieContainer != null)
                {
                    myRequest.CookieContainer = cookieContainer;
                }
                else
                {
                    myRequest.CookieContainer = new CookieContainer();
                    _cookieContainer = myRequest.CookieContainer;
                }
                myRequest.Method = "GET";
                myRequest.Timeout = 30000;
                myRequest.CookieContainer = _cookieContainer;
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                myRequest.KeepAlive = true;
                myRequest.Headers["Cache-control"] = "no-cache";
                myRequest.Headers["Accept-Language"] = "zh-cn";
                myRequest.Headers.Add("Cookie", _cookiesString);
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Trident/4.0; GTB7.4; GTB7.1; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; InfoPath.2)";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.Accept = "*/*";
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                //保存返回cookie  
                myResponse.Cookies = myRequest.CookieContainer.GetCookies(myRequest.RequestUri);
                _cookiesString = myRequest.CookieContainer.GetCookieHeader(myRequest.RequestUri);
                return Bitmap.FromStream(myResponse.GetResponseStream());
            }
            catch
            {
                return null;
            }
        }



    }
}
