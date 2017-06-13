﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiContrib.Formatting.Xlsx
{
    public class SqadXlsxDocumentBuilder : IXlsxDocumentBuilder
    {
        private ExcelPackage Package { get; set; }
        
        private Stream _stream;

        public SqadXlsxDocumentBuilder(Stream stream)
        {
            _stream = stream;

            // Create a worksheet
            Package = new ExcelPackage();

        }

        

        public Task WriteToStream()
        {
            return Task.Factory.StartNew(() => Package.SaveAs(_stream));
        }

        public ExcelWorksheet AppendSheet(string sheetName)
        {
            return Package.Workbook.Worksheets.Add(sheetName);
        }

        public bool IsExcelSupportedType(object expression)
        {
            return expression is string
                || expression is short
                || expression is int
                || expression is long
                || expression is decimal
                || expression is float
                || expression is double
                || expression is DateTime;
        }
    }
}
