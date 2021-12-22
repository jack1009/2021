using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Melsec;
using System.Threading;

namespace _21_805
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer t500ms;
        MelsecMcNet plc1;
        SaveData sData;
        short ScanCount;
        public Form1()
        {
            InitializeComponent();
            DataInitial();
            PLCInitial();
            TimerInitial();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t500ms.Stop();
            t500ms.Dispose();
            plc1.ConnectClose();
            plc1.Dispose();
        }
        private void DataInitial()
        {
            sData = new SaveData();
            showBarcodeLabel();
        }
        private void PLCInitial()
        {
            plc1 = new MelsecMcNet("10.5.3.116", 5001);
        }
        private void TimerInitial()
        {
            t500ms = new System.Windows.Forms.Timer();
            t500ms.Interval = 500;
            t500ms.Tick += T500ms_Tick;
            t500ms.Start();
        }
        private void T500ms_Tick(object sender, EventArgs e)
        {
            t500ms.Stop();
        #region Connet PLC
        mark1:
            OperateResult<Int16> readInt16 = plc1.ReadInt16("D1000");
            if (readInt16.IsSuccess)
            {
                ScanCount = readInt16.Content;
            }
            else
            {
                goto mark1;
            }
        mark2:
            OperateResult<Int16[]> readInt16Array = plc1.ReadInt16("W0", 48);
            List<short> readData = new List<short>();
            if (readInt16Array.IsSuccess)
            {
                foreach (var x in readInt16Array.Content)
                {
                    readData.Add(x);
                }
            }
            else
            {
                goto mark2;
            }
        mark3:
            OperateResult<Int16[]> writeDataArray = plc1.ReadInt16("W100", 16);
            List<short> writeData = new List<short>();
            //取得barcode
            if (writeDataArray.IsSuccess)
            {
                foreach (var x in writeDataArray.Content)
                {
                    writeData.Add(x);
                }
            }
            else
            {
                goto mark3;
            }
            #endregion
            //barcode傳入
            if (readData[0]==1)
            {
                int r = Convert.ToInt16(Math.Ceiling((double)readData[1] / 2 - 1));
                short[] barcode = new short[1024];
                short[] bs = readData.ToArray();
                Array.ConstrainedCopy(bs, 2, barcode, 0, r);
                Array.Resize(ref barcode, r);
                string getBarcode = BarcodeRecevied(barcode);
                //barcode1
                if (readData[1]-2 == 29)
                {
                    sData.MotorBarcode = getBarcode;
                }
                else
                {
                    //barcode2
                    if (readData[1] - 2 == 25)
                    {
                        sData.WormGearBarcode = getBarcode;
                    }
                    else
                    {
                        //barcode3
                        if (readData[1] - 2 == 22)
                        {
                            sData.BIBarcode = getBarcode;
                        }
                    }
                }
                showBarcodeLabel();
            mark4:
                OperateResult w = plc1.Write("W100", 1);
                if (w.IsSuccess)
                {
                    ;
                }
                else
                {
                    goto mark4;
                }
            }
            else
            {
                //清除finish flag
                if (writeData[0] == 1)
                {
                mark5:
                    OperateResult w = plc1.Write("W100", 0);
                    if (w.IsSuccess)
                    {
                        ;
                    }
                    else
                    {
                        goto mark5;
                    }
                }
            }
           
            //取得screw data
            if (readData[32]==1)
            {
                getScrewData();
                mark6:
                OperateResult w = plc1.Write("W101", 1);
                if (w.IsSuccess)
                {
                    ;
                }
                else
                {
                    goto mark6;
                }
            }
            else
            {
                //清除finish  flag
                if (writeData[1] == 1)
                {
                    mark7:
                    OperateResult w = plc1.Write("W101", 0);
                    if (w.IsSuccess)
                    {
                        ;
                    }
                    else
                    {
                        goto mark7;
                    }
                }
            }

            //清除資料
            if (readData[33]==1)
            {
                clearData();
                showBarcodeLabel();
                mark8:
                OperateResult w = plc1.Write("W21", 0);
                if (w.IsSuccess)
                {
                    ;
                }
                else
                {
                    goto mark8;
                }
            }
            //檢查barcode數量
            if (sData.MotorBarcode!="" && sData.WormGearBarcode!="" && sData.BIBarcode!="")
            {
                rp1:
                OperateResult result = plc1.Write("D1000", 1);
                if (!result.IsSuccess)
                {
                    goto rp1;
                }
            }
            else
            {
            rp2:
                OperateResult result = plc1.Write("D1000", 0);
                if (!result.IsSuccess)
                {
                    goto rp2;
                }
            }
            t500ms.Start();
        }
        //資料顯示
        private void showListbox()
        {
            string s = sData.MotorBarcode + "," + sData.WormGearBarcode + "," + sData.BIBarcode + "," + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
            foreach (var x in sData.ScrewDatas)
            {
                s += x.stringTotaljudgment.ToString() + "," +  x.TorqueValue.ToString() + "," + ((double)x.AngleValue/100000).ToString() + ",";
            }
            s += Environment.NewLine;
            listBox1.Items.Add(s);
        }
        //取得鎖付資料
        private void getScrewData()
        {
            UInt16 count;
            List<ScrewData> sds = new List<ScrewData>();
        #region Connet PLC
        go1:
            OperateResult<UInt16> read = plc1.ReadUInt16("R9017");
            if (read.IsSuccess)
            {
                count = read.Content;
            }
            else
            {
                goto go1;
            }
            #endregion
            for (int i = 0; i < count; i++)
            {
                ScrewData sd = new ScrewData();
            go2:
                OperateResult<bool[]> result = plc1.ReadBool($"L{(i + 1) * 3}", 3);
                if (result.IsSuccess)
                {
                    bool[] bits = result.Content;
                    sd.TotalJudgment = bits[0];
                    sd.NgStrip = bits[1];
                    sd.NGNotOnseat = bits[2];
                }
                else
                {
                    goto go2;
                }
            go3:
                OperateResult<float> result1 = plc1.ReadFloat($"R{(i + 1) * 4 + 30000}");
                if (result1.IsSuccess)
                {
                    float d = result1.Content;
                    sd.TorqueValue = d;
                }
                else
                {
                    goto go3;
                }
            go4:
                OperateResult<Int32> result2 = plc1.ReadInt32($"R{(i + 1) * 4 + 30002}");
                if (result2.IsSuccess)
                {
                    Int32 i32 = result2.Content;
                    sd.AngleValue = i32;
                }
                else
                {
                    goto go4;
                }
                sds.Add(sd);
            }
            sData.ScrewDatas = sds;
            showListbox();
            sData.SaveFile();
            clearData();
            showBarcodeLabel();
        }
        private void clearData()
        {
            sData.MotorBarcode = "";
            sData.WormGearBarcode = "";
            sData.BIBarcode = "";
            sData.ScrewDatas.Clear();
        }
        private void showBarcodeLabel()
        {
            label2.Text = sData.MotorBarcode;
            label3.Text = sData.WormGearBarcode;
            label5.Text = sData.BIBarcode;
        }
        //轉成字串
        private string BarcodeRecevied(short[] data)
        {
            byte[] bs =new byte[data.Length*2];
            for (int i = 0; i < data.Length; i++)
            {
                bs[i*2+0] = (byte)(data[i] & 0x00FF);
                bs[i * 2 + 1] = (byte)(data[i] >> 8);
            }
            string s = Encoding.ASCII.GetString(bs);
            return s;
        }
    }
}
