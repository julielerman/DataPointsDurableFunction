using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DataPoints.Function
{
    public static class CounterReset
    {

        [FunctionName("CounterReset")]
        public static async Task Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
          [DurableClient] IDurableEntityClient client)
        {
             var entityId = new EntityId(nameof(Counter), "HelloCounter");
             await client.SignalEntityAsync(entityId, "Reset");
      }
    }

}
