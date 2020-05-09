using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMI.Web
{
    public class PCMonitor//: System.Web.Hosting.IRegisteredObject
    {
        public PCMonitor()
        {
        }
    }

    /// <summary>
    /// Cpu资源监控
    /// 注意该Counter的第一个值是无效的0
    /// 刷得过快会导致0/100 出现,这些都是无效的
    /// </summary>
    public sealed class CPUMonitor
    {
        private static readonly CPUMonitor instance = new CPUMonitor();
        private System.Diagnostics.PerformanceCounter pcCpuLoad;
        private static DateTime? LastGetTime { get; set; }
        private static float CpuUsed { get; set; }
        private CPUMonitor()
        {
            //初始化CPU计数器
            pcCpuLoad = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.MachineName = ".";
            CpuUsed= pcCpuLoad.NextValue();
            if (CpuUsed == 0 || CpuUsed == 100)
            {
                getCpuUsedAverage();
            }
        }

        public static CPUMonitor getMonitor()
        {
            return instance;
        }

        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        public float getValue()
        {
            getCpuUsedAverage();
            return CpuUsed;
        }

        /// <summary>
        /// 取100毫秒内平均值
        /// </summary>
        private void getCpuUsedAverage()
        {
            //60秒内不再获取
            if (LastGetTime.HasValue)
            {
                TimeSpan OTS = LastGetTime.Value - DateTime.Now;
                if (OTS.Seconds <= 60)
                    return;
            }
            LastGetTime = DateTime.Now;
            List<float> ArrCpuNum = new List<float>();
            int Num = 0;
            while (Num < 10)
            {
                Num++;
                var _CpuUsed = pcCpuLoad.NextValue();
                if (_CpuUsed > 0 && _CpuUsed < 100)
                    ArrCpuNum.Add(_CpuUsed);
                System.Threading.Thread.Sleep(10);
            }
            CpuUsed = ArrCpuNum.Average();
            if (CpuUsed == 0 || CpuUsed == 100)
            {
                CpuUsed = pcCpuLoad.NextValue();
            }
        }
    }
}