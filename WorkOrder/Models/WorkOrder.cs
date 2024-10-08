using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models
{
    public class WorkOrderModel
    {
        [ExcelFormat("yyyyMMdd")]
        public string 工单日期 { get; set; }

        public string 工单类型 { get; set; }

        public string 工单号码 { get; set; }
    }

}
