using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileMointorUploadTool.Entity;
using System.Data;

namespace FileMointorUploadTool.Dao
{
    public class QueueDao
    {
        //取最前面的没有处理的
        public static FileMointorUploadTool.Entity.Queue pop()
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "select * from t_queue where  status = 0 order by id desc limit 0,1";
                DataSet ds = FileMointorUploadTool.Utils.SQLiteHelper.ExecuteDataSet(sql, null);
                FileMointorUploadTool.Entity.Queue queue = new Queue();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    queue.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    queue.Status = Convert.ToInt32(ds.Tables[0].Rows[0]["status"].ToString());
                    queue.Path = ds.Tables[0].Rows[0]["path"].ToString();
                    return queue;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
            return null;
        }

        public static bool Save(FileMointorUploadTool.Entity.Queue entity)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "insert into t_queue (status, path) values (" + entity.Status + ", '" + entity.Path + "')";

                return FileMointorUploadTool.Utils.SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally 
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
        }

        public static bool update(FileMointorUploadTool.Entity.Queue entity)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "update t_queue set status = " + entity.Status + " where id = "+ entity.Id;
                return FileMointorUploadTool.Utils.SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
        }

        public static bool delete(int id)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "delete from t_queue where id = " + id;

                return FileMointorUploadTool.Utils.SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
        }
    }
}
