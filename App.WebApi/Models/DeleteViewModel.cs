using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebApi.ViewModels
{
    public class DeleteViewModel
    {
        public Guid Id { get; set; }
        public string Remark { get; set; }
    }
}