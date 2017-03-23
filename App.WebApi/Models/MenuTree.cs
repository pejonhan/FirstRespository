using System;
using System.Collections.Generic;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;

namespace App.WebApi.ViewModels
{
    public class MenuTree : TreeData<MenuTree>
    {
        public Guid? ParentId { get; set; }
        public string MenuText { get; set; }
        public string MenuLink { get; set; }
        public string MenuType { get; set; }
        public string IconCls { get; set; }
        public int SortOrder { get; set; }
        public string IsActivated { get; set; }
        public IList<ButtonDto> Buttons { get; set; }
    }
}
