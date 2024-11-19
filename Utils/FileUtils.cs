using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LifeManager.Utils
{
    public class FileUtils
    {
        public static string savePath = @"D:\\Notes";

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

        public static void SaveTextToFile(string content,string filePath)
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

        public static string ReadTextFromFile(string filePath)
        {
            // 从文件读取内容
            return File.ReadAllText(filePath);
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

        public static List<string> GetAllTxtFiles()
        {
            string directoryPath = "";
            if (Common.IsAll)
            {
                //读取全部
                 directoryPath = savePath;
            }
            else {
                //读取选择的
                directoryPath = savePath + @$"\{Common.SelectedDateTime}";
            }

            List<string> txtFiles = new List<string>();
            try
            {
                // 获取当前目录下所有的 .txt 文件
                txtFiles.AddRange(Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取.tex文件失败: {ex.Message}");
            }
            return txtFiles;

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

        public static void CreateFile(string filePath)
        {
            // 创建空文件
            using (FileStream fs = File.Create(filePath))
            {
                // 不需要写入任何内容，文件已创建
            }
        }
    }
}
