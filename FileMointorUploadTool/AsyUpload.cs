using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FileMointorUploadTool
{
    public class AsyUpload
    {
        public void run()
        {
            while (true)
            {
                FileMointorUploadTool.Entity.Queue queue = FileMointorUploadTool.Dao.QueueDao.pop();
                if (null == queue)
                {
                    Thread.Sleep(3000);
                }
                else
                {
                    FileMointorUploadTool.Entity.UploadLog uploadLog = new FileMointorUploadTool.Entity.UploadLog();
                    uploadLog.Title = queue.Path;
                    uploadLog.Fingerprint = FileMointorUploadTool.Utils.Md5.getMD5Hash(queue.Path);
                    uploadLog.Dt = DateTime.Now;
                    string waterPath = queue.Path.Substring(0, queue.Path.Length - 4) + "_water" + queue.Path.Substring(queue.Path.Length - 4, 4) ;
                    if (!FileMointorUploadTool.Dao.UploadDao.isUpdate(uploadLog.Fingerprint))
                    {
                        queue.Status = 1;
                        FileMointorUploadTool.Dao.QueueDao.update(queue);
                        FileMointorUploadTool.Utils.ImageWaterMark.WaterImage(queue.Path, "E:\\MyGitSource\\FileMointorUploadTool\\FileMointorUploadTool\\assert\\logowater3.png", "@behake.com", waterPath);
                        string key = FileMointorUploadTool.Utils.QiNiuHelper.uploadFile(waterPath);
                        if ("error" != key)
                        {
                            addArticle(queue, key);
                        }
                        FileMointorUploadTool.Dao.UploadDao.save(uploadLog);
                    }
                    //已经删除，从任务列队中删除
                    FileMointorUploadTool.Utils.FileHelper.DeleteFile(queue.Path);
                    FileMointorUploadTool.Utils.FileHelper.DeleteFile(waterPath);
                    FileMointorUploadTool.Dao.QueueDao.delete(queue.Id);
                }
            }
        }

        public bool addArticle(FileMointorUploadTool.Entity.Queue entity, string key)
        {
            try
            {
                Entity.Article article = FileMointorUploadTool.Utils.StringHelper.parase(entity.Path);
                string poststr = "title=" + article.Title;
                //if (FileMointorUploadTool.Utils.StringHelper.EARTICLETYPE.EIMAGE
                //    == FileMointorUploadTool.Utils.StringHelper.getArticleEnum(entity.Path))
                //{
                //}
                //else if (FileMointorUploadTool.Utils.StringHelper.EARTICLETYPE.ETXT
                //    == FileMointorUploadTool.Utils.StringHelper.getArticleEnum(entity.Path))
                //{
                //    poststr += "&txt="+article.Txt;
                //}
                //else
                //{
                //    return false;
                //}
                poststr += "&channelId=" + article.ChannelId + "&cid="+article.ChannelId;
                poststr += "&picPaths=" + "http://img.behake.com/" + key + "&picDescs=";
                poststr += "&tagStr=" + article.TagStr;
                poststr += "&txt=" + article.Txt;
                if (0 == article.TypeId)
                {
                    article.TypeId = 4;
                }
                poststr += "&typeId="+article.TypeId ;
                //poststr += "&link=&shortTitle=&titleColor=&bold=false&description=&author=&origin=&originUrl=&topLevel=0&sortDate=&releaseDate=&recommend=false&draft=false&tplContent=&titleImg=";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(poststr);
                string text = FileMointorUploadTool.Utils.HttpHelper.doPost(
                    FileMointorUploadTool.Utils.HttpHelper.publishUrl, data,
                    FileMointorUploadTool.Utils.HttpHelper._cookieContainer,
                    "utf-8", FileMointorUploadTool.Utils.HttpHelper.publishUrl);
                if (text.Contains("操作成功"))
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
