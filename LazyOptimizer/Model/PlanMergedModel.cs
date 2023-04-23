using ESAPIInfo.Plan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public sealed class PlanMergedModel : PlanBaseModel, IPlanMergedModel
    {
        private readonly List<IPlanBaseModel> mergedPlans = new List<IPlanBaseModel>();
        private readonly ObservableCollection<IStructureModel> mergedStructures = new ObservableCollection<IStructureModel>();
        private readonly NtoInfo mergedNtoInfo = new NtoInfo();
        public PlanMergedModel(IPlanInfo currentPlan) : base(currentPlan)
        {
            Description = "The average objectives and NTO of the added plans.";
        }

        public void Merge(IPlanBaseModel plan)
        {
            if (plan == null)
            {
                return;
            }
            mergedPlans.Add(plan);
            SelectionFrequency = mergedPlans.Count;
            RecalcMerged();
        }

        private void RecalcMerged()
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

            MergeStructures();
        }

        private void MergeStructures()
        {
            mergedStructures.Clear();
            StructuresBroker.Reset();

            foreach (var plan in mergedPlans)
            {
                foreach (var structure in plan.Structures)
                {
                    var findedStructure = mergedStructures.FirstOrDefault(s => s.CurrentPlanStructure == structure.CurrentPlanStructure);
                    if (findedStructure == null)
                    {
                        findedStructure = new StructureModel(structure.CurrentPlanStructure.Id, StructuresBroker)
                        {
                            CurrentPlanStructure = structure.CurrentPlanStructure
                        };
                        mergedStructures.Add(findedStructure);
                    }

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
        }

        private void AverageObjectives(IStructureModel structure)
        {
            if (structure == null || structure.Objectives.Count <= 1)
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
                   Dose = obj.Average(o => o.Dose),
                   ParameterA = obj.Average(o => o.ParameterA),
                   Priority = obj.Average(o => o.Priority)
               };

            foreach (var objective in query)
            {
                structure.AddObjective(objective);
            }
            while (initialCount-- > 0)
            {
                structure.Objectives.RemoveAt(0);
            }
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
        public int MergedPlansCount => mergedPlans.Count;
    }
}
