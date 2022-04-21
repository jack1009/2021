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
using Timer = System.Windows.Forms.Timer;
using System.IO;

namespace _21_910
{
    public partial class Form1 : Form
    {
        PoriteDBModelContext _db = new PoriteDBModelContext();
        MelsecMcNet Fx5_908 = new MelsecMcNet("10.5.13.107", 5002);
        MelsecMcNet Fx5_909 = new MelsecMcNet("10.5.13.108", 5002);
        MelsecMcNet Fx5_910 = new MelsecMcNet("10.5.13.109", 5002);
        Thread t910,t909,t908;
        int ConnectStatus908, ConnectStatus909, ConnectStatus910;
        int LastIDMC908, LastIDMC909, LastIDMC910;
        public Form1()
        {
            InitializeComponent();
            InitialTimer();
        }
        private void InitialTimer()
        {
            Fx5_908.ConnectTimeOut = 3000;
            Fx5_909.ConnectTimeOut = 3000;
            Fx5_910.ConnectTimeOut = 3000;
            t910 = new Thread(T910_Tick);
            t909 = new Thread(T909_Tick);
            t908 = new Thread(T908_Tick);
            t910.IsBackground = true;
            t909.IsBackground = true;
            t908.IsBackground = true;
            t910.Start();
            t909.Start();
            t908.Start();
        }
        private void SaveData(IQueryable<MC910Table> tables,DateTime date)
        {
            string savetext = "QR Code,測試日期時間,測試結果,測試角度,測試扭力" + Environment.NewLine;
            foreach (var item in tables)
            {
                savetext += item.QRCode + "," + item.TestingDateTime + "," + item.JudgmentResult + "," + item.TestingAngle + "," + item.TestingTorque + Environment.NewLine;
            }
            File.WriteAllText($"D:/DailyReport/扭力測試機/{date.ToString("yyyyMMdd")}.csv", savetext,Encoding.Default);
        }
        private void SaveData(IEnumerable<MC909Table> tables,DateTime date)
        {
            string savetext = "QR Code,測試日期時間,正轉測試結果,反轉測試結果,正轉峰值速度,反轉峰值速度,正轉峰值電流,反轉峰值電流,正轉平均速度,反轉平均速度,正轉平均電流,反轉平均電流" + Environment.NewLine;
            foreach (var item in tables)
            {
                savetext += item.QRCode + "," + item.TestingDateTime + ","
                    + item.CCWJudgmentResult + "," + item.CWJudgmentResult + ","
                    + item.CCWSpeed + "," + item.CWSpeed + ","
                    + item.CCWCurrent + "," + item.CWCurrent + ","
                    + item.CCWAvgSpeed + "," + item.CWAvgSpeed + ","
                    + item.CCWAvgCurrent + "," + item.CWAvgCurrent + Environment.NewLine;
            }
            File.WriteAllText($"D:/DailyReport/無負載測試機/{date.ToString("yyyyMMdd")}.csv", savetext,Encoding.Default);
        }
        private void SaveData(IEnumerable<MC908Table> tables,DateTime date)
        {
            string savetext = "QR Code,測試日期時間,正轉測試結果,反轉測試結果,正轉峰值速度,反轉峰值速度,正轉峰值電流,反轉峰值電流,正轉平均速度,反轉平均速度,正轉平均電流,反轉平均電流" + Environment.NewLine;
            foreach (var item in tables)
            {
                savetext += item.QRCode + "," + item.TestingDateTime + ","
                     + item.CCWJudgmentResult + "," + item.CWJudgmentResult + ","
                     + item.CCWSpeed + "," + item.CWSpeed + ","
                     + item.CCWCurrent + "," + item.CWCurrent + ","
                     + item.CCWAvgSpeed + "," + item.CWAvgSpeed + ","
                     + item.CCWAvgCurrent + "," + item.CWAvgCurrent + Environment.NewLine;
            }
            File.WriteAllText($"D:/DailyReport/有負載測試機/{date.ToString("yyyyMMdd")}.csv", savetext, Encoding.Default);
        }
        private void SaveDailyReport(DateTime date)
        {
            DateTime today = date.Date;
            DateTime today_1 = date.AddDays(1);
            using (PoriteDBModelContext db=new PoriteDBModelContext())
            {
                try
                {
                    IQueryable<MC908Table> x = db.MC908Table.Where(p => p.TestingDateTime >= today && p.TestingDateTime<today_1);
                    IQueryable<MC909Table> y = db.MC909Table.Where(p => p.TestingDateTime >= today && p.TestingDateTime < today_1);
                    IQueryable<MC910Table> z = db.MC910Table.Where(p => p.TestingDateTime >= today && p.TestingDateTime < today_1);

                    Directory.CreateDirectory(@"D:\DailyReport\有負載測試機\");
                    Directory.CreateDirectory(@"D:\DailyReport\無負載測試機\");
                    Directory.CreateDirectory(@"D:\DailyReport\扭力測試機\");
                    SaveData(x,date);
                    SaveData(y,date);
                    SaveData(z,date);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("發生異常" + ex.ToString());
                }
            }
        }
        private void T910_Tick()
        {
            while (true)
            {
                #region --910--
                OperateResult<short[]> result = Fx5_910.ReadInt16("W0", 2);
                if (result.IsSuccess)
                {
                    ConnectStatus910 = 1;
                    LabelConnectStatus(lbConnectStatus910, ConnectStatus910);
                    short[] Cmds910 = result.Content;
                    //資料傳送flag=1
                    if (Cmds910[0] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 10000;
                        int DataCount = Cmds910[1];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC910Table table = new MC910Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_910.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                                //取得日期及總合判斷
                                mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_910.ReadInt16($"R{DataAddressBase+i*50+11}", 7);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]),DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6]==1)
                                    {
                                        table.JudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        table.JudgmentResult = "NG";
                                    }
                                    //取得角度
                                    mark4:
                                    OperateResult<int> readIntResult=Fx5_910.ReadInt32($"R{DataAddressBase + i * 50 + 18}");
                                    if (!readIntResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        table.TestingAngle = (float)readIntResult.Content / 100000;
                                    //取得扭力
                                    mark5:
                                        OperateResult<float> readFloatResult = Fx5_910.ReadFloat($"R{DataAddressBase + i * 50 + 20}");
                                        if (!readFloatResult.IsSuccess)
                                        {
                                            goto mark5;
                                        }
                                        else
                                        {
                                            //得到扭力
                                            table.TestingTorque = Math.Round(readFloatResult.Content, 3, MidpointRounding.AwayFromZero);
                                            try
                                            {
                                                int count = _db.MC910Table.Count();
                                                if (count == 0)
                                                {
                                                    LastIDMC910 = 0;
                                                    table.ID = LastIDMC910 + 1;
                                                    _db.MC910Table.Add(table);
                                                    _db.SaveChanges();
                                                }
                                                else
                                                {
                                                    LastIDMC910 = _db.MC910Table.OrderByDescending(p => p.ID).First().ID;
                                                    table.ID = LastIDMC910 + 1;
                                                    _db.MC910Table.Add(table);
                                                    _db.SaveChanges();
                                                }
                                            //清除flag
                                            mark1:
                                                Int16[] ws = new Int16[2] { 0, 0 };
                                                OperateResult writeResult = Fx5_910.Write("W0", ws);
                                                if (!writeResult.IsSuccess)
                                                {
                                                    goto mark1;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ConnectStatus910 = 0;
                    LabelConnectStatus(lbConnectStatus910, ConnectStatus910);
                }
                #endregion
                //=====================================
                //存檔檢查
                if (DateTime.Now.Hour==23 && DateTime.Now.Minute==59 && DateTime.Now.Second==59)
                {
                    SaveDailyReport(DateTime.Today);
                }
                Thread.Sleep(1000);
            }
        }

        private void T909_Tick()
        {
            while (true)
            {
                #region --909--
                OperateResult<short[]> result = Fx5_909.ReadInt16("W0", 8);
                if (result.IsSuccess)
                {
                    ConnectStatus909 = 1;
                    LabelConnectStatus(lbConnectStatus909, ConnectStatus909);
                    short[] Cmds909 = result.Content;
                    #region 模具1
                    //資料傳送flag=1
                    if (Cmds909[0] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 10000;
                        int DataCount = Cmds909[1];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC909Table table = new MC909Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_909.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_909.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_909.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度,電流
                                        float[] Value = readFloatsResult.Content;
                                        
                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC909Table.Count();
                                            if (count==0)
                                            {
                                                LastIDMC909 = 0;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC909 = _db.MC909Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_909.Write("W0", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具2
                    //資料傳送flag=1
                    if (Cmds909[2] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 12500;
                        int DataCount = Cmds909[3];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC909Table table = new MC909Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_909.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_909.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_909.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;

                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC909Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC909 = 0;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC909 = _db.MC909Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_909.Write("W2", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具3
                    //資料傳送flag=1
                    if (Cmds909[4] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 15000;
                        int DataCount = Cmds909[5];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC909Table table = new MC909Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_909.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_909.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_909.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;

                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC909Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC909 = 0;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC909 = _db.MC909Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_909.Write("W4", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具4
                    //資料傳送flag=1
                    if (Cmds909[6] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 17500;
                        int DataCount = Cmds909[7];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC909Table table = new MC909Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_909.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_909.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_909.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;

                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC909Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC909 = 0;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC909 = _db.MC909Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC909 + 1;
                                                _db.MC909Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_909.Write("W6", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    ConnectStatus909 = 0;
                    LabelConnectStatus(lbConnectStatus909, ConnectStatus909);
                }
                #endregion
                Thread.Sleep(1000);
            }
        }

        private void T908_Tick()
        {
            while (true)
            {
                #region --908--
                OperateResult<short[]> result = Fx5_908.ReadInt16("W0", 8);
                if (result.IsSuccess)
                {
                    ConnectStatus908 = 1;
                    LabelConnectStatus(lbConnectStatus908, ConnectStatus908);
                    short[] Cmds908 = result.Content;
                    #region 模具1
                    //資料傳送flag=1
                    if (Cmds908[0] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 10000;
                        int DataCount = Cmds908[1];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC908Table table = new MC908Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_908.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_908.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_908.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;

                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC908Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC908 = 0;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC908 = _db.MC908Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            //清除flag
                                            markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_908.Write("W0", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具2
                    //資料傳送flag=1
                    if (Cmds908[2] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 12500;
                        int DataCount = Cmds908[3];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC908Table table = new MC908Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_908.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_908.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_908.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;

                                        table.CCWSpeed = Math.Round(Value[0], 3, MidpointRounding.AwayFromZero);
                                        table.CWSpeed = Math.Round(Value[1], 3, MidpointRounding.AwayFromZero);
                                        table.CCWCurrent = Math.Round(Value[2], 3, MidpointRounding.AwayFromZero);
                                        table.CWCurrent = Math.Round(Value[3], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgSpeed = Math.Round(Value[4], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgSpeed = Math.Round(Value[5], 3, MidpointRounding.AwayFromZero);
                                        table.CCWAvgCurrent = Math.Round(Value[6], 3, MidpointRounding.AwayFromZero);
                                        table.CWAvgCurrent = Math.Round(Value[7], 3, MidpointRounding.AwayFromZero);
                                        try
                                        {
                                            int count = _db.MC908Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC908 = 0;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC908 = _db.MC908Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_908.Write("W2", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具3
                    //資料傳送flag=1
                    if (Cmds908[4] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 15000;
                        int DataCount = Cmds908[5];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC908Table table = new MC908Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_908.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_908.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_908.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;
                                        int[] iValue = new int[Value.Length];
                                        for (int ix = 0; ix < Value.Length; ix++)
                                        {
                                            iValue[ix] = (int)(Value[ix] * 1000);
                                            float ff = (float)iValue[ix];
                                            Value[ix] = ff / 1000;
                                        }
                                        table.CCWSpeed = Value[0];
                                        table.CWSpeed = Value[1];
                                        table.CCWCurrent = Value[2];
                                        table.CWCurrent = Value[3];
                                        table.CCWAvgSpeed = Value[4];
                                        table.CWAvgSpeed = Value[5];
                                        table.CCWAvgCurrent = Value[6];
                                        table.CWAvgCurrent = Value[7];
                                        try
                                        {
                                            int count = _db.MC908Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC908 = 0;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC908 = _db.MC908Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_908.Write("W4", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 模具4
                    //資料傳送flag=1
                    if (Cmds908[6] == 1)
                    {
                        //取得資料
                        int DataAddressBase = 17500;
                        int DataCount = Cmds908[7];
                        for (int i = 0; i < DataCount; i++)
                        {
                            MC908Table table = new MC908Table();
                        //取得條碼
                        mark2:
                            OperateResult<string> readStringResult = Fx5_908.ReadString($"R{DataAddressBase + i * 50 + 0}", 8);
                            if (!readStringResult.IsSuccess)
                            {
                                goto mark2;
                            }
                            else
                            {
                                //得到條碼
                                table.QRCode = readStringResult.Content.Trim();
                            //取得日期及總合判斷
                            mark3:
                                OperateResult<Int16[]> readInt16Result = Fx5_908.ReadInt16($"R{DataAddressBase + i * 50 + 11}", 8);
                                if (!readInt16Result.IsSuccess)
                                {
                                    goto mark3;
                                }
                                else
                                {
                                    //得到日期及總合判斷
                                    Int16[] i16 = readInt16Result.Content;
                                    //日期
                                    DateTime dt = new DateTime(Convert.ToInt32(i16[0]), Convert.ToInt32(i16[1]), Convert.ToInt32(i16[2]), Convert.ToInt32(i16[3]), Convert.ToInt32(i16[4]), Convert.ToInt32(i16[5]), DateTimeKind.Local);
                                    table.TestingDateTime = dt;
                                    //總合判斷
                                    if (i16[6] == 1)
                                    {
                                        table.CCWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[6] == 2)
                                        {
                                            table.CCWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[6] == 3)
                                            {
                                                table.CCWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[6] == 4)
                                                {
                                                    table.CCWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CCWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                    if (i16[7] == 1)
                                    {
                                        table.CWJudgmentResult = "OK";
                                    }
                                    else
                                    {
                                        if (i16[7] == 2)
                                        {
                                            table.CWJudgmentResult = "SNG";
                                        }
                                        else
                                        {
                                            if (i16[7] == 3)
                                            {
                                                table.CWJudgmentResult = "CNG";
                                            }
                                            else
                                            {
                                                if (i16[7] == 4)
                                                {
                                                    table.CWJudgmentResult = "ANG";
                                                }
                                                else
                                                {
                                                    table.CWJudgmentResult = "NG";
                                                }
                                            }
                                        }
                                    }
                                //取得速度,電流
                                mark4:
                                    OperateResult<float[]> readFloatsResult = Fx5_908.ReadFloat($"R{DataAddressBase + i * 50 + 19}", 8);
                                    if (!readFloatsResult.IsSuccess)
                                    {
                                        goto mark4;
                                    }
                                    else
                                    {
                                        //得到角度
                                        float[] Value = readFloatsResult.Content;
                                        int[] iValue = new int[Value.Length];
                                        for (int ix = 0; ix < Value.Length; ix++)
                                        {
                                            iValue[ix] = (int)(Value[ix] * 1000);
                                            float ff = (float)iValue[ix];
                                            Value[ix] = ff / 1000;
                                        }
                                        table.CCWSpeed = Value[0];
                                        table.CWSpeed = Value[1];
                                        table.CCWCurrent = Value[2];
                                        table.CWCurrent = Value[3];
                                        table.CCWAvgSpeed = Value[4];
                                        table.CWAvgSpeed = Value[5];
                                        table.CCWAvgCurrent = Value[6];
                                        table.CWAvgCurrent = Value[7];
                                        try
                                        {
                                            int count = _db.MC908Table.Count();
                                            if (count == 0)
                                            {
                                                LastIDMC908 = 0;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                LastIDMC908 = _db.MC908Table.OrderByDescending(p => p.ID).First().ID;
                                                table.ID = LastIDMC908 + 1;
                                                _db.MC908Table.Add(table);
                                                _db.SaveChanges();
                                            }
                                        //清除flag
                                        markJig1:
                                            Int16[] J1ws = new Int16[2] { 0, 0 };
                                            OperateResult J1writeResult = Fx5_908.Write("W6", J1ws);
                                            if (!J1writeResult.IsSuccess)
                                            {
                                                goto markJig1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    ConnectStatus908 = 0;
                    LabelConnectStatus(lbConnectStatus908, ConnectStatus908);
                }
                #endregion
                Thread.Sleep(1000);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDailyReport(DateTime.Today);
        }

        private void pbSaveData_Click(object sender, EventArgs e)
        {
            DateTime d = dateTimePicker1.Value;
            SaveDailyReport(d);
        }
        #region --UI--
        delegate void dgLabelConnectStatus(Label ctl,int state);
        private void LabelConnectStatus(Label ctl,int state)
        {
            if (ctl.InvokeRequired)
            {
                dgLabelConnectStatus d = new dgLabelConnectStatus(LabelConnectStatus);
                this.Invoke(d, new object[] { ctl, state });
            }
            else
            {
                if (state==1)
                {
                    ctl.BackColor = Color.Green;
                    ctl.Text = "連線中";
                }
                else
                {
                    ctl.BackColor = Color.Red;
                    ctl.Text = "未連線";
                }
            }
        }

        delegate void dgLabelShowData(Label ctl, string text);
        private void LabelShowData(Label ctl,string text)
        {
            if (ctl.InvokeRequired)
            {
                dgLabelShowData d = new dgLabelShowData(LabelShowData);
                this.Invoke(d, new object[] { ctl, text });
            }
            else
            {
                ctl.Text = text;
            }
        }

        #endregion
        #region 無用
        #endregion
    }
}
