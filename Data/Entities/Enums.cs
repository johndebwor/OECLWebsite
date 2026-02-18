namespace OECLWebsite.Data.Entities;

public enum ApprovalStatus
{
    Draft,
    PendingApproval,
    Approved,
    Rejected
}

public enum ProjectStatus
{
    Planned,
    Ongoing,
    Completed
}

public enum InquiryStatus
{
    New,
    InProgress,
    Responded,
    Closed
}

public enum ApprovalAction
{
    Submitted,
    Approved,
    Rejected,
    Returned
}

public enum ApproverLevel
{
    DepartmentHead = 1,
    GeneralManager = 2,
    CEO = 3
}

public enum AnnouncementPriority
{
    High = 1,
    Medium = 2,
    Low = 3
}
