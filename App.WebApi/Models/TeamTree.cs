using System;
using System.Collections.Generic;
using App.Core.Domain.BaseObject;

namespace App.WebApi.ViewModels
{
    public class TeamTree : TreeData<TeamTree>
    {
        public Guid? ParentId { get; set; }
        public string TeamName { get; set; }
        public string TeamDesc { get; set; }
        public int SortOrder { get; set; }
    }
}
