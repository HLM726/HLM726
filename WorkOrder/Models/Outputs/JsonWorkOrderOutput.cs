using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models.Outputs
{
    public class JsonWorkOrderOutput
    {
        public string itemType { get; set; }

        public string itemNo { get; set; }

        public string qty { get; set; }

        public string unit { get; set; }

    }
}
