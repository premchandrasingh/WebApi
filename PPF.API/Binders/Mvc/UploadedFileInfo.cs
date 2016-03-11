using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPF.API.Binders.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadedFileInfo
    {
        /// <summary>
        /// Model name
        /// </summary>
        public string ModelName { get; private set; }

        /// <summary>
        /// Raw posted file
        /// </summary>
        public HttpPostedFileBase File { get; private set; }

        /// <summary>
        /// File as byte array
        /// </summary>
        public byte[] Bytes { get; private set; }



        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// File Extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// MIME(Multi-purpose Internet Mail Extensions) type
        /// </summary>
        public string MIMEType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="file"></param>
        public UploadedFileInfo(string modelName, HttpPostedFileBase file)
        {
            ModelName = modelName;

            if (file != null)
            {
                this.File = file;
                this.FileName = file.FileName;
                this.Extension = System.IO.Path.GetExtension(file.FileName).Substring(1);
                this.Size = file.ContentLength;
                this.MIMEType = file.ContentType;


                using (System.IO.Stream inputStream = file.InputStream)
                {
                    System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new System.IO.MemoryStream();
                        inputStream.CopyTo(memoryStream);
                        Bytes = memoryStream.ToArray();
                    }
                }

            }
        }
    }
}