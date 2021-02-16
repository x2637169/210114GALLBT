using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RNGUtility
{
    /// <summary>
    /// 一個產生的亂數值會介於 0~255 之間且小於固定值的亂數產生器。
    /// </summary>
    public class RNGInt8 : RNG
    {
        private const uint MaximumValue = 0x100;
        public readonly uint Maximum;
        private readonly uint AcceptableMaximum;
        private readonly byte[] Buff = new byte[1];

        /// <summary>
        /// 準備一個亂數產生器，生成的亂數必定小於指定的數值(不包含)。
        /// </summary>
        /// <param name="maximum">生成的亂數最大值(不包含)</param>
        public RNGInt8(uint maximum)
        {
            Maximum = maximum;

            // 先算出有幾組完整的亂數可以用
            AcceptableMaximum = MaximumValue / maximum;

            // 然後算出亂數值可接受的最大值
            AcceptableMaximum *= Maximum;
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
                    rngCsp.GetBytes(Buff);
                }
                while (Buff[0] >= AcceptableMaximum);

                // 把亂數對最大值取餘回傳。
                return Buff[0] % Maximum;
            }
        }
    }
}
