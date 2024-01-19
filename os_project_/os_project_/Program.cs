using System;
using System.IO;
using System.Text;
namespace OS_project2
{
    class Program
    {
        public static Directory Current_Directory;
        public static string Current_Path;
        static void Main(string[] args)
        {
            Virtual_Disk.initalize("Data.txt");
            Current_Path = new string(Current_Directory.File_Or_DirName);
            while (true)
            {
                Console.Write(Current_Path.Trim());
                string Inputuser = Console.ReadLine();
                if (!Inputuser.Contains(" "))
                {
                    if (Inputuser.ToLower() == "help")
                    {
                        cmd.help();
                    }
                    else if (Inputuser.ToLower() == "quit")
                    {
                        cmd.quit();
                    }
                    else if (Inputuser.ToLower() == "cls")
                    {
                        cmd.cls();
                    }
                    else if (Inputuser.ToLower() == "md")
                    {
                        Console.WriteLine("Syntax Of Command Is Incorrect.");
                    }
                    else if (Inputuser.ToLower() == "rd")
                    {
                        Console.WriteLine("Syntax Of Command Is Incorrect.");
                    }
                    else if (Inputuser.ToLower() == "cd")
                    {
                        if (Current_Directory.Parent != null)
                        {
                            Current_Directory = Current_Directory.Parent;
                            Current_Path = new string(Current_Directory.File_Or_DirName);
                        }
                        Current_Path = new string(Current_Directory.File_Or_DirName);
                    }
                    else if (Inputuser.ToLower() == "dir")
                    {
                        cmd.dir();
                    }
                    else
                    {
                        Console.WriteLine(Inputuser + " " + "Is Not Recognized As An Internal Or External Command , Operable Program Or Batch File.");
                    }
                }
                else if (Inputuser.Contains(" "))
                {
                    string[] arrInput = Inputuser.Split(' ');
                    if (arrInput[0] == "md")
                    {
                        cmd.md(arrInput[1]);
                    }
                    else if (arrInput[0] == "type")
                    {
                        cmd.type(arrInput[1]);
                    }
                    else if (arrInput[0] == "del")
                    {
                        cmd.del(arrInput[1]);
                    }
                    else if (arrInput[0] == "import")
                    {
                        cmd.import(arrInput[1]);
                    }
                    else if (arrInput[0] == "rd")
                    {
                        cmd.rd(arrInput[1]);
                    }
                    else if (arrInput[0] == "cd")
                    {
                        cmd.cd(arrInput[1]);
                    }
                    else if (arrInput[0] == "help")
                    {
                        if (arrInput[1] == "md")
                        {
                            Console.WriteLine("md > Creates a directory");
                        }
                        else if (arrInput[1] == "cd")
                        {
                            Console.WriteLine("cd > Display The Name Of Or Change The Current Directory");
                        }
                        else if (arrInput[1] == "cls")
                        {
                            Console.WriteLine("cls > Clear The Screen");
                        }
                        else if (arrInput[1] == "quit")
                        {
                            Console.WriteLine("quit > Quits The CMD.exe Program");
                        }
                        else if (arrInput[1] == "rd")
                        {
                            Console.WriteLine("rd > Removes <Deletes> a Directory.");
                        }
                        else if (arrInput[1] == " ")
                        {
                            cmd.help();
                        }
                    }
                    else if (arrInput[0] == "export")
                    {
                        cmd.export(arrInput[1], arrInput[2]);
                    }
                    else if (arrInput[0] == "rename")
                    {
                        cmd.rename(arrInput[1], arrInput[2]);
                    }
                }
                else
                {
                    Console.WriteLine(Inputuser + " " + "Is Not Recognized As An Internal Or External Command , Operable Program Or Batch File.");
                }
            }
        }
    }
}