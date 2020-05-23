using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace AltLoader
{
    class Program
    {
        static string[] findDirs = new string[] { "data", "modules" };
        static string[] findFiles = new string[] { "AltV.Net.Host.dll", "AltV.Net.Host.runtimeconfig.json", "start.sh" };
        static string[] containtFiles = new string[] { "altv-server", "libnode", "libnode.so" };

        static string mainUrl = "https://cdn.altv.mp";
        static string serverUrl = $"{mainUrl}/server";
        static string coreClrModuleUrl = $"{mainUrl}/coreclr-module";
        static string nodeModuleUrl = $"{mainUrl}/node-module";

        static void Main(string[] args)
        {
            Console.WriteLine("Input the path or press Enter if the Loader is in the server folder");
            Console.Write("Path: ");

            String path = Console.ReadLine().Trim();

            String branch = null;

            while (branch == null)
            {
                branch = RequestBranches();
            }

            if (String.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory();
            }

            Console.WriteLine($"\nScanning: {path}");

            RemoveFiles(path);

            Console.WriteLine($"\nDownload files to: {path}");

            DownloadFiles(path, branch, "x64_win32");

            Console.WriteLine("\nFinish, Press enter to close...");
            Console.ReadLine();
        }

        private static string RequestBranches()
        {
            Console.WriteLine("\nInput type BRANCH: release (r), rc and dev");
            Console.Write($"Branch: ");

            string branch = Console.ReadLine();

            if (branch == "release" || branch == "rc" || branch == "dev")
                return branch;
            else if (branch == "r")
                return "release";
            else
                return null;
        }

        private static void DownloadFiles(String path, String branch, string platformPrefix)
        {
            Directory.CreateDirectory($"{path}/data");
            Directory.CreateDirectory($"{path}/modules");

            string serverUrl = GenerateFullPath(Program.serverUrl, branch, platformPrefix);
            string nodeModuleUrl = GenerateFullPath(Program.nodeModuleUrl, branch, platformPrefix);
            string coreClrModuleUrl = GenerateFullPath(Program.coreClrModuleUrl, branch, platformPrefix);

            WebClient wb = new WebClient();
            wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");

            Console.WriteLine($"Download server files");

            DownloadFile(wb, serverUrl, path, "altv-server.exe");
            DownloadFile(wb, serverUrl, path, "/data/vehmodels.bin");
            DownloadFile(wb, serverUrl, path, "/data/vehmods.bin");

            Console.WriteLine($"Download C# modules files");

            DownloadFile(wb, coreClrModuleUrl, path, "/AltV.Net.Host.runtimeconfig.json");
            DownloadFile(wb, coreClrModuleUrl, path, "/AltV.Net.Host.dll");
            DownloadFile(wb, coreClrModuleUrl, path, "/modules/csharp-module.dll");

            Console.WriteLine($"Download Node modules files");

            DownloadFile(wb, nodeModuleUrl, path, "libnode.dll");
            DownloadFile(wb, nodeModuleUrl, path, "/modules/node-module.dll");
        }

        private static void DownloadFile(WebClient wb, String pathUrl, String pathDisk, String pathFile)
        {
            try
            {
                wb.DownloadFile(new Uri($"{pathUrl}/{pathFile}"), $"{pathDisk}/{pathFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n {ex.Message}");
            }
        }

        private static string GenerateFullPath(String url, String branch, String platform)
        {
            return $"{url}/{branch}/{platform}";
        }

        private static void RemoveFiles(String path)
        {  
            if (Directory.Exists(path))
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);

                    if (findInArray(dirInfo.Name, findDirs))
                    {
                        dirInfo.Delete(true);

                        Console.WriteLine($"Remove Directory: {dirInfo.Name}");
                    }
                }
                

                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    String fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

                    if (findInArray(fileInfo.Name, findFiles) || containtsInArray(fileName, containtFiles))
                    {
                        fileInfo.Delete();

                        Console.WriteLine($"Remove File: {fileInfo.Name}");
                    }
                }
            }
        }

        private static Boolean findInArray(String value, string[] array)
        {
            foreach (string item in array)
            {
                if (item == value)
                {
                    return true;
                }
            }

            return false;
        }

        private static Boolean containtsInArray(String value, string[] array)
        {
            foreach (string item in array)
            {
                if (item.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
