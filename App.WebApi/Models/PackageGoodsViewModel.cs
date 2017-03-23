using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebApi.Models
{
    public class PackageGoodsViewModel
    {
        public List<Guid> GoodsId { get; set; }
    }

    public class CreateRedeemCodeInput {
        public int CreateNumber { get; set; }
    }
}