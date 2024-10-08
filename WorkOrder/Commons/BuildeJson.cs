using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOrder.Models.Outputs;
using WorkOrder.Models;
using Newtonsoft.Json;
using WorkOrder.ViewModel;

namespace WorkOrder.Commons
{
    public class BuildeJson
    {
        public static string GetJson(List<HandbookModel> Handbooks, List<PartNumberModel> PartNumbers, List<UnitsModel> Unitss, List<WorkOrderModel> WorkOrders, List<OutputConsumptionModel> OutputConsumptions)
        {
            List<WorkOrderModel> range = WorkOrders.GetRange(0, WorkOrders.Count);
            List<HandbookModel> range2 = Handbooks.GetRange(0, Handbooks.Count);
            List<PartNumberModel> range3 = PartNumbers.GetRange(0, PartNumbers.Count);
            List<OutputConsumptionModel> range4 = OutputConsumptions.GetRange(0, OutputConsumptions.Count);
            List<UnitsModel> range5 = Unitss.GetRange(0, Unitss.Count);
            JsonModel jsonModel = new JsonModel();
            JsonZipModel.GetZipFileName.Clear();
            foreach (HandbookModel handbook in Handbooks)
            {
                JsonWorkOrder WorkOrderData = new JsonWorkOrder();
                WorkOrderData.woNo = range.FirstOrDefault((WorkOrderModel f) => f.工单类型 == handbook.工单类型)?.工单号码;
                WorkOrderData.tradeCode = handbook.企业编码;
                string value = range.FirstOrDefault((WorkOrderModel f) => f.工单类型 == handbook.工单类型)?.工单日期;
                WorkOrderData.woDate = Convert.ToDateTime(value).ToString("yyyyMMdd");
                WorkOrderData.emsNo = handbook.账册编号;
                IEnumerable<JsonWorkOrderInput> source = from w in range4.Where((OutputConsumptionModel w) => w.工单号码 == WorkOrderData.woNo).ToList()
                                                         join p in range3.Where((PartNumberModel p) => p.工单类型.Equals(handbook.工单类型) && p.产出耗用 == "I").ToList() on w.料号.Substring(0, 3) equals p.料号前缀
                                                         join u in range5 on w.单位 equals u.单位
                                                         select new JsonWorkOrderInput
                                                         {
                                                             itemNo = w.料号,
                                                             qty = w.数量,
                                                             unit = u.申报计量单位,
                                                             itemType = p.物料类型
                                                         };
                IEnumerable<JsonWorkOrderOutput> source2 = from w in range4.Where((OutputConsumptionModel w) => w.工单号码 == WorkOrderData.woNo).ToList()
                                                           join p in range3.Where((PartNumberModel p) => p.工单类型.Equals(handbook.工单类型) && p.产出耗用 == "O").ToList() on w.料号.Substring(0, 3) equals p.料号前缀
                                                           join u in range5 on w.单位 equals u.单位
                                                           select new JsonWorkOrderOutput
                                                           {
                                                               itemNo = w.料号,
                                                               qty = w.数量,
                                                               unit = u.申报计量单位,
                                                               itemType = p.物料类型
                                                           };
                WorkOrderData.woInput = source.ToList();
                WorkOrderData.woOutput = source2.ToList();
                jsonModel.jsonModel.Add(WorkOrderData);
            }

            JsonZipModel.GetZipFileName.Add("WO");
            JsonZipModel.GetZipFileName.Add(jsonModel.jsonModel[0].tradeCode);
            JsonZipModel.GetZipFileName.Add(jsonModel.jsonModel[0].emsNo);
            JsonZipModel.GetZipFileName.Add(jsonModel.jsonModel[0].woDate);
            JsonZipModel.GetZipFileName.Add("001");
            JsonZipModel.GetZipFileName.Add("_");
            JsonZipModel.GetZipFileName.Add(MainViewModel.EdiNo.ToString());  ///
            JsonZipModel.GetZipFileName.Add("_");
            JsonZipModel.GetZipFileName.Add("NEPZ_WO_");
            JsonZipModel.GetZipFileName.Add(jsonModel.jsonModel[0].woDate);

            return JsonConvert.SerializeObject(jsonModel.jsonModel);
        }
    }

}
