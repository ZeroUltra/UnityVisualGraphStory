using System.Text;
using UnityEngine;
using Object = System.Object;
/// <summary>
/// ____AUTHOR:    Luojie
/// ____DESC:      文件描述
/// </summary>
public class UnityJsonUtility
{
    /*  \ 是转义符
        \’ 单引号
    \” 双引号
    \\ 反斜杠
    \0 空
    \a 警告（产生峰鸣）
    \b 退格
    \f 换页
    \n 换行
    \r 回车
    \t 水平制表符
    \v 垂直制表符*/

    /// <summary>
    /// 将对象序列化成json 数组 也就是 <code>[...json...]</code>
    /// </summary>
    public static string ToJsonArray<T>(T t, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(t, prettyPrint);
    }
    /// <summary>
    /// 将对象序列化成json 对象,也就是 <code>{...json...}</code>
    /// </summary>
    public static string ToJsonObject<T>(T t, string prefix = "listJson", bool prettyPrint = false)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ \"" + prefix + "\": ");
        sb.Append(JsonUtility.ToJson(t, prettyPrint));
        sb.Append("}");
        return sb.ToString();
    }
    /// <summary>
    /// 将Json数组转换化成json 对象,也就是从<code>[...json...]</code> To <code>{...json...}</code>
    /// </summary>
    /// <param name="srcJson">原始json字符串</param>
    /// <param name="prefix">前缀</param>
    /// <returns></returns>
    public static string ToJsonObject(string srcJson, string prefix = "listJson")
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ \"" + prefix + "\": ");
        sb.Append(srcJson);
        sb.Append("}");
        return sb.ToString();
    }


    /// <summary>
    /// 将json字符串数组反序列化成对象,
    /// </summary>
    public static T FormJson<T>(string strJsonArray)
    {
        return JsonUtility.FromJson<T>(strJsonArray);
    }
    /// <summary>
    /// 将json字符串数组反序列化成对象,覆盖原来的类
    /// </summary>
    public static void FromJsonOverwrite(string strJson, Object @object)
    {
        JsonUtility.FromJsonOverwrite(strJson, @object);
    }

}
