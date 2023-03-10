using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIInfo.Plan
{
    public enum ObjectiveType { Point = 0, Mean = 1, EUD = 2, Unknown = 99 }
    public class ObjectiveInfo
    {
        public static void GetObjectives(PlanInfo planInfo, IList<ObjectiveInfo> destination)
        {
            if (planInfo?.Plan == null)
            {
                Logger.Write(null, "Can't get objectives. The Plan is null", LogMessageType.Error);
            }
            else if (destination == null)
            {
                Logger.Write(null, "Can't get objectives. The Destination list is null", LogMessageType.Error);
            }
            else
            {
                foreach (OptimizationObjective objective in planInfo.Plan.OptimizationSetup.Objectives)
                {
                    ObjectiveInfo oi = new ObjectiveInfo
                    {
                        StructureId = objective.StructureId,
                        Priority = objective.Priority,
                        Operator = objective.Operator
                    };

                    if (objective is OptimizationPointObjective pointObjective)
                    {
                        oi.Type = ObjectiveType.Point;
                        oi.Dose =  pointObjective.Dose.Dose / (pointObjective.Dose.Unit == DoseValue.DoseUnit.cGy ? 100 : 1);
                        oi.Volume = pointObjective.Volume;
                    }
                    else if (objective is OptimizationMeanDoseObjective meanObjective)
                    {
                        oi.Type = ObjectiveType.Mean;
                        oi.Dose = meanObjective.Dose.Dose / (meanObjective.Dose.Unit == DoseValue.DoseUnit.cGy ? 100 : 1);
                    }
                    else if (objective is OptimizationEUDObjective eudObjective)
                    {
                        oi.Type = ObjectiveType.EUD;
                        oi.Dose = eudObjective.Dose.Dose / (eudObjective.Dose.Unit == DoseValue.DoseUnit.cGy ? 100 : 1);
                        oi.ParameterA = eudObjective.ParameterA;
                    }
                    else
                    {
                        oi.Type = ObjectiveType.Unknown;
                    }

                    destination.Add(oi);
                }
            }
        }

        public void LoadIntoPlan(PlanInfo plan)
        {
            if (plan?.Plan == null)
            {
                Logger.Write(this, "Can't load the objective. The Plan is null", LogMessageType.Error);
            }
            else
            {
                if (!plan.IsReadyForOptimizerLoad)
                {
                    Logger.Write(this, "Can't load the objective. The Plan is not unapproved, or it doesn't have beams", LogMessageType.Error);
                }
                else
                {
                    if (Structure == null)
                    {
                        Logger.Write(this, "Can't load the objective. Structure is not defined", LogMessageType.Error);
                    }
                    else
                    {
                        switch (Type)
                        {
                            case ObjectiveType.Point:
                                plan.Plan.OptimizationSetup.AddPointObjective(Structure, Operator, new DoseValue(Dose, DoseValue.DoseUnit.Gy), Volume, Priority);
                                break;
                            case ObjectiveType.Mean:
                                plan.Plan.OptimizationSetup.AddMeanDoseObjective(Structure, new DoseValue(Dose, DoseValue.DoseUnit.Gy), Priority);
                                break;
                            case ObjectiveType.EUD:
                                plan.Plan.OptimizationSetup.AddEUDObjective(Structure, Operator, new DoseValue(Dose, DoseValue.DoseUnit.Gy), ParameterA, Priority);
                                break;
                            case ObjectiveType.Unknown:
                                Logger.Write(this, "Can't load the objective. Type is unknown.", LogMessageType.Error);
                                break;
                        }
                    }
                }
            }
        }

        public ObjectiveType Type { get; set; } = ObjectiveType.Unknown;
        public Structure Structure { get; set; } = null;
        public string StructureId { get; set; } = "";
        public double Priority { get; set; } = .0;
        public OptimizationObjectiveOperator Operator { get; set; } = OptimizationObjectiveOperator.None;
        public double Dose { get; set; } = .0;
        public double Volume { get; set; } = .0;
        public double ParameterA { get; set; } = .0;
    }
}
