// VMS.TPS.Common.Model.Types PlanSetupApprovalStatus Copy

namespace ESAPIInfo.Plan
{
    /// <summary>
    /// The enumeration of plan approval statuses. A copy of VMS.TPS.Common.Model.Types.PlanSetupApprovalStatus
    /// </summary> 
    public enum PlanSetupApprovalStatus
    {
        //
        // Summary:
        //     Rejected for actual use in treatment, unlocked in the database.
        Rejected = 0,
        //
        // Summary:
        //     Unapproved, unlocked in the database.
        UnApproved = 1,
        //
        // Summary:
        //     Has typically been initially verified, but still requires further approvals,
        //     locked in the database.
        Reviewed = 2,
        //
        // Summary:
        //     Intended to be used for the treatment of a patient, locked in the database. Must
        //     be scheduled and given the Treatment Approval status to use it for the actual
        //     patient treatment.
        PlanningApproved = 3,
        //
        // Summary:
        //     Intended to be used for the treatment of a patient, limited changes are allowed.
        TreatmentApproved = 4,
        //
        // Summary:
        //     Completed early status.
        CompletedEarly = 5,
        //
        // Summary:
        //     Completed status.
        Completed = 6,
        //
        // Summary:
        //     The original plan is set to the Retired state when a revision is created. No
        //     changes are possible to Retired plans.
        Retired = 7,
        //
        // Summary:
        //     Unplanned treatment status.
        UnPlannedTreatment = 8,
        //
        // Summary:
        //     Refers to an imported plan that has the approval status in its DICOM data. Locked
        //     in the database, and managed in the same way as Planning Approved plans.
        ExternallyApproved = 9,
        //
        // Summary:
        //     Unknown status.
        Unknown = 999
    }
}
