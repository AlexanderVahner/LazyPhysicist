using Common;
using LazyPhysicist.Common;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace LazyContouring.Models
{
    public sealed class AppSettings
    {
        private static string appPath;
        private static readonly string appSettingsFileName = "LazyContouringSettings.xml";
        private static string appSettingsFullFileName;

        private string userPath = @"%APPDATA%\LazyContouring";

        public static AppSettings ReadAppSettings()
        {
            AppSettings settings = null;
            appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            appSettingsFullFileName = Path.Combine(appPath, appSettingsFileName);

            if (File.Exists(appSettingsFullFileName))
            {
                Xml.ReadXmlToObject(appSettingsFullFileName, ref settings);
            }
            else
            {
                settings = new AppSettings();
                try
                {
                    Xml.WriteXmlFromObject(appSettingsFullFileName, settings);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            settings.CheckUserPath();

            return settings;
        }

        public void CheckUserPath()
        {
            if (!FileSystem.CheckPathOrCreate(Environment.ExpandEnvironmentVariables(userPath)))
            {
                MessageBox.Show($"Can't create user path \"{userPath}\".");
            }
        }

        public void Save()
        {
            Xml.WriteXmlFromObject(SettingsFullName, this);
        }

        public string SettingsFullName => Path.Combine(appPath, appSettingsFileName);
        public string UserPath { get => userPath; set => userPath = value; }
    }
}
