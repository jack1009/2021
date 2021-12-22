using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21_306
{
    class ScrewData
    {
        private double rScrewTorque;
        public bool CCDTrigerNG { get; set; }
        public bool ScrewStrip { get; set; }
        public bool ScrewLower { get; set; }
        public bool ScrewHigher { get; set; }
        public int ScrewTorque { get; set; }
        public string GetSaveData()
        {
            rScrewTorque = (double)ScrewTorque / 100.0;
            string data = "";
            if (!CCDTrigerNG && !ScrewStrip && !ScrewLower && !ScrewHigher)
            {
                   data += $"Screw OK,{rScrewTorque}NM";
            }
            else
            {
                if (CCDTrigerNG)
                {
                    data += "CCD NG";
                }
                else
                {
                    if (ScrewStrip)
                    {
                        data += "Screw strip";
                    }
                    else
                    {
                        if (ScrewLower)
                        {
                            data += $"Too low,{rScrewTorque}NM";
                        }
                        else
                        {
                            if (ScrewHigher)
                            {
                                data += $"Too high,{rScrewTorque}NM";
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}
