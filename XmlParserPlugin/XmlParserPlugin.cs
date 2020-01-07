using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Rainmeter;

namespace XmlParserPlugin
{
    class Measure
    {
        public string result;
        API api;

        static public implicit operator Measure(IntPtr data)
        {
            return (Measure)GCHandle.FromIntPtr(data).Target;
        }
        public IntPtr buffer = IntPtr.Zero;

        public double Update()
        {
            var source = api.ReadString("Source", "");
            var query = api.ReadString("Query", "");
            var join = api.ReadString("Join", ",");

            api.Log(API.LogType.Debug, $"Source: {source}");
            api.Log(API.LogType.Debug, $"Query: {query}");

            result = Parse(source, query, join);

            double numericResult;
            var isNumeric = double.TryParse(result, out numericResult);
            return isNumeric ? numericResult : 0.0;
        }
        public void Reload(API rm)
        {
            api = rm;
        }

        public string Parse(string source, string query, string join)
        {
            var result = "";

            try
            {
                var doc = XDocument.Parse(source);
                var eval = doc.XPathEvaluate(query);
                if (eval is bool || eval is double || eval is string)
                {
                    result = eval.ToString();
                }
                else
                {
                    var list = ((IEnumerable)eval).Cast<XObject>();
                    if (list.Any())
                    {
                        result = string.Join(join, list.Select(x => x.ToString()));
                    }
                    else 
                    {
                        api.Log(API.LogType.Warning, "Query Result was empty");
                    }
                }
            }
            catch (XmlException e)
            {
                api.Log(API.LogType.Error, e.Message);
            }
            catch (XPathException e)
            {
                api.Log(API.LogType.Error, e.Message);
            }
            api.Log(API.LogType.Debug, $"Result: {result}");
            return result;
        }
    }

    public class Plugin
    {

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
            }
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = data;
            measure.Reload(rm);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = data;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
                measure.buffer = IntPtr.Zero;
            }

            measure.buffer = Marshal.StringToHGlobalUni(measure.result);

            return measure.buffer;
        }
    }
}

