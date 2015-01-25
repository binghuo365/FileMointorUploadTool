using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Dao
{
    public class UploadDao
    {
        public static bool save(FileMointorUploadTool.Entity.UploadLog entity)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "insert into t_upload (title, fingerprint, dt) values ('" + entity.Title + "', '" + entity.Fingerprint + "', '" + entity.Dt.ToString("yyyy-MM-dd hh:mm:ss") + "')";

                return FileMointorUploadTool.Utils.SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                FileMointorUploadTool.Utils.LogHelper.WriteLog(ex);
                return false;
            }
            finally
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
        }

        public static bool isUpdate(string fingerprint)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "select * from t_upload where fingerprint = '"+ fingerprint +"'";

                return FileMointorUploadTool.Utils.SQLiteHelper.ExecuteDataSet(sql, null).Tables[0].Rows.Count > 0;
            }
            catch (Exception ex)
            {
                FileMointorUploadTool.Utils.LogHelper.WriteLog(ex);
                return false;
            }
            finally
            {
                FileMointorUploadTool.Utils.SQLiteHelper.fina();
            }
        }
    }
}
