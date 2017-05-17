using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// WordCount 的摘要说明
/// 已检验，未作修改
/// </summary>
public class WordCount
{
    private Dictionary<string, int> dic = null;

    public Dictionary<string, int> Dic
    {
        get
        {
            return dic;
        }

        set
        {
            dic = value;
        }
    }
    public WordCount()
    {
        dic = new Dictionary<string, int>();
    }
    /// <summary>
    /// 添加一个单词
    /// </summary>
    /// <param name="str">要添加的</param>
    public void add(string str)
    {
        if (dic.ContainsKey(str))
        {
            int count = dic[str];
            count++;
            dic[str] = count;
        }
        else
        {
            dic.Add(str, 1);
        }
    }
    /// <summary>
    /// 删除str这个元素
    /// </summary>
    /// <param name="str">要删除的数</param>
    public void remove(string str)
    {
        dic.Remove(str);
    }
    /// <summary>
    /// 返回增序
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public Dictionary<string, int> Asc(string types)
    {
        switch (types)
        {
            case "key":
                {
                    Dictionary<string, int> dic1Asc = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
                    return dic1Asc;
                }
            default:
                {
                    Dictionary<string, int> dic1Asc = dic.OrderBy(o => o.Value).ToDictionary(o => o.Key, p => p.Value);
                    return dic1Asc;
                }
        }
    }
    /// <summary>
    /// 返回降序排列
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public Dictionary<string, int> Desc(string types)
    {
        switch (types)
        {
            case "key":
                {
                    Dictionary<string, int> dic1Asc = dic.OrderByDescending(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
                    return dic1Asc;
                }
            default:
                {
                    Dictionary<string, int> dic1Asc = dic.OrderByDescending(o => o.Value).ToDictionary(o => o.Key, p => p.Value);
                    return dic1Asc;
                }
        }
    }
}