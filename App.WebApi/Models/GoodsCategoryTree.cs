using App.Core.Domain.BaseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebApi.ViewModels
{
    public class GoodsCategoryTree : TreeData<GoodsCategoryTree>
    {
        public Guid? ParentId { get; set; }
        public string CategoryName { get; set; }
    }
}