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
                    Thread.Sleep(1000);
                }
                else
                {
                    int i = 0;
                }
            }
        }
    }
}
