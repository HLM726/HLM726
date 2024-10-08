using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models.Outputs
{
    public class JsonMD5Model
    {
        public List<MD5Model> Data { get; set; }

        public JsonMD5Model()
        {
            Data = new List<MD5Model>();
        }
    }
}
