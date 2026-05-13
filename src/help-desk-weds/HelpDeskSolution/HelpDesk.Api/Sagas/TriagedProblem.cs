using HelpDesk.Api.ReadModels;
using JasperFx.Core;
using Wolverine;

namespace HelpDesk.Api.Sagas;

public record StartTriage(Guid Id);
public record AwaitingTriage(Guid Id) : TimeoutMessage(3.Minutes());
public class TriagedProblem : Saga
{
   public Guid Id { get; set; }
   public bool Overdue { get; set; }
   public string Link
   {
      get
      {
         return $"/techs/triaged/{Id}";
      }
   }

   public static async Task<(TriagedProblem, AwaitingTriage)> Start(StartTriage problem)
   {
      return (new TriagedProblem()
      {
         Id = problem.Id,
      }, new AwaitingTriage(problem.Id));
   }

   public void Handle(AwaitingTriage notification)
   {
      Overdue = true;
   }
}