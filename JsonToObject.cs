using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// JsonToObject 的摘要说明
/// </summary>
public static class JsonToObject
{

    public static List<T> JSONStringToList<T>(this string JsonStr)
    {
        JavaScriptSerializer Serializer = new JavaScriptSerializer();
        List<T> objs = Serializer.Deserialize<List<T>>(JsonStr);
        return objs;
    }

    public static T Deserialize<T>(string json)
    {
        T obj = Activator.CreateInstance<T>();
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            return (T)serializer.ReadObject(ms);
        }
    }

}