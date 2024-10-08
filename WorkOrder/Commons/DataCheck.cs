using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOrder.Models;

namespace WorkOrder.Commons
{
    public class DataCheck
    {
        public static List<string> checkSpace(List<HandbookModel> Handbooks, List<PartNumberModel> PartNumbers, List<UnitsModel> Unitss, List<WorkOrderModel> WorkOrders, List<OutputConsumptionModel> OutputConsumptions)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Handbooks.Count; i++)
            {
                if (IsNulltool.AreAllPropertiesNullOrEmpty(Handbooks[i]))
                {
                    list.Add("手册表第 " + (i + 1) + "项数据为空");
                }
            }
            int num = 0;
            foreach (PartNumberModel PartNumber in PartNumbers)
            {
                if (IsNulltool.AreAllPropertiesNullOrEmpty(PartNumber))
                {
                    list.Add($"料号定义表第 {num + 1} 项数据为空");
                }
                num++;
            }
            int num2 = 0;
            foreach (UnitsModel item in Unitss)
            {
                if (IsNulltool.AreAllPropertiesNullOrEmpty(item))
                {
                    list.Add($"单位定义表第 {num2 + 1} 项数据为空");
                }
                num2++;
            }
            int num3 = 0;
            foreach (WorkOrderModel WorkOrder in WorkOrders)
            {
                if (IsNulltool.AreAllPropertiesNullOrEmpty(WorkOrder))
                {
                    list.Add($"工单表第 {num3 + 1} 项数据为空");
                }
                num3++;
            }
            int num4 = 0;
            foreach (OutputConsumptionModel OutputConsumption in OutputConsumptions)
            {
                if (IsNulltool.AreAllPropertiesNullOrEmpty(OutputConsumption))
                {
                    list.Add($"工单产出耗用表的第 {num4 + 1} 项数据为空");
                }
                num4++;
            }
            return list;
        }

        public static List<string> checkDateIsDisff(List<WorkOrderModel> WorkOrders)
        {
            List<string> list = new List<string>();
            List<WorkOrderModel> range = WorkOrders.GetRange(0, WorkOrders.Count);
            try
            {
                List<string> list2 = (from k in range
                                      group k by k.工单日期 into k
                                      select k.Key.Replace(" 00:00:00", "")).ToList();
                if (list2.Count > 1)
                {
                    list.Add("工单表第 " + string.Join(",", list2).Replace(" 00:00:00", "") + "日期不一致");
                }
                return list;
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static List<string> Exists(List<HandbookModel> Handbooks, List<PartNumberModel> PartNumbers, List<UnitsModel> Unitss, List<WorkOrderModel> WorkOrders, List<OutputConsumptionModel> OutputConsumptions)
        {
            List<string> list = new List<string>();
            int num = 0;
            List<WorkOrderModel> range = WorkOrders.GetRange(0, WorkOrders.Count);
            List<HandbookModel> range2 = Handbooks.GetRange(0, Handbooks.Count);
            List<PartNumberModel> range3 = PartNumbers.GetRange(0, PartNumbers.Count);
            foreach (WorkOrderModel item in range)
            {
                if (range2.FirstOrDefault((HandbookModel f) => f.工单类型 == item.工单类型) == null)
                {
                    list.Add($"{num + 1}项，工单类型：{item.工单类型}，在手册表中不存在");
                }
                num++;
            }
            List<OutputConsumptionModel> range4 = OutputConsumptions.GetRange(0, OutputConsumptions.Count);
            int num2 = 0;
            foreach (OutputConsumptionModel item in range4)
            {
                WorkOrderModel currWorkOrder = range.FirstOrDefault((WorkOrderModel f) => f.工单号码 == item.工单号码);
                if (currWorkOrder == null)
                {
                    list.Add($"工单产出耗用表 {num2 + 1}项，工单号码：{item.工单号码}，在工单表中不存在");
                }
                else
                {
                    string parts = item.料号.Substring(0, 3);
                    PartNumberModel partNumber = range3.FirstOrDefault((PartNumberModel f) => f.工单类型 == currWorkOrder.工单类型 && f.料号前缀 == parts);
                    if (partNumber == null)
                    {
                        list.Add($"工单产出耗用表 {num2 + 1}项，工单类型：{currWorkOrder.工单类型} 料号前缀：{parts}，在料号定义表中不存在");
                    }
                    if (partNumber != null)
                    {
                        HandbookModel handbook = range2.FirstOrDefault((HandbookModel f) => f.工单类型 == currWorkOrder.工单类型 && f.保税库位 == item.库位);
                        if (handbook == null)
                        {
                            list.Add($"工单产出耗用表 {num2 + 1}项，工单类型：{currWorkOrder.工单类型} 保税库位：{item.库位}，在手册表中不存在");
                        }
                    }
                    UnitsModel units = Unitss.FirstOrDefault((UnitsModel f) => f.单位 == item.单位);
                    if (units == null)
                    {
                        list.Add($"工单产出耗用表 {num2 + 1}项，单位：{units.单位}，在单位定义表中不存在");
                    }
                }
                num2++;
            }
            return list;
        }
    }
}
