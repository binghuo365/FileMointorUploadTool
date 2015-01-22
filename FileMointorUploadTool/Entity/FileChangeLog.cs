using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Entity
{
    public class FileChangeLog
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private DateTime _logDt;

        public DateTime LogDt
        {
            get { return _logDt; }
            set { _logDt = value; }
        }
        private int _changeType;

        public int ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }
        private string _beforeValue;

        public string BeforeValue
        {
            get { return _beforeValue; }
            set { _beforeValue = value; }
        }
        private string _afterValue;

        public string AfterValue
        {
            get { return _afterValue; }
            set { _afterValue = value; }
        }
    }
}
