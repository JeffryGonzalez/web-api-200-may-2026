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


- Resource (URI) /tacos
- What is the Method? POST 
- What is the representation? (data they send in the body)

"employee/problems" - "what is 'employee'"?  "Store" archetype

- Collections
- Documents
- Stores - maintaining client state on the server, weird description, but this is it
- Controllers - more on this later, but it isn't a controller in our API.




POST /employee/problems
Authorization: token identity token from the IDP the WHO IS DOING THIS QUESTION.
Content-Type: application/json

{
   "softwareId": "{guid}",
   "description": "...",
   "impact": "WorkStoppage"
}

// what do I return from this?

- Do I do all those checks first? Is this software still supported? Is the employee "entitled" to use this software? 
- Unit of Work:
    - check with the software folks
    - check with the software entitlement folks

Return:

The employee ID here can be ANYTHING WE WANT IT TO BE IN OUR SYSTEM
- An Alias. 


201 Created
Location: /employees/{guid}/issues/{guidId}

{
    "reported": "{datetimeoffset}",
    "id": "id",
    "issue": {
            
        "softwareId": "{guid}",
        "description": "...",
        "impact": "WorkStoppage"

    },
    "status": "Approved - Waiting Tier 1 Assignment"
    
}


What if the employee can't get their browser to work to do this?


Operation: Managers can report issues on behalf of their employees.

"You can only add an item to a vendor that already exists - Managers have to create the vendors"

POST /vendors/{vendorId}/catalog - 404 is better than a 400.




