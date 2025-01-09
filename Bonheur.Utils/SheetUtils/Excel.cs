using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MiddlewareTool.Utility;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using System.Xml;

namespace MiddlewareTool.OpenXML
{
    /// <summary>
    /// Excel
    /// </summary>
    public class Excel
    {
        #region Constructors
        public Excel()
        {
            this.ParameterData = new Dictionary<string, object>();
        }

        #endregion

        #region Properties
        public byte[] TemplateFileData { get; set; }
        public ConfigInfo ConfigInfo { get; set; }
        public IDictionary<string, object> ParameterData { get; set; }
        public byte[] OutputData { get; set; }
        public Action<ExcelWorksheet, ExcelCellBase, FieldInfo> AfterFillParameter { get; set; }
        public Action<ExcelWorksheet> PrepareTemplate { get; set; }
        public Action<ExcelWorksheet> AfterFillAllData { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Export excel
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="entities">data</param>
        /// <returns></returns>
        public byte[] Export<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            return this.Export(entities, null);
        }
        /// <summary>
        /// Export excel
        /// </summary>
        /// <typeparam name="TEntity">Entiry</typeparam>
        /// <param name="entities">data</param>
        /// <param name="sheetInfo">sheet Info</param>
        /// <returns></returns>
        public byte[] Export<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo) where TEntity : class
        {
            //1. Read Config
            this.ReadConfig(sheetInfo);

            //2. Fill Paremter
            this.FillParameter(sheetInfo);

            //3. Export
            this.FillData(entities, sheetInfo);

            return this.OutputData;
        }
        /// <summary>
        /// ExportControl
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public byte[] ExportControl<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            return this.ExportControl(entities, null);
        }
        /// <summary>
        /// Export Control
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="entities">data</param>
        /// <param name="sheetInfo">sheet Info</param>
        /// <returns></returns>
        public byte[] ExportControl<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo) where TEntity : class
        {
            //1. Read Config
            this.ReadConfigControl(sheetInfo);

            //2. Fill Paremter
            this.FillParameter(sheetInfo);

            //3. Export
            this.FillDataControl(entities, sheetInfo);

            return this.OutputData;
        }


        /// <summary>
        /// ReadConfig
        /// </summary>
#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "")]
        private void ReadConfig()
        {
            this.ReadConfig(null);
        }
#pragma warning restore S1144 // Unused private types or members should be removed
        /// <summary>
        /// ReadConfig
        /// </summary>
        /// <param name="sheetInfo">sheet Info</param>
        private void ReadConfig(SheetInfo sheetInfo)
        {
            this.ConfigInfo = new ConfigInfo();

            // Thiết lập LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //1. Read data file
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                //Open Excel + Get WorkSheet
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }

                //Get Worksheet
                ExcelWorksheet m_ExcelWorksheet = this.GetWorkSheet(m_ExcelPackage, sheetInfo);
                if (m_ExcelWorksheet == null)
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                }

                //Prepare Template
                PrepareTemplate?.Invoke(m_ExcelWorksheet);

                //Get Config
                var m_Dimension = m_ExcelWorksheet.Dimension;
                var m_Cells = m_ExcelWorksheet.Cells;
                for (int m_RowIndex = 1; m_RowIndex <= m_Dimension.Rows; m_RowIndex++)
                {
                    for (int m_ColumnIndex = 1; m_ColumnIndex <= m_Dimension.Columns; m_ColumnIndex++)
                    {
                        var m_Cell = m_Cells[m_RowIndex, m_ColumnIndex];
                        string m_Text = m_Cell.Text;

                        var m_FieldInfo = this.ParseConfig(m_Text);
                        if (m_FieldInfo != null)
                        {
                            m_FieldInfo.ExcelAddress = m_Cell.Address;
                            m_FieldInfo.ExcelRow = m_RowIndex;
                            m_FieldInfo.ExcelColumn = m_ColumnIndex;
                            this.ConfigInfo.Fields.Add(m_FieldInfo);
                        }
                    }
                }

                this.TemplateFileData = m_ExcelPackage.GetAsByteArray();
            }
        }


        /// <summary>
        /// Read Config Control
        /// </summary>
#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "")]
        private void ReadConfigControl()
        {
            this.ReadConfigControl(null);
        }
#pragma warning restore S1144 // Unused private types or members should be removed
        /// <summary>
        /// Read Config Control
        /// </summary>
        /// <param name="sheetInfo">sheet Info</param>
        private void ReadConfigControl(SheetInfo sheetInfo)
        {
            this.ConfigInfo = new ConfigInfo();

            //1. Read data file
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                //Open Excel + Get WorkSheet
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }

                //Get Worksheet
                ExcelWorksheet m_ExcelWorksheet = this.GetWorkSheet(m_ExcelPackage, sheetInfo);
                if (m_ExcelWorksheet == null)
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                }
                //Prepare Template
                PrepareTemplate?.Invoke(m_ExcelWorksheet);

                //Get Config
                var m_Dimension = m_ExcelWorksheet.Dimension;
                var m_Cells = m_ExcelWorksheet.Cells;
                for (int m_RowIndex = 1; m_RowIndex <= m_Dimension.Rows + 1; m_RowIndex++)
                {
                    for (int m_ColumnIndex = 1; m_ColumnIndex <= m_Dimension.Columns; m_ColumnIndex++)
                    {
                        var m_Cell = m_Cells[m_RowIndex, m_ColumnIndex];
                        string m_Text = m_Cell.Text;

                        var m_FieldInfo = this.ParseConfig(m_Text);
                        if (m_FieldInfo != null)
                        {
                            m_FieldInfo.ExcelAddress = m_Cell.Address;
                            m_FieldInfo.ExcelRow = m_RowIndex;
                            m_FieldInfo.ExcelColumn = m_ColumnIndex;
                            this.ConfigInfo.Fields.Add(m_FieldInfo);
                        }
                    }
                }

                this.TemplateFileData = m_ExcelPackage.GetAsByteArray();
            }
        }
        protected FieldInfo ParseConfig(string text)
        {
            FieldInfo m_FieldInfo = null;

            if (text.Contains(Key_Start) && text.Contains(Key_End))
            {
                string m_TextNoKey = text.Replace(Key_Start, string.Empty).Replace(Key_End, string.Empty);
                string[] m_TextNoKeyParts = m_TextNoKey.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (m_TextNoKeyParts.Length == 2)
                {
                    m_FieldInfo = new FieldInfo()
                    {
                        Type = m_TextNoKeyParts[0],
                        Name = m_TextNoKeyParts[1]
                    };
                }
            }
            else
            {
                m_FieldInfo = null;
            }
            return m_FieldInfo;
        }
#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "")]
        private void FillParameter()
        {
            this.FillParameter(null);
        }
#pragma warning restore S1144 // Unused private types or members should be removed
        private void FillParameter(SheetInfo sheetInfo)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }

                //Get Worksheet
                ExcelWorksheet m_ExcelWorksheet = this.GetWorkSheet(m_ExcelPackage, sheetInfo);
                if (m_ExcelWorksheet == null)
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                }
                using (var m_Cells = m_ExcelWorksheet.Cells)
                {
                    FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Parameter).ToArray();
                    foreach (var m_FieldInfo in m_FieldInfos)
                    {
                        object m_Value = string.Empty;
                        if (this.ParameterData.TryGetValue(m_FieldInfo.Name, out m_Value))
                        {
                            using (var m_Cell = m_Cells[m_FieldInfo.ExcelAddress])
                            {
                                m_Cell.Value = m_Value;

                                AfterFillParameter?.Invoke(m_ExcelWorksheet, m_Cell, m_FieldInfo);
                            }
                        }
                    }
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
        }
#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "")]
        private void FillData<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            this.FillData(entities, null);
        }
#pragma warning restore S1144 // Unused private types or members should be removed
        private void FillData<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo) where TEntity : class
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }

                //Get Worksheet
                ExcelWorksheet m_ExcelWorksheet = this.GetWorkSheet(m_ExcelPackage, sheetInfo);
                if (m_ExcelWorksheet == null)
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                }
                using (var m_Cells = m_ExcelWorksheet.Cells)
                {
                    FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Field).ToArray();
                    if (m_FieldInfos.Length > 0)
                    {
                        int m_RowBeginIndex = m_FieldInfos.FirstOrDefault().ExcelRow;
                        //Insert Zone
                        m_ExcelWorksheet.InsertRow(m_RowBeginIndex + 1, entities.Count, m_RowBeginIndex); /// Previous Edit : entities.Count - 1

                        //Fill
                        int m_RowIndex = m_RowBeginIndex;
                        //Type m_EntityType = typeof(TEntity);
                        foreach (var m_Entity in entities)
                        {
                            foreach (var m_FieldInfo in m_FieldInfos)
                            {
                                var m_Value = ReflectorUtility.FollowPropertyPath(m_Entity, m_FieldInfo.Name);

                                m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //PropertyInfo m_PropertyInfo = m_EntityType.GetProperty(m_FieldInfo.Name);
                                //if (m_PropertyInfo != null)
                                //{
                                //    object m_Value = m_PropertyInfo.GetValue(m_Entity);
                                //    m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //}
                            }
                            m_RowIndex++;
                        }

                        if (AfterFillAllData != null)
                        {
                            this.AfterFillAllData(m_ExcelWorksheet);
                        }
                    }
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
        }
#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "")]
        private void FillDataControl<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            this.FillDataControl(entities, null);
        }
#pragma warning restore S1144 // Unused private types or members should be removed
        private void FillDataControl<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo) where TEntity : class
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }

                //Get Worksheet
                ExcelWorksheet m_ExcelWorksheet = this.GetWorkSheet(m_ExcelPackage, sheetInfo);
                if (m_ExcelWorksheet == null)
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                }
                using (var m_Cells = m_ExcelWorksheet.Cells)
                {
                    FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Field).ToArray();
                    if (m_FieldInfos.Length > 0)
                    {
                        int m_RowBeginIndex = m_FieldInfos.FirstOrDefault().ExcelRow;
                        //Insert Zone
                        m_ExcelWorksheet.InsertRow(m_RowBeginIndex + 1, entities.Count - 1, m_RowBeginIndex);

                        //Fill
                        int m_RowIndex = m_RowBeginIndex;
                        // Type m_EntityType = typeof(TEntity);
                        foreach (var m_Entity in entities)
                        {
                            int start = 0;
                            foreach (var m_FieldInfo in m_FieldInfos)
                            {
                                var m_Value = ReflectorUtility.FollowPropertyPath(m_Entity, m_FieldInfo.Name);
                                if (m_FieldInfo.ExcelColumn == 2 && start >= 2)
                                {
                                    m_RowIndex++;
                                }
                                m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //PropertyInfo m_PropertyInfo = m_EntityType.GetProperty(m_FieldInfo.Name);
                                //if (m_PropertyInfo != null)
                                //{
                                //    object m_Value = m_PropertyInfo.GetValue(m_Entity);
                                //    m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //}
                                start++;
                            }
                            m_RowIndex++;
                        }

                        if (AfterFillAllData != null)
                        {
                            this.AfterFillAllData(m_ExcelWorksheet);
                        }
                    }
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
        }
        public object[,] ReadData(byte[] data)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (var stream = new MemoryStream(data))
            {
                m_DataOutput = this.ReadData(stream, null);
            }

            return m_DataOutput;
        }
        public object[,] ReadData(byte[] data, SheetInfo sheetInfo)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (var stream = new MemoryStream(data))
            {
                m_DataOutput = this.ReadData(stream, sheetInfo);
            }

            return m_DataOutput;
        }
        public object[,] ReadData(Stream stream)
        {
            return this.ReadData(stream, null);
        }
        public object[,] ReadData(Stream stream, SheetInfo sheetInfo)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    var m_Dimension = m_ExcelWorksheet.Dimension;
                    var m_Cells = m_ExcelWorksheet.Cells;
                    m_DataOutput = new object[m_Dimension.Rows, m_Dimension.Columns];
                    for (int m_RowIndex = 0; m_RowIndex < m_Dimension.Rows; m_RowIndex++)
                    {
                        for (int m_ColumnIndex = 0; m_ColumnIndex < m_Dimension.Columns; m_ColumnIndex++)
                        {
                            m_DataOutput[m_RowIndex, m_ColumnIndex] = m_Cells[m_RowIndex + 1, m_ColumnIndex + 1].Value;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
            }

            return m_DataOutput;
        }
        public Dictionary<string, object> ReadCellData(byte[] data)
        {
            return this.ReadCellData(data, null);
        }


#pragma warning disable S2190 // Recursion should not be infinite
        public Dictionary<string, object> ReadCellData(byte[] data, SheetInfo sheetInfo)
#pragma warning restore S2190 // Recursion should not be infinite
        {
            using (var stream = new MemoryStream(data))
            {
                return this.ReadCellData(data, sheetInfo);
            }
        }
        public Dictionary<string, object> ReadCellData(Stream stream)
        {
            return this.ReadCellData(stream, null);
        }
        public Dictionary<string, object> ReadCellData(Stream stream, SheetInfo sheetInfo)
        {
            Dictionary<string, object> m_DataOutput = new Dictionary<string, object>();
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    //Get Define Name
                    foreach (var DefineName in m_ExcelPackage.Workbook.Names)
                    {
                        m_DataOutput.Add(DefineName.Name, m_ExcelWorksheet.Cells[DefineName.Start.Row, DefineName.Start.Column].Text);
                        DefineName.Dispose();
                    }

                    //Get Cells
                    foreach (var cell in m_ExcelWorksheet.Cells)
                    {
                        m_DataOutput.Add(cell.Address, cell.Value);
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
            }
            return m_DataOutput;
        }
        public byte[] Export(Dictionary<string, string> dicEntities)
        {
            return Export(dicEntities, null);
        }


        public byte[] Export(Dictionary<string, string> dicEntities, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    using (var m_Cells = m_ExcelWorksheet.Cells)
                    {
                        foreach (var m_dicEntitie in dicEntities)
                        {
                            using (var DefineName = m_ExcelPackage.Workbook.Names[m_dicEntitie.Key])
                            {
                                if (DefineName != null)
                                {
                                    m_Cells[DefineName.Start.Row, DefineName.Start.Column].Value = m_dicEntitie.Value;
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }

                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] MergeRow(byte[] excelData, int[] mergeColum, int startRow, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                Stream stream = new MemoryStream(excelData);
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    for (int m = 0; m < mergeColum.Length; m++)
                    {
                        int startMarkRow = 1;
                        int endMarkRow = 1;
                        string currentValue = m_ExcelWorksheet.Cells[1, mergeColum[m], 1, mergeColum[m]].Value != null ? m_ExcelWorksheet.Cells[1, mergeColum[m], 1, mergeColum[m]].Value.ToString() : string.Empty;
                        startRow = startRow > m_ExcelWorksheet.Dimension.End.Row ? m_ExcelWorksheet.Dimension.End.Row : startRow;
                        for (int i = startMarkRow; i < m_ExcelWorksheet.Dimension.End.Row; i++)
                        {
                            if (m_ExcelWorksheet.Cells[i, mergeColum[m], i, mergeColum[m]].Value != null && currentValue != m_ExcelWorksheet.Cells[i, mergeColum[m], i, mergeColum[m]].Value.ToString())
                            {
                                if (endMarkRow > startMarkRow)
                                {
                                    m_ExcelWorksheet.Cells[startMarkRow, mergeColum[m], endMarkRow, mergeColum[m]].Merge = true;
                                }
                                startMarkRow = i;
                                currentValue = m_ExcelWorksheet.Cells[i, mergeColum[m], i, mergeColum[m]].Value.ToString();
                            }
                            else
                            {
                                endMarkRow = i;
                            }
                        }
                    }

                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                //m_ExcelPackage.Save();
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] MergeRow(byte[] excelData, int[] mergeColumn, int startRow, SheetInfo sheetInfo, int endRows)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                Stream stream = new MemoryStream(excelData);
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    if (endRows <= 0)
                    {
                        endRows = m_ExcelWorksheet.Dimension.End.Row;
                    }
                    for (int m = 0; m < mergeColumn.Length; m++)
                    {
                        int countMergeRow = 0;
                        for (int i = startRow + 1; i <= endRows; i++)
                        {
                            string currentValue = m_ExcelWorksheet.Cells[i, mergeColumn[m]].Value != null ? m_ExcelWorksheet.Cells[i, mergeColumn[m]].Value.ToString() : string.Empty;
                            string oldValue = m_ExcelWorksheet.Cells[i - 1, mergeColumn[m]].Value != null ? m_ExcelWorksheet.Cells[i - 1, mergeColumn[m]].Value.ToString() : string.Empty;
                            if (oldValue == currentValue && !string.IsNullOrEmpty(currentValue))
                            {
                                countMergeRow++;
                            }
                            else
                            {
                                if (countMergeRow > 0)
                                {
                                    m_ExcelWorksheet.Cells[i - 1 - countMergeRow, mergeColumn[m], i - 1, mergeColumn[m]].Merge = true;
                                    countMergeRow = 0;
                                }
                            }
                        }
                        if (countMergeRow > 0)
                        {
                            m_ExcelWorksheet.Cells[endRows - countMergeRow, mergeColumn[m], endRows, mergeColumn[m]].Merge = true;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                //m_ExcelPackage.Save();
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] MergeColumn(byte[] excelData, int[] mergeRows, int startColumn, SheetInfo sheetInfo, int endRow = -1)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                Stream stream = new MemoryStream(excelData);
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    if (endRow <= 0)
                    {
                        endRow = m_ExcelWorksheet.Dimension.End.Column;
                    }
                    for (int m = 0; m < mergeRows.Length; m++)
                    {
                        startColumn = startColumn > m_ExcelWorksheet.Dimension.End.Column ? m_ExcelWorksheet.Dimension.End.Column : startColumn;
                        for (int i = startColumn; i < endRow; i++)
                        {
                            try
                            {
                                string currentValue = m_ExcelWorksheet.Cells[mergeRows[m], i, mergeRows[m], i].Value != null ? m_ExcelWorksheet.Cells[mergeRows[m], i, mergeRows[m], i].Value.ToString() : string.Empty;
                                string nextValue = string.Empty;
                                if (i + 1 > m_ExcelWorksheet.Dimension.End.Column)
                                {
                                    break;
                                }
                                nextValue = m_ExcelWorksheet.Cells[mergeRows[m], i + 1, mergeRows[m], i + 1].Value != null ? m_ExcelWorksheet.Cells[mergeRows[m], i + 1, mergeRows[m], i + 1].Value.ToString() : string.Empty;

                                if (nextValue == currentValue && !string.IsNullOrEmpty(currentValue) && !string.IsNullOrEmpty(nextValue))
                                {
                                    m_ExcelWorksheet.Cells[mergeRows[m], i, mergeRows[m], i + 1].Merge = true;
#pragma warning disable S1854 // Unused assignments should be removed
                                    currentValue = nextValue;
#pragma warning restore S1854 // Unused assignments should be removed
                                }
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Lỗi");
                            }
                        }
                    }

                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                //m_ExcelPackage.Save();
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] InsertColum(int fromColumn, int totalColumnInsert, string[] headerText, string[] Fields, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    m_ExcelWorksheet.InsertColumn(fromColumn, totalColumnInsert);
                    if (headerText.Length > 0)
                    {
                        // header text
                        for (int i = fromColumn; i < fromColumn + totalColumnInsert; i++)
                        {
                            m_ExcelWorksheet.Cells[1, i].Value = string.Format("{0}", headerText[i - fromColumn]);
                        }
                    }
                    // Filter data 
                    if (Fields.Length > 0)
                    {
                        for (int i = fromColumn; i < fromColumn + totalColumnInsert; i++)
                        {
                            m_ExcelWorksheet.Cells[2, i].Value = Fields[i - fromColumn];
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] InsertColum(int fromColumn, int totalColumnInsert, string[] headerText, string[] Fields, SheetInfo sheetInfo, int FromRow, int IndexRangeColumn = 1)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    m_ExcelWorksheet.InsertColumn(fromColumn, totalColumnInsert);
                    for (int i = 0; i < Fields.Count(); i++)
                    {
                        m_ExcelWorksheet.Cells[FromRow + i, i].Value = string.Format("{0}", headerText[i - fromColumn]);
                        m_ExcelWorksheet.Cells[FromRow + i + IndexRangeColumn, i].Value = Fields[i - fromColumn];
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] DeleteColumn(int columnFrom, int columns, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheet != null)
                {
                    m_ExcelWorksheet.DeleteColumn(columnFrom, columns);
                    for (int i = columnFrom; i <= columns; i++)
                    {
                        m_ExcelWorksheet.Column(i).Hidden = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] DeleteColumn(int columns, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheet != null)
                {
                    m_ExcelWorksheet.DeleteColumn(columns);
                    m_ExcelWorksheet.Column(columns).Hidden = true;
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] HideColumn(int columnFrom, int columns, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }
                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheet != null)
                {
                    var m_ColumnTo = (columnFrom + columns);
                    for (int i = columnFrom; i < m_ColumnTo; i++)
                    {
                        m_ExcelWorksheet.Column(i).Hidden = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        //public byte[] FormatTemplate(FormatStyle formatStyle, SheetInfo sheetInfo)
        //{
        //    byte[] m_OutputData = null;
        //    using (ExcelPackage m_ExcelPackage = new ExcelPackage())
        //    {
        //        // Open the Excel file and load it to the ExcelPackage
        //        using (var stream = new MemoryStream(this.TemplateFileData))
        //        {
        //            m_ExcelPackage.Load(stream);
        //        }
        //        ExcelWorksheet m_ExcelWorksheet = null;
        //        if (sheetInfo != null)
        //        {
        //            m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
        //        }

        //        else
        //        {
        //            m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
        //        }
        //        if (m_ExcelWorksheet != null)
        //        {
        //            //Get the final row for the column in the worksheet
        //            //int finalrows = m_ExcelWorksheet.Dimension.End.Row;
        //            //Convert the range to the color Red
        //            var allCells = m_ExcelWorksheet.Cells[1, 1, m_ExcelWorksheet.Dimension.End.Row, m_ExcelWorksheet.Dimension.End.Column];
        //            allCells.Style.WrapText = formatStyle.WrapText;
        //            if (formatStyle.AutoFitColumns)
        //            {
        //                allCells.AutoFitColumns();
        //            }
        //            var cellFont = allCells.Style.Font;
        //            if (formatStyle.Font != null)
        //            {
        //                cellFont.SetFromFont(formatStyle.Font);
        //            }
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Không tìm thấy Sheet tương ứng");
        //        }
        //        m_OutputData = m_ExcelPackage.GetAsByteArray();
        //    }
        //    return m_OutputData;
        //}
        public byte[] WrapText(SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    //Get the final row for the column in the worksheet
                    // int finalrows = m_ExcelWorksheet.Dimension.End.Row;
                    //Convert into a string for the range.
                    //Convert the range to the color Red
                    // var allCells = m_ExcelWorksheet.Cells[1, 1, m_ExcelWorksheet.Dimension.End.Row, m_ExcelWorksheet.Dimension.End.Column];
                    //var cellFont = allCells.Style.WrapText = true;
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] CopySheet(SheetInfo sheetTemplate, SheetInfo sheetNew)
        {
            //Prepare template excel
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                //Copy Range
                if (m_ExcelPackage.Workbook.Worksheets[sheetNew.SheetName] == null)
                {
                    m_ExcelPackage.Workbook.Worksheets.Copy(sheetTemplate.SheetName, sheetNew.SheetName);
                }
                return m_ExcelPackage.GetAsByteArray();
            }
        }
        public byte[] DeleteSheet(SheetInfo sheetDetele)
        {
            //Prepare template excel
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                //delete sheet
                if (!string.IsNullOrWhiteSpace(sheetDetele.SheetName))
                {
                    m_ExcelPackage.Workbook.Worksheets.Delete(sheetDetele.SheetName);
                }

                else if (sheetDetele.SheetIndex > 0)
                {
                    m_ExcelPackage.Workbook.Worksheets.Delete(sheetDetele.SheetIndex);
                }
                else
                {
                    //throw new Exception();
                }
                return m_ExcelPackage.GetAsByteArray();
            }
        }
        public byte[] AddImage(string imageName, string address, int height, int width, byte[] imageBytes)
        {
            return AddImage(imageName, null, address, height, width, imageBytes);
        }
        public byte[] AddImage(string imageName, SheetInfo sheetInfo, string address, int height, int width, byte[] imageBytes)
        {
            byte[] m_DataOutput = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheet != null)
                {
                    Bitmap image;
                    using (var img = new MemoryStream(imageBytes))
                    {
                        image = new Bitmap(img);
                        ExcelPicture excelImage = null;
                        if (img.Length > 0)
                        {
                            using (var DefineName = m_ExcelPackage.Workbook.Names[address])
                            {
                                if (DefineName != null)
                                {
                                    //excelImage = m_ExcelWorksheet.Drawings.AddPicture(imageName, image);
                                    excelImage.From.Column = DefineName.Start.Column;
                                    excelImage.From.Row = DefineName.Start.Row;
                                    excelImage.SetSize(width, height);
                                    // 2x2 px space for better alignment
                                    excelImage.From.ColumnOff = Pixel2MTU(2);
                                    excelImage.From.RowOff = Pixel2MTU(2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }

                m_DataOutput = m_ExcelPackage.GetAsByteArray();
            }
            return m_DataOutput;
        }
        private ExcelWorksheet GetWorkSheet(ExcelPackage excelPackage, SheetInfo sheetInfo)
        {
            ExcelWorksheet m_ExcelWorksheet = null;
            if (sheetInfo != null)
            {
                m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? excelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : excelPackage.Workbook.Worksheets[sheetInfo.SheetName];
            }

            else
            {
                m_ExcelWorksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
            }
            return m_ExcelWorksheet;
        }
        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }


        public static List<string> ReadStringTable(Stream input)
        {
            List<string> stringTable = new List<string>();

            try
            {
                using (XmlReader reader = XmlReader.Create(input))
                {
                    //for (reader.MoveToContent(); reader.Read();)
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "t")
                        {
                            stringTable.Add(reader.ReadElementContentAsString());
                        }
                    }

                }
            }
            catch
            {
                return new List<string>();
            }
            return stringTable;
        }

        public static List<List<string>> ReadExcelSheet(Stream input)
        {
            //var path = @"C:\ProductImportTemplate- COMFORT (1).xlsx";
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(input, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                SharedStringTable sst = sstpart.SharedStringTable;
                IEnumerable<Row> rows = sheetData.Elements<Row>();

                var numCol = rows.First().Descendants<Cell>().Count();
                var lst = new List<List<string>>();
                var count = rows.Count();
                for (uint rowidx = 2; rowidx <= count; rowidx++)
                {
                    var _row = new List<string>();
                    Row r = GetRow(workSheet, rowidx);
                    for (int col = 1; col <= numCol; col++)
                    {
                        Cell c = GetCell(workSheet, GetExcelColumnName(col), rowidx);
                        if (c != null)
                        {
                            if (c.DataType != null && c.DataType == CellValues.SharedString)
                            {
                                int ssid = int.Parse(c.CellValue.Text);
                                string str = sst.ChildElements[ssid].InnerText;
                                _row.Add(str);
                            }
                            else
                            {
                                _row.Add(c.CellValue?.InnerText);
                            }

                        }
                        else
                        {
                            _row.Add("");
                        }
                    }
                    lst.Add(_row);
                }
                return lst;
            }
        }

        public static List<List<string>> ReadExcelSheet(Stream input, int sheetIndex, int numCol = -1)
        {
            //var path = @"C:\ProductImportTemplate- COMFORT (1).xlsx";
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(input, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.Skip(sheetIndex).First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                SharedStringTable sst = sstpart.SharedStringTable;
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                if (numCol < 0)
                    numCol = rows.First().Descendants<Cell>().Count();
                var lst = new List<List<string>>();
                var rowCount = rows.Count();
                for (uint rowidx = 1; rowidx <= rowCount + 1; rowidx++)
                {
                    var _row = new List<string>();
                    Row r = GetRow(workSheet, rowidx);
                    for (int col = 1; col <= numCol; col++)
                    {
                        Cell c = GetCell(workSheet, GetExcelColumnName(col), rowidx);
                        if (c != null)
                        {
                            if (c.DataType != null && c.DataType == CellValues.SharedString)
                            {
                                int ssid = int.Parse(c.CellValue.Text);
                                string str = sst.ChildElements[ssid].InnerText;
                                _row.Add(str);
                            }
                            else if (c.DataType != null && c.DataType == CellValues.Date)
                            {
                                var str = c.CellValue.Text;
                                _row.Add(str);
                            }
                            else
                            {
                                _row.Add(c.CellValue?.InnerText);
                            }

                        }
                        else
                        {
                            _row.Add("");
                        }
                    }
                    if (rowidx == rowCount + 1)
                    {
                        if (_row.All(x => string.IsNullOrEmpty(x)))
                            continue;
                    }
                    lst.Add(_row);
                }
                return lst;
            }
        }
        public static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
            {

                return null;
            }

            return row.Elements<Cell>().
                Where(c => c.CellReference != null && string.Compare(c.CellReference.Value, columnName + rowIndex, true) == 0).FirstOrDefault();
        }

        public static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            var a = worksheet.GetFirstChild<SheetData>();
            var ra = worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            return ra;
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public static List<string> ReadStringTableNew(Stream input)
        {
            //List<List<string>> stringTable = new List<List<string>>();
            List<string> stringTable = new List<string>();

            try
            {
                string fileName = @"C:\ProductImportTemplate- COMFORT (1).xlsx";

                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;

                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;

                        var cells = sheet.Descendants<Cell>();
                        var rows = sheet.Descendants<Row>();

                        // One way: go through each cell in the sheet
                        //foreach (Cell cell in cells)
                        //{
                        //    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                        //    {
                        //        int ssid = int.Parse(cell.CellValue.Text);
                        //        string str = sst.ChildElements[ssid].InnerText;
                        //        stringTable.Add(str);
                        //        Console.WriteLine("Shared string {0}: {1}", ssid, str);
                        //    }
                        //    else if (cell.CellValue != null)
                        //    {
                        //        Console.WriteLine("Cell contents: {0}", cell.CellValue.Text);
                        //    }
                        //}

                        SharedStringTablePart stringTablePart = doc.WorkbookPart.SharedStringTablePart;
                        // Or... via each row
                        foreach (Row row in rows)
                        {
                            List<string> rowString = new List<string>();

                            foreach (Cell c in row.Elements<Cell>())
                            {
                                string value = c.CellValue.InnerXml;

                                if (c.DataType != null && c.DataType.Value == CellValues.SharedString)
                                {
                                    stringTable.Add(stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText);
                                }
                                //else
                                //{
                                //    stringTable.Add(value); ;
                                //}
                            }
                            //stringTable.Add(rowString);
                        }
                    }
                }
            }
            catch
            {
                return new List<string>();
            }
            return stringTable;
        }

        public static List<string> getWorkbookNames(Stream workbook, int count)
        {
            List<string> workBookTable = new List<string>();
            int Count = count;
            using (XmlReader reader = XmlReader.Create(workbook))
            {
                //for (reader.MoveToContent(); reader.Read();)
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "sheets")
                    {
                        reader.Read();
                        if (reader.Name == "sheet")
                        {
                            for (int i = 0; i < Count; i++)
                            {
                                workBookTable.Add(reader.GetAttribute("name"));

                                reader.Read();

                            }
                        }
                    }
                }
            }
            return workBookTable;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "<Pending>")]
        public static object[] ReadWorksheet(Stream input, List<string> stringTable, Stream style, int rowID)
        {
            List<List<string>> workbook = new List<List<string>>();
            List<List<string>> headers = new List<List<string>>();
            List<string> cHeader = new List<string>();
            List<string> cValue = new List<string>();
            List<string> row = new List<string>();

            Stream StyleStream = style;
            String type;
            String STRFinder;
            Int32 StyleIndex = 0;
            Int32 StyleInt = 0;
            String nullFinder;
            Int32 val = -1;
            String firstLook = null;
            int headerCount = 0;
            bool runRow = false;
            List<string> styleValues = new List<string>();
            int getRowID = rowID;


            string newstr = string.Empty;
            object[] objects = new object[2];

            if (getRowID == 0)
            {
                getRowID = 1;
            }

            XmlReader Stylereader = Stylereader = XmlReader.Create(StyleStream);
            Stylereader.MoveToContent();

            while (Stylereader.Read())
            {
                if (Stylereader.NodeType == XmlNodeType.Element)
                {
                    switch (Stylereader.Name)
                    {
                        case "cellXfs":
                            Int16 Count = Convert.ToInt16(Stylereader.GetAttribute("count"));

                            Stylereader.Read();

                            for (int i = 0; i < Count; i++)
                            {

                                if (Stylereader.Name == "xf")
                                {

                                    styleValues.Add(Stylereader.GetAttribute("numFmtId"));
                                    Stylereader.Skip();

                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            using (XmlReader reader = XmlReader.Create(input))
            {
                int currentPostion = 0;
                int postionFound = 0;

                for (reader.MoveToContent(); reader.Read();)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "row":
                                int getRowVale = Convert.ToInt32(reader.GetAttribute("r"));
                                if (getRowVale >= getRowID)
                                {
                                    runRow = true;
                                }
                                else
                                {
                                    runRow = false;
                                }
                                break;
                            case "c":
                                if (runRow == true)
                                {
                                    type = reader.GetAttribute("t");
                                    nullFinder = reader.GetAttribute("r");
                                    if (nullFinder == "K3")
                                    {
                                    }
                                    StyleIndex = Convert.ToInt16(reader.GetAttribute("s"));
                                    STRFinder = reader.GetAttribute("t");
                                    if (StyleIndex > 0)
                                    {
                                        //StyleInt = Convert.ToInt32(styleValues[StyleIndex]);
                                    }
                                    newstr = string.Empty;
                                    postionFound = 0;
                                    if (nullFinder != null)
                                    {
                                        foreach (char c in nullFinder)
                                        {
                                            int i = (int)c;
                                            if ((i >= 48) && (i <= 57))
                                            {
                                                continue;
                                            }
                                            newstr += c.ToString();
                                        }
                                        foreach (char c in newstr)
                                        {
                                            postionFound += ((int)c);
                                        }

                                        if (newstr.Length == 1)
                                        {
                                            postionFound -= 65;
                                        }
                                        else if (newstr.Length == 2)
                                        {
                                            postionFound -= 104;
                                        }
                                        else if (newstr.Length == 3)
                                        {
                                            postionFound -= 119;
                                        }
                                        else { }

                                        if (currentPostion != postionFound)
                                        {
                                            for (int i = currentPostion; i < postionFound; i++)
                                            {
                                                row.Add(Convert.ToString(null));
                                            }
                                            currentPostion = postionFound + 1;
                                        }
                                        else
                                        {
                                            currentPostion = postionFound + 1;
                                        }
                                    }
                                    try
                                    {
                                        //skips the formula element to the value
                                        //only allows "c" with elements skips empty elemets
                                        if (reader.IsEmptyElement == false)
                                        {
                                            reader.Read();
                                            if (reader.Name.Equals("f"))
                                            {
                                                reader.Skip();
                                                firstLook = reader.ReadElementContentAsString().ToString();
                                                row.Add(firstLook);
                                                break;
                                            }
                                            else
                                            {
                                                firstLook = reader.ReadElementContentAsString().ToString();
                                                if (firstLook.Contains("."))
                                                {
                                                    row.Add(firstLook);
                                                    break;
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        val = int.Parse(firstLook);
                                                    }
                                                    catch
                                                    {
                                                        row.Add(firstLook);
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            row.Add(Convert.ToString(null));
                                            break;
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        throw new FormatException("invaild data type " + reader.ReadElementContentAsString());
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                if (type == "s")
                                {
                                    if (headerCount == 0)
                                    {
                                        cHeader.Add(newstr);
                                        cValue.Add(stringTable[val]);
                                        newstr = string.Empty;
                                    }
                                    else
                                    {
                                        row.Add(stringTable[val]);
                                    }
                                    break;
                                }

                                else if (StyleInt > 13 && StyleInt < 23)
                                {
                                    DateTime dt = DateTime.FromOADate(val);
                                    String DateandTime = Convert.ToString(dt.ToShortDateString());
                                    row.Add(DateandTime);
                                    StyleInt = 0;
                                    StyleIndex = 0;
                                }
                                //else if (STRFinder == "str")
                                //{
                                //    //string readSTR = reader.ReadElementContentAsString().ToString();
                                //    row.Add(Convert.ToString(readSTR));
                                //    break;
                                //}
                                else
                                {
                                    row.Add(Convert.ToString(val));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "row")
                        {
                            if (headerCount == 0 && cValue.Count != 0)
                            {
                                headers.Add(cHeader);
                                headers.Add(cValue);
                                headerCount += 1;
                                currentPostion = 0;
                                postionFound = 0;
                            }
                            else if (row.Count != 0)
                            {
                                if (row.Count != cHeader.Count)
                                {
                                    for (int i = row.Count; i < cHeader.Count; i++)
                                    {
                                        row.Add(Convert.ToString(null));
                                    }
                                }

                                List<string> newRow = new List<string>();
                                foreach (string item in row)
                                {
                                    newRow.Add(item);
                                }
                                workbook.Add(newRow);

                                currentPostion = 0;
                                postionFound = 0;
                                row.Clear();
                            }
                            else { }
                        }
                    }
                    else { }
                }
                objects[0] = workbook;
                objects[1] = headers;
                return objects;
            }
        }

        public static List<string> GetSheetsName(Stream input)
        {
            var sheets = new List<string>();
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(input, false))
            {
                sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Select(s => s.Name.Value).ToList();
            }
            return sheets;
        }
        public static List<string> GetMaxColumnCount(Stream input)
        {
            var sheets = new List<string>();
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(input, false))
            {
                sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Select(s => s.Name.Value).ToList();
            }
            return sheets;
        }

        public WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.

                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        public void FillDataToExcel(string filePath, string sheetName, List<string> columns, List<int> rows, List<object> Values)
        {
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filePath, true))
            {
                WorksheetPart worksheetPart = string.IsNullOrEmpty(sheetName) ? myDoc.WorkbookPart.WorksheetParts.FirstOrDefault() : GetWorksheetPartByName(myDoc, sheetName);
                Stylesheet styleSheet = myDoc.WorkbookPart.WorkbookStylesPart.Stylesheet;

                for (int i = 0; i < Values.Count; i++)
                {
                    if (Values[i] == null)
                        continue;

                    var row = rows[i];
                    var col = columns[i];
                    var valueType = Values[i].GetType().Name;
                    var dataType = CellValues.String;
                    switch (valueType)
                    {
                        case "Int32":
                        case "UInt32":
                        case "Int64":
                        case "UInt64":
                        case "Decimal":
                        case "Double":
                        case "Single":
                            dataType = CellValues.Number;
                            break;
                        case "DateTime":
                            dataType = CellValues.Date;
                            break;
                        case "Boolean":
                            dataType = CellValues.Boolean;
                            break;
                        default:
                            break;
                    }

                    Row r = GetRow(worksheetPart.Worksheet, (uint)row);
                    if (r != null)
                    {
                        Cell c = GetCell(worksheetPart.Worksheet, col, (uint)row);
                        if (c != null)
                        {
                            if (!Values[i].ToString().StartsWith("="))
                            {
                                c.CellValue = new CellValue();
                                c.CellValue.Text = Values[i].ToString();
                                c.DataType = dataType;
                            }
                            else
                            {
                                c.CellFormula = new CellFormula(Values[i].ToString().TrimStart(new char[] { '=' }));
                            }
                        }
                        else
                        {
                            c = new Cell();
                            c.CellValue = new CellValue();
                            c.CellValue.Text = Values[i].ToString();
                            c.DataType = dataType;
                            r.Append(c);
                        }
                    }
                    else
                    {
                        var t = false;
                    }
                }

                worksheetPart.Worksheet.Save();
                styleSheet.Save();
                myDoc.Dispose();
            }
        }

        public byte[] FillDataToCellExcel(byte[] buffer, int sheetIndex, string col, int row, object data)
        {
            if (data == null)
                return buffer;

            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(buffer, 0, buffer.Length);
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(mem, true))
                {
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.Skip(sheetIndex).First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Stylesheet styleSheet = spreadSheetDocument.WorkbookPart.WorkbookStylesPart.Stylesheet;

                    var valueType = data.GetType().Name;
                    var dataType = CellValues.String;
                    switch (valueType)
                    {
                        case "Int32":
                        case "UInt32":
                        case "Int64":
                        case "UInt64":
                        case "Decimal":
                        case "Double":
                        case "Single":
                            dataType = CellValues.Number;
                            break;
                        case "DateTime":
                            dataType = CellValues.Date;
                            break;
                        case "Boolean":
                            dataType = CellValues.Boolean;
                            break;
                        default:
                            break;
                    }

                    Row r = GetRow(worksheetPart.Worksheet, (uint)row);
                    if (r != null)
                    {
                        Cell c = GetCell(worksheetPart.Worksheet, col, (uint)row);
                        if (c != null)
                        {
                            if (!data.ToString().StartsWith("="))
                            {
                                c.CellValue = new CellValue();
                                c.CellValue.Text = data.ToString();
                                c.DataType = dataType;
                            }
                            else
                            {
                                c.CellFormula = new CellFormula(data.ToString().TrimStart(new char[] { '=' }));
                            }
                        }
                        else
                        {
                            c = new Cell();
                            c.CellValue = new CellValue();
                            c.CellValue.Text = data.ToString();
                            c.DataType = dataType;
                            r.Append(c);
                        }
                    }
                    worksheetPart.Worksheet.Save();
                    styleSheet.Save();
                    spreadSheetDocument.Dispose();

                    return mem.ToArray();
                }
            }
            return buffer;
        }
        #endregion

        #region Constants

        public const string Key_Start = "[[%";
        public const string Key_End = "%]]";
        public const string Key_Seperator = ":";
        public const string KeyType_Parameter = "Parameter";
        public const string KeyType_Field = "Field";

        #endregion
    }

    public class ExcelEx : Excel
    {
        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods
        public byte[] Export<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo, SheetInfo paramSheetInfo, RangeInfo rangeInfo, bool insertNewRows = true, bool fillStaticData = false) where TEntity : class
        {
            //1. Read Config
            this.ReadConfig(new SheetInfo[] { sheetInfo, paramSheetInfo });

            //2. Fill Paremter
            this.FillParameter(paramSheetInfo);

            //3. Export
            this.FillData(entities, sheetInfo, insertNewRows, fillStaticData);

            return this.OutputData;
        }
        private void ReadConfig(SheetInfo[] sheetInfo)
        {
            this.ConfigInfo = new ConfigInfo();

            //1. Read data file
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                //Open Excel + Get WorkSheet
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                for (int i = 0; i < sheetInfo.Length; i++)
                {
                    ExcelWorksheet m_ExcelWorksheet = null;
                    if (sheetInfo != null)
                    {
                        m_ExcelWorksheet = sheetInfo[i].SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo[i].SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo[i].SheetName];
                    }

                    else
                    {
                        m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                    }

                    if (m_ExcelWorksheet != null)
                    {
                        //Prepare Template
                        PrepareTemplate?.Invoke(m_ExcelWorksheet);

                        //Get Config
                        var m_Dimension = m_ExcelWorksheet.Dimension;
                        var m_Cells = m_ExcelWorksheet.Cells;
                        RangeInfo rangeInfo = new RangeInfo()
                        {
                            FromColumn = 1,
                            ToColumn = m_Dimension.Columns,
                            FromRow = 1,
                            ToRow = m_Dimension.Rows
                        };

                        for (int m_RowIndex = rangeInfo.FromRow; m_RowIndex <= rangeInfo.ToRow; m_RowIndex++)
                        {
                            for (int m_ColumnIndex = rangeInfo.FromColumn; m_ColumnIndex <= rangeInfo.ToColumn; m_ColumnIndex++)
                            {
                                var m_Cell = m_Cells[m_RowIndex, m_ColumnIndex];
                                string m_Text = m_Cell.Text;

                                var m_FieldInfo = this.ParseConfig(m_Text);
                                if (m_FieldInfo != null)
                                {
                                    m_FieldInfo.ExcelAddress = m_Cell.Address;
                                    m_FieldInfo.ExcelRow = m_RowIndex;
                                    m_FieldInfo.ExcelColumn = m_ColumnIndex;
                                    this.ConfigInfo.Fields.Add(m_FieldInfo);
                                }
                            }
                        }
                    }
                }
                this.TemplateFileData = m_ExcelPackage.GetAsByteArray();
            }
        }
        private void FillParameter(SheetInfo sheetInfo)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheet != null)
                {
                    using (var m_Cells = m_ExcelWorksheet.Cells)
                    {
                        FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Parameter).ToArray();
                        foreach (var m_FieldInfo in m_FieldInfos)
                        {
                            object m_Value = string.Empty;
                            if (this.ParameterData.TryGetValue(m_FieldInfo.Name, out m_Value))
                            {
                                using (var m_Cell = m_Cells[m_FieldInfo.ExcelAddress])
                                {
                                    m_Cell.Value = m_Value;

                                    AfterFillParameter?.Invoke(m_ExcelWorksheet, m_Cell, m_FieldInfo);
                                }
                            }
                        }
                    }

                    this.OutputData = m_ExcelPackage.GetAsByteArray();
                }
            }
        }
        private void FillData<TEntity>(IList<TEntity> entities, SheetInfo sheetInfo, bool insertNewRows = true, bool fillStaticData = false) where TEntity : class
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                {
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                }

                else
                {
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                if (m_ExcelWorksheet != null)
                {
                    using (var m_Cells = m_ExcelWorksheet.Cells)
                    {
                        FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Field).ToArray();
                        if (m_FieldInfos.Length > 0)
                        {
                            int m_RowBeginIndex = m_FieldInfos.FirstOrDefault().ExcelRow;
                            //Insert Zone
                            if (insertNewRows)
                            {
                                m_ExcelWorksheet.InsertRow(m_RowBeginIndex + 1, entities.Count - 1, m_RowBeginIndex);
                            }
                            //Fill
                            int m_RowIndex = m_RowBeginIndex;
                            //Type m_EntityType = typeof(TEntity);
                            foreach (var m_Entity in entities)
                            {
                                foreach (var m_FieldInfo in m_FieldInfos)
                                {
                                    if (fillStaticData)
                                    {
                                        var m_Value = ReflectorUtility.FollowPropertyPath(m_Entity, m_FieldInfo.Name);
                                        m_Cells[m_FieldInfo.ExcelRow, m_FieldInfo.ExcelColumn].Value = m_Value;
                                    }
                                    else
                                    {
                                        var m_Value = ReflectorUtility.FollowPropertyPath(m_Entity, m_FieldInfo.Name);
                                        m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                    }
                                }
                                m_RowIndex++;
                            }

                            if (AfterFillAllData != null)
                            {
                                this.AfterFillAllData(m_ExcelWorksheet);
                            }
                        }
                    }

                    this.OutputData = m_ExcelPackage.GetAsByteArray();
                }
            }
        }
        public byte[] SetChartData(SheetInfo sheetData, SheetInfo sheetChart, RangeInfo rangeSerie, RangeInfo rangeXSerie, string[] chartName)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                ExcelWorksheet m_ExcelWorksheetReport = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }

                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }


                if (sheetChart != null)
                {
                    m_ExcelWorksheetReport = sheetChart.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetChart.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetChart.SheetName];
                }

                else
                {
                    m_ExcelWorksheetReport = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }


                if (m_ExcelWorksheetData != null && m_ExcelWorksheetReport != null)
                {
                    for (int i = 0; i < chartName.Length; i++)
                    {
                        var m_pieChart = m_ExcelWorksheetReport.Drawings[chartName[i]] as ExcelBarChart;
                        for (int s = 0; s < m_pieChart.Series.Count; s++)
                        {
                            m_pieChart.Series.Delete(s);
                        }
                        m_pieChart.Series.Add(m_ExcelWorksheetData.Cells[rangeSerie.FromRow, rangeSerie.FromColumn, rangeSerie.ToRow, rangeSerie.ToColumn].FullAddress, m_ExcelWorksheetData.Cells[rangeXSerie.FromRow, rangeXSerie.FromColumn, rangeXSerie.ToRow, rangeXSerie.ToColumn].FullAddress);

                    }
                }
                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return this.OutputData;
        }
        public byte[] DeleteRows(SheetInfo sheetData, RangeInfo rangeDelete)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }

                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                m_ExcelWorksheetData.DeleteRow(rangeDelete.FromRow, rangeDelete.ToRow - rangeDelete.FromRow, true);

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return this.OutputData;
        }
        public byte[] DeleteRows(byte[] data, SheetInfo sheetData, RangeInfo rangeDelete)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(data))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }

                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                m_ExcelWorksheetData.DeleteRow(rangeDelete.FromRow, rangeDelete.ToRow - rangeDelete.FromRow, true);

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return this.OutputData;
        }
        public byte[] DeleteChart(SheetInfo sheetData, string[] chartName)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }
                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                for (int i = 0; i < chartName.Length; i++)
                {
                    m_ExcelWorksheetData.Drawings.Remove(chartName[i]);
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return this.OutputData;
        }
        public byte[] SetPositionChart(SheetInfo sheetData, string chartName, RangeInfo rangeInfo)
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }

                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }
                var m_Chart = m_ExcelWorksheetData.Drawings[chartName] as ExcelBarChart;
                m_Chart.SetPosition(rangeInfo.FromRow, 0, rangeInfo.FromColumn, 0);

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return this.OutputData;
        }
        /// <summary>
        /// push datatable to excel
        /// </summary>
        /// <param name="excelData"></param>
        /// <param name="mergeColum"></param>
        /// <param name="startRow"></param>
        /// <param name="sheetInfo"></param>
        /// <returns></returns>
        public byte[] SetDatatable(byte[] m_Data, SheetInfo sheetData, int startRow, int startColumn, DataTable dataTable, RangeInfo Range = null)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage pck = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(m_Data))
                {
                    pck.Load(m_MemoryStream);
                }
                ExcelWorksheet ws = pck.Workbook.Worksheets[sheetData.SheetName];
                if (pck.Workbook.Worksheets[sheetData.SheetName] == null)
                {
                    ws = pck.Workbook.Worksheets.Add(sheetData.SheetName);
                }
                ws.Cells[startRow, startColumn].LoadFromDataTable(dataTable, true);

                if (Range != null && Range.FromRow > 0 && Range.ToRow > 0 && Range.ToColumn > 0)
                {
                    var modelTable = ws.Cells[Range.FromRow, 1, Range.ToRow, Range.ToColumn];
                    modelTable.Style.Font.Bold = true;
                    modelTable.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    modelTable.Style.Fill.BackgroundColor.SetColor(Range.BackGround);
                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    modelTable.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                m_OutputData = pck.GetAsByteArray();
            }
            return m_OutputData;
        }
        public byte[] MergeCells(byte[] m_Data, SheetInfo sheetData, RangeInfo[] rangeInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(m_Data))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheetData = null;
                if (sheetData != null)
                {
                    m_ExcelWorksheetData = sheetData.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetData.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetData.SheetName];
                }

                else
                {
                    m_ExcelWorksheetData = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();
                }

                if (m_ExcelWorksheetData != null)
                {
                    foreach (var m_rangeInfo in rangeInfo)
                    {
                        m_ExcelWorksheetData.Cells[m_rangeInfo.FromRow, m_rangeInfo.FromColumn, m_rangeInfo.ToRow, m_rangeInfo.ToColumn].Merge = true;
                    }
                }
                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }

        #endregion

        #region Constants

        #endregion
    }
}
