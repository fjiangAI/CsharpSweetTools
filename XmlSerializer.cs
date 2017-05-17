using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
/// <summary>
/// XmlSerializer 的摘要说明
/// </summary>
public static class XmlSerializer
{

    //public static void SaveToXml(string filePath, object sourceObj, Type type = null, string xmlRootName = "")
    //{
    //    if (!string.IsNullOrWhiteSpace(filePath) && sourceObj != null)
    //    {
    //        type = type != null ? type : sourceObj.GetType();

    //        using (StreamWriter writer = new StreamWriter(filePath))
    //        {
    //            System.Xml.Serialization.XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
    //                new System.Xml.Serialization.XmlSerializer(type) :
    //                new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlRootName));
    //            xmlSerializer.Serialize(writer, sourceObj);
    //        }
    //    }
    //}

    public static string SaveToXmlString(object sourceObj, Type type = null, string xmlRootName = "")
    {
        if (sourceObj != null)
        {
            type = type != null ? type : sourceObj.GetType();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();//定义一个内存流


            using (XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
                    new System.Xml.Serialization.XmlSerializer(type) :
                    new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlRootName));

                xmlSerializer.Serialize(writer, sourceObj);
                return System.Text.Encoding.UTF8.GetString(ms.GetBuffer());
            }
        }
        return "";
    }

    //public static object LoadFromXml(string filePath, Type type)
    //{
    //    object result = null;

    //    if (File.Exists(filePath))
    //    {
    //        using (StreamReader reader = new StreamReader(filePath))
    //        {
    //            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
    //            result = xmlSerializer.Deserialize(reader);
    //        }
    //    }

    //    return result;
    //}
}