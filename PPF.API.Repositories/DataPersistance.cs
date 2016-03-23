using FileLock;
using PPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPF.API.Repositories
{
    /// <summary>
    /// Impliment CRUD
    /// </summary>
    internal class DataPersistance
    {
        #region CRUD
        public Op<T> Create<T>(Table tableName, T data) where T : class
        {
            var path = MakesureTableExist(tableName);



            var readResult = ReadJson<T>(path);
            if (!readResult.Succeeded)
                return new Op<T>(readResult.Meta.Message, readResult.Meta.Code, null);
            List<T> list = null;
            if (readResult.Data == null || readResult.Data.Count() == 0)
                list = new List<T>();
            else
                list = readResult.Data.ToList();

            list.Add(data);

            var result = Persist<T>(path, list);

            return new Op<T>(result.Meta.Message, result.Meta.Code, data);
        }


        public Op<IEnumerable<T>> Read<T>(Table tableName) where T : class
        {
            var path = MakesureTableExist(tableName);
            var readResult = ReadJson<T>(path);
            if (!readResult.Succeeded)
                return new Op<IEnumerable<T>>(readResult.Meta.Message, readResult.Meta.Code, null);

            return new Op<IEnumerable<T>>(readResult.Data);
        }


        public Op<int> Update<T>(Table tableName, Predicate<T> where) where T : class
        {
            var path = MakesureTableExist(tableName);

            return new Op<int>(1);
        }

        public Op<int> Delete<T>(Table tableName, Predicate<T> where) where T : class
        {
            var path = MakesureTableExist(tableName);

            return new Op<int>(1);
        }
        #endregion

        #region Helper
        private string MakesureTableExist(Table tableName)
        {

            var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = path.Replace("file:///", "");
            path = System.IO.Path.GetDirectoryName(path);
            path = path.Substring(0, path.LastIndexOf('\\'));
            var directory = System.IO.Path.Combine(path, "App_Data");
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            var file = System.IO.Path.Combine(directory, string.Format("{0}.table.json", tableName));
            if (!System.IO.File.Exists(file))
                System.IO.File.Create(file);

            return file;

        }

        private Op<IEnumerable<T>> ReadJson<T>(string path) where T : class
        {

            IEnumerable<T> data = null;
            try
            {
                var rawData = System.IO.File.ReadAllText(path);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(rawData);
                if (data == null)
                    data = new List<T>();
            }
            catch (Exception ex)
            {
                return new Op<IEnumerable<T>>(ex.Message, 500, data);
            }

            return new Op<IEnumerable<T>>("Ok", data);


        }


        private Op<bool> Persist<T>(Table tableName, IEnumerable<T> data) where T : class
        {
            var path = MakesureTableExist(tableName);
            return Persist<T>(path, data);

        }

        private Op<bool> Persist<T>(string path, IEnumerable<T> data) where T : class
        {

            try
            {
                // Lock the file while writting. This is indirect locking

                var name = System.IO.Path.GetFileName(path);
                var fileLock = FileLock.SimpleFileLock.Create(name, 10, "App_Data");
                var intercheckWaitSec = 3;

                for (var i = 0; i < fileLock.LockTimeout.TotalSeconds / intercheckWaitSec; i++)
                {
                    var acquired = fileLock.TryAcquireLock();
                    if (acquired)
                    {
                        var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText(path, jsonStr);
                        fileLock.ReleaseLock();
                        return new Op<bool>("Data successfully saved");
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(intercheckWaitSec));
                    }
                }
                return new Op<bool>("Data failed to saved, may be file is locked with another process", 500, false);
            }
            catch (Exception ex)
            {
                return new Op<bool>(ex.Message, 500, false);
            }


        }
        #endregion
    }




    internal enum Table : int
    {
        Member,
        ExternalLogin,
        Role,
        UserRole

    }
}
