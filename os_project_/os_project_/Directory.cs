using System;
using System.Collections.Generic;
using System.Text;
namespace OS_project2
{
    class Directory : Directory_Entry
    {
        public List<Directory_Entry> Dir_Table;
        public Directory Parent = null;
        public Directory(string Name_File, byte Attribute_File, int First_Clustum_File, int Length, Directory p)
            : base
            (Name_File,
             Attribute_File,
             First_Clustum_File,
             Length
             )
        {
            Dir_Table = new List<Directory_Entry>();
            if (p != null)
            {
                Parent = p;
            }
        }
        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry D = new Directory_Entry(new string(File_Or_DirName), Fila_Attribute, File_FirstCluster, File_Size);
            return D;
        }
        public void writeDirectory()
        {
            byte[] directoryTableByte = new byte[Dir_Table.Count * 32];
            for (int i = 0; i < Dir_Table.Count; i++)
            {
                byte[] derictoryEntryByte = Dir_Table[i].GetBytes();
                for (int j = 0, x = i * 32; j < 32 && x < Dir_Table.Count * 32; j++, x++)
                {
                    directoryTableByte[x] = derictoryEntryByte[j];
                }
            }
            double Num_Of_Blocks = directoryTableByte.Length / 1024;
            int Num_Of_RequiredBlock = Convert.ToInt32(Math.Ceiling(Num_Of_Blocks));
            int Num_Of_FullSizeBlock = Convert.ToInt32(Math.Floor(Num_Of_Blocks));
            double Reminder = directoryTableByte.Length % 1024;
            List<byte[]> bytes = new List<byte[]>();
            byte[] b = new byte[1024];

            for (int x = 0; x < Num_Of_FullSizeBlock; x++)
            {
                for (int i = 0, j = x * 1024; i < 1024 && j < directoryTableByte.Length; i++, j++)
                {
                    b[i] = directoryTableByte[j];
                }
                bytes.Add(b);
            }
            int Fat_Index;
            if (File_FirstCluster != 0)
            {
                Fat_Index = File_FirstCluster;
            }
            else
            {
                Fat_Index = Fat_Table.Getavaliableblock();
                File_FirstCluster = Fat_Index;
            }
            int lastIndex = -1;
            for (int i = 0; i < bytes.Count; i++)
            {
                if (Fat_Index != -1)
                {
                    Virtual_Disk.writeBlock(bytes[i], Fat_Index);
                    Fat_Table.set_Next(Fat_Index, -1);
                    if (lastIndex != -1)
                    {
                        lastIndex = Fat_Index;
                        Fat_Table.set_Next(lastIndex, Fat_Index);
                    }
                    Fat_Index = Fat_Table.Getavaliableblock();
                    Fat_Table.Write_Fat_Table();
                }
            }
        }
        public void readDirectory()
        {
            if (this.File_FirstCluster != 0)
            {
                int fatIndex = this.File_FirstCluster;

                int next = Fat_Table.get_Next(fatIndex);
                List<byte> ls = new List<byte>();
                List<Directory_Entry> dt = new List<Directory_Entry>();
                do
                {
                    ls.AddRange(Virtual_Disk.readBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = Fat_Table.get_Next(fatIndex);
                    }
                } while (next != -1);
                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                        break;
                    dt.Add(GetDirectoryEntry(b));
                }
            }
        }
        public int searchDirectory(string name)
        {
            if (name.Length < 11)
            {
                name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < this.Dir_Table.Count; i++)
            {
                string n = new string(this.Dir_Table[i].File_Or_DirName);
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }
        public int searchFile(string name)
        {
            if (name.Length < 11)
            {
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < this.Dir_Table.Count; i++)
            {
                string n = new string(this.Dir_Table[i].File_Or_DirName);   
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }
        public void updateContent(Directory_Entry d)
        {
            int index = searchDirectory(new string(d.File_Or_DirName));
            if (index != -1)
            {
                Dir_Table.RemoveAt(index);
                Dir_Table.Insert(index, d);
            }
        }
        public void deleteDirectory()
        {
            if (this.File_FirstCluster != 0)
            {
                int index = this.File_FirstCluster;
                int next = -1;
                do
                {
                    Fat_Table.set_Next(index, 0);
                    next = index;

                    if (index != -1)
                        index = Fat_Table.get_Next(index);

                } while (next != -1);
            }
            if (this.Parent != null)
            {
                Parent.readDirectory();
                int Index = Parent.searchDirectory(new string(File_Or_DirName));
                if (Index != -1)
                {
                    this.Parent.Dir_Table.RemoveAt(Index);
                    this.Parent.writeDirectory();
                }
            }
        }
    }
}