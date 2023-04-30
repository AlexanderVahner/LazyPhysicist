using LazyPhysicist.Common;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIInfo.Plan
{
    public enum ObjectiveType { Point = 0, Mean = 1, EUD = 2, Unknown = 99 }
    public enum Operator // Copy of VMS.TPS.Common.Model.Types.OptimizationObjectiveOperator
    {
        //
        // Summary:
        //     Less than.
        Upper = 0,
        //
        // Summary:
        //     Greater or equal.
        Lower = 1,
        //
        // Summary:
        //     Exact (target).
        Exact = 2,
        //
        // Summary:
        //     None.
        None = 99
    }
    public sealed class ObjectiveInfo : IObjectiveInfo
    {
        public static void GetObjectives(IPlanInfo planInfo, IList<ObjectiveInfo> destination)
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
                        Operator = (Operator)(int)objective.Operator
                    };

                    if (objective is OptimizationPointObjective pointObjective)
                    {
                        oi.Type = ObjectiveType.Point;
                        oi.Dose = pointObjective.Dose.Dose / (pointObjective.Dose.Unit == DoseValue.DoseUnit.cGy ? 100 : 1);
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

        public ObjectiveType Type { get; set; } = ObjectiveType.Unknown;
        public Structure Structure { get; set; } = null;
        public string StructureId { get; set; } = "";
        public double Priority { get; set; } = .0;
        public Operator Operator { get; set; } = Operator.None;
        public double Dose { get; set; } = .0;
        public double Volume { get; set; } = .0;
        public double ParameterA { get; set; } = .0;
    }
}
