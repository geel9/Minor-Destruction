using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static bool BitSet(this byte toCheck, int index)
        {
            return ((toCheck & ((byte)1 << index)) != 0);
        }

        public static bool BitSet(this short toCheck, int index)
        {
            return ((toCheck & ((short)1 << index)) != 0);
        }

        public static bool BitSet(this int toCheck, int index)
        {
            return ((toCheck & ((int)1 << index)) != 0);
        }

        public static bool BitSet(this long toCheck, int index)
        {
            return ((toCheck & ((long)1 << index)) != 0);
        }

        public static byte SetBit(this byte toSet, int index, bool set = true)
        {
            if (set)
            {
                return (byte)(toSet | (1 << index));
            }
            else
            {
                return (byte) (toSet & ~(1 << index));
            }
        }

        public static short SetBit(this short toSet, int index, bool set = true)
        {
            if (set)
            {
                return (short)(toSet | (1 << index));
            }
            else
            {
                return (short)(toSet & ~(1 << index));
            }
        }

        public static int SetBit(this int toSet, int index, bool set = true)
        {
            if (set)
            {
                return (toSet | (1 << index));
            }
            else
            {
                return (toSet & ~(1 << index));
            }
        }

        public static long SetBit(this long toSet, int index, bool set = true)
        {
            if (set)
            {
                return (toSet | (1 << index));
            }
            else
            {
                return (toSet & ~(1 << index));
            }
        }

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
        public static Vector2 ApproachZeroX(this Vector2 vec)
        {
            if (vec.X < 1 && vec.X > -1)
                vec.X = 0;
            if (vec.X < 0)
                vec.X++;
            if (vec.X > 0)
                vec.X--;
            return vec;
        }
        public static Vector2 ApproachZeroY(this Vector2 vec)
        {
            if (vec.Y < 1 && vec.Y > -1)
                vec.Y = 0;
            else if (vec.Y < 0)
                vec.Y++;
            else if (vec.Y > 0)
                vec.Y--;
            return vec;
        }

        public static Vector2 ApproachZero(this Vector2 vec)
        {
            vec = vec.ApproachZeroX();
            vec = vec.ApproachZeroY();
            return vec;
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
