namespace DurableFunctionsSample
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;


    public static class DurableDeleteFilesSample
    {
        [FunctionName("StartFileDeletion")]
        public static async Task<HttpResponseMessage> StartPeriodicDeletion(
            [HttpTrigger("DeleteFiles","get",Route =null)] HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient client,
            TraceWriter tw
            )
        {
            var instanceId = await client.StartNewAsync("O_DurableDeleteFilesSample",0);
            return client.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("O_DurableDeleteFilesSample")]
        public static async Task<int> DeleteFilesPeriodically(
            [OrchestrationTrigger] DurableOrchestrationContext ctx, TraceWriter tw)
        {
            var timesRun = ctx.GetInput<int>();
            timesRun++;
            if (!ctx.IsReplaying) tw.Info($"starting a new instance {ctx.InstanceId} , {timesRun}");

            await ctx.CallActivityAsync("A_Delete_File", timesRun);
            var nextRun = ctx.CurrentUtcDateTime.AddSeconds(20);
            await ctx.CreateTimer(nextRun, CancellationToken.None);
            ctx.ContinueAsNew(timesRun);
            return timesRun;
        }

        [FunctionName("A_Delete_File")]
        public static async Task<int> SimulateDeleteOperation([ActivityTrigger] int timesRun, ILogger log)
        {
            log.LogWarning("running delete");
            await Task.Delay(2000);
            return timesRun;
        }
        
    }
}