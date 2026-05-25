using System;
using System.Collections.Generic;

namespace sw_control_hierachy.Models;

public partial class Calog
{
    public int CalogId { get; set; }

    public int? BbslogId { get; set; }

    public int? HhlogId { get; set; }

    public int? InspectionLogId { get; set; }

    public int? DatlogId { get; set; }

    public int? IncidentLogId { get; set; }

    public int? MoclogId { get; set; }

    public int? BehaviorTypeId { get; set; }

    public string? Description { get; set; }

    public string? CurrentStatus { get; set; }

    public DateTime? OpenDate { get; set; }

    public DateTime? CommitDate { get; set; }

    public DateTime? CompleteDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public int? SubmitByUserId { get; set; }

    public int? AssignedUserId { get; set; }

    public int? ClosedByUserId { get; set; }

    public int CastatusId { get; set; }

    public bool IsClosed { get; set; }

    public int CariskId { get; set; }

    public int LocationId { get; set; }

    public int ActivityId { get; set; }

    public int CustomerId { get; set; }

    public int WorksiteId { get; set; }

    public int ReportId { get; set; }

    public bool IsArchived { get; set; }

    public bool IsArchivedNcr { get; set; }

    public int? SwalogId { get; set; }

    public DateTime? Cadate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int? ItquestionCalogId { get; set; }

    public int? CacontrolId { get; set; }

    public bool? IsNearMiss { get; set; }

    public int? JseariskRegistryLogId { get; set; }

    public double? QratingPoint { get; set; }

    public string? QratetingMsg { get; set; }

    public int? UserDeviceId { get; set; }

    public int? LastUpdateByUserId { get; set; }

    public Guid GlobalId { get; set; }

    public bool IsNcrca { get; set; }

    public int DistrictId { get; set; }

    public int? SupervisorId { get; set; }
}
