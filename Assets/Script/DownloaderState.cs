
using System;
using System.Collections.Generic;

namespace ImagineClass.Library
{
    public class DownloaderState
    {
        public string url;
        public string tip;
        public string page;
        public float progress;
        public Action clickConfirm;
    }
}