using System;
using System.IO;

namespace LazyPhysicist.Common
{
    public static class FileSystem
    {
        private static readonly char[] invalidPathChars = Path.GetInvalidFileNameChars();

        public static string ClearFileNameFromInvalidChars(string input)
        {
            if (input == null) return "";

            int i;
            do
            {
                i = input.IndexOfAny(invalidPathChars);
                if (i != -1)
                {
                    input = input.Replace(input[i], '_');
                }
            }
            while (i != -1);

            return input;
        }

        public static bool CheckPathOrCreate(string path)
        {
            path = Environment.ExpandEnvironmentVariables(path);
            bool r;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                r = Directory.Exists(path);
            }
            else
            {
                r = true;
            }

            return r;
        }

        public static string MakeUniqueFilePath(string path, string fileName, string extension)
        {
            if (CheckPathOrCreate(path))
            {
                if (File.Exists($"{path}\\{fileName}.{extension}"))
                {
                    int i = 1;
                    do
                    {
                        fileName = $"{fileName}({i++})";
                    }
                    while (File.Exists($"{path}\\{fileName}.{extension}"));
                }

            }
            else
            {
                throw new DirectoryNotFoundException($"Can't create the path: {path}");
            }
            return $"{path}\\{fileName}.{extension}";
        }
    }
}
