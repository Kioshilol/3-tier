﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace Core
{
    public class AppSetting
    {
        public static string GetConnectionString()
        {
            var connectionString = AddJsonFile().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            return connectionString;
        }
        public static int GetPageSize()
        {
            var size = AddJsonFile().GetSection("PageSize").GetSection("RowsPerPage").Value;
            return int.Parse(size);
        }

        private static IConfigurationRoot AddJsonFile()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            return root;
        }

        public static bool EfConnect()
        {
            var connection = AddJsonFile().GetSection("Connection").GetSection("EfEnable").Value;
            return bool.Parse(connection);
        }

        public static bool isAutoMapperEnable()
        {
            var isMapperEnable = AddJsonFile().GetSection("Mapper").GetSection("AutoMapperEnable").Value;
            return bool.Parse(isMapperEnable);
        }

        public static string GetPicturesFilePath()
        {
            var filePath = AddJsonFile().GetSection("Files").GetSection("Pictures").Value;
            return filePath;
        }
        
        public static string GetFullPathToPictures()
        {
            var filePath = AddJsonFile().GetSection("Files").GetSection("FullPathToPictures").Value;
            return filePath;
        }

        public static string SetDefaultAvatar()
        {
            var filePath = AddJsonFile().GetSection("Files").GetSection("DefaultAvatar").Value;
            return filePath;
        }

        public static string SetExcelFilesPath()
        {
            var filePath = AddJsonFile().GetSection("Files").GetSection("FilePathForExcel").Value;
            return filePath;
        }

        public static string SetXMLFilesPath()
        {
            var filePath = AddJsonFile().GetSection("Files").GetSection("FilePathForXML").Value;
            return filePath;
        }

    }
}
