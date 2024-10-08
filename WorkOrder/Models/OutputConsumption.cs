using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrder.Models
{
    public class OutputConsumptionModel
    {
        [ExcelColumnIndex(0)]
        public string 料号 { get; set; }

        [ExcelColumnIndex(1)]
        public string 数量 { get; set; }

        [ExcelColumnIndex(2)]
        public string 单位 { get; set; }

        [ExcelColumnIndex(3)]
        public string 库位 { get; set; }

        [ExcelColumnIndex(4)]
        public string 工单号码 { get; set; }
    }

}
