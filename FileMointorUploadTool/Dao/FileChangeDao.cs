using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Dao
{
    public static class FileChangeDao
    {
        public static bool Save(FileMointorUploadTool.Entity.FileChangeLog entity)
        {
            try
            {
                FileMointorUploadTool.Utils.SQLiteHelper.init();
                string sql = "insert into t_files_change_log (change_type, before_value, after_value) values (" + entity.ChangeType + ", '" + entity.BeforeValue + "', '" + entity.AfterValue + "')";

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
