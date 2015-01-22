using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Entity
{
    public class UploadLog
    {
        private int _int;

        public int _int1
        {
            get { return _int; }
            set { _int = value; }
        }
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string _fingerprint;

        public string Fingerprint
        {
            get { return _fingerprint; }
            set { _fingerprint = value; }
        }
        private DateTime _dt;

        public DateTime Dt
        {
            get { return _dt; }
            set { _dt = value; }
        }
    }
}
