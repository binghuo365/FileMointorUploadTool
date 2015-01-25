using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO; //for fileSystemWatcher
using System.Diagnostics;
using FileMointorUploadTool.Utils;

namespace FileMointorUploadTool.WinForm
{
    public partial class fileSystemWatcherFrm : Form
    {
        private Thread asyUploadThread;
        public fileSystemWatcherFrm()
        {
            InitializeComponent();
        }

        private ContextMenu notifyiconMnu;

        private delegate void setLogTextDelegate(FileSystemEventArgs e);

        private delegate void renamedDelegate(RenamedEventArgs e);

        FileSystemWatcher[] fswArr;

        private void start_tsbtn_Click(object sender, EventArgs e)
        {
            if (this.spcifiedFloder_rbtn.Checked)
            {
                if (!string.IsNullOrEmpty(this.filePath_txt.Text))
                {
                    fswArr = new FileSystemWatcher[1];
                    fswArr[0] = new FileSystemWatcher();
                    fswArr[0].Path = this.filePath_txt.Text;  //设置监控路径
                    fswArr[0].IncludeSubdirectories = this.subDirWatcher_chk.Checked;  //获取或设置一个值，该值指示是否监视指定路径中的子目录。
                    fswArr[0].NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                    fswArr[0].Created += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                    fswArr[0].Deleted += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                    fswArr[0].Changed += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                    fswArr[0].Renamed += new RenamedEventHandler(this.fileSystemWatcher_Renamed);
                    fswArr[0].EnableRaisingEvents = true;
                }
                else
                {
                    MessageBox.Show("请先选择监控路径！");
                    return;
                }
            }
            else
            {
                int checkedCount = this.dir_lvw.CheckedItems.Count;
                if (checkedCount > 0)
                {
                    fswArr = new FileSystemWatcher[checkedCount];
                    for (int i = 0; i < this.dir_lvw.CheckedItems.Count; i++)
                    {
                        fswArr[i] = new FileSystemWatcher();
                        fswArr[i].Path = this.dir_lvw.CheckedItems[i].Text;
                        fswArr[i].IncludeSubdirectories = this.subDirWatcher_chk.Checked;  //获取或设置一个值，该值指示是否监视指定路径中的子目录。
                        fswArr[i].NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                        fswArr[i].Created += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                        fswArr[i].Deleted += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                        fswArr[i].Changed += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
                        fswArr[i].Renamed += new RenamedEventHandler(this.fileSystemWatcher_Renamed);
                        fswArr[i].EnableRaisingEvents = true;
                    }
                }
                else
                {
                    MessageBox.Show("您尚未选择任何监控路径！");
                    return;
                }
            }
            this.start_tsbtn.Enabled = false;
            this.stop_tsbtn.Enabled = true;

            //开启上传线程
            asyUploadThread = new Thread(new ThreadStart(new AsyUpload().run));
            asyUploadThread.Start();
        }

        private void fileSystemWatcherFrm_Load(object sender, EventArgs e)
        {
            Initializenotifyicon();
            ListViewGroup hardDisk_lvg = new ListViewGroup();
            hardDisk_lvg.Header = "可监控硬盘";
            hardDisk_lvg.Name = "hardDisk";
            ListViewGroup removableDisk_lvg = new ListViewGroup();
            removableDisk_lvg.Header = "可监控移动硬盘";
            removableDisk_lvg.Name = "removableDisk";
            ListViewGroup other_lvg = new ListViewGroup();
            other_lvg.Header = "其它";
            other_lvg.Name = "other";
            this.dir_lvw.Groups.Add(hardDisk_lvg);
            this.dir_lvw.Groups.Add(removableDisk_lvg);
            this.dir_lvw.Groups.Add(other_lvg);
            this.dir_lvw.ShowGroups = true;
            this.dir_lvw.LargeImageList = this.file_ilst;
            foreach (string driverName in Directory.GetLogicalDrives())
            {
                DriveInfo driverInfo = new DriveInfo(driverName);
                if (driverInfo.IsReady == true)
                {
                    if (driverInfo.DriveType == DriveType.Fixed)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.ImageIndex = 0;
                        lvi.Text = driverInfo.Name;
                        lvi.Group = hardDisk_lvg;
                        this.dir_lvw.Items.Add(lvi);
                    }
                    else if (driverInfo.DriveType == DriveType.Removable)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.ImageIndex = 1;
                        lvi.Text = driverInfo.Name;
                        lvi.Group = removableDisk_lvg;
                        this.dir_lvw.Items.Add(lvi);
                    }
                }
            }
            this.dir_lvw.Items.Add(new ListViewItem("系统盘", 2, other_lvg));  
        }

        private void setLogText(FileSystemEventArgs e)
        {
            string[] strArr = e.FullPath.Split(new char[] { '\\' });
            if (!strArr[1].Equals("$RECYCLE.BIN"))  //回收站
            {
                this.fileEventLog_lvw.BeginUpdate();
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add(DateTime.Now.ToString());
                lvi.SubItems.Add(e.ChangeType.ToString());
                lvi.SubItems.Add(strArr[0]);  //硬盘
                lvi.SubItems.Add(strArr[strArr.Length - 1]); //文件名
                lvi.SubItems.Add(e.FullPath);
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        lvi.ForeColor = Color.Lime;
                        lvi.SubItems.Add("在" + strArr[0] + "盘创建了【" + strArr[strArr.Length - 1] + "】");
                        FileMointorUploadTool.Entity.FileChangeLog log = new Entity.FileChangeLog();
                        log.BeforeValue = e.FullPath;
                        log.AfterValue = "";
                        log.ChangeType = 1;
                        FileMointorUploadTool.Dao.FileChangeDao.Save(log);

                        //添加到列队中 jpg gif 文件添加到列队中
                        if (strArr[strArr.Length - 1].Split('.')[1].ToLower() == ("gif")
                            || strArr[strArr.Length - 1].Split('.')[1].ToLower() == ("jpg")
                            || strArr[strArr.Length - 1].Split('.')[1].ToLower() == ("png"))
                        {
                            FileMointorUploadTool.Entity.Queue queue = new Entity.Queue();
                            queue.Path = e.FullPath;
                            queue.Status = 0;
                            FileMointorUploadTool.Dao.QueueDao.Save(queue);
                        }
                        break;
                    case WatcherChangeTypes.Deleted:
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems.Add("在" + strArr[0] + "盘删除了【" + strArr[strArr.Length - 1] + "】");
                        FileMointorUploadTool.Entity.FileChangeLog log2 = new Entity.FileChangeLog();
                        log2.BeforeValue = e.FullPath;
                        log2.AfterValue = "";
                        log2.ChangeType = 4;
                        FileMointorUploadTool.Dao.FileChangeDao.Save(log2);
                        break;
                    case WatcherChangeTypes.Changed:
                        lvi.ForeColor = Color.Fuchsia;
                        lvi.SubItems.Add("在" + strArr[0] + "盘修改了【" + strArr[strArr.Length - 1] + "】");
                        break;
                }
                this.fileEventLog_lvw.Items.Add(lvi);
                this.fileEventLog_lvw.EndUpdate();
            }
        }

        private void setRenamedLogText(RenamedEventArgs e)
        {
            string[] oldStrArr = e.OldFullPath.Split(new char[] { '\\' });
            string[] newStrArr = e.Name.Split(new char[] { '\\' });
            ListViewItem lvi = new ListViewItem();
            lvi.ForeColor = Color.Yellow;
            lvi.SubItems.Add(DateTime.Now.ToString());
            lvi.SubItems.Add(e.ChangeType.ToString());
            lvi.SubItems.Add(oldStrArr[0]);  //硬盘
            lvi.SubItems.Add(oldStrArr[oldStrArr.Length - 1] + " → " + newStrArr[newStrArr.Length - 1]);   //文件名
            lvi.SubItems.Add(e.FullPath);
            lvi.SubItems.Add("在" + oldStrArr[0] + "盘将【" + oldStrArr[oldStrArr.Length - 1] + "】重命名为 【" + newStrArr[newStrArr.Length - 1] + "】");
            this.fileEventLog_lvw.Items.Add(lvi);

            FileMointorUploadTool.Entity.FileChangeLog log3 = new Entity.FileChangeLog();
            log3.BeforeValue = e.OldFullPath;
            log3.AfterValue = e.FullPath;
            log3.ChangeType = 2;
            FileMointorUploadTool.Dao.FileChangeDao.Save(log3);
        }

        private void fileSystemWatcher_EventHandle(object sender, FileSystemEventArgs e)
        {
            if (this.fileEventLog_lvw.InvokeRequired)
            {
                this.fileEventLog_lvw.Invoke(new setLogTextDelegate(setLogText), new object[] { e });
            }
        }

        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (this.fileEventLog_lvw.InvokeRequired)
            {
                this.fileEventLog_lvw.Invoke(new renamedDelegate(setRenamedLogText), new object[] { e });
            }
        }

        private void stop_tsbtn_Click(object sender, EventArgs e)
        {
            foreach (FileSystemWatcher fsw in fswArr)
            {
                fsw.EnableRaisingEvents = false; //获取或设置一个值，该值指示是否启用此组件。
            }
            this.stop_tsbtn.Enabled = false;
            this.start_tsbtn.Enabled = true;
        }

        private void clear_tsbtn_Click(object sender, EventArgs e)
        {
            this.fileEventLog_lvw.Items.Clear();
        }

        private void save_tsbtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath + "\\Logs";
            sfd.RestoreDirectory = true;
            sfd.FileName = DateTime.Now.ToString("yyyyMMdd hh-mm-ss") + ".log";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd.FileName);
                for (int i = 0; i < this.fileEventLog_lvw.Items.Count; i++)
                {
                    string str = "[" + this.fileEventLog_lvw.Items[i].SubItems[1].Text + "]【" + this.fileEventLog_lvw.Items[i].SubItems[2].Text + "】["
                        + this.fileEventLog_lvw.Items[i].SubItems[3].Text + "][" + this.fileEventLog_lvw.Items[i].SubItems[4].Text + "]["
                        + this.fileEventLog_lvw.Items[i].SubItems[5].Text + "][" + this.fileEventLog_lvw.Items[i].SubItems[4].Text + "]";
                    sw.WriteLine(str);
                }
                sw.Close();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1_showfrom(sender, e);
        }

        public void notifyIcon1_showfrom(object sender, System.EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = true;
        }

        public void ExitSelect(object sender, System.EventArgs e)
        {
            //隐藏托盘程序中的图标 
            notifyIcon1.Visible = false;
            //关闭系统 
            this.Close();
            this.Dispose(true);
            notifyIcon1.Dispose();
            Application.Exit();
        }

        private void Initializenotifyicon()
        {
            //定义一个MenuItem数组，并把此数组同时赋值给ContextMenu对象 
            MenuItem[] mnuItms = new MenuItem[3];
            mnuItms[0] = new MenuItem();
            mnuItms[0].Text = "显示窗口";
            mnuItms[0].Click += new System.EventHandler(this.notifyIcon1_showfrom);

            mnuItms[1] = new MenuItem("-");

            mnuItms[2] = new MenuItem();
            mnuItms[2].Text = "退出系统";
            mnuItms[2].Click += new System.EventHandler(this.ExitSelect);
            mnuItms[2].DefaultItem = true;

            notifyiconMnu = new ContextMenu(mnuItms);
            notifyIcon1.ContextMenu = notifyiconMnu;
            //为托盘程序加入设定好的ContextMenu对象 
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.behake.com");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.behake.com/jeeadmin/jeecms/index.do?locale=zh_CN");
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void fileWatcherPath_btn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "请选择要监控的文件目录:";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.filePath_txt.Text = fbd.SelectedPath;
            }
        }

        private void fileSystemWatcherFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            notifyIcon1.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.ShowInTaskbar = false;
            return;
        }

        private void fileSystemWatcherFrm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) //判断是否最小化
            {
                notifyIcon1.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void allDisk_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (this.allDisk_chk.Checked)
            {
                foreach (ListViewItem lvi in this.dir_lvw.Items)
                {
                    if (lvi.Group.Name.Equals("hardDisk") || lvi.Group.Name.Equals("removableDisk"))
                    {
                        lvi.Checked = true;
                    }
                }
            }
            else
            {
                foreach (ListViewItem lvi in this.dir_lvw.Items)
                {
                    if (lvi.Group.Name.Equals("hardDisk") || lvi.Group.Name.Equals("removableDisk"))
                    {
                        lvi.Checked = false;
                    }
                }
            }
        }

        private void dir_lvw_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.dir_lvw.Items[e.Index].Group.Name.Equals("other"))
            {
                if (this.dir_lvw.Items[0].Checked)
                {
                    MessageBox.Show("已选择C:盘监控");

                    e.NewValue = CheckState.Unchecked;
                }
            }
        }
    }
}
