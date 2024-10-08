using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models.Outputs
{
    public class JsonWorkOrder
    {
        public string woNo { get; set; }

        public string woDate { get; set; }

        public string tradeCode { get; set; }

        public string emsNo { get; set; }

        public List<JsonWorkOrderInput> woInput { get; set; }

        public List<JsonWorkOrderOutput> woOutput { get; set; }

        public JsonWorkOrder()
        {
            woInput = new List<JsonWorkOrderInput>();
            woOutput = new List<JsonWorkOrderOutput>();
        }
    }
}
