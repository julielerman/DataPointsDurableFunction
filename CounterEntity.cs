using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataPoints.Function
{
    public class Counter
    {
        [JsonProperty("value")]
        public int CurrentValue { get; set; }
         
       public void Add(int amount) => this.CurrentValue += amount;
     
        public void Reset() => this.CurrentValue = 0;

        public int Get() => this.CurrentValue;

        [FunctionName(nameof(Counter))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Counter>();
    }

}
