using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class OrderViewModel<T>
    {
        public List<T> Items { get; set; }
        public PagerViewModel Pager { get; set; }
        public Dictionary<string, int> OrderStatus { get; set; }
    }
}