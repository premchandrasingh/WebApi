using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FileLock.FileSys
{
    internal static class LockIO
    {
        private static readonly DataContractJsonSerializer JsonSerializer;

        static LockIO()
        {
            JsonSerializer = new DataContractJsonSerializer(typeof(FileLockContent), new[] { typeof(FileLockContent) }, int.MaxValue, true, null, true);
        }

        public static string GetFilePath(string lockName)
        {
            return Path.Combine(Path.GetTempPath(), lockName + ".lock");
        }

        public static string GetExecutingAssemblyPath(string lockName, string pathPostFix)
        {

            var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = path.Replace("file:///", "");
            path = System.IO.Path.GetDirectoryName(path);
            path = path.Substring(0, path.LastIndexOf('\\'));
            var directory = System.IO.Path.Combine(path, pathPostFix);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            directory = System.IO.Path.Combine(directory, lockName + ".lock");

            return directory;
            
        }

        public static bool LockExists(string lockFilePath)
        {
            return File.Exists(lockFilePath);
        }

        public static FileLockContent ReadLock(string lockFilePath)
        {
            try
            {
                using (var stream = File.OpenRead(lockFilePath))
                {
                    var obj = JsonSerializer.ReadObject(stream);
                    return (FileLockContent) obj ?? new MissingFileLockContent();
                }
            }
            catch (FileNotFoundException)
            {
                return new MissingFileLockContent();
            }
            catch (IOException)
            {
                return new OtherProcessOwnsFileLockContent();
            }
            catch (Exception) //We have no idea what went wrong - reacquire this lock
            {
                return new MissingFileLockContent();
            }
        }

        public static bool WriteLock(string lockFilePath, FileLockContent lockContent)
        {
            try
            {
                using (var stream = File.Create(lockFilePath))
                {
                    JsonSerializer.WriteObject(stream, lockContent);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteLock(string lockFilePath)
        {
            try
            {
                File.Delete(lockFilePath);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
