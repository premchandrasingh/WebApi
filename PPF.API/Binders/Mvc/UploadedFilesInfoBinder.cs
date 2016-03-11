using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPF.API.Binders.Mvc
{
    /// <summary>
    /// Single upload file binder
    /// </summary>
    public class UploadedFilesInfoBinder : IModelBinder
    {
        /// <summary>
        ///  Bind MULTIPLE upload file
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            if (controllerContext.HttpContext.Request.Files.Count == 0)
                return null;

            var files = controllerContext.HttpContext.Request.Files;
            var list = new List<UploadedFileInfo>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength == 0)
                    continue;
                var name = files.AllKeys[i];
                var fileInfo = new UploadedFileInfo(name, file);
                list.Add(fileInfo);
            }
            if (list.Count > 1)
                return list.ToArray();
            else if (list.Count == 1)
                return list.First();
            else
                return null;
        }
    }
}