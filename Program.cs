using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO
{
    class Program
    {
        static string StorageName = "HardDrive.txt";
        static int fileSystemSize = 1000000;

        static void Main(string[] args)
        {
            var currentFolder = "";
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\{StorageName}"))
            {
                WriteFile("");
            }
            else if (ReadFile(false).Length < fileSystemSize)
            {
                var emptySpace = fileSystemSize - ReadFile(false).Length;
                WriteFile(ReadFile() + new string('°', emptySpace));
            }

            bool willExit = false;
            string result = "";
            while (!willExit)
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Console.WriteLine();
                }
                Console.Write($"OS{((!string.IsNullOrWhiteSpace(currentFolder)) ? $"\\{currentFolder}" : "")}>");

                result = Console.ReadLine();
                string[] results = result.Split(' ');
                var command = results[0].ToLower();

                if (command == "dir")
                {
                    ShowDirectory(currentFolder);
                }
                else if (command == "help")
                {

                    if (results.Length == 2)
                    {
                        ShowHelp(results[1]);
                    }
                    else
                    {
                        ShowHelp(results[0]);
                    }
                }
                else if (command == "create")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: filename argument is mandatory");
                    }
                    else
                    {
                        CreateFile(currentFolder, results[1], result.Replace($"{results[0]} {results[1]} ", ""));
                    }
                }
                else if (command == "edit")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: filename argument is mandatory");
                    }
                    else
                    {
                        EditFile(currentFolder, results[1], result.Replace($"{results[0]} {results[1]} ", ""));
                    }
                }
                else if (command == "del")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: filename argument is mandatory");
                    }
                    else
                    {
                        DeleteFile(currentFolder, results[1]);
                    }
                }
                else if (command == "ren")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: old filename and new filename arguments are mandatory");
                    }
                    else
                    {
                        Rename(currentFolder, results[1], results[2]);
                    }
                }
                else if (command == "md")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: directory argument is mandatory");
                    }
                    else
                    {
                        CreateFolder(currentFolder, results[1]);
                    }
                }
                else if (command == "rd")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: directory argument is mandatory");
                    }
                    else
                    {
                        DeleteFolder(currentFolder, results[1]);
                    }
                }
                else if (command == "cd")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: directory argument is mandatory");
                    }
                    else
                    {
                        currentFolder = $"{((string.IsNullOrWhiteSpace(currentFolder)) ? results[1] : $"{currentFolder}\\{results[1]}")}";
                    }
                }
                else if (command == "cd..")
                {
                    var directoryToRemove = currentFolder.Split('\\').Last();
                    currentFolder = currentFolder.Replace(directoryToRemove, "").Trim('\\');
                }
                else if (command == "cd\\")
                {
                    currentFolder = "";
                }
                else if (command == "show")
                {
                    if (results.Length == 1)
                    {
                        Console.WriteLine("Error: filename argument is mandatory");
                    }
                    else
                    {
                        ShowFileContent(currentFolder, results[1]);
                    }
                }
                else if (command == "exit")
                {
                    willExit = true;
                }
                else if (command == "format")
                {
                    StorageFormat();
                }
                else
                {
                    currentFolder = "";
                    Console.WriteLine("Command Not Found");
                }
            }
        }

        public static string ReadFile(bool removeWhiteSpace = true)
        {
            var result = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\{StorageName}");
            return (removeWhiteSpace) ? result.Replace("°", "") : result;
        }

        public static bool WriteFile(string input)
        {
            if (input.Length > fileSystemSize)
            {
                Console.WriteLine("Error: Storage limit reached");
                return false;
            }
            else
            {
                var emptySpace = fileSystemSize - input.Length;
                var emptyString = new string('°', emptySpace);
                File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\{StorageName}", input + emptyString);
                return true;
            }
        }

        public static List<FileItem> GetFiles()
        {
            var result = new List<FileItem>();
            var fileSplit = ReadFile().Split('¶');
            if (fileSplit.Length > 0)
            {
                foreach (var p in fileSplit)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        var fileItems = p.Split('§');
                        result.Add(new FileItem()
                        {
                            Content = ((fileItems.Length > 2) ? fileItems[2] : ""),
                            FileName = fileItems[1],
                            FileType = ((fileItems.Length > 2) ? FileTypeEnum.File : FileTypeEnum.Folder),
                            ParentFolder = fileItems[0]
                        });
                    }
                }
            }

            return result;
        }

        public static string GenerateString(List<FileItem> files)
        {
            var result = "";
            foreach (var p in files)
            {
                result = $"{result}¶{p.ParentFolder}§{p.FileName}{((p.FileType == FileTypeEnum.File) ? $"§{p.Content}" : "")}¶";
            }
            return result;
        }

        public static void ShowDirectory(string directory = "")
        {
            Console.WriteLine($"Files and folders under directory {((string.IsNullOrWhiteSpace(directory)) ? "\\" : directory)}\n");

            foreach (var p in (from q in GetFiles() where q.ParentFolder.ToLower() == directory.ToLower() select q).ToList())
            {
                Console.WriteLine(p.FileType.ToString() == "Folder" ? p.FileName + "\\f" : p.FileName + "\\d");
            }
        }

        public static void CreateFile(string directory, string filename, string content)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var files = GetFiles();
                if ((from p in files where p.ParentFolder.ToLower() == directory.ToLower() && p.FileName.ToLower() == filename.ToLower() select p).Count() > 0)
                {
                    Console.WriteLine("Error: A file with the same name already exists.");
                }
                else
                {
                    files.Add(new FileItem()
                    {
                        Content = content,
                        FileName = filename,
                        FileType = FileTypeEnum.File,
                        ParentFolder = directory
                    });
                    if (WriteFile($"{GenerateString(files)}"))
                    {
                        Console.WriteLine($"File {filename} created");
                    }
                }
            }
        }

        public static void EditFile(string directory, string filename, string content)
        {
            var files = GetFiles();
            foreach (var p in files)
            {
                if (p.FileName.ToLower() == filename.ToLower() && p.ParentFolder.ToLower() == directory.ToLower())
                {
                    p.Content = content;
                }
            }

            if (WriteFile($"{GenerateString(files)}"))
            {
                Console.WriteLine($"File {filename} edited");
            }
        }

        public static void DeleteFile(string directory, string filename)
        {
            var files = GetFiles();
            files.RemoveAll(x => x.FileType == FileTypeEnum.File && x.FileName.ToLower() == filename.ToLower() && x.ParentFolder.ToLower() == directory.ToLower());
            if (WriteFile($"{GenerateString(files)}"))
            {
                Console.WriteLine($"File {filename} removed");
            }
        }

        public static void Rename(string directory, string filename, string newFilename)
        {
            var files = GetFiles();
            if ((from p in files where p.ParentFolder.ToLower() == directory.ToLower() && p.FileName.ToLower() == newFilename.ToLower() select p).Count() > 0)
            {
                Console.WriteLine("Error: A file with the same name already exists.");
            }
            else
            {
                foreach (var p in files)
                {
                    if (p.FileName.ToLower() == filename.ToLower() && p.ParentFolder.ToLower() == directory.ToLower())
                    {
                        p.FileName = newFilename;
                    }
                }

                if (WriteFile($"{GenerateString(files)}"))
                {
                    Console.WriteLine($"File {filename} renamed");
                }
            }
        }

        public static void CreateFolder(string directory, string folder)
        {
            var files = GetFiles();
            if ((from p in files where p.ParentFolder.ToLower() == directory.ToLower() && p.FileName.ToLower() == folder.ToLower() select p).Count() > 0)
            {
                Console.WriteLine("Error: A file ro folder with the same name already exists.");
            }
            else
            {
                files.Add(new FileItem()
                {
                    Content = "",
                    FileName = folder,
                    FileType = FileTypeEnum.Folder,
                    ParentFolder = directory
                });

                if (WriteFile($"{GenerateString(files)}"))
                {
                    Console.WriteLine($"Directory {folder} created");
                }
            }
        }

        public static void DeleteFolder(string directory, string folder)
        {
            var files = GetFiles();
            files.RemoveAll(x => x.FileType == FileTypeEnum.Folder && x.FileName.ToLower() == folder.ToLower() && x.ParentFolder.ToLower() == directory.ToLower());
            if (WriteFile($"{GenerateString(files)}"))
            {
                Console.WriteLine($"Directory {folder} removed");
            }
        }

        public static void ShowFileContent(string directory, string filename)
        {
            var file = (from p in GetFiles() where p.FileName.ToLower() == filename.ToLower() && p.ParentFolder.ToLower() == directory.ToLower() select p).FirstOrDefault();
            if (file.FileType == FileTypeEnum.Folder)
            {
                Console.WriteLine("there's no file with this name, directories can't be used");
            }
            else
            {
                Console.Write(file.Content);
                Console.WriteLine();
            }
        }

        public static void EmulateKeyboard(string input)
        {

        }

        public static void StorageFormat()
        {
            WriteFile("");
            Console.WriteLine("All data erased");
        }

        public static void ShowHelp(string command)
        {
            switch (command)
            {
                case "dir":
                    Console.WriteLine("dir > displays the files and folder under current directory");
                    break;
                case "help":
                    Console.WriteLine("help [command] > displays the sintaxys for this \"command\"");
                    ShowHelp("cd");
                    Console.WriteLine("");
                    ShowHelp("cd..");
                    Console.WriteLine("");
                    ShowHelp("create");
                    Console.WriteLine("");
                    ShowHelp("del");
                    Console.WriteLine("");
                    ShowHelp("dir");
                    Console.WriteLine("");
                    ShowHelp("edit");
                    Console.WriteLine("");
                    ShowHelp("exit");
                    Console.WriteLine("");
                    ShowHelp("format");
                    Console.WriteLine("");
                    ShowHelp("md");
                    Console.WriteLine("");
                    ShowHelp("rd");
                    Console.WriteLine("");
                    ShowHelp("ren");
                    Console.WriteLine("");
                    ShowHelp("show");
                    Console.WriteLine("");
                    break;
                case "create":
                    Console.WriteLine("create [filename] [content string] > creates a file with the name \"filename\" and with the content \"content string\"");
                    break;
                case "edit":
                    Console.WriteLine("edit [filename] [content string] > replaces the content of \"filename\" with the \"content string\"");
                    break;
                case "del":
                    Console.WriteLine("del [filename] > deletes the file with name \"filename\"");
                    break;
                case "ren":
                    Console.WriteLine("ren [old name] [new name]> renames the file or folder \"old name\" with \"new name\"");
                    break;
                case "md":
                    Console.WriteLine("md [folder name] > creates a folder with name \"folder name\"");
                    break;
                case "rd":
                    Console.WriteLine("rd [folder name] > deletes the folder with name \"folder name\"");
                    break;
                case "cd":
                    Console.WriteLine("cd [folder name] > enters into the folder with name \"folder name\"");
                    break;
                case "cd..":
                    Console.Write("cd.. > returns to parent folder");
                    break;
                case "cd\\":
                    Console.WriteLine("cd\\ > returns to root folder");
                    break;
                case "show":
                    Console.WriteLine("show [filename] > shows the content of file with name [filename]");
                    break;
                case "exit":
                    Console.WriteLine("exit > closes the program");
                    break;
                case "format":
                    Console.WriteLine("format > removes all files and folders from storage file");
                    break;
            }
        }
    }

    class FileItem
    {
        public string FileName { get; set; }
        public string ParentFolder { get; set; }
        public string Content { get; set; }
        public FileTypeEnum FileType { get; set; }
    }

    enum FileTypeEnum
    {
        File = 1,
        Folder = 2
    }
}
