using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models.Outputs
{
    public class JsonModel
    {
        public List<JsonWorkOrder> jsonModel { get; set; }

        public JsonModel()
        {
            jsonModel = new List<JsonWorkOrder>();
        }
    }
}
