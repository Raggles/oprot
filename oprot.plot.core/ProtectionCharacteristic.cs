﻿using MicroMvvm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace oprot.plot.core
{
    public abstract class ProtectionCharacteristic: GraphFeature
    {
        public ProtectionCharacteristic() : base() { }
        public ProtectionCharacteristic(GraphFeature g) : base(g) { }

        /// <summary>
        /// The equation for the curve as a function of current
        /// </summary>
        /// <param name="d">Current</param>
        /// <returns></returns>
        [Obsolete]
        public abstract double Curve(double d);

        public List<string> CurveNames { get; }
        public double Curve(double d, int index) { return double.NaN; }
        public double Curve(double d, string name) { return double.NaN; }

        public override string ToString()
        {
            return "";
        }

        public string SerializeJson()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Newtonsoft.Json.Formatting.Indented

            };
            return JsonConvert.SerializeObject(this, jsonSerializerSettings);
        }

        public string SerializeBase64()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Newtonsoft.Json.Formatting.Indented

            };
            var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
            return System.Convert.ToBase64String(Util.Zip(json));

        }
        //public abstract double MaximumMargin(double d);
        //public abstract double MinimumMargin(double d);
    }
}
