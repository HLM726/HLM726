using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models.Outputs
{
    public class JsonZipModel
    {
        public static List<string> GetZipFileName { get; set; } = new List<string>();


        public static List<string> GetJsonFileName { get; set; } = new List<string>();


        public JsonZipModel()
        {
            GetZipFileName = new List<string>();
            GetJsonFileName = new List<string>();
        }
    }
}
