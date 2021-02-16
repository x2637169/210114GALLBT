using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class log
    {
        private static object lockme = new object();

        /// <summary>
        ///  創建txt檔案
        /// </summary>
        /// <param name="record">欲寫入的文字</param>
        public void create_log(string record)
        {
            //取得現在時間
            DateTime dateTime = DateTime.Now;
            //創建Log
            string strPicFile = @"C:\" + string.Format("{0}{1}.txt", DateTime.Now.ToString("yyyyMMdd"), " log");
            //文字加上當下時間
            string message = dateTime + "";
            message += "  " + record;

            if (File.Exists(strPicFile) == true)
            {
                write(message, strPicFile);
                //view(message);
            }
            else
            {
                FileStream fileStream = new FileStream(strPicFile, FileMode.Create);
                fileStream.Close();
                write(message, strPicFile);
               // view(message);
            }
        }

        /// <summary>
        /// 寫入資料
        /// </summary>
        /// <param name="record">欲寫入的文字</param>
        /// <param name="file_name">檔案路徑</param>
        public void write(string record, string file_name)
        {
            lock (lockme)
            {
                using (FileStream fs = new FileStream(file_name, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(record);
                        sw.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Log顯示
        /// </summary>
        /// <param name="view">顯示的文字(含時間)</param>
        //public void view(string view)
        //{
        //    Console.WriteLine(view);
        //}
    }
}