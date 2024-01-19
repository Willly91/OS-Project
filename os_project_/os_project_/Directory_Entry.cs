using System;
using System.Collections.Generic;
using System.Text;
namespace OS_project2
{
    class Directory_Entry
    {
        public char[] File_Or_DirName = new char[11];
        public byte Fila_Attribute;
        public byte[] File_Empty = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int File_Size;
        public int File_FirstCluster;
        public Directory_Entry(string name, byte attribute, int firstCluster, int Size)
        {
            this.Fila_Attribute = attribute;

            if (this.Fila_Attribute == 0x0)
            {
                string[] filename = name.Split('.');
                assignFileName(filename[0].ToCharArray(), filename[1].ToCharArray());
            }
            else
            {
                assignDIRName(name.ToCharArray());
            }
            this.File_FirstCluster = firstCluster;
            this.File_Size = Size;
        }
        public void assignFileName(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.File_Or_DirName[i] = name[i];
                }
                this.File_Or_DirName[j] = '.';
                for (int i = 0; i < extension.Length; i++)
                {
                    j++;
                    this.File_Or_DirName[j] = extension[i];
                }
                for (int i = ++j; i < File_Or_DirName.Length; i++)
                {
                    this.File_Or_DirName[i] = ' ';
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.File_Or_DirName[i] = name[i];
                }
                this.File_Or_DirName[7] = '.';
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    this.File_Or_DirName[j] = extension[i];
                }
            }
        }
        public void assignDIRName(char[] name)
        {
            if (name.Length <= 11)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.File_Or_DirName[i] = name[i];
                }
                for (int i = ++j; i < File_Or_DirName.Length; i++)
                {
                    this.File_Or_DirName[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 11; i++)
                {
                    j++;
                    this.File_Or_DirName[i] = name[i];
                }
            }
        }
        public byte[] GetBytes()
        {
            byte[] b = new byte[32];
            b[11] = Fila_Attribute;
            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                b[i] = File_Empty[j];
            }
            for (int i = 24; i < 28; i++)
            {
                b[i] = (byte)File_FirstCluster;
            }
            for (int i = 28; i < 32; i++)
            {
                b[i] = (byte)File_Size;
            }
            return b;
        }
        public Directory_Entry GetDirectoryEntry(byte[] b)
        {
            for (int i = 0; i < 11; i++)
            {
                File_Or_DirName[i] = (char)b[i];
            }
            Fila_Attribute = b[11];
            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                File_Empty[j] = b[i];
            }
            for (int i = 24; i < 28; i++)
            {
                File_FirstCluster = b[i];
            }
            for (int i = 28; i < 32; i++)
            {
                File_Size = b[i];
            }
            Directory_Entry d1 = new Directory_Entry(new string(File_Or_DirName), Fila_Attribute, File_FirstCluster, File_Size);
            return d1;
        }
    }
}