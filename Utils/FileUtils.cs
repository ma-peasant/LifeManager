using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LifeManager.Utils
{
    public class FileUtils
    {
        public static string savePath;

        // 静态构造函数，用于根据操作系统设置 savePath
        static FileUtils()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                savePath = Path.Combine(@"D:\", "Notes");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes");
                //  ~/.local / share / Notes
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "LifeManager", "Notes");
                //~/Library/Application Support/Notes
            }
            else
            {
                throw new PlatformNotSupportedException("当前操作系统不受支持");
            }
        }

        public static void CreateFile(string filePath)
        {
            // 创建空文件
            using (FileStream fs = File.Create(filePath))
            {
                // 不需要写入任何内容，文件已创建
            }
        }

        public static List<string> GetAllTxtFiles()
        {
            DateTime startDate, endDate;
            // 尝试解析 SelectedDateTime 和 SelectedDateTimeEnd
            if (!DateTime.TryParse(Common.SelectedDateTime, out startDate) || !DateTime.TryParse(Common.SelectedDateTimeEnd, out endDate))
            {
                Console.WriteLine("日期格式不正确");
                return new List<string>();
            }

            List<string> txtFiles = new List<string>();
            try
            {
                // 获取根目录下的所有文件夹
                var directories = Directory.GetDirectories(savePath);

                if (Common.IsAll)
                {
                    txtFiles.AddRange(Directory.GetFiles(savePath, "*.txt", SearchOption.AllDirectories));
                }
                else
                {
                    foreach (var directory in directories)
                    {
                        // 假设文件夹名称是 "yyyy-MM-dd" 格式
                        string folderName = Path.GetFileName(directory);

                        // 尝试将文件夹名解析为 DateTime
                        if (DateTime.TryParseExact(folderName, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime folderDate))
                        {
                            // 判断日期是否在范围内
                            if (folderDate >= startDate && folderDate <= endDate)
                            {
                                // 获取该文件夹下所有的 .txt 文件
                                txtFiles.AddRange(Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 .txt 文件失败，错误信息: {ex.Message}");
            }

            return txtFiles;
        }

        public static void ReadFileWithStreamReader(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public static string ReadTextFromFile(string filePath)
        {
            // 从文件读取内容
            return File.ReadAllText(filePath);
        }

        public static string ReadTxtFile(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时发生错误: {ex.Message}");
                return string.Empty;
            }
        }

        public static void RenameFile(string oldFilePath, string newFileName)
        {
            try
            {
                // 获取文件所在的目录
                string directory = Path.GetDirectoryName(oldFilePath);

                // 构造新文件的完整路径
                string newFilePath = Path.Combine(directory, newFileName);

                // 重命名文件
                File.Move(oldFilePath, newFilePath);

                Console.WriteLine($"File renamed from {oldFilePath} to {newFilePath}");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error renaming file: {ioEx.Message}");
            }
        }

        public static void SaveTextToFile(string content, string filePath)
        {
            try
            {
                // 使用 StreamWriter 写入文件，写入前可以进行更多的操作
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入文件时发生错误: {ex.Message}");
            }
        }
    }
}