using Common;
using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.IO;

namespace LazyContouring.Models
{
    public sealed class UserSettings
    {
        private static string currentUserId;
        private static string userPath;
        private static string settingsFileName;
        private static string settingsFullFileName;

        public static UserSettings ReadUserSettings(string settingsPath, string userId)
        {
            currentUserId = FileSystem.ClearFileNameFromInvalidChars(userId);
            userPath = Environment.ExpandEnvironmentVariables(settingsPath);
            settingsFileName = $"lc_settings_{currentUserId}.xml";

            settingsFullFileName = Path.Combine(userPath, settingsFileName);

            UserSettings settings = null;

            if (File.Exists(settingsFullFileName))
            {
                try
                {
                    Xml.ReadXmlToObject(settingsFullFileName, ref settings);
                }
                catch
                {
                    settings = new UserSettings();
                }

            }
            else
            {
                settings = new UserSettings();
            }

            return settings;
        }

        public void Save()
        {
            Xml.WriteXmlFromObject(settingsFullFileName, this);
        }

        public TemplateManager TemplateManager { get; set; } = new TemplateManager();

    }
}
