using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;


public static class ExtensionTools
{

    #region Mathf
    /// <summary>
    /// 相对于Untiy.Mathf.Abs绝对值计算效率要高
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Abs(this float v)
    {
        return (v < 0) ? -v : v;
    }

    /// <summary>
    /// 相对于Untiy.Mathf.Abs绝对值计算效率要高
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static int Abs(this int v)
    {
        return (v < 0) ? -v : v;
    }

    /// <summary>
    /// 判断是否为奇数
    ///一般普遍采用 n % 2 == 0 的方式 位运算效率高
    ///采用(&1)运算方
    ///在二进制中，末位为 0 必然是偶数，否则是奇数，并且不论正负
    /// </summary>
    /// <returns> 如果是奇数，返回true，否则返回false</returns>
    public static bool isOdd(this int v)
    {
        return (v & 1) == 1;
    }
    #endregion

    #region String
    public const string NONE = "None";
    public static bool IsNullOrEmtryOrNone(this string str)
    {
        return string.IsNullOrEmpty(str)||str.Equals(NONE,System.StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}
