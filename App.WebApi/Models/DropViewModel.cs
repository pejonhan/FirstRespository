using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WebApi.ViewModels
{
    public class DropViewModel
    {
        public class Target
        {
            public Guid Id { get; set; }
            public Guid? ParentId { get; set; }
            public int SortOrder { get; set; }
        }

        public class Source
        {
            public Guid Id { get; set; }
            public Guid? ParentId { get; set; }
            public int SortOrder { get; set; }
        }

        public string point { get; set; }
        public Target target { get; set; }
        public Source source { get; set; }
    }
}
