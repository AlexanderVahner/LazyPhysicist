using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlansCache
{
    public sealed class Cache
    {
        private readonly IPlansContext plansContext;
        private readonly Vars vars;
        private readonly List<CachedPlan> plans = new List<CachedPlan>();

        public Cache(IPlansContext plansContext)
        {
            this.plansContext = plansContext;
            vars = plansContext.GetVars();
        }

        public void ClearData(bool fromLastCheck)
        {
            if (fromLastCheck)
            {
                plansContext.ClearData(vars.LastCheckDate);
            }
            else
            {
                plansContext.ClearData();
            }
        }

        public void AddPlan(PlanInfo plan)
        {
            CachedPlan cachedPlan = PlanToCachedPlan(plan);
            if (cachedPlan != null)
            {
                plans.Add(cachedPlan);
            }

        }

        public void WritePlans()
        {
            if (plans.Count() > 0)
            {
                plansContext.InsertPlans(plans);
                plansContext.UpdateVars(vars);
                plans.Clear();
            }
        }

        private CachedPlan PlanToCachedPlan(PlanInfo plan)
        {
            CachedPlan cachedPlan = null;

            if (plan != null)
            {
                cachedPlan = new CachedPlan()
                {
                    PatientId = plan.PatientId,
                    CourseId = plan.CourseId,
                    PlanId = plan.PlanId,
                    CreationDate = plan.CreationDate,
                    FractionsCount = plan.FractionsCount,
                    SingleDose = plan.SingleDose,
                    Technique = plan.Technique,
                    MachineId = plan.MachineId,
                    StructuresString = plan.StructuresPseudoHash
                };

                List<ObjectiveInfo> objectives = new List<ObjectiveInfo>();
                ObjectiveInfo.GetObjectives(plan, objectives);
                cachedPlan.Objectives = new List<CachedObjective>();

                foreach (var objective in objectives)
                {
                    CachedObjective cachedObjective = new CachedObjective()
                    {
                        StructureId = objective.StructureId,
                        ObjType = (long)objective.Type,
                        Priority = objective.Priority,
                        Operator = (long)objective.Operator,
                        Dose = objective.Dose,
                        Volume = objective.Volume,
                        ParameterA = objective.ParameterA
                    };
                    cachedPlan.Objectives.Add(cachedObjective);
                }

                if (plan.Nto != null && !(plan.Nto.IsAutomatic && plan.Nto.Priority == 100)) // TODO: Make the default NTO and compare with it. Default NTO will not be added
                {
                    cachedPlan.Nto = new CachedNto()
                    {
                        IsAutomatic = plan.Nto.IsAutomatic,
                        DistanceFromTargetBorderInMM = plan.Nto.DistanceFromTargetBorderInMM,
                        StartDosePercentage = plan.Nto.StartDosePercentage,
                        EndDosePercentage = plan.Nto.EndDosePercentage,
                        FallOff = plan.Nto.FallOff,
                        Priority = plan.Nto.Priority
                    };
                }
            }

            return cachedPlan;
        }

        public DateTime LastCheckDate
        {
            get => vars.LastCheckDate;
            set => vars.LastCheckDate = value;
        }

    }
}
