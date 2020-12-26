using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class GradingOptions
    {
        public double MinCurrent { get; set; } = 1;
        public double MaxCurrent { get; set; } = 10000;
        public int Samples { get; set; } = 1000;

    }
    public static class Grader
    {
        public static GradingResult Grade(ProtectionCharacteristic fastCurve, ProtectionCharacteristic slowCurve)
        {
            int n = 1000;
            double logX0 = Math.Log10(1);
            double logX1 = Math.Log10(100000);
            double interval = (logX1 - logX0) / n;
            List<GradingSection> sections = new List<GradingSection>();
            GradingSection currentSection;

            currentSection.From = Math.Pow(10, logX0);
            currentSection.Grades = true;  //assume they always grade at the start

            for (int i = 1; i < n - 1; i++)
            {
                double x = Math.Pow(10, logX0 + interval * i);
                
                double y1 = fastCurve.Curve(x);
                double y2 = slowCurve.Curve(x);


                if (y1 > y2)
                {
                    //doesn't grade
                    if (currentSection.Grades)
                    {
                        //then end the section and start a new one
                        currentSection.To = x;
                        sections.Add(currentSection);
                        currentSection.From = x;
                        currentSection.Grades = false;

                    }
                    else
                    {
                        //continue
                    }
                }
                else
                {
                    //does grade
                    if (currentSection.Grades)
                    {
                        //continue
                    }
                    else
                    {
                        //end the section and start a new one
                        //then end the section and start a new one
                        currentSection.To = x;
                        sections.Add(currentSection);
                        currentSection.From = x;
                        currentSection.Grades = true;
                    }
                }

            }
            currentSection.To = Math.Pow(10, logX0 + interval * n);
            sections.Add(currentSection);
            GradingResult r = new GradingResult() { Curve1 = fastCurve, Curve2 = slowCurve, Sections = sections } ;
            return r;
        }

        public static List<GradingResult> Grade (List<ProtectionCharacteristic> curves)
        {
            if (curves.Count > 1)
            {
                List<GradingResult> results = new List<GradingResult>();
                for (int i = 0; i < curves.Count - 1; i++)
                {
                    results.Add(Grade(curves[i + 1], curves[i]));
                }
                return results;
            }
            return null;
        }
    }
}
