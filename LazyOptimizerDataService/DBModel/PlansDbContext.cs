using LazyOptimizerDataService.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyOptimizerDataService.DBModel
{
    public class PlansDbContext : IPlansContext
    {
        private readonly IDbService db;
        public PlansDbContext(IDbService db)
        {
            this.db = db;
        }

        public List<CachedPlan> GetPlans(PlansFilterArgs args)
        {
            List<CachedPlan> plans = null;

            if (Connected)
            {
                plans = new List<CachedPlan>();

                StringBuilder sqlRequest = new StringBuilder("SELECT RowId, PatientId, CourseId, PlanId, CreationDate, Technique, MachineId, SelectionFrequency, StructuresString, Description,");
                sqlRequest.AppendLine($@"Levenshtein(StructuresString, ?) AS LDistance");
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
                    if (args.MatchTechnique)
                    {
                        sqlRequest.Append($@" AND (Technique='{args.Technique}')");
                    }
                    if (args.MatchMachine)
                    {
                        sqlRequest.Append($@" AND (MachineId='{args.MachineId}')");
                    }

                }
                sqlRequest.AppendLine(" ORDER BY LDistance, rowid DESC ");
                if (args != null)
                {
                    sqlRequest.Append(args.Limit > 0 ? $" LIMIT {args.Limit};" : ";");
                }

                var reader = db.Select(sqlRequest.ToString(), new object[] { args.StructuresString });
                while (reader.Read())
                {
                    CachedPlan plan = new CachedPlan
                    {
                        RowId = reader.GetInt64(0),
                        PatientId = reader.GetString(1),
                        CourseId = reader.GetString(2),
                        PlanId = reader.GetString(3),
                        CreationDate = DateTime.Parse(reader.GetString(4)),
                        Technique = reader.GetString(5),
                        MachineId = reader.GetString(6),
                        SelectionFrequency = reader.GetInt64(7),
                        StructuresString = reader.GetString(8),
                        Description = reader.GetString(9),
                        LDistance = reader.GetInt64(10)
                    };

                    plans.Add(plan);
                }
                reader?.Dispose();
            }

            return plans;
        }
        public void InsertPlans(IEnumerable<CachedPlan> plans)
        {
            if (Connected && plans != null)
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var plan in plans)
                    {
                        string sql = "INSERT INTO Plans (PatientId, CourseId, PlanId, CreationDate, FractionsCount, SingleDose, Technique, MachineId, StructuresString) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?); "
                            + "SELECT last_insert_rowid();";

                        int rowId;
                        rowId = int.Parse(db.GetValue(sql, new object[] { plan.PatientId, plan.CourseId, plan.PlanId, plan.CreationDate, plan.FractionsCount, plan.SingleDose, plan.Technique, plan.MachineId, plan.StructuresString }).ToString());

                        if (plan.Objectives != null)
                        {
                            foreach (CachedObjective objective in plan.Objectives)
                            {
                                sql = "INSERT INTO Objectives (PlanRowId, StructureId, ObjType, Priority, Operator, Dose, Volume, ParameterA) VALUES (?, ?, ?, ?, ?, ?, ?, ?);";
                                db.Execute(sql, new object[] { rowId, objective.StructureId, objective.ObjType, objective.Priority, (int)objective.Operator, objective.Dose, objective.Volume, objective.ParameterA });
                            }
                        }

                        if (plan.Nto != null)
                        {
                            sql = "INSERT INTO Ntos (PlanRowId, IsAutomatic, DistanceFromTargetBorderInMM, StartDosePercentage, EndDosePercentage, FallOff, Priority) VALUES (?, ?, ?, ?, ?, ?, ?);";
                            db.Execute(sql, new object[] { rowId, plan.Nto.IsAutomatic, plan.Nto.DistanceFromTargetBorderInMM, plan.Nto.StartDosePercentage, plan.Nto.EndDosePercentage, plan.Nto.FallOff, plan.Nto.Priority });
                        }
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
        public void UpdatePlan(CachedPlan plan)
        {
            if (Connected && plan != null)
            {
                string sql = "UPDATE Plans SET SelectionFrequency = ?, Description = ? WHERE RowId = ?;";
                db.Execute(sql, new object[] { plan.SelectionFrequency, plan.Description, plan.RowId });
            }
        }
        public List<CachedObjective> GetObjectives(long planRowId)
        {
            List<CachedObjective> objectives = null;

            if (Connected)
            {
                objectives = new List<CachedObjective>();

                string sqlRequest = "SELECT RowId, PlanRowId, StructureId, ObjType, Priority, Operator, Dose, Volume, ParameterA FROM Objectives WHERE (PlanRowId = ?) ORDER BY StructureId;";

                var reader = db.Select(sqlRequest, new object[] { planRowId });
                while (reader.Read())
                {
                    CachedObjective objective = new CachedObjective
                    {
                        RowId = reader.GetInt64(0),
                        PlanRowId = reader.GetInt64(1),
                        StructureId = reader.GetString(2),
                        ObjType = reader.GetInt64(3),
                        Priority = reader.GetDouble(4),
                        Operator = reader.GetInt64(5),
                        Dose = reader.GetDouble(6),
                        Volume = reader.GetDouble(7),
                        ParameterA = reader.GetDouble(8)
                    };

                    objectives.Add(objective);
                }
                reader?.Dispose();
            }

            return objectives;
        }
        public CachedNto GetNto(long planRowId)
        {
            CachedNto nto = null;

            if (Connected)
            {
                nto = new CachedNto();

                string sqlRequest = "SELECT RowId, PlanRowId, IsAutomatic, DistanceFromTargetBorderInMM, StartDosePercentage, EndDosePercentage, FallOff, Priority FROM Ntos WHERE (PlanRowId = ?) LIMIT 1;";

                var reader = db.Select(sqlRequest, new object[] { planRowId });
                if (reader.Read())
                {
                    nto.RowId = reader.GetInt64(0);
                    nto.PlanRowId = reader.GetInt64(1);
                    nto.IsAutomatic = reader.GetInt64(2) != 0;
                    nto.DistanceFromTargetBorderInMM = reader.GetDouble(3);
                    nto.StartDosePercentage = reader.GetDouble(4);
                    nto.EndDosePercentage = reader.GetDouble(5);
                    nto.FallOff = reader.GetDouble(6);
                    nto.Priority = reader.GetDouble(7);
                }
                else
                {
                    nto = new CachedNto();
                }
                reader?.Dispose();
            }

            return nto;
        }

        public Vars GetVars()
        {
            Vars vars = null;

            if (Connected)
            {
                vars = new Vars
                {
                    LastCheckDate = default
                };

                string sqlRequest = "SELECT LastCheckDate FROM Vars LIMIT 1;";

                var reader = db.Select(sqlRequest);
                if (reader.Read())
                {
                    if (DateTime.TryParse(reader.GetString(0), out DateTime tempDateTime))
                    {
                        vars.LastCheckDate = tempDateTime;
                    }
                }
                reader?.Dispose();
            }

            return vars;
        }
        public void UpdateVars(Vars vars)
        {
            if (Connected)
            {
                if (db.Execute($@"UPDATE Vars SET LastCheckDate = '{vars.LastCheckDate}';") == 0)
                {
                    db.Execute($@"INSERT INTO Vars (LastCheckDate) VALUES ('{vars.LastCheckDate}');");
                }
            }
        }
        public void ClearData(DateTime fromDate = default)
        {
            if (Connected)
            {
                if (fromDate == default)
                {
                    Logger.Write(this, "Clearing Data.", LogMessageType.Debug);
                    db.Execute("DELETE FROM Plans; DELETE FROM Objectives; DELETE FROM Ntos;");
                }
                else
                {
                    string strFromDate = fromDate.ToString("s");
                    Logger.Write(this, $"Clearing Data from {strFromDate:g}.", LogMessageType.Debug);

                    StringBuilder sql = new StringBuilder("BEGIN TRANSACTION;");
                    sql.AppendLine("CREATE TEMP TABLE _PlanIds (PlanId INTEGER);")
                        .AppendLine("INSERT INTO _PlanIds (PlanId) SELECT rowid FROM Plans WHERE CreationDate >= ?;")

                        .AppendLine("DELETE FROM Ntos WHERE PlanRowId IN (SELECT PlanId FROM _PlanIds);")
                        .AppendLine("DELETE FROM Objectives WHERE PlanRowId IN (SELECT PlanId FROM _PlanIds);")
                        .AppendLine("DELETE FROM Plans WHERE rowid IN (SELECT PlanId FROM _PlanIds);")

                        .AppendLine("DROP TABLE _PlanIds;")
                        .AppendLine("COMMIT;");

                    db.Execute(sql.ToString(), new object[] { strFromDate });
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
    }
}
