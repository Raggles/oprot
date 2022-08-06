using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace oprot.plot.core
{
    public  abstract partial class ProtectionCharacteristic : ObservableObject, IComparable
    {
        /// <summary>
        /// TempMultiplier is a scaling factor that may be applied to a curve to take into account different network configurations
        /// such as a circuit or transformer out of service
        /// </summary>
        [ObservableProperty]
        private double tempMultiplier = 1.0;

        /// <summary>
        /// The nominal voltage for the protection device (can be p.u., V, kV etc)
        /// </summary>
        [ObservableProperty]
        private double voltage  = 11000;

        [ObservableProperty]
        private double maximumFaultLevel  = double.PositiveInfinity;

        [ObservableProperty]
        private double minimumFaultLevel  = double.NaN;

        public string Description => $"{this}";

        [ObservableProperty]
        private string name = "Name";

        public int CompareTo(object obj)
        {
            int faster = 0;
            int equal = 0;
            int slower = 0;


            if (obj is ProtectionCharacteristic c)
            {
                int n = 1000;
                double logX0 = Math.Log10(1);
                double logX1 = Math.Log10(100000);
                double interval = (logX1 - logX0) / n;

                for (int i = 0; i < n; i++)
                {
                    double x = Math.Pow(10, logX0 + interval * i);
                    double y1 = Curve(x);
                    double y2 = c.Curve(x);
                    if (y1 == y2 || (double.IsPositiveInfinity(y1) && double.IsPositiveInfinity(y2)))
                    {
                        equal++;
                    }
                    else if (y1 < y2)
                    {
                        faster++;
                    }
                    else if (y1 > y2)
                    {
                        slower++;
                    }
                }
                if (slower == 0 && faster == 0)
                    return 0;
                if (slower > faster)
                    return -1;
                return 1;
            }
            else
            {
                throw new ArgumentException("Object is not a valid ProtectionCharacteristic");
            }
        }

        /// <summary>
        /// The equation for the curve as a function of current
        /// </summary>
        /// <param name="d">Current</param>
        /// <returns></returns>
        public abstract double Curve(double d);
        public abstract double LowerMargin(double d);
        public abstract double UpperMargin(double d);

        protected virtual void PretendCopyConstructor(ProtectionCharacteristic c)
        {
            if (c != null)
            {
                Name = c.Name;
                //Color = c.Color;
                Voltage = c.Voltage;
                TempMultiplier = c.TempMultiplier;
            }
        }

        /// <summary>
        /// Clone the feature
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return (ProtectionCharacteristic)this.MemberwiseClone();
        }
    }

}
