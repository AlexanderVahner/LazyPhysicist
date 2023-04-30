using ESAPIInfo.Plan;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.Model
{
    public sealed class PlanMergedModel : PlanBaseModel, IPlanMergedModel
    {
        private readonly List<IPlanBaseModel> mergedPlans = new List<IPlanBaseModel>();
        private readonly ObservableCollection<IStructureModel> mergedStructures = new ObservableCollection<IStructureModel>();
        private INtoInfo mergedNtoInfo = new NtoInfo();

        public PlanMergedModel(IPlanInfo currentPlan, PlanInteractions planInteractions) : base(currentPlan, planInteractions)
        {
            Description = "The average objectives and NTO of the added plans.";
        }

        public void Merge(IPlanBaseModel plan)
        {
            if (plan == null)
            {
                return;
            }

            if (mergedPlans.FirstOrDefault(mp => mp == plan) != null)
            {
                Logger.Write(this, "This plan has already been merged.", LogMessageType.Warning);
                return;
            }

            mergedPlans.Add(plan);
            SelectionFrequency = mergedPlans.Count;
            RecalcMerged(mergedPlans);
            
            Logger.Write(this, $"Merged Plan now contains {mergedPlans.Count} plan" + (mergedPlans.Count == 1 ? "." : "s."));
        }

        private void RecalcMerged(List<IPlanBaseModel> mergedPlans)
        {
            if (mergedPlans.Count == 0)
            {
                return;
            }
            if (mergedPlans.Count == 1)
            {
                StructuresBroker.Reset(mergedPlans[0].Structures);
                return;
            }

            MergeStructures(mergedPlans);
            MergeNto(mergedPlans);
        }

        private void MergeStructures(List<IPlanBaseModel> mergedPlans)
        {
            mergedStructures.Clear();
            

            foreach (var plan in mergedPlans)
            {
                foreach (var structure in plan.Structures)
                {
                    if (structure.CurrentPlanStructure == null)
                    {
                        continue;
                    }

                    var findedStructure = DefineStructure(structure.CurrentPlanStructure); 

                    // Put all objectives from all merged plans in the same structure. Then we'll average them
                    foreach (var objective in structure.Objectives)
                    {
                        findedStructure.Objectives.Add(objective);
                    }
                }
            }

            foreach (var structure in mergedStructures)
            {
                AverageObjectives(structure);
            }

            StructuresBroker.Reset(Structures);
        }

        private IStructureModel DefineStructure(IStructureSuggestionModel structure)
        {
            var findedStructure = mergedStructures.FirstOrDefault(s => s.CurrentPlanStructure.Id == structure.Id);
            if (findedStructure == null)
            {
                findedStructure = new StructureModel(structure.Id, StructuresBroker)
                {
                    CurrentPlanStructure = structure
                };
                mergedStructures.Add(findedStructure);
            }
            return findedStructure;
        }

        private void AverageObjectives(IStructureModel structure)
        {
            if (structure == null || structure.Objectives.Count < 2)
            {
                return;
            }
            int initialCount = structure.Objectives.Count;

            var query =
               from o in structure.Objectives
               group o by new
               {
                   o.ObjType,
                   o.Operator,
                   o.Volume,
                   pGrp = o.Priority == 0 ? 0 : 1
               } into obj
               select new ObjectiveModel()
               {
                   ObjType = obj.Key.ObjType, 
                   Operator = obj.Key.Operator,
                   Volume = obj.Key.Volume,
                   Dose = Math.Round(obj.Average(o => o.Dose) ?? 0, 2),
                   ParameterA = Math.Round(obj.Average(o => o.ParameterA) ?? 0, 1),
                   Priority = Math.Round(obj.Average(o => o.Priority), 0)
               };

            if (query.Count() != initialCount)
            {
                foreach (var objective in query)
                {
                    structure.AddObjective(objective);
                }
                while (initialCount-- > 0)
                {
                    structure.Objectives.RemoveAt(0);
                }
            }
            
        }

        private void MergeNto(List<IPlanBaseModel> mergedPlans)
        {
            List<INtoInfo> ntoList = new List<INtoInfo>();
            foreach(var plan in mergedPlans)
            {
                ntoList.Add(plan.NtoInfo);
            }
            
            var manuals = ntoList.Where(n => !n.IsAutomatic);
            var averageList = manuals.Any() ? manuals : ntoList;

            if (averageList.Count() == 1)
            {
                mergedNtoInfo = averageList.First();
                return;
            }

            var query = averageList
                .GroupBy(n => n.IsAutomatic)
                .Select(g => new NtoInfo()
                    {
                        IsAutomatic = g.Key,
                        DistanceFromTargetBorderInMM = Math.Round(g.Average(x => x.DistanceFromTargetBorderInMM), 0),
                        StartDosePercentage = Math.Round(g.Average(x => x.StartDosePercentage), 0),
                        EndDosePercentage = Math.Round(g.Average(x => x.EndDosePercentage), 0),
                        FallOff = Math.Round(g.Average(x => x.FallOff), 2),
                        Priority = Math.Round(g.Average(x => x.Priority), 0)
                    }
                )/* TODO: Simplify code with .OrderBy(s => s.IsAutomatic)*/;

            mergedNtoInfo = query.First();
        }

        protected override ObservableCollection<IStructureModel> GetStructures()
        {
            return mergedPlans.Count == 1 ? mergedPlans[0].Structures : mergedStructures;
        }

        protected override INtoInfo GetNto()
        {
            return mergedPlans.Count == 1 ? mergedPlans[0].NtoInfo : mergedNtoInfo;
        }

        public override string PlanTitle => "Merged plan";
        public IEnumerable<IPlanBaseModel> MergedPlans => mergedPlans;
    }
}
