using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace PPF.API
{
    public class Formatter
    {
        public static JsonMediaTypeFormatter JsonFormatter { get { return GlobalConfiguration.Configuration.Formatters.JsonFormatter; } }

        public static XmlMediaTypeFormatter XmlFormatter { get { return GlobalConfiguration.Configuration.Formatters.XmlFormatter; } }

        public static void SetJsonFormatter()
        {
            //var formatter = new JsonMediaTypeFormatter();
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            var jsonSetting = formatter.SerializerSettings;

            jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            jsonSetting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

            // This is responsible to convert enum types into string form
            jsonSetting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            //// Ignore all null value properties from serialization. 
            //jsonSetting.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            // This need to verify the impact
            jsonSetting.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            jsonSetting.Culture = new CultureInfo("en-US");
#if  DEBUG
            // not willing to format at production
            jsonSetting.Formatting = Newtonsoft.Json.Formatting.Indented;
#endif

        }

        public static void SetXmlFormatter()
        {
            var formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            
            // Configure XmlMediaTypeFormatter to use the XmlSerializer instead of the DataContractSerializer
            formatter.UseXmlSerializer = true;
        }
    }
}