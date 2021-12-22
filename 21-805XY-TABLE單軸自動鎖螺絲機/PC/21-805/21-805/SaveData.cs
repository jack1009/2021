using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _21_805
{
    class SaveData
    {
        public SaveData()
        {
            MotorBarcode = "";
            WormGearBarcode = "";
            BIBarcode = "";
            ScrewDatas = new List<ScrewData>();
        }
        public string MotorBarcode { get; set; }
        public string WormGearBarcode { get; set; }
        public string BIBarcode { get; set; }
        public List<ScrewData> ScrewDatas { get; set; }
        public void SaveFile()
        {
            string filepath = $"D:/Data/{DateTime.Now.ToString("yyyyMM")}/";
            string filename = $"{MotorBarcode.Trim()}.csv";
            Directory.CreateDirectory(filepath);
            string text = MotorBarcode.Trim() + "," + WormGearBarcode.Trim() + "," + BIBarcode.Trim() + "," + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
            foreach (var x in ScrewDatas)
            {
                text += x.stringTotaljudgment + "," + x.TorqueValue.ToString() + "," + ((double)x.AngleValue/100000).ToString() + ",";
            }
            text = text + Environment.NewLine;
            File.AppendAllText(filepath + filename, text);
        }
    }
}
