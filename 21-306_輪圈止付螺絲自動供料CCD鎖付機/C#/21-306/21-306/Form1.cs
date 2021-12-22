using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using HslCommunication;
using HslCommunication.Profinet.Melsec;

namespace _21_306
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitialPlc();
            if (!Directory.Exists(@"D:\ScrewData\"))
            {
                Directory.CreateDirectory(@"D:\ScrewData\");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t1s.Abort();
        }
        #region UI不同執行緒顯示
        delegate void LabelAppendCallBack(Label Ctl, string Str);
        delegate void ListBoxAppendCallBack(ListBox Ctl, string Str);
        private void AppendListBoxText(ListBox ctr, string str)
        {
            if (ctr.InvokeRequired)
            {
                ListBoxAppendCallBack d = new ListBoxAppendCallBack(AppendListBoxText);
                this.Invoke(d, new object[] { ctr, str });
            }
            else
            {
                ctr.Items.Add(str);
            }
        }
        private void showLabelText(Label ctr, string str)
        {
            if (ctr.InvokeRequired)
            {
                LabelAppendCallBack d = new LabelAppendCallBack(showLabelText);
                this.Invoke(d, new object[] { ctr, str });
            }
            else
            {
                ctr.Text = str;
            }
        }
        #endregion
        #region PLC
        MelsecMcNet Fx5u;
        Thread t1s;
        private void InitialPlc()
        {
            Fx5u = new MelsecMcNet("10.5.43.100", 1000);
            Fx5u.ConnectServer();
            Thread.Sleep(3000);
            t1s = new Thread(ReadPLC);
            t1s.Start();
        }
        private void ReadPLC()
        {
            while (true)
            {
                ushort startflag = Fx5u.ReadUInt16("W0").Content;
                ushort connection = Fx5u.ReadUInt16("W1").Content;
                showLabelText(label1, startflag.ToString());
                showLabelText(label4, connection.ToString());
                if (startflag>0)
                {
                    listBox1.Items.Clear();
                    List<ScrewData> screwDatas=ReadScrewData();
                    if (screwDatas!=null)
                    {
                        SaveScrewData(screwDatas);
                    }
                    Fx5u.Write("W0", 0);
                }
                Thread.Sleep(1000);
            }
        }
        //讀取鎖付資料
        private List<ScrewData> ReadScrewData()
        {
            List<ScrewData> screwDatas = new List<ScrewData>();
            int countM = 1500;
            int countD = 1500;
            for (int i = 0; i < 30; i++)
            {
                bool[] bs = Fx5u.ReadBool($"M{countM+i*4}", 4).Content;
                int[] torque = Fx5u.ReadInt32($"D{countD+i*2}", 1).Content;
                ScrewData sd = new ScrewData();
                if (bs != null && torque!= null)
                {
                    sd.CCDTrigerNG = bs[0];
                    sd.ScrewStrip = bs[1];
                    sd.ScrewLower = bs[2];
                    sd.ScrewHigher = bs[3];
                    sd.ScrewTorque = torque[0];
                    if (sd.ScrewTorque!=0)
                    {
                        screwDatas.Add(sd);
                    }
                }
            }
            return screwDatas;
        }
        //鎖付資料存檔
        private void SaveScrewData(List<ScrewData> screwDatas)
        {
            string filepath = $"D:/ScrewData/{DateTime.Now.ToString("yyMMddHHmmss")}.csv";
            File.AppendAllText(filepath, "Index,Screw Status,Screw Torque"+Environment.NewLine);
            List<string> datas = new List<string>();
            int count = 1;
            foreach (var x in screwDatas)
            {
                string data = count.ToString() + "," + x.GetSaveData();
                AppendListBoxText(listBox1, data);
                datas.Add(data);
                count += 1;
            }
            File.AppendAllLines(filepath, datas,System.Text.Encoding.UTF8);
        }
        #endregion
    }
}
