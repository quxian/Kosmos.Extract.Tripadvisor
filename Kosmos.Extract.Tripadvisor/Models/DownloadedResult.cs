using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kosmos.Extract.Tripadvisor
{
    public class DownloadedResult
    {
        /// <summary>
        /// 下载结果所属站点
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 从该URL下载内容
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// url深度
        /// </summary>
        public int Depth { get; set; }
        /// <summary>
        /// URI对应的内容
        /// </summary>
        public string Result { get; set; }
    }
}