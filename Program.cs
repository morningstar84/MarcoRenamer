using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcoRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            // var dir = "/Users/lcauzzi/Downloads/palladino";
            var dir = args[0];
            var dirInfo = new DirectoryInfo(dir);
            var hiddenFolders = dirInfo.GetDirectories("*", SearchOption.AllDirectories)
                .Where(d => (d.Attributes & FileAttributes.Hidden) != 0)
                .Select(d => d.FullName);

            var files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => (f.Attributes & FileAttributes.Hidden) == 0 && 
                            !hiddenFolders.Any(d => f.FullName.StartsWith(d)));
            
            var destDir = Path.Combine(dir, "new");
            var prepare = PrepareDestinationDirectory(destDir);
            if (!prepare)
            {
                System.Environment.Exit(1);
                return;
            }

            foreach (var file in files)
            {
                var result =CreateSingleFile(file.FullName, destDir);
                if (!result)
                    Console.WriteLine($"Could not copy {file}");
            }

            Console.WriteLine("Program ended!");
        }

        private static bool CreateSingleFile(string filePath, string destDir)
        {
            try
            {
                var file = Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);
                
                var magazine = file.Substring(0, 4);
                var year = file.Substring(4, 4);
                var month = file.Substring(8, 2);
                var day = file.Substring(10, 2);
                var vintage = file.Substring(12, 6);
                var dossier = file.Substring(18, 8);
                var pageNumber = file.Substring(26, 6);
                
                var magazineDir = Path.Combine(destDir, magazine);
                var numberVintageYearDir = Path.Combine(magazineDir, $"{vintage}-{year}");
                var dossierDir = Path.Combine(numberVintageYearDir, $"{dossier}-{year}{month}{day}");
                var subDir = new List<string>()
                {
                    magazineDir,
                    numberVintageYearDir,
                    dossierDir
                };
                
                foreach (var d in subDir)
                {
                    CreateDir(d);
                }

                var destFile = Path.Combine(dossierDir, $"{file}{ext}");
                File.Copy(filePath, destFile);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not rename file {filePath}\n" + e.Message);
            }
            return false;
        }

        private static bool CreateDir(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not create dir {dir}:\n" + e.Message);
            }
            return false;
        }

        private static bool PrepareDestinationDirectory(string dir)
        {
            try
            {
                
                if (Directory.Exists(dir))
                {
                    DeleteDirectory(dir);
                }

                Directory.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("could not create destination directory -> abort\n" + ex.Message);
            }

            return false;
        }
        public static void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
        

        // private static void process

        private static string GetNewName(string oldName)
        {

            return $"";
        }
    }
}
