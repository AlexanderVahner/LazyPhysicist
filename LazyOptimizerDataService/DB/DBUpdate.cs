using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizerDataService.DB
{
    enum DBStatus { UpToDate = 10, UpdatedOk = 20, RecheckIsRecommended = 30, MustRecheck = 40 }
    internal class DBUpdate
    {
        private readonly SQLiteService sqliteService;
        private DBStatus status = DBStatus.UpToDate;
        private int version;
        public DBUpdate(SQLiteService sqliteService)
        {
            this.sqliteService = sqliteService;
            Update();
            LogUpdateInfo();
        }

        private void Update()
        {
            version = GetDBVersion();
            switch (version)
            {
                case 0:
                    UpdateScriptExecute(UpdateToVersion_1);
                    break;
            }
        }

        private void UpdateScriptExecute(Func<DBStatus> updater)
        {
            int oldVersion = version;
            try
            {
                sqliteService.BeginTransaction();
                DBStatus updateStatus = updater();
                sqliteService.CommitTransaction();
                if (updateStatus > status)
                {
                    status = updateStatus;
                }
            }
            catch (Exception e)
            {
                sqliteService.RollbackTransaction();
                Logger.Write(this, "DB update error:\n" + e.Message, LogMessageType.Error);
            }

            if (oldVersion < version)
            {
                Update();
            }
            
        }

        private DBStatus UpdateToVersion_1()
        {
            sqliteService.Execute("ALTER TABLE Plans ADD COLUMN ApprovalStatus INTEGER;");
            sqliteService.Execute("ALTER TABLE Plans ADD COLUMN Starred INTEGER;");

            SetDBVersion(1); // Don't foget to increment

            Logger.Write(this, "DB updated: Plans.ApprovalStatus, Plans.Starred fields added.", LogMessageType.Info);
            return DBStatus.RecheckIsRecommended;
        }

        private int GetDBVersion()
        {
            return int.Parse(sqliteService.GetValue("PRAGMA user_version;").ToString());
        }

        private void SetDBVersion(int newVersion)
        {
            sqliteService.Execute($"PRAGMA user_version = {newVersion};");
            version = GetDBVersion();
        }

        private void LogUpdateInfo()
        {
            switch (status)
            {
                case DBStatus.UpdatedOk:
                    Logger.Write(this, $"DB updated to version {version}.", LogMessageType.Info);
                    break;
                case DBStatus.RecheckIsRecommended:
                    Logger.Write(this, $"DB updated to version {version}.\nUpdating all your plans is recommended for the correct work of all features (Settings > Recheck all patients next time).", LogMessageType.Warning);
                    break;
                case DBStatus.MustRecheck:
                    Logger.Write(this, $"DB updated to version {version}.\nYou must completely recheck your plans (Settings > Recheck all patients next time).\nSorry.", LogMessageType.Error);
                    break;
            }
        }
    }
}
