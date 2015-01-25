using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Entity
{
    public class UploadLog
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
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
