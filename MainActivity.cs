namespace DurableFunctionsSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public static class MainActivity
    {
        [FunctionName("O_MainActivity")]
        public static async Task<object> ActivityManager(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            TraceWriter log
            )
        {
            if (!ctx.IsReplaying) log.Info("starting activity 1");
            var inputForStep1 = ctx.GetInput<string>();
            var outputOfStep1 = await ctx.CallActivityAsync<List<object>>("A_Step_1", inputForStep1);

            if (!ctx.IsReplaying) log.Info("starting activity 2");
            var outputOfStep2 = await ctx.CallActivityAsync<List<object>>("A_Step_2", outputOfStep1);

            return new {
                OutputFromStep1 = outputOfStep1,
                OutputFromStep2 = outputOfStep2
            };
        }
    }
}
