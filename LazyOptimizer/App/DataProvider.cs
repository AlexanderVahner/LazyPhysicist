using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class DataProvider : IDisposable
    {
        private readonly SQLiteService db;
        public DataProvider(DataProviderSettings settings)
        {
            this.settings = settings;
            string dbPath = Environment.ExpandEnvironmentVariables(settings.DBPath);
            if (!File.Exists(dbPath))
            {
                Logger.Write(this, $"{dbPath} doesn't exist", LogMessageType.Error);
            }
            else
            {
                db = new SQLiteService(dbPath);
                if (!db.Connected)
                {
                    Logger.Write(this, $@"Can't connect to DB ""{dbPath}""", LogMessageType.Error);
                }
            }
        }
        public void GetPlans(IList<PlanDBRecord> destination, PlansFilterArgs args)
        {
            if (Connected)
            {
                StringBuilder sqlRequest = new StringBuilder("SELECT rowid, PatientId, CourseId, PlanId, Technique, MachineId, SelectionFrequency, StructuresString, Description,");
                sqlRequest.AppendLine($@"Levenshtein(StructuresString, '{args.StructuresString}') AS LDistance");
                sqlRequest.AppendLine("FROM Plans");

                if (args != null)
                {
                    sqlRequest.AppendLine("WHERE (1=1)");
                    if (args.SingleDose != 0)
                    {
                        sqlRequest.Append($@" AND (SingleDose={args.SingleDose})");
                    }
                    if (args.FractionsCount != 0)
                    {
                        sqlRequest.Append($@" AND (FractionsCount={args.FractionsCount})");
                    }
                    if (args.Technique != "")
                    {
                        sqlRequest.Append($@" AND (Technique='{args.Technique}')");
                    }
                    if (args.MachineId != "")
                    {
                        sqlRequest.Append($@" AND (MachineId='{args.MachineId}')");
                    }
                    
                }
                sqlRequest.AppendLine("ORDER BY LDistance DESC ");
                sqlRequest.Append(args.Limit > 0 ? $" LIMIT {args.Limit};" : ";");

                db.Select(destination, sqlRequest.ToString());
            }
        }
        public void GetObjectives(IList<ObjectiveDBRecord> destination, long PlanRowId)
        {
            if (Connected)
            {
                string sqlRequest = "SELECT rowid, PlanRowId, StructureId, ObjType, Priority, Operator, Dose, Volume, ParameterA  FROM Objectives WHERE (PlanRowId = ?);";
                db.Select(destination, sqlRequest, new object[] { PlanRowId });
            }
        }
        public NtoDBRecord GetNto(long PlanRowId)
        {
            NtoDBRecord result = null;
            if (Connected)
            {
                string sqlRequest = "SELECT rowid, PlanRowId, IsAutomatic, DistanceFromTargetBorderInMM, StartDosePercentage, EndDosePercentage, FallOff, Priority FROM NTO WHERE (PlanRowId = ?) LIMIT 1;";
                db.SelectOne(out result, sqlRequest, new object[] { PlanRowId });
            }
            return result;
        }
        public void SavePlanToDB(PlanInfo plan)
        {
            if (Connected)
            {
                if (plan?.Plan == null)
                {
                    Logger.Write(this, "The Plan is null.", LogMessageType.Error);
                }
                else
                {
                    if (plan.ObjectivesCount == 0)
                    {
                        Logger.Write(this, "The Plan has no objectives. Skipped.", LogMessageType.Debug);
                    }
                    else
                    {
                        db.BeginTransaction();
                        try
                        {
                            string sql = "INSERT INTO Plans (PatientId, CourseId, PlanId, FractionsCount, SingleDose, Technique, MachineId, StructuresString) VALUES (?, ?, ?, ?, ?, ?, ?, ?); "
                                + "SELECT last_insert_rowid();";

                            int rowId;
                            //object result = db.GetValue(sql.ToString(), new object[] { plan.PatientId, plan.CourseId, plan.PlanId, plan.FractionsCount, plan.SingleDose, plan.Technique, plan.MachineId, plan.StructuresString });
                            //Logger.Write(this, "rowid " + db.LastInsertedRowId().ToString(), LogMessageType.Info);
                            rowId = int.Parse(db.GetValue(sql, new object[] { plan.PatientId, plan.CourseId, plan.PlanId, plan.FractionsCount, plan.SingleDose, plan.Technique, plan.MachineId, plan.StructuresString }).ToString());

                            List<ObjectiveInfo> objectives = new List<ObjectiveInfo>();
                            ObjectiveInfo.GetObjectives(plan, objectives);

                            foreach (ObjectiveInfo objective in objectives)
                            {
                                sql = "INSERT INTO Objectives (PlanRowId, StructureId, ObjType, Priority, Operator, Dose, Volume, ParameterA) VALUES (?, ?, ?, ?, ?, ?, ?, ?);";
                                db.Execute(sql, new object[] { rowId, objective.StructureId, (int)objective.Type, objective.Priority, (int)objective.Operator, objective.Dose, objective.Volume, objective.ParameterA });
                            }

                            if (plan.Nto != null)
                            {
                                sql = "INSERT INTO NTO (PlanRowId, IsAutomatic, DistanceFromTargetBorderInMM, StartDosePercentage, EndDosePercentage, FallOff, Priority) VALUES (?, ?, ?, ?, ?, ?, ?);";
                                db.Execute(sql, new object[] { rowId, plan.Nto.IsAutomaticLong, plan.Nto.DistanceFromTargetBorderInMM, plan.Nto.StartDosePercentage, plan.Nto.EndDosePercentage, plan.Nto.FallOff, plan.Nto.Priority });
                            }

                            db.CommitTransaction();
                        }
                        catch (Exception e)
                        {
                            db.RollbackTransaction();
                            Logger.Write(this, e.Message, LogMessageType.Error);
                        }
                    }
                }
            }
        }
        public void ClearData()
        {
            if (Connected)
            {
                Logger.Write(this, "Clearing Data.", LogMessageType.Debug);
                db.Execute("DELETE FROM Plans; DELETE FROM Objectives; DELETE FROM NTO;");
            }
        }
        public DateTime? GetLastCheckDate()
        {
            DateTime? result = null;
            if (Connected)
            {
                try
                {
                    string stringDate = (db.GetValue("SELECT LastCheckDate FROM Vars LIMIT 1;") ?? result).ToString();
                    result = DateTime.Parse(stringDate);
                }
                catch (Exception e)
                {
                    Logger.Write(this, e.Message, LogMessageType.Error);
                }
            }
            return result;
        }
        public void SetLastCheckDate(DateTime date)
        {
            if (Connected)
            {
                string stringDate = date.ToString("s");
                if (db.Execute($@"UPDATE Vars SET LastCheckDate = '{stringDate}';") == 0)
                {
                    db.Execute($@"INSERT INTO Vars (LastCheckDate) VALUES ('{stringDate}');");
                }
            }
        }
        public bool Connected
        {
            get
            {
                bool result = db?.Connected ?? false;
                if (!result)
                {
                    Logger.Write(this, "Connection is lost.", LogMessageType.Error);
                }
                return result;
            }
            set
            {
                if (db != null)
                {
                    db.Connected = value;
                }
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
        private readonly DataProviderSettings settings;
        public DataProviderSettings Settings => settings;
    }

    public class DataProviderSettings
    {
        public string DBPath;
    }
}
