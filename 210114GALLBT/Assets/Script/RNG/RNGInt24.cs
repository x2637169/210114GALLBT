using System;
using System.Security.Cryptography;

namespace RNGUtility
{
    /// <summary>
    /// 一個產生的亂數值會介於 0~16777215 之間且小於固定值的亂數產生器。
    /// </summary>
    public class RNGInt24 : RNG
    {
        private const uint MaximumValue = 0x1000000;
        public readonly uint Maximum;
        private readonly uint AcceptableMaximum;
        private readonly byte[] Buff = new byte[4];
        private uint Num;

        /// <summary>
        /// 準備一個亂數產生器，生成的亂數必定小於指定的數值(不包含)。
        /// </summary>
        /// <param name="maximum">生成的亂數最大值(不包含)</param>
        public RNGInt24(uint maximum)
        {
            Maximum = maximum;

            // 先算出有幾組完整的亂數可以用
            AcceptableMaximum = MaximumValue / maximum;

            // 然後算出亂數值可接受的最大值
            AcceptableMaximum *= Maximum;

            // 預先清零Buff的最後一個欄位，因為這個欄位不會變動。
            Buff[3] = 0;
        }

        /// <summary>
        /// 取得一個亂數值。
        /// </summary>
        /// <returns>亂數值</returns>
        public uint Next()
        {
            lock (this)
            {
                // 演算法: 產生一個亂數，檢查是否在可接受範圍內。
                // 如果不能接受，就重新產生新的亂數，重新檢查，
                // 一直到出現可接受的數值為止。
                do
                {
                    rngCsp.GetBytes(Buff, 0, 3);
                    Num = BitConverter.ToUInt32(Buff, 0);
                }
                while (Num >= AcceptableMaximum);

                // 把亂數對最大值取餘回傳。
                return Num % Maximum;
            }
        }
    }
}
