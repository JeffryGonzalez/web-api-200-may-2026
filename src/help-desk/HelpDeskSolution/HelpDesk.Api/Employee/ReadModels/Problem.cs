using HelpDesk.Api.Employee.Handlers;

namespace HelpDesk.Api.Employee;

public class Problem
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public bool IsVip { get; set; } = false;
    public bool IsSupportedSoftware { get; set; } = false;

    public static Problem Create(ProblemCreated problem)
    {
        return new Problem();
        
    }
    public void Apply(SubmitterIsVip _, Problem current)
    {
        current.IsVip = true;
    }
    public void Apply(SubmitterIsNotVip _, Problem current)
    {
        current.IsVip = false;
    }
    public void Apply(SoftwareVerified _, Problem current)
    {
        current.IsSupportedSoftware = true;
    }
}

/*
 * POST /employee/problems
Authorization: token identity token from the IDP the WHO IS DOING THIS QUESTION.
Content-Type: application/json

{
   "softwareId": "{guid}",
   "description": "...",
   "impact": "WorkStoppage"
}
*/