using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21_805
{
    class ScrewData
    {
        private bool mTotalJudgment, mNgStrip, mNgnotOnSeat;
        public bool TotalJudgment { set {mTotalJudgment=value; } }
        public bool NgStrip { set { mNgStrip = value; } }
        public bool NGNotOnseat { set { mNgnotOnSeat = value; } }
        public string stringTotaljudgment
        {
            get
            {
                if (mTotalJudgment && !mNgStrip && !mNgnotOnSeat)
                {
                    return "鎖付OK";
                }
                else
                {
                    if (mNgStrip)
                    {
                        return "鎖付滑牙";
                    }
                    else
                    {
                        if (mNgnotOnSeat)
                        {
                            return "鎖付浮鎖";
                        }
                        else
                        {
                            return "鎖付NG";
                        }
                    }
                }
            } 
        }
       
        public float TorqueValue { get; set; }
        public Int32 AngleValue { get; set; }
    }
}
