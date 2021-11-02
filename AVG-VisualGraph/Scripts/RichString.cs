using System.Collections.Generic;
public class RichString
{
    public static string Bold(string msg)
    {
        return string.Format("<b>{0}</b>", msg);
    }
    public static string Italic(string msg)
    {
        return string.Format("<i>{0}</i>", msg);
    }
    public static string Size(string msg,int sizeValue)
    {
        return string.Format("<size={0}>{1}</size>",sizeValue.ToString(), msg);
    }

    #region 颜色
    public enum ColorType
    {
        None,
        Red, Yellow, Blue, Green, Balck, White, Cyan, Orange, Purple, Brown, Grey,
        /// <summary>
        /// 蓝绿色
        /// </summary>
        Teal,
        /// <summary>
        /// 青绿色
        /// </summary>
        Lime
    }
    private static Dictionary<ColorType, string> DictColor = new Dictionary<ColorType, string>()
    {
        [ColorType.None] = "",
        [ColorType.Green] = "#008000ff",
        [ColorType.Yellow] = "#ffff00ff",
        [ColorType.Red] = "#ff0000ff",
        [ColorType.Blue] = "#0000ffff",
        [ColorType.Balck] = "#000000ff",
        [ColorType.White] = "#ffffffff",
        [ColorType.Cyan] = "#00ffffff",
        [ColorType.Orange] = "#ffa500ff",
        [ColorType.Purple] = "#800080ff",
        [ColorType.Brown] = "#a52a2aff",
        [ColorType.Grey] = "#808080ff",
        [ColorType.Teal] = "#008080ff",
        [ColorType.Lime] = "#00ff00ff",

    };

    public static string ColorRed(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Red]);
    }
    public static string ColorYellow(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Yellow]);
    }
    public static string ColorBlue(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Blue]);
    }
    public static string ColorGreen(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Green]);
    }
    public static string ColorCyan(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Cyan]);
    }
    public static string ColorOrange(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Orange]);
    }
    public static string ColorLime(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.Lime]);
    }
    public static string ColorWhite(string msg)
    {
        return ColorHex(msg, DictColor[ColorType.White]);
    }
    /// <summary>
    /// Color 富文本
    /// </summary>
    /// <param name="msg">字符信息</param>
    /// <param name="colorType">颜色类型</param>
    /// <param name="hex_value">颜色16进制(如红:#ff0000ff) 如果有优先使用该值</param>
    /// <returns></returns>
    public static string Color(string msg, ColorType colorType = ColorType.None, string hex_value = null)
    {
        if (string.IsNullOrEmpty(hex_value))
        {
            return ColorHex(msg, DictColor[colorType]);
        }
        else
        {
            return ColorHex(msg, hex_value);
        }
    }

    private static string ColorHex(string msg, string hex_value)
    {
        return string.Format("<color={0}>{1}</color>", hex_value, msg);
    } 
    #endregion
}


