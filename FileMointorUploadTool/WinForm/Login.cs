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

using FileMointorUploadTool.Utils;

namespace FileMointorUploadTool.WinForm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string poststr = "username=" + username.Text.Trim() + "&password=" + password.Text.Trim() + "&captcha=" + yzm.Text.Trim();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(poststr);
            string text = HttpHelper.doPost(HttpHelper.LoginUrl, data, HttpHelper._cookieContainer, "utf-8", "");
            if (text.Contains("验证码错误") || text.Contains("captcha 为必填项"))
            {
                MessageBox.Show("验证码无效");
            }
            else if (text.Contains("username not found"))
            {
                MessageBox.Show("用户名错误");
            }
            else if (text.Contains("password invalid"))
            {
                MessageBox.Show("密码错误");
            }
            else if (text.Contains("jeecms-main"))
            {
                fileSystemWatcherFrm frm1 = new fileSystemWatcherFrm();
                frm1.Show();
                this.Hide();
                return;
            }
            else
            {
                MessageBox.Show("无效输入");
            }
            refreshCaptcha(HttpHelper._cookieContainer);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.behake.com");
        }

        public void refreshCaptcha(CookieContainer sc)
        {
            pictureBox1.Image = HttpHelper.doGetImg(HttpHelper.UrlCap, sc);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.behake.com/jeeadmin/jeecms/index.do?locale=zh_CN"); 
        }

        private void Login_Load(object sender, EventArgs e)
        {
            refreshCaptcha(HttpHelper._cookieContainer);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            refreshCaptcha(HttpHelper._cookieContainer);
        }
    }
}
