using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using QPP.Wpf.Dialogs;
using QPP.Wpf.UI.Controls;
using QPP.Wpf.UI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QPP.Wpf.Utils
{
    public enum ExportType { Xls }

    public static class ExportHelper
    {
        public static void Export(object columns, IEnumerable data, ExportType type = ExportType.Xls)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Filter = type == ExportType.Xls ? "(*.xls)|*.xls" : "(*.*)|*.*";
            fileDialog.FileName = "data_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (fileDialog.ShowDialog() == true)
            {
                using (var stream = fileDialog.OpenFile())
                {
                    var cols = GetDataColumns(columns);
                    if (cols != null)
                        DoExport(stream, cols, data, type);
                    else
                        DoExport(stream, data, type);
                }
                //TODO: open the folder of the file   
                var dialog = new ExportResultDialog(fileDialog.FileName);
                dialog.Owner = Window.GetWindow(Application.Current.MainWindow);
                dialog.ShowDialog();
            }
        }

        static void DoExport(Stream stream, IEnumerable<DataGridColumn> columns, IEnumerable data, ExportType type)
        {
            if (type == ExportType.Xls)
                WriteToXls(stream, columns, data);
        }

        static void DoExport(Stream stream, IEnumerable data, ExportType type)
        {
            if (type == ExportType.Xls)
                WriteToXls(stream, data);
        }

        static IRow GetOrCreateRow(this ISheet workSheet, int rowIndex)
        {
            IRow sheetRow = workSheet.GetRow(rowIndex);
            if (sheetRow == null)
                sheetRow = workSheet.CreateRow(rowIndex);
            return sheetRow;
        }

        static ICell GetOrCreateCell(this IRow sheetRow, int colIndex)
        {
            ICell cell = sheetRow.GetCell(colIndex);
            if (cell == null)
                cell = sheetRow.CreateCell(colIndex);
            return cell;
        }

        static void SetCellValue(this ICell cell, object value)
        {
            if (value is double)
                cell.SetCellValue((double)value);
            else if (value is DateTime)
            {
                cell.SetCellValue((DateTime)value);
                cell.SetCellType(CellType.Numeric);
            }
            else if (value is bool)
                cell.SetCellValue((bool)value);
            cell.SetCellValue(value.ToSafeString());
        }

        static ICellStyle GetHeaderCellStyle(IWorkbook book)
        {
            ICellStyle style = book.CreateCellStyle();
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.FillForegroundColor = HSSFColor.LightTurquoise.Index;
            style.FillPattern = FillPattern.SolidForeground;
            IFont font = book.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);
            return style;
        }

        static ICellStyle GetCellStyle(IWorkbook book)
        {
            ICellStyle style = book.CreateCellStyle();
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }

        static ICellStyle GetReadOnlyCellStyle(IWorkbook book)
        {
            ICellStyle style = book.CreateCellStyle();
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        static ICellStyle GetKeyCellStyle(IWorkbook book)
        {
            ICellStyle style = book.CreateCellStyle();
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.FillForegroundColor = HSSFColor.Tan.Index;
            style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        static ICellStyle GetRequireCellStyle(IWorkbook book)
        {
            ICellStyle style = book.CreateCellStyle();
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.FillForegroundColor = HSSFColor.LightBlue.Index;
            style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        static ICellStyle GetCellStyle(DataGridColumn col, ISheet sheet)
        {
            return GetCellStyle(sheet.Workbook);
        }

        static object GetValue(object obj, string propertyName)
        {
            try
            {
                object value = null;
                if (obj is System.Data.DataRow)
                    value = (obj as System.Data.DataRow)[propertyName];
                //else if (obj is QPP.ComponentModel.ObservableObject)
                //    value = (obj as QPP.ComponentModel.ObservableObject)[propertyName];
                else
                    value = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                return value;
            }
            catch
            {
                return string.Empty;
            }
        }

        static string GetHeaderText(object header)
        {
            if (header is TextBlock)
                return ((TextBlock)header).Text;
            else if (header is Label)
                return GetHeaderText(((Label)header).Content);
            return header.ToSafeString();
        }

        static void CreateCells(ISheet sheet, DataGridColumn col,
            IEnumerable source, int colIndex)
        {
            int rowIndex = 0;
            #region header
            var headerStyle = GetHeaderCellStyle(sheet.Workbook);
            var rowHeader = sheet.GetOrCreateRow(rowIndex++);
            var cellHeader = rowHeader.GetOrCreateCell(colIndex);
            cellHeader.SetCellValue(GetHeaderText(col.Header));
            cellHeader.SetCellType(CellType.String);
            cellHeader.CellStyle = headerStyle;
            #endregion
            var cellStyle = GetCellStyle(col, sheet);
            if (col is DataGridBoundColumn)
            {
                var c = (DataGridBoundColumn)col;
                var be = new BindingEvaluator(c.Binding);
                foreach (var obj in source)
                {
                    IRow row = sheet.GetOrCreateRow(rowIndex++);
                    ICell cell = row.GetOrCreateCell(colIndex);
                    cell.CellStyle = cellStyle;
                    be.DataContext = obj;
                    cell.SetCellValue(be.Value);
                }
            }
            else if (col is System.Windows.Controls.DataGridComboBoxColumn)
            {
                var c = (System.Windows.Controls.DataGridComboBoxColumn)col;
                var be = new BindingEvaluator(c.SelectedValueBinding);
                var bValue = new BindingEvaluator(new Binding(c.SelectedValuePath));
                var bDisplay = new BindingEvaluator(new Binding(c.DisplayMemberPath));
                var itemSource = new Dictionary<object, object>();
                foreach (var i in c.ItemsSource)
                {
                    bValue.DataContext = i;
                    bDisplay.DataContext = i;
                    var value = bValue.Value;
                    if (value != null)
                        itemSource[value] = bDisplay.Value;
                }
                foreach (var obj in source)
                {
                    IRow row = sheet.GetOrCreateRow(rowIndex++);
                    ICell cell = row.GetOrCreateCell(colIndex);
                    cell.CellStyle = cellStyle;
                    be.DataContext = obj;
                    var value = be.Value;
                    if (itemSource.ContainsKey(value))
                        cell.SetCellValue(itemSource[value]);
                    else
                        cell.SetCellValue(be.Value);
                }
            }
            else if (col is System.Windows.Controls.DataGridTemplateColumn)
            {
                var c = (DataGridTemplateColumn)col;
                var x = DataGridHelper.GetAttachedBinding(c);
                if (x == null) return;
                foreach (var obj in source)
                { 
                    var be = new BindingEvaluator(new Binding(x));
                    be.DataContext = obj;
                    IRow row = sheet.GetOrCreateRow(rowIndex++);
                    ICell cell = row.GetOrCreateCell(colIndex);
                    cell.CellStyle = cellStyle;
                    cell.SetCellValue(be.Value);
                }
            }
            sheet.AutoSizeColumn(colIndex);
        }

        static void WriteToXls(Stream stream, IEnumerable<DataGridColumn> columns, IEnumerable source)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet();
            sheet.PrintSetup.FitWidth = 1;
            sheet.PrintSetup.FitHeight = 0;
            sheet.PrintSetup.PaperSize =(short)PaperSize.A4;            
            sheet.SetMargin(MarginType.BottomMargin, (double)0.5);// 页边距（下）
            sheet.SetMargin(MarginType.LeftMargin, (double)0.1);// 页边距（左）
            sheet.SetMargin(MarginType.RightMargin, (double)0.1);// 页边距（右）
            sheet.SetMargin(MarginType.TopMargin, (double)0.5);// 页边距（上）            
            IRow row = sheet.GetOrCreateRow(0);
            int colIndex = 0;
            foreach (var col in columns)
            {

                if ((col is DataGridTemplateColumn))
                {
                    var c = (DataGridTemplateColumn)col;
                    var x = DataGridHelper.GetAttachedBinding(c);
                    if (x != null)
                    {
                        CreateCells(sheet, col, source, colIndex);
                        colIndex++;
                    }

                }
                else
                {
                    CreateCells(sheet, col, source, colIndex);
                    colIndex++;
                }

            }
            book.Write(stream);
        }

        static void WriteToXls(Stream stream, IEnumerable data)
        {
            using (MemoryStream ms = new MemoryStream())
            {

            }
        }

        static IEnumerable<DataGridColumn> GetDataColumns(object columns)
        {
            var c = columns as ObservableCollection<DataGridColumn>;
            if (c == null)
                return null;
            return c.Where(p => p.Visibility == System.Windows.Visibility.Visible
                && !(p is DataGridCommandColumn || p is DataGridSelectColumn)).OrderBy(p => p.DisplayIndex);
        }
    }
}
