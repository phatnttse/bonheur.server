using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using AD = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace MiddlewareTool.OpenXML
{
    public class Word
    {
        #region Function
        /// <summary>
        /// Append Line Break
        /// </summary>
        /// <param name="run"></param>
        /// <param name="textualData"></param>
        private void AppendLineBreak(ref Run run, string textualData)
        {
            string[] newLineArray = { Environment.NewLine, "\n" };
            string[] textArray = textualData.Split(newLineArray, StringSplitOptions.None);

            bool first = true;

            foreach (string line in textArray)
            {
                if (!first)
                {
                    run.Append(new Break());
                }

                first = false;

                Text txt = new Text();
                txt.Text = line;
                run.Append(txt);
            }
        }
        #endregion

        #region Properties
        public byte[] FileContent { get; set; }
        private const string COLUMN_IMAGE = "COLUMN_IMAGE";
        private const string QR_CODE = "QRCode";
        #endregion

        #region Constructors

        public Word(byte[] fileContent)
        {
            this.FileContent = fileContent;
        }

        #endregion

        #region Methods

        public MemoryStream InsertBookMarkText(byte[] fileContent, Dictionary<string, string> bookMarkData, byte[] image = null)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file                       
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);

                    // tìm kiếm bookmark trùng với tên truyền vào
                    foreach (var element in bookMarks)
                    {
                        if (bookMarkData.ContainsKey(element.Key))
                        {
                            //thực hiện chèn 1 run với nội dung text truyền vào
                            Text textElement = new Text(bookMarkData[element.Key]);
                            textElement.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
                            Run runElement = new Run();
                            runElement.Append(textElement);
                            var m_RunProperties = new RunProperties();
                            foreach (var m_Property in element.Value.Parent.Descendants<ParagraphProperties>().ElementAt(0).GetFirstChild<ParagraphMarkRunProperties>())
                            {
                                m_RunProperties.AppendChild(m_Property.CloneNode(true));
                            }
                            element.Value.InsertAfterSelf(runElement);
                        }
                    }
                }
                return memStr;
            }
            catch
            {
                return null;
            }
        }
        public MemoryStream InsertBookMarkTextTable(byte[] fileContent, Dictionary<string, string> bookMarkData, string bookMarkTableName, DataTable dataTables, byte[] qrCodeToByte = null)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file                       
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                    //Add QR code nếu có
                    if (qrCodeToByte != null && qrCodeToByte.Length > 0)
                    {
                        var bookMarksQRCode = bookMarks[QR_CODE];
                        if (bookMarksQRCode != null)
                        {

                            using (MemoryStream stream = new MemoryStream(qrCodeToByte))
                            {
                                int iWidth = 0;
                                int iHeight = 0;
                                ImagePart imagePart = wordDoc.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                                imagePart.FeedData(stream);
                                MainDocumentPart mainPart = wordDoc.MainDocumentPart;
                                var imageStream = imagePart.GetStream();
                                //Do somtehing with the stream here. 
                                var image = new System.Drawing.Bitmap(imageStream);
                                iWidth = image.Width <= 135 ? image.Width : 135;
                                iHeight = image.Height <= 135 ? image.Height : 135;
                                AddImageToBody(mainPart.GetIdOfPart(imagePart), bookMarksQRCode, iWidth, iHeight);
                            }
                        }
                    }
                    // tìm kiếm bookmark trùng với tên truyền vào
                    foreach (var element in bookMarks)
                    {
                        if (bookMarkData.ContainsKey(element.Key))
                        {
                            var m_RunProperties = new RunProperties();
                            foreach (var m_Property in element.Value.Parent.Descendants<ParagraphProperties>().ElementAt(0).GetFirstChild<ParagraphMarkRunProperties>())
                            {
                                m_RunProperties.AppendChild(m_Property.CloneNode(true));
                            }
                            var elm = bookMarkData[element.Key];
                            if (elm != null && elm?.Length > 0)
                            {
                                Run runElement = new Run();
                                AppendLineBreak(ref runElement, bookMarkData[element.Key]);
                                runElement.PrependChild(m_RunProperties);
                                element.Value.InsertAfterSelf(runElement);
                            }
                        }
                        if (element.Key == bookMarkTableName)
                        {
                            Table table = element.Value.Parent as Table;
                            if (table != null)
                            {
                                var m_DataRows = table.ChildElements.Where(e => e is TableRow)?.LastOrDefault() as TableRow;
                                for (int i = 0; i < dataTables.Rows.Count; i++)
                                {
                                    // new row
                                    TableRow tableRow = new TableRow();

                                    for (int j = 0; j < dataTables.Columns.Count; j++)
                                    {
                                        // new cell
                                        TableCell tableCell = new TableCell();
                                        string _colName = dataTables.Columns[j].ColumnName;
                                        if (_colName != COLUMN_IMAGE)
                                        {
                                            if (m_DataRows != null)
                                            {
                                                // new cell properties   
                                                var _tableCellPropertiesSample = m_DataRows.Descendants<TableCellProperties>().ElementAt(j);
                                                // new paragraph for cell
                                                Paragraph paragraph = new Paragraph();
                                                var _paragraphPropertiesSample = m_DataRows.Descendants<ParagraphProperties>().ElementAt(j);

                                                //New run
                                                var m_RunProperties = GetRunPropertyFromTableCell(m_DataRows, j);
                                                var run1 = new Run(new Text(dataTables.Rows[i][_colName].ToString()));
                                                run1.PrependChild(m_RunProperties);

                                                paragraph.Append(_paragraphPropertiesSample.CloneNode(true));
                                                paragraph.Append(run1);

                                                tableCell.Append(_tableCellPropertiesSample.CloneNode(true));
                                                tableCell.Append(paragraph);

                                                tableRow.Append(tableCell);
                                            }
                                        }
                                        else
                                        {
                                            byte[] bit = dataTables.Rows[i][_colName] as byte[];
                                            if (bit != null && bit.Length > 0)
                                            {
                                                MemoryStream ms = new MemoryStream(bit);
                                                var imagePart = AddImagePart(wordDoc.MainDocumentPart, ms);
                                                AddImageToCell(tableCell, wordDoc.MainDocumentPart.GetIdOfPart(imagePart));
                                            }
                                            else
                                            {
                                                if (m_DataRows != null)
                                                {
                                                    // new cell properties   
                                                    var _tableCellPropertiesSample = m_DataRows.Descendants<TableCellProperties>().ElementAt(j);
                                                    // new paragraph for cell
                                                    Paragraph paragraph = new Paragraph();
                                                    var _paragraphPropertiesSample = m_DataRows.Descendants<ParagraphProperties>().ElementAt(j);

                                                    //New run
                                                    var m_RunProperties = GetRunPropertyFromTableCell(m_DataRows, j);
                                                    var run1 = new Run(new Text(dataTables.Rows[i][_colName].ToString()));
                                                    run1.PrependChild(m_RunProperties);

                                                    paragraph.Append(_paragraphPropertiesSample.CloneNode(true));
                                                    paragraph.Append(run1);

                                                    tableCell.Append(_tableCellPropertiesSample.CloneNode(true));
                                                    tableCell.Append(paragraph);

                                                    tableRow.Append(tableCell);
                                                }
                                            }
                                        }
                                    }
                                    table.Append(tableRow);
                                }
                                table.RemoveChild(m_DataRows);
                            }
                        }
                    }
                }
                return memStr;

            }
            catch
            {
                return null;
            }
        }
        public MemoryStream InsertBookMarkText(byte[] fileContent, Dictionary<string, string> bookMarkData, WordFontStyles wordFontStyles)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);

                    // tìm kiếm bookmark trùng với tên truyền vào
                    foreach (var element in bookMarks)
                    {
                        if (bookMarkData.ContainsKey(element.Key))
                        {
                            // thực hiện chèn 1 run với nội dung text truyền vào
                            Text textElement = new Text(bookMarkData[element.Key]);
                            textElement.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
                            RunProperties runProperties = new RunProperties();
                            if (wordFontStyles.Font != null)
                            {
                                RunFonts runFont = new RunFonts() { Ascii = wordFontStyles.Font, HighAnsi = wordFontStyles.Font };
                                runProperties.Append(runFont);
                            }
                            if (wordFontStyles.FontSize != null)
                            {
                                FontSize fontSize = new FontSize() { Val = wordFontStyles.FontSize };
                                runProperties.Append(fontSize);
                            }
                            if (element.Key.Contains(wordFontStyles.Contains))
                            {
                                RunStyle runStyle = new RunStyle() { Val = wordFontStyles.HyperLink };
                                //var value = bookMarkData[element.Key];
                                runProperties.Append(runStyle);
                                Run runElement = new Run(runProperties);
                                runElement.Append(new Text(wordFontStyles.HyperLinkText));
                                var rel = wordDoc.MainDocumentPart.AddHyperlinkRelationship(new Uri(bookMarkData[element.Key]), true);
                                Hyperlink hyperlink = new Hyperlink(runElement)
                                {
                                    History = OnOffValue.FromBoolean(true),
                                    Id = rel.Id
                                };
                                element.Value.InsertAfterSelf(hyperlink);
                            }
                            else
                            {
                                Run runElement = new Run(runProperties);
                                runElement.Append(textElement);
                                element.Value.InsertAfterSelf(runElement);
                            }
                        }
                    }
                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public MemoryStream FillDataToTable2(byte[] fileContent, string bookMarkTableName, DataTable dataTable, WordFontStyles wordFontStyles)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                    foreach (var element in bookMarks)
                    {
                        if (element.Key == bookMarkTableName)
                        {
                            Table m_Table = element.Value.Parent as Table;
                            if (m_Table != null)
                            {
                                //OpenXmlElement[] openXmlElement = m_Table.ChildElements.Where(e => e is TableRow).ToArray();
                                //var m_DataRows = openXmlElement;
                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    // new row
                                    TableRow tableRow = new TableRow();

                                    for (int j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        // new cell
                                        TableCell tableCell = new TableCell();
                                        string _colName = dataTable.Columns[j].ColumnName;
                                        // new cell properties
                                        TableCellProperties tableCellProperties = new TableCellProperties();
                                        TableCellWidth tableCellWidth = new TableCellWidth();

                                        tableCellProperties.Append(tableCellWidth);
                                        //
                                        RunProperties runProperties = new RunProperties();
                                        if (wordFontStyles.Font != null)
                                        {
                                            RunFonts runFont = new RunFonts() { Ascii = wordFontStyles.Font, HighAnsi = wordFontStyles.Font };
                                            runProperties.Append(runFont);
                                        }
                                        if (wordFontStyles.FontSize != null)
                                        {
                                            FontSize fontSize = new FontSize() { Val = wordFontStyles.FontSize };
                                            runProperties.Append(fontSize);
                                        }
                                        if (element.Key.Contains(wordFontStyles.Contains))
                                        {
                                            RunStyle runStyle = new RunStyle() { Val = wordFontStyles.HyperLink };
                                            runProperties.Append(runStyle);
                                        }
                                        // new paragraph for cell
                                        Paragraph paragraph = new Paragraph();

                                        ParagraphProperties paragraphProperties = new ParagraphProperties();

                                        Run run1 = new Run(runProperties);
                                        Text text1 = new Text();
                                        text1.Text = dataTable.Rows[i][_colName].ToString();

                                        run1.Append(text1);

                                        paragraph.Append(paragraphProperties);
                                        paragraph.Append(run1);

                                        tableCell.Append(tableCellProperties);
                                        tableCell.Append(paragraph);

                                        tableRow.Append(tableCell);
                                    }
                                    m_Table.Append(tableRow);
                                }
                            }
                            break;
                        }
                    }

                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public MemoryStream InsertBookMarkSignPage(byte[] fileContent, Dictionary<string, string> bookMarkData)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);

                    // tìm kiếm bookmark trùng với tên truyền vào
                    foreach (var element in bookMarks)
                    {
                        if (bookMarkData.ContainsKey(element.Key))
                        {
                            // thực hiện chèn 1 run với nội dung text truyền vào
                            Text textElement = new Text(bookMarkData[element.Key]);
                            textElement.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
                            if (element.Key == "BM_DON_VI" || element.Key == "BM_CHUC_DANH")
                            {
                                Run runElement = new Run(new RunProperties(new Bold()));
                                runElement.Append(textElement);
                                element.Value.InsertAfterSelf(runElement);
                            }
                            else
                            {
                                Run runElement = new Run();
                                runElement.Append(textElement);

                                element.Value.InsertAfterSelf(runElement);
                            }
                        }
                    }
                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public MemoryStream FillDataToTable(byte[] fileContent, string bookMarkTableName, DataTable dataTables)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                var memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                    foreach (var element in bookMarks)
                    {
                        if (element.Key == bookMarkTableName)
                        {
                            Table table = element.Value.Parent as Table;

                            if (table != null)
                            {

                                for (int i = 0; i < dataTables.Rows.Count; i++)
                                {
                                    // new row
                                    TableRow tableRow = new TableRow();

                                    for (int j = 0; j < dataTables.Columns.Count; j++)
                                    {
                                        // new cell
                                        TableCell tableCell = new TableCell();
                                        string _colName = dataTables.Columns[j].ColumnName;
                                        // new cell properties
                                        TableCellProperties tableCellProperties = new TableCellProperties();
                                        TableCellWidth tableCellWidth = new TableCellWidth();

                                        tableCellProperties.Append(tableCellWidth);

                                        // new paragraph for cell
                                        Paragraph paragraph = new Paragraph();

                                        ParagraphProperties paragraphProperties = new ParagraphProperties();

                                        Run run1 = new Run();
                                        Text text1 = new Text();
                                        text1.Text = dataTables.Rows[i][_colName].ToString();

                                        run1.Append(text1);

                                        paragraph.Append(paragraphProperties);
                                        paragraph.Append(run1);

                                        tableCell.Append(tableCellProperties);
                                        tableCell.Append(paragraph);

                                        tableRow.Append(tableCell);
                                    }

                                    table.Append(tableRow);
                                }
                            }
                            break;
                        }
                    }

                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public MemoryStream FillDataToTable2(byte[] fileContent, string bookMarkTableName, DataTable dataTable)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                    foreach (var element in bookMarks)
                    {
                        if (element.Key == bookMarkTableName)
                        {
                            Table m_Table = element.Value.Parent as Table;
                            if (m_Table != null)
                            {
                                //var m_DataRows = m_Table.ChildElements.Where(e => e is TableRow).ToArray();
                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    // new row
                                    TableRow tableRow = new TableRow();

                                    for (int j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        // new cell
                                        TableCell tableCell = new TableCell();
                                        string _colName = dataTable.Columns[j].ColumnName;
                                        // new cell properties
                                        TableCellProperties tableCellProperties = new TableCellProperties();
                                        TableCellWidth tableCellWidth = new TableCellWidth();

                                        tableCellProperties.Append(tableCellWidth);

                                        // new paragraph for cell
                                        Paragraph paragraph = new Paragraph();

                                        ParagraphProperties paragraphProperties = new ParagraphProperties();

                                        Run run1 = new Run();
                                        Text text1 = new Text();
                                        text1.Text = dataTable.Rows[i][_colName].ToString();

                                        run1.Append(text1);

                                        paragraph.Append(paragraphProperties);
                                        paragraph.Append(run1);

                                        tableCell.Append(tableCellProperties);
                                        tableCell.Append(paragraph);

                                        tableRow.Append(tableCell);
                                    }
                                    m_Table.Append(tableRow);
                                }
                            }
                            break;
                        }
                    }

                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public MemoryStream FillDataToTable3(byte[] fileContent, string bookMarkTableName, DataTable dataTable)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream memStr = new MemoryStream();
                // đổ mảng byte vào chuỗi
                memStr.Write(fileContent, 0, fileContent.Length);
                // mở 
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                {
                    Document document = wordDoc.MainDocumentPart.Document;
                    // thực hiện lấy tất cả bookmark trong file
                    var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                    foreach (var element in bookMarks)
                    {
                        if (element.Key == bookMarkTableName)
                        {
                            Table m_Table = element.Value.Parent as Table;
                            if (m_Table != null)
                            {
                                var m_DataRow = m_Table.Elements<TableRow>().Last();
                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    //Get Row Copy
                                    TableRow m_RowCopy = (TableRow)m_DataRow.CloneNode(true);

                                    for (int j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        var m_ColName = dataTable.Columns[j].ColumnName;
                                        var m_RunProperties = GetRunPropertyFromTableCell(m_RowCopy, j);
                                        var m_Run = new Run(new Text(dataTable.Rows[i][m_ColName].ToString()));

                                        m_Run.PrependChild<RunProperties>(m_RunProperties);

                                        m_RowCopy.Descendants<TableCell>().ElementAt(j).RemoveAllChildren<Paragraph>();
                                        m_RowCopy.Descendants<TableCell>().ElementAt(j).Append(new Paragraph(m_Run));
                                    }
                                    m_Table.Append(m_RowCopy);
                                }
                                m_Table.RemoveChild(m_DataRow);
                            }
                            break;
                        }
                    }

                }
                return memStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private RunProperties GetRunPropertyFromTableCell(TableRow row, int cellIndex)
        {
            var m_RunProperties = new RunProperties();

            foreach (var m_Property in row.Descendants<TableCell>().ElementAt(cellIndex).GetFirstChild<Paragraph>().GetFirstChild<ParagraphProperties>().GetFirstChild<ParagraphMarkRunProperties>())
            {
                m_RunProperties.AppendChild(m_Property.CloneNode(true));
            }

            return m_RunProperties;
        }
        private Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> results = null, Dictionary<string, string> unmatched = null)
        {
            results = results ?? new Dictionary<string, BookmarkEnd>();
            unmatched = unmatched ?? new Dictionary<string, string>();

            foreach (var child in documentPart.Elements())
            {
                if (child is BookmarkStart)
                {
                    var bStart = child as BookmarkStart;
                    unmatched.Add(bStart.Id, bStart.Name);
                }

                if (child is BookmarkEnd)
                {
                    var bEnd = child as BookmarkEnd;
                    foreach (var orphanName in unmatched)
                    {
                        if (bEnd.Id == orphanName.Key)
                        {
                            results.Add(orphanName.Value, bEnd);
                        }
                    }
                }

                FindBookmarks(child, results, unmatched);
            }

            return results;
        }
        public MemoryStream InsertHtmlToWord(List<String> htmls, byte[] fileContent)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                MemoryStream mem = new MemoryStream();
                mem.Write(fileContent, 0, fileContent.Length);
                using (WordprocessingDocument wordDoc_To = WordprocessingDocument.Open(mem, true))
                {
                    MainDocumentPart mainPart = wordDoc_To.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = wordDoc_To.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }
                    htmls.ToList().ForEach(e =>
                    {
                        string m_AltChunkId = KeyChunk + this.UniqueID();
                        MainDocumentPart mainDocPart = wordDoc_To.MainDocumentPart;
                        MemoryStream ms = new MemoryStream(ToBytes(string.Format(HtmlFormatForWord, e), Encoding.UTF8));
                        AlternativeFormatImportPart formatImportPart = mainDocPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, m_AltChunkId);
                        formatImportPart.FeedData(ms);
                        AltChunk altChunk = new AltChunk();
                        altChunk.Id = m_AltChunkId;
                        mainDocPart.Document.Body.Append(altChunk);
                    });
                    return mem;
                }
            }
            catch
            {
                return null;
            }
        }
        private string UniqueID()
        {
            int length = 6;
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) { throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize)); }

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) { continue; }
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }
        public byte[] ToBytes(string value, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream, encoding))
            {
                sw.Write(value);
                sw.Flush();
                return stream.ToArray();
            }
        }
        public void MergeDocument(string[] filepaths)
        {
            for (int i = 1; i < filepaths.Length; i++)
            {
                using (WordprocessingDocument myDoc = WordprocessingDocument.Open(@filepaths[0], true))
                {
                    MainDocumentPart mainPart = myDoc.MainDocumentPart;
                    string altChunkId = "AltChunkId" + i;
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                        AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                    using (FileStream fileStream = System.IO.File.Open(@filepaths[i], FileMode.Open))
                    {
                        chunk.FeedData(fileStream);
                    }

                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;

                    Body body = mainPart.Document.Body;

                    Paragraph paragraphSectionBreak = new Paragraph();
                    ParagraphProperties paragraphSectionBreakProperties = new ParagraphProperties();
                    SectionProperties SectionBreakProperties = new SectionProperties();
                    SectionType SectionBreakType = new SectionType() { Val = SectionMarkValues.NextPage };
                    SectionBreakProperties.Append(SectionBreakType);
                    paragraphSectionBreakProperties.Append(SectionBreakProperties);
                    paragraphSectionBreak.Append(paragraphSectionBreakProperties);
                    body.InsertAfter(paragraphSectionBreak, body.LastChild);
                    mainPart.Document.Save();

                    body.InsertAfter(altChunk, body.Elements<Paragraph>().Last());
                    mainPart.Document.Save();
                    myDoc.Dispose();
                }
            }
        }

        #region Add text in Footer
        public void ChangeFooter(String documentPath, string text)
        {
            // Replace header in target document with header of source document.
            using (WordprocessingDocument document = WordprocessingDocument.Open(documentPath, true))
            {
                // Get the main document part
                MainDocumentPart mainDocumentPart = document.MainDocumentPart;

                // Delete the existing header and footer parts
                mainDocumentPart.DeleteParts(mainDocumentPart.FooterParts);

                // Create a new header and footer part
                FooterPart footerPart = mainDocumentPart.AddNewPart<FooterPart>();

                // Get Id of the headerPart and footer parts
                string footerPartId = mainDocumentPart.GetIdOfPart(footerPart);

                GenerateFooterPartContent(footerPart, text);

                // Get SectionProperties and Replace HeaderReference and FooterRefernce with new Id
                IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Elements<SectionProperties>();

                foreach (var section in sections)
                {
                    // Delete existing references to headers and footers
                    section.RemoveAllChildren<HeaderReference>();
                    section.RemoveAllChildren<FooterReference>();

                    // Create the new header and footer reference node
                    section.PrependChild<FooterReference>(new FooterReference() { Id = footerPartId });
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>")]
        private void GenerateFooterPartContent(FooterPart part, string text)
        {
            Footer footer1 = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Footer" };
            Justification justification1 = new Justification() { Val = JustificationValues.Center };
            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphStyleId1);

            Text text1 = new Text();
            text1.Text = text;
            Run run1 = new Run(new RunProperties(new Bold(), new FontSize() { Val = "20" }, new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" }));
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            footer1.Append(paragraph1);

            part.Footer = footer1;
        }

        #endregion

        #region Insert Picture
        public byte[] ReplaceBookmarksWithImage(byte[] fileContent, Dictionary<string, MemoryStream> bookMarkData)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = document.MainDocumentPart.RootElement.Descendants<BookmarkStart>();

                        // tìm kiếm bookmark trùng với tên truyền vào
                        foreach (var element in bookMarks)
                        {
                            if (bookMarkData.ContainsKey(element.Name))
                            {
                                InsertImageIntoBookmark(wordDoc, element, bookMarkData[element.Name]);
                            }
                        }
                    }
                    return memStr.ToArray();
                }
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }
        private void InsertImageIntoBookmark(WordprocessingDocument doc, BookmarkStart bookmarkStart, MemoryStream imageFile)
        {
            // Remove anything present inside the bookmark
            OpenXmlElement elem = bookmarkStart.NextSibling();
            while (elem != null && !(elem is BookmarkEnd))
            {
                OpenXmlElement nextElem = elem.NextSibling();
                elem.Remove();
                elem = nextElem;
            }

            // Create an imagepart
            var imagePart = AddImagePart(doc.MainDocumentPart, imageFile);

            // insert the image part after the bookmark start
            AddImageToBody(doc.MainDocumentPart.GetIdOfPart(imagePart), bookmarkStart);
        }

        public ImagePart AddImagePart(MainDocumentPart mainPart, MemoryStream stream)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
            imagePart.FeedData(stream);
            return imagePart;
        }
        private void AddImageToBody(string relationshipId, BookmarkStart bookmarkStart)
        {
            // Define the reference of the image.
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent()
                        {
                            Cx = 990000L,
                            Cy = 792000L
                        },
                        new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                        new DW.DocProperties() { Id = (UInt32Value)1U, Name = Guid.NewGuid().ToString() },
                        new DW.NonVisualGraphicFrameDrawingProperties(new AD.GraphicFrameLocks() { NoChangeAspect = true }),
                        new AD.Graphic(
                            new AD.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = Guid.NewGuid().ToString()
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new AD.Blip(
                                            new AD.BlipExtensionList(
                                                new AD.BlipExtension()
                                                {
                                                    Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                }))
                                        {
                                            Embed
                                                    =
                                                    relationshipId,
                                            CompressionState
                                                    =
                                                    AD
                                                    .BlipCompressionValues
                                                    .Print
                                        },
                                        new AD.Stretch(new AD.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new AD.Transform2D(new AD.Offset() { X = 0L, Y = 0L }, new AD.Extents() { Cx = 990000L, Cy = 792000L }),
                                        new AD.PresetGeometry(new AD.AdjustValueList()) { Preset = AD.ShapeTypeValues.Rectangle })))
                            {
                                Uri =
                                        "http://schemas.openxmlformats.org/drawingml/2006/picture"
                            }))
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        EditId = "50D07946"
                    });

            // add the image element to body, the element should be in a Run.
            bookmarkStart.Parent.InsertAfter<Run>(new Run(element), bookmarkStart);
        }

        private void AddImageToBody(string relationshipId, BookmarkEnd bookmarkEnd, int iWidth = 0, int iHeight = 0)
        {
            Int64Value LCX = iWidth > 0 ? (Int64Value)(iWidth * 9525) : (Int64Value)990000L;
            Int64Value LCY = iHeight > 0 ? (Int64Value)(iHeight * 9525) : (Int64Value)792000L;
            // Define the reference of the image.
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent()
                        {
                            Cx = LCX,
                            Cy = LCY
                        },
                        new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                        new DW.DocProperties() { Id = (UInt32Value)1U, Name = Guid.NewGuid().ToString() },
                        new DW.NonVisualGraphicFrameDrawingProperties(new AD.GraphicFrameLocks() { NoChangeAspect = true }),
                        new AD.Graphic(
                            new AD.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = Guid.NewGuid().ToString()
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new AD.Blip(
                                            new AD.BlipExtensionList(
                                                new AD.BlipExtension()
                                                {
                                                    Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                }))
                                        {
                                            Embed
                                                    =
                                                    relationshipId,
                                            CompressionState
                                                    =
                                                    AD
                                                    .BlipCompressionValues
                                                    .Print
                                        },
                                        new AD.Stretch(new AD.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new AD.Transform2D(new AD.Offset() { X = 0L, Y = 0L }, new AD.Extents() { Cx = LCX, Cy = LCY }),
                                        new AD.PresetGeometry(new AD.AdjustValueList()) { Preset = AD.ShapeTypeValues.Rectangle })))
                            {
                                Uri =
                                        "http://schemas.openxmlformats.org/drawingml/2006/picture"
                            }))
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        EditId = "50D07946"
                    });

            // add the image element to body, the element should be in a Run.
            bookmarkEnd.Parent.InsertAfter<Run>(new Run(element), bookmarkEnd);
        }
        private static void AddImageToCell(TableCell cell, string relationshipId)
        {
            // Define the reference of the image.           

            // add the image element to body, the element should be in a Run.

            var element =
        new Drawing(
          new DW.Inline(
            new DW.Extent() { Cx = 990000L, Cy = 792000L },
            new DW.EffectExtent()
            {
                LeftEdge = 0L,
                TopEdge = 0L,
                RightEdge = 0L,
                BottomEdge = 0L
            },
            new DW.DocProperties()
            {
                Id = (UInt32Value)1U,
                Name = "Picture 1"
            },
            new DW.NonVisualGraphicFrameDrawingProperties(
                new AD.GraphicFrameLocks() { NoChangeAspect = true }),
            new AD.Graphic(
              new AD.GraphicData(
                new PIC.Picture(
                  new PIC.NonVisualPictureProperties(
                    new PIC.NonVisualDrawingProperties()
                    {
                        Id = (UInt32Value)0U,
                        Name = "New Bitmap Image.jpg"
                    },
                    new PIC.NonVisualPictureDrawingProperties()),
                  new PIC.BlipFill(
                    new AD.Blip(
                      new AD.BlipExtensionList(
                        new AD.BlipExtension()
                        {
                            Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                        })
                     )
                    {
                        Embed = relationshipId,
                        CompressionState =
                          AD.BlipCompressionValues.Print
                    },
                    new AD.Stretch(
                      new AD.FillRectangle())),
                    new PIC.ShapeProperties(
                      new AD.Transform2D(
                        new AD.Offset() { X = 0L, Y = 0L },
                        new AD.Extents() { Cx = 990000L, Cy = 792000L }),
                      new AD.PresetGeometry(
                        new AD.AdjustValueList()
                      )
                      { Preset = AD.ShapeTypeValues.Rectangle }))
              )
              { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
          )
          {
              DistanceFromTop = (UInt32Value)0U,
              DistanceFromBottom = (UInt32Value)0U,
              DistanceFromLeft = (UInt32Value)0U,
              DistanceFromRight = (UInt32Value)0U
          });

            cell.Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }), new Run(element)));
        }
        //public byte[] MergeDoubleWords(byte[] dest, byte[] src, string paths = "")
        //{
        //    MemoryStream mainStream = new MemoryStream();
        //    mainStream.Write(dest, 0, dest.Length);
        //    mainStream.Position = 0;
        //    Body body2Insert = null;
        //    List<ImagePart> imagePart2Insert = null;
        //    byte[] ret;
        //    try
        //    {
        //        using (WordprocessingDocument mainDocument = WordprocessingDocument.Open(mainStream, true))
        //        {

        //            // XElement newBody = XElement.Parse(mainDocument.MainDocumentPart.Document.Body.OuterXml);

        //            WordprocessingDocument tempDocument = WordprocessingDocument.Open(new MemoryStream(src), true);
        //            //Get body of sample file and copy it
        //            body2Insert = tempDocument.MainDocumentPart.Document.Body.Clone() as Body;

        //            //Get the image parts
        //            imagePart2Insert = tempDocument.MainDocumentPart.ImageParts.ToList();
        //            //Insert copied body elements without SectionProperties
        //            mainDocument.MainDocumentPart.Document.Body.Append(body2Insert.Elements().Where(e => !(e is SectionProperties)).Select(e => e.Clone() as OpenXmlElement));
        //            mainDocument.MainDocumentPart.Document.Save();

        //            foreach (var imagePart in imagePart2Insert)
        //            {
        //                mainDocument.MainDocumentPart.AddImagePart(imagePart.ContentType);
        //            }
        //            //XElement tempBody = XElement.Parse(tempDocument.MainDocumentPart.Document.Body.OuterXml);

        //            //newBody.Add(tempBody);
        //            //mainDocument.MainDocumentPart.Document.Body = new Body(newBody.ToString());
        //            mainDocument.MainDocumentPart.Document.Save();
        //            //mainDocument.Package.Flush();
        //        }
        //    }
        //    catch
        //    {
        //        return new byte[0];
        //    }
        //    finally
        //    {
        //        ret = mainStream.ToArray();
        //        mainStream.Close();
        //        mainStream.Dispose();
        //    }
        //    return (ret);
        //}

        #endregion

        #endregion
        #region New

        public static void InsertAPictureNew(string document, string fileName)
        {
            using (WordprocessingDocument docs =
                WordprocessingDocument.Open(document, true))
            {
                MainDocumentPart mainPart = docs.MainDocumentPart;

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                AddImageToBodyNew(docs, mainPart.GetIdOfPart(imagePart));
            }
        }
        private static void AddImageToBodyNew(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new AD.GraphicFrameLocks() { NoChangeAspect = true }),
                         new AD.Graphic(
                             new AD.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new AD.Blip(
                                             new AD.BlipExtensionList(
                                                 new AD.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             AD.BlipCompressionValues.Print
                                         },
                                         new AD.Stretch(
                                             new AD.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new AD.Transform2D(
                                             new AD.Offset() { X = 0L, Y = 0L },
                                             new AD.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new AD.PresetGeometry(
                                             new AD.AdjustValueList()
                                         )
                                         { Preset = AD.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }
        #endregion

        #region Contants

        public const string HtmlFormatForWord = "<html><head></head><body>{0}</body></html>";
        public const string KeyChunk = "CQ";

        #endregion
    }
}
