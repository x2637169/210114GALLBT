using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class AGP_Func
{
    #region 調用DLL函式

    #region IO

    ///<summary>要使用APG DIO必須先使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_Open();
    ///<summary>不使用APG DIO使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_Close();
    ///<summary>
    ///<para>寫入GPO</para>
    ///<para>pins輸入0xFFFFFFFF為寫入範圍0~31的GPO狀態</para>
    ///<para>value為16進位值 , 2進位為單獨GPO的個別位置(ex: 1111 1111 1111 1111 1111 1111 1111 1110 , 第一個(右數到左)GPO 0 設為Low(開啟)) , 轉為16進位就為value的值(ex: FFFFFFFE)</para>
    ///</summary>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_Write(uint pins, uint value);
    ///<summary>
    ///<para>單獨寫入GPO</para>
    ///<para>pins範圍0~31</para>
    ///<para>value範圍0~1 , 0為Low(開啟)、1為High(關閉)</para>
    ///</summary>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_WriteBit(byte pins, byte value);
    ///<summary>
    ///<para>單獨讀取GPO狀態</para>
    ///<para>pins範圍0~31</para>
    ///</summary>
    ///<returns>GPO狀態 , 0為Low(開啟)、1為High(關閉)</returns>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_DoReadBit(byte pins, ref byte pstatus);
    ///<summary>
    ///<para>讀取GPO狀態</para>
    ///<para>pins輸入0xFFFFFFFF為讀取範圍0~31的GPO狀態</para>
    ///</summary>
    ///<returns>GPO狀態 , ex : ref pvalues = 0xFFFFFFFE , 轉換為二進位 = 1111 1111 1111 1111 1111 1111 1111 1110 , 第一個(右數到左)0為GPO 0 設為Low(開啟) 其他為 High(關閉)</returns>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_DoRead(uint pins, ref uint pvalues);
    ///<summary>
    ///<para>單獨讀取GPI狀態</para>
    ///<para>pins範圍0~31</para>
    ///</summary>
    ///<returns>GPI狀態 , 0為Low(開啟)、1為High(關閉)</returns>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_DiReadBit(byte pins, ref byte pstatus);
    ///<summary>
    ///<para>讀取GPI狀態</para>
    ///<para>pins輸入0xFFFFFFFF為讀取範圍0~31的GPI狀態</para>
    ///</summary>
    ///<returns>GPI狀態 , ex : ref pvalues = 0xFFFFFFFE , 轉換為二進位 = 1111 1111 1111 1111 1111 1111 1111 1110 , 第一個(右數到左)0為GPI 0 設為Low(開啟) 其他為 High(關閉)</returns>
    [DllImport("agp")]
    public static extern int AXGMB_DIO_DiRead(uint pins, ref uint pvalues);

    #endregion

    #region Intrusion

    ///<summary>要使用APG Intrusion必須先使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Intr_Open();
    ///<summary>不使用APG Intrusion使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Intr_Close();
    [DllImport("agp")]
    public static extern int AXGMB_Intr_GetRtc(ref RTC_TIME time);
    ///<summary>IPC時間與電腦時間同步</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Intr_SyncRtc();
    // 機門目前未用到
    // [DllImport("agp")]
    // public static extern int AXGMB_Intr_GetBitStatus(byte pin, ref byte status);

    #endregion

    #region Nvram

    ///<summary>要使用APG Nvram必須先使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Nvram_Open();
    ///<summary>不使用APG Nvram使用此方法</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Nvram_Close();
    // Nvram模式切換目前未用到
    //[DllImport("agp")]
    //public static extern int AXGMB_Nvram_RWModeStatus(ref byte RWModeStatus);
    ///<summary>查看記憶體大小</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Nvram_Size(ref uint ramSize);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Write(uint address, uint length, ref byte byteBuffer);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Write(uint address, uint length, ref ushort ushortBuffer);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Write(uint address, uint length, ref uint uintBuffer);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Read(uint address, uint length, ref byte byteBuffer);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Read(uint address, uint length, ref ushort ushortBuffer);
    [DllImport("agp")]
    private static extern int AXGMB_Nvram_Read(uint address, uint length, ref uint uintBuffer);
    ///<summary>清空記憶體</summary>
    [DllImport("agp")]
    public static extern int AXGMB_Nvram_Clear(uint value);

    #endregion

    #endregion

    #region AGP_Info

    ///<summary>
    ///調用AGP API回傳的狀態數值資訊
    ///</summary>
    public enum AGP_ReturnValue
    {
        ERR_Success = 0,
        ERR_Error = -1,
        ERR_NotExist = -2,
        ERR_Opened = -3,
        ERR_NotOpened = -4,
        ERR_OutOfRange = -5,
        ERR_InnerFails = -6,
        ERR_Invalid = -7,
        ERR_FileError = -8,
        ERR_FirmwareError = -9,
        ERR_UpgradeFails = -10,
        ERR_NotSupport = -11,
    }

    ///<summary>
    ///IPC時間
    ///</summary>
    public struct RTC_TIME
    {
        public ushort year;
        public byte month;
        public byte day;
        public byte hour;
        public byte minute;
        public byte second;
    }

    #endregion

    #region Nvram寫入、讀取方法

    ///<summary>
    ///<para>記憶體寫入</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~1 , 正常應輸入1</param></para>
    ///<para><param name="byteBuffer">byteBuffer範圍0~255 , 欲寫入的值</param></para>
    ///</summary>
    public static void AXGMB_Nvram_Write8(byte byteBuffer, uint address, uint length)
    {
        AXGMB_Nvram_Write(address, length, ref byteBuffer);
    }

    ///<summary>
    ///<para>記憶體寫入</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~2 , 正常應輸入2</param></para>
    ///<para><param name="byteBuffer">byteBuffer範圍0~65535 , 欲寫入的值</param></para>
    ///</summary>
    public static void AXGMB_Nvram_Write16(ushort ushortBuffer, uint address, uint length)
    {
        AXGMB_Nvram_Write(address, length, ref ushortBuffer);
    }

    ///<summary>
    ///<para>記憶體寫入</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~4 , 正常應輸入4</param></para>
    ///<para><param name="byteBuffer">byteBuffer範圍0~4294967295 , 欲寫入的值</param></para>
    ///</summary>
    public static void AXGMB_Nvram_Write32(uint uintBuffer, uint address, uint length)
    {
        AXGMB_Nvram_Write(address, length, ref uintBuffer);
    }

    ///<summary>
    ///<para>記憶體讀取</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~1 , 正常應輸入1</param></para>
    ///</summary>
    ///<returns>該記憶題位址儲存的值</returns>
    public static void AXGMB_Nvram_Read8(out byte byteBuffer, uint address, uint length)
    {
        byteBuffer = 0;
        AXGMB_Nvram_Read(address, length, ref byteBuffer);
    }

    ///<summary>
    ///<para>記憶體讀取</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~2 , 正常應輸入2</param></para>
    ///</summary>
    ///<returns>該記憶題位址儲存的值</returns>
    public static void AXGMB_Nvram_Read16(out ushort ushortBuffer, uint address, uint length)
    {
        ushortBuffer = 0;
        AXGMB_Nvram_Read(address, length, ref ushortBuffer);
    }

    ///<summary>
    ///<para>記憶體讀取</para>
    ///<para><param name="address">address範圍0~NvramSize</param></para>
    ///<para><param name="length">length範圍0~4 , 正常應輸入4</param></para>
    ///</summary>
    ///<returns>該記憶題位址儲存的值</returns>
    public static void AXGMB_Nvram_Read32(out uint uintBuffer, uint address, uint length)
    {
        uintBuffer = 0;
        AXGMB_Nvram_Read(address, length, ref uintBuffer);
    }

    #endregion
}