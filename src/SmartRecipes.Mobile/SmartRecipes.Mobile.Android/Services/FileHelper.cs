﻿using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SmartRecipes.Mobile.Droid.Services.FileHelper))]
namespace SmartRecipes.Mobile.Droid.Services
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}