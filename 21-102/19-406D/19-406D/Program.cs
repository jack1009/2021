using HslCommunication;
using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace _19_406D
{
    class Program
    {
        static MelsecMcNet myPLC = null;
        static string[] HtmlScrewDriver = new string[] { "", "" };
        static void Main(string[] args)
        {
            InitialFlow();
            OperateResult opr = myPLC.ConnectServer();
            while (!opr.IsSuccess)
            {
                Console.WriteLine("Waitting for connect to PLC...... " + DateTime.Now.ToString("HH:mm:ss.fff"));
                opr = myPLC.ConnectServer();
                Thread.Sleep(1000);
            }
            Console.WriteLine("PLC connect is success!! " + DateTime.Now.ToString("HH:mm:ss.fff"));
            while (opr.IsSuccess)
            {
                bool[] bb = myPLC.ReadBool("B0", 5).Content;
                Thread.Sleep(300);
                //st2 get torque
                //get screw torque table 2
                if (bb[1])
                {
                    Console.WriteLine("get st2 Torque:");
                    ushort filename = myPLC.ReadUInt16("W100").Content;
                    Thread.Sleep(1000);
                    string str = GetScrewTorque(HtmlScrewDriver[0], filename);
                    Console.WriteLine("ST2 Torque:"+str);
                    myPLC.Write("W0", str);
                    Thread.Sleep(300);
                    myPLC.Write("B1", false);
                    Thread.Sleep(300);
                }
                //第2站存最終位罝
                //save the finial position table 2
                if (bb[3])
                {
                    SaveFinialPositionS2();
                    myPLC.Write("B3", false);
                    Thread.Sleep(300);
                }
                
                //第3站取torque
                //get screw torque table 3
                if (bb[2])
                {
                    Console.WriteLine("get st3Torque:");
                    ushort filename = myPLC.ReadUInt16("W101").Content;
                    Thread.Sleep(1000);
                    string str = GetScrewTorque(HtmlScrewDriver[1], filename);
                    Console.WriteLine("ST3 Torque:" + str);
                    myPLC.Write("W6", str);
                    Thread.Sleep(300);
                    myPLC.Write("B2", false);
                    Thread.Sleep(300);
                }
                //第3站存最終位罝
                //save the finial position table 3
                if (bb[4])
                {
                    SaveFinialPositionS3();
                    myPLC.Write("B4", false);
                    Thread.Sleep(300);
                }
                Thread.Sleep(300);
            }
        }
        //PLC初始化
        static void InitialFlow()
        {
            Console.WriteLine("Run Initial funtion!");
            try
            {
                string PLCInitFileName = @"D:\Resource\PLCInitialData.txt";
                string[] PLCData = File.ReadAllLines(PLCInitFileName);
                Array.Resize(ref PLCData, 4);
                myPLC = new MelsecMcNet(PLCData[0], Convert.ToInt32(PLCData[1]));
                HtmlScrewDriver[0] = PLCData[2];
                HtmlScrewDriver[1] = PLCData[3];
                Console.WriteLine("Initial funtion opened!");
            }
            catch (Exception err)
            {
                Console.WriteLine("open initial file error!!:" + err.ToString());
            }
            
        }
        //菜單
        //取得鎖付資料
        //Get screw data
        static string GetScrewTorque(string inAdd, ushort iFileName)
        {
            ushort _filename = iFileName;
            string strAdd = inAdd;
            Console.WriteLine("Run get screw torque funtion: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                HttpWebRequest myWR = HttpWebRequest.CreateHttp(strAdd);
                myWR.Method = "GET";
                myWR.Timeout = 3000;
                HttpWebResponse myHWR = (HttpWebResponse)myWR.GetResponse();
                Stream myStream = myHWR.GetResponseStream();
                StreamReader mySR = new StreamReader(myStream);
                string strHtml0 = mySR.ReadLine();
                string strHtml1 = mySR.ReadLine();
                SaveScrewData(strHtml1, _filename.ToString());
                string[] arraystrHtml = strHtml1.Split(',');
                int _Length = arraystrHtml.GetLength(0);
                myHWR.Close();
                myStream.Close();
                mySR.Close();
                if (_Length > 12)
                {
                    Console.WriteLine("Get screw data : " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    return arraystrHtml[12];
                }
                else
                {
                    Console.WriteLine("Did not get screw data : Because under 12 lines!!" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    return "None";
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("It have some error when get rundown!! " + err.ToString() + " : " + DateTime.Now.ToString("HH:mm:ss.fff"));
                return "None";
            }
        }
        //儲存鎖付資料
        //Save screw data
        static void SaveScrewData(string iScrewRundown, string iFileName)
        {
            try
            {
                Console.WriteLine("Save screw data :" + DateTime.Now.ToString("HH:mm:ss.fff"));
                string pathScrewData = "";
                string _filename = iFileName;
                string filePath = @"D:\ScrewData\" + DateTime.Now.ToString("yyyyMMdd");
                pathScrewData = filePath + @"\" + _filename + @".txt";
                bool fe = Directory.Exists(filePath);
                if (!fe)
                {
                    Directory.CreateDirectory(filePath);
                }
                List<string> lstr = new List<string>();
                lstr.Add(iScrewRundown);
                File.AppendAllLines(pathScrewData, lstr);
                Console.WriteLine("Screw data saved!! :" + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            catch (Exception err)
            {
                Console.WriteLine("Save screw data error!! " + err.ToString() + " : " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            
        }
        //Save Finial Position S2
        static void SaveFinialPositionS2()
        {
            try
            {
                Console.WriteLine("Save S2 finial position: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                //create file path
                string filePath = @"D:\FinialPositionLog\S2\";
                string allFileName = filePath + DateTime.Now.ToString("yyyyMMdd") + @".txt";
                bool fe = Directory.Exists(filePath);
                if (!fe)
                {
                    Directory.CreateDirectory(filePath);
                }
                //get data
                int[] finialPosition = new int[15];
                string allText = "";
                for (int i = 0; i < 15; i++)
                {
                    finialPosition[i] = myPLC.ReadInt32("R" + (3514 + i * 8).ToString()).Content;
                    allText = allText + finialPosition[i].ToString() + ",";
                }
                allText = allText + "\r\n";
                File.AppendAllText(allFileName, allText);
                Console.WriteLine("S2 finial position saved!! " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            catch (Exception err)
            {
                Console.WriteLine("Save finial positiion s2 error!! " + err.ToString() + " : " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            
        }
        //Save Finial Position S3
        static void SaveFinialPositionS3()
        {
            try
            {
                Console.WriteLine("Save S3 finial position: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                //create file path
                string filePath = @"D:\FinialPositionLog\S3\";
                string allFileName = filePath + DateTime.Now.ToString("yyyyMMdd") + @".txt";
                bool fe = Directory.Exists(filePath);
                if (!fe)
                {
                    Directory.CreateDirectory(filePath);
                }
                //get data
                int[] finialPosition = new int[15];
                string allText = "";
                for (int i = 0; i < 15; i++)
                {
                    finialPosition[i] = myPLC.ReadInt32("R" + (3714 + i * 8).ToString()).Content;
                    allText = allText + finialPosition[i].ToString() + ",";
                }
                allText = allText + "\r\n";
                File.AppendAllText(allFileName, allText);
                Console.WriteLine("S3 finial position saved!! " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            catch (Exception err)
            {
                Console.WriteLine("Save finial positiion s3 error!! " + err.ToString() + " : " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            
        }
    }
}
