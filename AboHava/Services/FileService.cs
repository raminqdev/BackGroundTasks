using System;
using System.IO;
using System.Threading.Tasks;

namespace AboHava.Services
{
    public class FileService : IFileService
    {
        private static readonly object lockObject = new();
        private const string filePath = "Data";
        private const string fileName = "Weather.txt";

        public Task WriteToFile(string message)
        {
            string str = Path.Combine(Environment.CurrentDirectory, filePath);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            string path = Path.Combine(str, fileName);
            lock (lockObject)
            {
                return File.WriteAllTextAsync(path, message);
            }
        }

        public Task<string> ReadFromFile()
        {
            string str = Path.Combine(Environment.CurrentDirectory, filePath);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            string path = Path.Combine(str, fileName);
            lock (lockObject)
            {
                return File.ReadAllTextAsync(path);
            }
        }
    }


    public interface IFileService
    {
        Task WriteToFile(string message);
        Task<string> ReadFromFile();
    }
}
