using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models
{
    public class HandbookModel
    {
        public string 企业编码 { get; set; }

        public string 账册编号 { get; set; }

        public string 工单类型 { get; set; }

        public string 保税库位 { get; set; }
    }
    public class HandbookModelEdi
    {
        public string 企业编码 { get; set; }

        public string 账册编号 { get; set; }

        public string 工单类型 { get; set; }

        public string 保税库位 { get; set; }
        public string EDI { get; set; }
    }


}
