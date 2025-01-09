using System.Collections.Generic;
using System.Drawing;

namespace MiddlewareTool.OpenXML
{
    public class FormatStyle
    {
        public Font Font { set; get; }
        public bool WrapText { set; get; }
        public bool AutoFitColumns { set; get; }
    }
    public class ConfigInfo
    {
        public ConfigInfo()
        {
            this.Fields = new List<FieldInfo>();
        }
        public IList<FieldInfo> Fields { get; set; }
    }
    public class FieldInfo
    {
        public string Name { get; set; }
        public string ExcelAddress { get; set; }
        public int ExcelRow { get; set; }
        public int ExcelColumn { get; set; }
        public string Type { get; set; }
    }
    public class SheetInfo
    {
        public string SheetName { get; set; }
        public int SheetIndex { get; set; }
    }
    public class RangeInfo
    {
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public int FromColumn { get; set; }
        public int ToColumn { get; set; }
        public Color BackGround { get; set; }
    }
    public class MergeAddress
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
    public class ChartData
    {
        public string[] SeriesNames { get; set; }
        public string[] CategoryNames { get; set; }
        public double[][] Values { get; set; }
    }
    public class WordFontStyles
    {
        public string Font { get; set; }
        public string FontSize { get; set; }
        public string HyperLink { get; set; }
        public string Contains { get; set; }
        public string HyperLinkText { get; set; }
    }
}
