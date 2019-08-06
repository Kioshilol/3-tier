﻿using Core;
using Core.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace BLayer
{
    public class BaseService
    {
        protected IEnumerable<U> BaseMapper<T, U>(IMapper<T, U> mapper, IEnumerable<T> list)
        {
            IEnumerable<T> TList = list;
            var TListDTO = new List<U>();

            foreach (dynamic item in TList)
            {
                var itemDTO = mapper.Map(item);
                TListDTO.Add(itemDTO);
            }

            return TListDTO;
        }

        protected void ExportToExcel<T>(IEnumerable<T> collection)
        {
            List<T> list = new List<T>();

            foreach(var item in collection)
            {
                list.Add(item);
            }

            Type type = typeof(T);
            string fileName = $"{type.Name}Details{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(AppSetting.SetExcelFilesPath(), fileName));
            PropertyInfo[] propertyInfos = type.GetProperties();

            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add($"{type.Name}");
                int totalRows = list.Count;
                int column = 1;
                int row = 2;
                int count = 1;

                foreach (var property in propertyInfos)
                {
                    excelWorksheet.Cells[1, count].Value = property.Name;
                    count++;
                }

                foreach (var item in list)
                {
                    foreach(var property in propertyInfos)
                    {
                        excelWorksheet.Cells[row, column].Value = property.GetValue(item, null);
                        column++;
                    }
                    column = 1;
                    row++;
                }

                excelPackage.Save();
            }
        }

    }
}
