using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGame.ExtensionMethods
{
    public static class ExtensionMethods
    {
        #region Arrays

        public static string ConcatString(this IEnumerable array, string delim)
        {
            string ret = "";
            int lastCount = 0;
            foreach (object i in array)
            {
                ret += i.ToString();
                lastCount = ret.Length;
                ret += delim;
            }
            ret = ret.Remove(lastCount);
            return ret;
        }

        #endregion

        #region Vectors/Points
        public static void ApproachZeroX(this Vector2 vec)
        {
            if (vec.X < 1 && vec.X > -1)
                vec.X = 0;
            else if (vec.X < 0)
                vec.X++;
            else if (vec.X > 0)
                vec.X--;
        }
        public static void ApproachZeroY(this Vector2 vec)
        {
            if (vec.Y < 1 && vec.Y > -1)
                vec.Y = 0;
            else if (vec.Y < 0)
                vec.Y++;
            else if (vec.Y > 0)
                vec.Y--;
        }

        public static void ApproachZero(this Vector2 vec)
        {
            vec.ApproachZeroX();
            vec.ApproachZeroY();
        }

        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        #endregion

        #region Radian/Degrees

        public static float RToD(this float f)
        {
            return (float)((f * 180) / Math.PI);
        }

        public static float DToR(this float f)
        {
            return (float)((f * Math.PI) / 180);
        }

        public static float RToD(this double f)
        {
            return (float)((f * 180) / Math.PI);
        }

        public static float DToR(this double f)
        {
            return (float)((f * Math.PI) / 180);
        }

        public static float RToD(this int f)
        {
            return (float)((f * 180) / Math.PI);
        }

        public static float DToR(this int f)
        {
            return (float)((f * Math.PI) / 180);
        }

        public static float RToD(this short f)
        {
            return (float)((f * 180) / Math.PI);
        }

        public static float DToR(this short f)
        {
            return (float)((f * Math.PI) / 180);
        }
        #endregion
    }
}
