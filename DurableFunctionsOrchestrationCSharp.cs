using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;

namespace DataPoints.Function
{
  public static class DurableFunctionsOrchestrationCSharp
  {
    [FunctionName("DurableFunctionsOrchestrationCSharp")]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger]IDurableOrchestrationContext context)
    {
      var outputs = new List<string>();
      outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "Tokyo"));
      outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "Seattle"));
      outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "London"));


      var entityId = new EntityId(nameof(Counter), "HelloCounter");
      context.SignalEntity(entityId, "Add", 1);
      var finalValue = await context.CallEntityAsync<int>(entityId, "Get");
      outputs.Add($"Hello Orchestration has been run {finalValue} times");
      return outputs;


    }

    [FunctionName("DurableFunctionsOrchestrationCSharp_Hello")]
    public static string SayHello([ActivityTrigger] string name, ILogger log)
    {
      log.LogInformation($"Saying hello to {name}.");
      return $"Hello {name}!";
    }

    [FunctionName("DurableFunctionsOrchestrationCSharp_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
        [DurableClient]IDurableOrchestrationClient starter,
        ILogger log)
    {
      // Function input comes from the request content.
       string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestrationCSharp", null);

      log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
      //  return client.CreateCheckStatusResponse(req, id);

      DurableOrchestrationStatus status;
      while (true)
      {
        status = await starter.GetStatusAsync(instanceId);

        if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed ||
            status.RuntimeStatus == OrchestrationRuntimeStatus.Failed ||
            status.RuntimeStatus == OrchestrationRuntimeStatus.Terminated)
        {
          break;
        }

      }

      return req.CreateResponse(System.Net.HttpStatusCode.OK, status.Output);
    }
  }
}