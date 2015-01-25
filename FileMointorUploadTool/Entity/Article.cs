using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMointorUploadTool.Entity
{
    public class Article
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private int _channelId;

        public int ChannelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }
        private int _typeId;

        public int TypeId
        {
            get { return _typeId; }
            set { _typeId = value; }
        }
        private string _tagStr;

        public string TagStr
        {
            get { return _tagStr; }
            set { _tagStr = value; }
        }
        private string _picPaths;

        public string PicPaths
        {
            get { return _picPaths; }
            set { _picPaths = value; }
        }
        private string _txt;

        public string Txt
        {
            get { return _txt; }
            set { _txt = value; }
        }
    }
}
