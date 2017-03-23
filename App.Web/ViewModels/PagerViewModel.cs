using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class PagerViewModel
    {
        public long Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long PageCount
        {
            get
            {
                return (Total + PageSize - 1) / PageSize;
            }
        }
    }
}