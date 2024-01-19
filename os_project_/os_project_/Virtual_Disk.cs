using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace OS_project2
{
    class Virtual_Disk
    {
        public static FileStream Disk;
        public static void CREATE_Disk(string path)
        {
            Disk = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            Disk.Close();
        }
        public static int getFreeSpace()
        {
            return (1024 * 1024) - (int)Disk.Length;
        }
        public static void initalize(string path)
        {
            if (!File.Exists(path))
            {
                CREATE_Disk(path);
                byte[] X = new byte[1024];
                for (int i = 0; i < X.Length; i++)
                {
                    X[i] = 0;
                }
                writeBlock(X, 0);
                Fat_Table F = new Fat_Table();
                Fat_Table.initialize();
                Directory Root = new Directory("W:", 0x10, 5, 0, null);

                Root.writeDirectory();
                Fat_Table.set_Next(5, -1);
                Program.Current_Directory = Root;
                Fat_Table.Write_Fat_Table();
            }
            else
            {
                Fat_Table.get_fat_table();
                Directory Root = new Directory("W:", 0x10, 5, 0, null);
                Root.readDirectory();
                Program.Current_Directory = Root;
            }
        }
        public static void writeBlock(byte[] data, int Index, int offset = 0, int count = 1024)
        {
            Disk = new FileStream("Data.txt", FileMode.Open, FileAccess.Write);
            Disk.Seek(Index * 1024, SeekOrigin.Begin);
            Disk.Write(data, offset, count);
            Disk.Flush();
            Disk.Close();
        }
        public static byte[] readBlock(int clusterIndex)
        {
            Disk = new FileStream("Data.txt", FileMode.Open, FileAccess.Read);
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            Disk.Close();
            return bytes;
        }
    }
}