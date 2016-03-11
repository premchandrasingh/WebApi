using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.Models
{
    /// <summary>
    /// Operation result carrier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Op<T>
    {
        public Op(T data) : this(null, data)
        {

        }

        public Op(string message)
        {
            this.Meta = new Info(message);
        }

        public Op(string message, T data) : this(message, 200, data)
        {

        }

        public Op(string message, int code, T data)
        {
            this.Data = data;
            this.Meta = new Info(message ?? "", code);
        }

        public bool Succeeded
        {
            get
            {
                return this.Meta != null && this.Meta.Code == 200;
            }
        }

        public T Data { get; set; }

        public Info Meta { get; set; }

    }

    public class Info
    {

        #region Constructor
        public Info(string message) : this(message, 200) { }

        public Info(string message, int code)
        {
            this.Message = message ?? "";
            this.Code = code;
        }
        #endregion

        public string Message { get; set; }

        public int Code { get; set; }

    }
}
