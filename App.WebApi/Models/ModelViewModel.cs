using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Domain.BaseObject
{
    public class ModelViewModel
    {
        public string Id { get; set; }
        public string ModelName { get; set; }
        public string ModelDescription { get; set; }
        public string ModelGroup { get; set; }
        public string UISaveType { get; set; }
        public string ExistField { get; set; }
    }

    public class GenerationViewModel {
        public string option { get; set; }
        public string outputPath { get; set; }
        public ModelViewModel model { get; set; }
    }
}
