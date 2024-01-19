using System;
using System.IO;
namespace OS_project2
{
    class cmd
    {
        public static void cls()
        {
            Console.Clear();
        }
        public static void help()
        {
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                         cls  > Clear the screen.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                         quit > Quits the CMD.exe program.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                         help > Provides help information for windows command.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                          md  > Creates a directory.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                          cd  > Display the name of or change the current directory.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                          rd  > Removes (deletes) a directory.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                          dir > Displays a list of files and subdirectories in a directory.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                       import > Add a file from your PC to your Virtual Disk.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                       export > Add a file from your Virtual Disk to your PC.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                        type  > Displays the contents of a text file or files.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                       rename > Change the name of a directory or file.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("                                          del > Removes (deletes) a file.");
            Console.WriteLine("                                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        }
        public static void quit()
        {
            System.Environment.Exit(1);
        }

        public static void md(string Text)
        {
            if (Program.Current_Directory.searchDirectory(Text) == -1)
            {
                Directory_Entry New_Directory = new Directory_Entry(Text, 0x10, 0, 0);
                Program.Current_Directory.Dir_Table.Add(New_Directory);
                Program.Current_Directory.writeDirectory();
                if (Program.Current_Directory.Parent != null)
                {
                    Program.Current_Directory.Parent.updateContent(Program.Current_Directory.Parent);
                    Program.Current_Directory.Parent.writeDirectory();
                }
            }
            else
            {
                Console.WriteLine("A Subdirectory Or File " + Text + " Already Exists.");
            }
        }
        public static void rd(string Text)
        {
            int index = Program.Current_Directory.searchDirectory(Text);
            if (index != -1)
            {
                int firstCluster = Program.Current_Directory.Dir_Table[index].File_FirstCluster;
                Directory d1 = new Directory(Text, 0x10, firstCluster, 0, Program.Current_Directory);
                d1.deleteDirectory();
                Program.Current_Path = new string(Program.Current_Directory.File_Or_DirName).Trim();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void cd(string Text)
        {
            int index = Program.Current_Directory.searchDirectory(Text);
            if (index != -1)
            {
                int firstCluster = Program.Current_Directory.Dir_Table[index].File_FirstCluster;
                Directory d1 = new Directory(Text, 0x10, firstCluster, 0, Program.Current_Directory);
                Program.Current_Path = new string(Program.Current_Directory.File_Or_DirName).Trim() + "\\" + new string(d1.File_Or_DirName).Trim();
                Program.Current_Directory.writeDirectory();
                Program.Current_Directory.readDirectory();
                Program.Current_Directory = d1;
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void dir()
        {
            int F_Count = 0;
            int Directory_Count = 0;
            int Size_Count = 0;
            Console.WriteLine("Directory of " + new string(Program.Current_Directory.File_Or_DirName));
            for (int i = 0; i < Program.Current_Directory.Dir_Table.Count; i++)
            {
                if (Program.Current_Directory.Dir_Table[i].Fila_Attribute == 0x0)
                {
                    Console.WriteLine("  " + Program.Current_Directory.Dir_Table[i].File_Size + "  " + new string(Program.Current_Directory.Dir_Table[i].File_Or_DirName));
                    F_Count++;
                    Size_Count += Program.Current_Directory.Dir_Table[i].File_Size;
                }
                else
                {
                    Console.WriteLine("  " + "<DIR>" + "  " + new string(Program.Current_Directory.Dir_Table[i].File_Or_DirName));
                    Directory_Count++;
                }
            }
            Console.WriteLine("  " + F_Count + " File(s)" + "  " + Size_Count + " bytes");
            Console.WriteLine("  " + Directory_Count + " Dir(s)" + "  " + Fat_Table.GetFreeSpace() + " bytes free");
        }
        public static void import(string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                int name_start = filePath.LastIndexOf("\\");
                string filename = filePath.Substring(name_start + 1);
                int index = Program.Current_Directory.searchFile(filename);
                int firstcluster;
                if (index == -1)
                {
                    if (content.Length > 0)
                    {
                        firstcluster = Fat_Table.Getavaliableblock();

                    }
                    else
                    {
                        firstcluster = 0;
                    }
                    File_Entry fe = new File_Entry(filename, 0x0, firstcluster, content.Length, content, Program.Current_Directory);
                    fe.writeFileContent();
                    Directory_Entry d = new Directory_Entry(filename, 0x0, firstcluster, content.Length);
                    Program.Current_Directory.Dir_Table.Add(d);
                    Program.Current_Directory.writeDirectory();
                }
                else
                {
                    Console.WriteLine("This File Is Already Exist");
                }
            }
            else
            {
                Console.WriteLine("This File Is Not Exist");
            }
        }
        public static void type(string Name)
        {
            int index = Program.Current_Directory.searchFile(Name);
            if (index != -1)
            {
                if (Program.Current_Directory.Dir_Table[index].Fila_Attribute == 0x0)
                {
                    int FirstCluster = Program.Current_Directory.Dir_Table[index].File_FirstCluster;
                    int FileSize = Program.Current_Directory.Dir_Table[index].File_Size;
                    string Content = string.Empty;
                    File_Entry file = new File_Entry(Name, 0x0, FirstCluster, FileSize, Content, Program.Current_Directory);
                    file.readFileContent();
                    Console.WriteLine(file.content);
                }
            }
            else if (Program.Current_Directory.searchDirectory(Name) != -1)
            { Console.WriteLine("It's Folder Not File"); }
            else if (index == -1 && index == -1)
            { Console.WriteLine("File Not Been Created"); }
        }
        public static void export(string source, string dest)
        {
            int Start_Name = source.LastIndexOf(".");
            string File_Name = source.Substring(Start_Name + 1);
            if (File_Name == "txt")
            {
                int Index = Program.Current_Directory.searchFile(source);
                if (Index != -1)
                {
                    if (System.IO.Directory.Exists(dest))
                    {
                        int Cluster = Program.Current_Directory.Dir_Table[Index].File_FirstCluster;
                        int length = Program.Current_Directory.Dir_Table[Index].File_Size;
                        string content = null;
                        File_Entry F = new File_Entry(source, 0x0, Cluster, length, content, Program.Current_Directory);
                        F.readFileContent();
                        StreamWriter STW = new StreamWriter(dest + "\\" + source);
                        STW.Write(F.content);
                        STW.Flush();
                        STW.Close();
                    }
                    else
                    {
                        Console.WriteLine("The System Can Not Find The Path");
                    }
                }
                else
                {
                    Console.WriteLine("This File Is Not Exist");
                }
            }
        }
        public static void rename(string Old_Name, string New_Name)
        {
            int Old_Index2 = Program.Current_Directory.searchDirectory(Old_Name);
            int Old_Index1 = Program.Current_Directory.searchFile(Old_Name);
            if (Old_Index1 != -1)
            {
                int newIndex = Program.Current_Directory.searchFile(New_Name);
                if (newIndex == -1)
                {
                    Directory_Entry d1 = new Directory_Entry(New_Name, Program.Current_Directory.Dir_Table[Old_Index1].Fila_Attribute, Program.Current_Directory.Dir_Table[Old_Index1].File_FirstCluster, Program.Current_Directory.Dir_Table[Old_Index1].File_Size);
                    Program.Current_Directory.Dir_Table.RemoveAt(Old_Index1);
                    Program.Current_Directory.Dir_Table.Insert(Old_Index1, d1);
                    Program.Current_Directory.writeDirectory();
                }
                else
                {
                    Console.WriteLine("New Name Is Exist Write Another Name");
                }
            }
            else if (Old_Index2 != -1)
            {
                int newIndex2 = Program.Current_Directory.searchDirectory(New_Name);
                if (newIndex2 == -1)
                {
                    Directory_Entry d1 = new Directory_Entry(New_Name, Program.Current_Directory.Dir_Table[Old_Index2].Fila_Attribute, Program.Current_Directory.Dir_Table[Old_Index2].File_FirstCluster, Program.Current_Directory.Dir_Table[Old_Index2].File_Size);
                    Program.Current_Directory.Dir_Table.RemoveAt(Old_Index2);
                    Program.Current_Directory.Dir_Table.Insert(Old_Index2, d1);
                    Program.Current_Directory.writeDirectory();
                }
                else
                {
                    Console.WriteLine("New Name Is Exist Write Another Name");
                }
            }
            else
            {
                Console.WriteLine("No File Or Folder With This Name");
            }
        }
        public static void del(string FName)
        {
            int index = Program.Current_Directory.searchFile(FName);
            if (index != -1)
            {
                if (Program.Current_Directory.Dir_Table[index].Fila_Attribute == 0x0)
                {
                    int cluster = Program.Current_Directory.Dir_Table[index].File_FirstCluster;
                    int size = Program.Current_Directory.Dir_Table[index].File_Size;
                    File_Entry f = new File_Entry(FName, 0x0, cluster, size, null, Program.Current_Directory);
                    f.deleteFile();
                }
                else
                {
                    Console.WriteLine("No File With This Name");
                }
            }
        }
    }
}