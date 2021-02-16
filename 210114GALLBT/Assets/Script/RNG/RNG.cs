using System;
using System.Security.Cryptography;

namespace RNGUtility
{
    /// <summary>
    /// 亂數產生器:
    /// 請注意，請勿將最大值設的過小，會減慢亂數產生的取值速度。
    /// 可將最大值設成最大值的整數倍，再將回傳結果取餘數作為亂數值。
    /// 例如: 想要取一個0~9的亂數，可以用RNG.NextInt8(250)%10來取值。
    /// </summary>
    public class RNG
    {
        /// <summary>
        /// 主要取亂數的工具。
        /// </summary>
        protected static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        /// <summary>
        /// 取亂數會用到的暫存空間
        /// </summary>
        private static byte[] Buff = new byte[8];

        /// <summary>
        /// 產生一個亂數的 byte ，數值介於 0~255 之間。
        /// </summary>
        /// <returns>亂數結果</returns>
        public static byte NextInt8()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 1);
                return Buff[0];
            }
        }

        /// <summary>
        /// 產生一個亂數的 byte ，數值介於 0~255 之間，且不超過設定的最大值。
        /// 舉例: NextByte(250) 的結果範圍會在 0~249 之間
        /// </summary>
        /// <param name="maximum">亂數回傳的最大值(不包含)</param>
        /// <returns>亂數的結果</returns>
        public static byte NextInt8(byte maximum)
        {
            lock (Buff)
            {
                do
                {
                    rngCsp.GetBytes(Buff, 0, 1);
                }
                while (Buff[0] >= maximum);
                return Buff[0];
            }
        }

        /// <summary>
        /// 產生一個亂數的 uShort ，數值介於 0~65535 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static ushort NextInt16()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 2);
                return BitConverter.ToUInt16(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 ushort ，數值介於 0~65535 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static ushort NextInt16(ushort maximum)
        {
            ushort num;
            do
            {
                num = NextInt16();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 uint ，數值介於 0~16777215
        /// </summary>
        /// <returns></returns>
        public static uint NextInt24()
        {
            lock (Buff)
            {
                Buff[3] = 0;
                rngCsp.GetBytes(Buff, 0, 3);
                return BitConverter.ToUInt32(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 uint ，數值介於 0~16777215 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static uint NextInt24(uint maximum)
        {
            uint num;
            do
            {
                num = NextInt24();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 uint ，數值介於 0~4294967295 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static uint NextInt32()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 4);
                return BitConverter.ToUInt32(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 uint ，數值介於 0~4294967295 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static uint NextInt32(uint maximum)
        {
            uint num;
            do
            {
                num = NextInt32();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 ulong ，數值介於 0-1099511627775 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt40()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 5);
                Buff[5] = Buff[6] = Buff[7] = 0;
                return BitConverter.ToUInt64(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 ulong ，數值介於 0-1099511627775 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt40(ulong maximum)
        {
            ulong num;
            do
            {
                num = NextInt40();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 ulong 數值介於 0-281474976710655 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt48()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 6);
                Buff[6] = Buff[7] = 0;
                return BitConverter.ToUInt64(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 ulong ，數值介於 0-281474976710655 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt48(ulong maximum)
        {
            ulong num;
            do
            {
                num = NextInt48();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 ulong 數值介於 0-72057594037927935 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt56()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff, 0, 7);
                Buff[7] = 0;
                return BitConverter.ToUInt64(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 ulong ，數值介於 0-72057594037927935 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt56(ulong maximum)
        {
            ulong num;
            do
            {
                num = NextInt56();
            }
            while (num >= maximum);
            return num;
        }

        /// <summary>
        /// 產生一個亂數的 ulong 數值介於 0-18446744073709551615 之間。
        /// </summary>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt64()
        {
            lock (Buff)
            {
                rngCsp.GetBytes(Buff);
                return BitConverter.ToUInt64(Buff, 0);
            }
        }

        /// <summary>
        /// 產生一個亂數的 ulong ，數值介於 0-281474976710655 之間，且不超過設定的最大值。
        /// </summary>
        /// <param name="maximum">亂數的最大值</param>
        /// <returns>亂數的結果</returns>
        public static ulong NextInt64(ulong maximum)
        {
            ulong num;
            do
            {
                num = NextInt64();
            }
            while (num >= maximum);
            return num;
        }
    }
}
