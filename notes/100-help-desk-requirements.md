# Help Desk API Requirements

The Help Desk needs an API to keep track of issues with software.

As part of this class we will:

- Gather some "requirements"
- Design an API
- Emphasize interacting with the Software API (Catalog)


## What would "Employee Reporting an Issue With Software" Look Like?


Operation: Employee Submits Issue for Software They Are Using

The employee can only submit an issue for software that is supported by the company. (SoftwareCenter maintains the list)

Operands:
- Employee Identity  - Authorization Header FTW!
- The Software they have issues with 
- A description of the issue
- An impact level - Question | Inconvenience | WorkStoppage

This is an unsupported software item: 5dbe643d-37e0-44f2-bc51-a71fbeb5d0b4

This is a supported piece of software:
25ff5f2d-cdaa-4a6b-96f7-f018d3f27e59

This is retired Software:
d2a03441-2444-4453-bab3-0ad872068ff3

```http
POST https://localhost:1337/employee/problems
Authorization: token identity token from the IDP the WHO IS DOING THIS QUESTION.
Content-Type: application/json

{
   "softwareId": "25ff5f2d-cdaa-4a6b-96f7-f018d3f27e59",
   "description": "This excel issue thing won't print help",
   "impact": "WorkStoppage"
}
```

```http
GET https://localhost:1337/employee/x0039/problems/a5b9a10f-6541-4d97-8319-a7bf788b6176
```


```http
GET https://localhost:1337/employee/sagas
```

```http
GET https://localhost:1337/employee/triaged/a5b9a10f-6541-4d97-8319-a7bf788b6176
```

// what do I return from this?

- Do I do all those checks first? Is this software still supported? Is the employee "entitled" to use this software? 
- Unit of Work:
    - check with the software folks
    - check with the software entitlement folks

Return:

The employee ID here can be ANYTHING WE WANT IT TO BE IN OUR SYSTEM
- An Alias. 


```http
201 Created
Location: /employees/{employeeId}/problems/{issueId}
Content-Type: application/json 

{
    "reported": "{datetimeoffset}",
    "id": "{issueId}",
    "reportedIssue": {           
        "softwareId": "{guid}",
        "description": "...",
        "impact": "WorkStoppage"
    },
    "status": "Submitted"
}
```


### Enrichment

#### Check if Software Exists (`CheckForSupportedSoftware(guid ProblemId, guid SoftwareId)`)

```csharp 
public record SoftwareVerified(string Title, string Manufacturer);

public record SoftwareRetired(DateTimeOffset RetiredDate);

public record SoftwareIsUnknown();
```

#### Check if Submitter is Vip (`CheckForVipStatus(guid ProblemId, string EmployeeId)`)

```csharp
public record SubmitterIsVip();

public record SubmitterIsNotVip();
```

### Assignment

Once the software is verified and vip status is ascertained, problem is sent to assignment.

Assignment performs triage according to the triage rules, and assigns to tech (tier1 - tier3 and "concierge" for VIP with work stoppage, etc.)

- tier1 - non work stoppage for non supported software - no employee augmentation
- tier2 - work stoppage for non-supported software  - employee augmentation
- tier3 - work stoppage for supported software - employee augmentation
- concierge - any problem by a vip - employee augmentation

#### Employee Augmentation:

Connects employee name, email and phone to the problem submitted.



## Manager Submits Issue on Employee Behalf 

What if the employee can't get their browser to work to do this?


Operation: Managers can report issues on behalf of their employees.

"You can only add an item to a vendor that already exists - Managers have to create the vendors"

POST /vendors/{vendorId}/catalog - 404 is better than a 400.




