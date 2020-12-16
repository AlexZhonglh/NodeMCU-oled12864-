using System;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Collections;

namespace ConsoleApp10
{
    class Program
    {
        public class Comparer : IComparer  //图片序列按照文件名给出的的时间进行比较，确保播放顺序正确
        {
            public int Compare(Object x, Object y)
            {
                string s1 = (string)x;
                string s2 = (string)y;

                string name1 = s1.Substring(0, s1.Length - 4);
                string name2 = s2.Substring(0, s1.Length - 4);

                return Convert.ToDouble(name1) > Convert.ToDouble(name2) ? 1 : -1;
            }
        }
        static bool PicToByte(string path,ref Byte[] temp)  //将图片转换为单片机可用的字节数组
        {
            try
            {
                string bits = "";
                int count = 0;
                int byteCount = 0;
                Bitmap bitmap = new Bitmap(path);
                for (int i = 0; i < bitmap.Height; ++i)
                {
                    for (int j = 0; j < bitmap.Width; ++j)
                    {
                        Color c = bitmap.GetPixel(j,i);
                        int rgb= (int)(c.R * .3 + c.G * .59 + c.B * .11)  ;//灰度公式
                        bits += rgb > 100 ? '1' : '0';                        
                        ++count;                        
                        if (count%8 == 0)
                        {
                            temp[byteCount++] = Convert.ToByte(bits, 2);
                            bits = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            Array.Reverse(temp); //切换大小端
            return true;
        }
        static void Main(string[] args)
       {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length == 0)
                {
                    Console.WriteLine("No Avaliable Serial Port...");
                    return;
                }
                SerialPort port = new SerialPort(ports[0], 1000000, Parity.None, 8);//端口的初始化设置
                port.StopBits = StopBits.One;
                port.Open();
                Byte[] test = new byte[1024];
                string path = @"F:\截取\星际穿越";
                DirectoryInfo dInfo = new DirectoryInfo(path);
                FileInfo[] fInfo = dInfo.GetFiles();
                Console.Clear();
                string[] fileName = new string[fInfo.Length];
                int count = 0;
                foreach (FileInfo f in fInfo)
                {
                    fileName[count++] = f.Name;
                }
                Array.Sort(fileName, new Comparer());//按照时间对所有文件名排序

                Console.WriteLine("Pree Any Key to Start");
                Console.ReadKey();
                Console.WriteLine("Playing...");

                int p = 0;
                for (; ; )
                {
                    PicToByte(path + @"\" + fileName[p], ref test);
                    p += 1; //每4张图片播放一张，配合下面的sleep可以达到稍微能看的播放速度
                    if (p >= fileName.Length)
                    {
                        p = 0;  //播放完从头播放
                    }
                    port.Write(test, 0, 1024);
                    Thread.Sleep(60);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
