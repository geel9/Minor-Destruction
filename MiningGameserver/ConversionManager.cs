using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameserver
{
    public class ConversionManager
    {
        public static double DegreeToRadians(double degrees)
        {
            return (degrees*Math.PI)/180;
        }
        public static double RadianToDegree(double radians)
        {
            return (radians * 180) / Math.PI;
        }
    }
}
