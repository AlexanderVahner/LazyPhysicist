using Common;
using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public sealed class GeneralSettings
    {
        private static string appPath;
        private static readonly string generalSettingsFileName = "_GeneralSettings.xml";
        private static string generalSettingsFullFileName;

        private string userPath = @"%APPDATA%\LazyOptimizer";
        private string plansCacheAppPath = Path.Combine(appPath, "PlansCache.exe");

        public static GeneralSettings ReadGeneralSettings()
        {
            GeneralSettings settings = null;
            appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //AppDomain.CurrentDomain.BaseDirectory;
            generalSettingsFullFileName = Path.Combine(appPath, generalSettingsFileName);

            if (File.Exists(generalSettingsFullFileName))
            {
                Xml.ReadXmlToObject(generalSettingsFullFileName, ref settings);
            }
            else
            {
                settings = new GeneralSettings();
                try
                {
                    Xml.WriteXmlFromObject(generalSettingsFullFileName, settings);
                    Logger.Write(null, $"User Path is \"{settings.UserPath}\".\nYou can change it in the file \"{settings.SettingsFullName}\".", LogMessageType.Warning);
                }
                catch (Exception e)
                {
                    Logger.Write(null, e.Message, LogMessageType.Error);
                }
                
            }

            settings.CheckUserPath();

            return settings;
        }

        public void CheckUserPath()
        {
            if (!FileSystem.CheckPathOrCreate(Environment.ExpandEnvironmentVariables(userPath)))
            {
                Logger.Write(null, $"Can't create user path \"{userPath}\".", LogMessageType.Error);
            }
        }

        public void Save()
        {
            Xml.WriteXmlFromObject(SettingsFullName, this);
        }

        public string SettingsFullName => Path.Combine(appPath, generalSettingsFileName);
        public string UserPath { get => userPath; set => userPath = value; }
        public string PlansCacheFullFileName { get => plansCacheAppPath; set => plansCacheAppPath = value; }
    }
}
