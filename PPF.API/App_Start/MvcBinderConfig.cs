using PPF.API.Binders.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace PPF.API
{
    public class MvcBinderConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //ModelBinders.Binders.Remove(typeof(byte[]));
            //ModelBinders.Binders.Add(typeof(byte[]), new FileModelBinder());
            ModelBinders.Binders.Add(typeof(UploadedFileInfo), new UploadedFilesInfoBinder());
            ModelBinders.Binders.Add(typeof(IEnumerable<UploadedFileInfo>), new UploadedFilesInfoBinder());
            
        }
    }
}