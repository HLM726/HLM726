using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Windows;
using WorkOrder.Commons;
using WorkOrder.Models.Outputs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Threading.Tasks;
using MiniExcelLibs;
using WorkOrder.Models;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WorkOrder.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public static string EdiNo = "";
        private Visibility _progreState;

        private bool _checkNgBtn;

        private string _showMsg;

        private List<string> checkNg = new List<string>();

        public Visibility progreState
        {
            get
            {
                return _progreState;
            }
            set
            {
                Set(ref _progreState, value, broadcast: false, "progreState");
            }
        }

        public bool checkNgBtn
        {
            get
            {
                return _checkNgBtn;
            }
            set
            {
                Set(ref _checkNgBtn, value, broadcast: false, "checkNgBtn");
            }
        }

        public string showMsg
        {
            get
            {
                return _showMsg;
            }
            set
            {
                Set(ref _showMsg, value, broadcast: false, "showMsg");
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        public ICommand ImportExcelCommand => new RelayCommand<object>(async delegate
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                checkNgBtn = false;
                checkNg.Clear();
                openFileDialog.Title = "选择文件";
                if (openFileDialog.ShowDialog().GetValueOrDefault())
                {
                    string path = openFileDialog.FileName;
                    if (!openFileDialog.FileName.EndsWith(".xlsx"))
                    {
                        showMsg = "未选择正确文件";
                    }
                    else
                    {
                        progreState = Visibility.Visible;
                        showMsg = "处理中...";
                        await Task.Delay(1000);
                        IEnumerable<HandbookModel> Handbook = await MiniExcel.QueryAsync<HandbookModel>(path, "手册表");
                        IEnumerable<HandbookModelEdi> Edi = await MiniExcel.QueryAsync<HandbookModelEdi>(path, "手册表");
                        EdiNo= Edi.FirstOrDefault().EDI;
                        IEnumerable<PartNumberModel> PartNumber = await MiniExcel.QueryAsync<PartNumberModel>(path, "料号定义表");
                        IEnumerable<UnitsModel> Units = await MiniExcel.QueryAsync<UnitsModel>(path, "单位定义表");
                        IEnumerable<WorkOrderModel> WorkOrder = await MiniExcel.QueryAsync<WorkOrderModel>(path, "工单表");
                        IEnumerable<OutputConsumptionModel> OutputConsumption = await MiniExcel.QueryAsync<OutputConsumptionModel>(path, "工单产出耗用表", startCell: "A3");
                        List<HandbookModel> Handbooks = Handbook.ToList();
                        List<PartNumberModel> PartNumbers = PartNumber.ToList();
                        List<UnitsModel> Unitss = Units.ToList();
                        List<WorkOrderModel> WorkOrders = WorkOrder.ToList();
                        List<OutputConsumptionModel> OutputConsumptions = OutputConsumption.ToList();
                        checkNg.AddRange(DataCheck.checkSpace(Handbooks, PartNumbers, Unitss, WorkOrders, OutputConsumptions));
                        checkNg.AddRange(DataCheck.checkDateIsDisff(WorkOrders));
                        checkNg.AddRange(DataCheck.Exists(Handbooks, PartNumbers, Unitss, WorkOrders, OutputConsumptions));
                        if (checkNg.Count > 0)
                        {
                            checkNgBtn = true;
                            showMsg = "2、检查未通过，导出查看不通过结果";
                        }
                        else
                        {
                            showMsg = "3、检查通过，正在生成文件";
                            await Task.Delay(200);
                            string json = BuildeJson.GetJson(Handbooks, PartNumbers, Unitss, WorkOrders, OutputConsumptions);
                            new List<string>();
                            List<string> jsonpath = JsonZipModel.GetZipFileName;
                            SaveFile(jsonpath, json);
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                showMsg = "异常：" + ex.Message;
            }
            finally
            {
                progreState = Visibility.Collapsed;
            }
        });


        /// <summary>
        /// 错误信息保存
        /// </summary>
        public ICommand CheckExcelCommand => new RelayCommand(delegate
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "保存检查结果",
                Filter = "Text files (*.txt)|*.txt|All files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (saveFileDialog.ShowDialog().GetValueOrDefault())
            {
                File.WriteAllLines(saveFileDialog.FileName, checkNg.ToArray());
            }
        });

        public MainViewModel()
        {
            checkNgBtn = false;
            progreState = Visibility.Collapsed;
            showMsg = "1、请先择导入工单文件";
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="data">数据</param>
        private void SaveFile(List<string> fileName, string data)
        {

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //文件保存works目录下
            folderPath = Path.Combine(folderPath, "works");
            /// WorkNo_ZIP 文件
            List<string> TempZIp = fileName;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //处理json 文件
            string[] files = Directory.GetFiles(folderPath, "*_ZIP");
            //json个数当前目录
            TempZIp.RemoveAt(4);
            TempZIp.Insert(4, (files.Length + 1).ToString().PadLeft(3, '0'));
            TempZIp.Add("_JSON");
            string text = Path.Combine(folderPath, string.Join("", TempZIp));
            File.WriteAllText(text, data);
            //------------------------------
            TempZIp.RemoveAt(TempZIp.Count - 1);
            TempZIp.Add("_ZIP");

            string text2 = Path.Combine(folderPath, string.Join("", TempZIp));

            Trace.WriteLine($"WO ZIPtext2文件： {text2}，text{text}");

            if (CompressDirectoryZip(text, text2))
            {
                JsonMD5Model jsonMD5Model = BuilderMd5(new List<string> { text2 });

                TempZIp.RemoveAt(0);
                TempZIp.Insert(0, "FN");

                TempZIp.RemoveAt(TempZIp.Count - 1);
                TempZIp.Add("_JSON");

                //写入JSON -FN XXXXXX_JSON
                File.WriteAllText(text, JsonConvert.SerializeObject(jsonMD5Model.Data));
                ///文件重命名

                File.Move(text, Path.Combine(folderPath, string.Join("", TempZIp)));
                showMsg = "5、文件保存路径：" + folderPath;


            }




        }


        private bool CompressDirectoryZip(string ActurePath, string zipPath)
        {
            try
            {
                using (FileStream stream = new FileStream(zipPath, FileMode.Create))
                {
                    using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                    {
                        ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(Path.GetFileName(ActurePath));
                        using (FileStream fileStream = new FileStream(ActurePath, FileMode.Open))
                        {
                            using (Stream destination = zipArchiveEntry.Open())
                                fileStream.CopyTo(destination);
                        }


                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void CompressDirectoryZipByFolder(string FolderPath, string zipPath)
        {
            try
            {
                try
                {
                    ZipFile.CreateFromDirectory(FolderPath, zipPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("压缩过程中发生错误：" + ex.Message);
                }
            }
            catch (Exception)
            {
            }
        }

        private JsonMD5Model BuilderMd5(List<string> allFiles)
        {
            JsonMD5Model jsonMD5Model = new JsonMD5Model();
            try
            {
                if (allFiles.Count == 0)
                {
                    return null;
                }
                List<string> range = allFiles.GetRange(0, allFiles.Count);
                MD5Model mD5Model = new MD5Model();
                foreach (string item in range)
                {
                    mD5Model = new MD5Model();
                    mD5Model.fileName = new FileInfo(item).Name;
                    string path = item;
                    string md = GetMd5(path);
                    mD5Model.MD5 = md;
                    if (!IsNulltool.AreAllPropertiesNullOrEmpty(mD5Model))
                    {
                        jsonMD5Model.Data.Add(mD5Model);
                    }
                }
            }
            catch (Exception)
            {
            }
            return jsonMD5Model;
        }

        private string GetMd5(string path)
        {
            string result = "";
            try
            {
                using (MD5 mD = MD5.Create())
                {
                    using (FileStream inputStream = File.OpenRead(path))
                    {
                        byte[] array = mD.ComputeHash(inputStream);
                        result = BitConverter.ToString(array).Replace("-", "").ToLowerInvariant();
                    }

                }
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
