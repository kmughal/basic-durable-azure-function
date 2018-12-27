namespace DurableFunctionsSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static DurableFunctionsSample.Activities;

    // Must avoid Infinite loops , must not have await async , must be non-blocking ( avoid IO operations etc) and must be deterministic
    public static class MainActivity
    {
        [FunctionName("O_SubOrchestrator_Sample")]
        public static async Task<List<FakeEnrichObject>> SampleSubOrchestrator([OrchestrationTrigger] DurableOrchestrationContext ctx)
        {
            // Run tasks in parallel - Fade in Fade out pattern - Map reduce.
            var fakeInput = ctx.GetInput<object>();
            var listOfFakes = new List<FakeEnrichObject>();
            var listOfFakeTasks = new List<Task<FakeEnrichObject>>();
            foreach (var fakeTask in listOfFakes)
            {
                var task = ctx.CallActivityAsync<FakeEnrichObject>("A_Step_Fake_EnrichObject", fakeTask);
                listOfFakeTasks.Add(task);
            }

            var result = await Task.WhenAll(listOfFakeTasks);
            return result.ToList();
        }

        [FunctionName("O_MainActivity")]
        public static async Task<object> ActivityManager(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            TraceWriter log
            )
        {
            try
            {
               
                var outputFromSubOrchestractor = await ctx.CallSubOrchestratorAsync<List<FakeEnrichObject>>("O_SubOrchestrator_Sample", new { FakeInput = "FakeString" });

                if (!ctx.IsReplaying) log.Info("starting activity 1");
                var inputForStep1 = ctx.GetInput<string>();

                // if you want to impose retry .
                // there are many ways to handle error.
                var outputOfStep1 = await ctx
                    .CallActivityWithRetryAsync<List<object>>("A_Step_1",
                    new RetryOptions(TimeSpan.FromSeconds(5), 4) {
                         Handle = ex => ex is InvalidOperationException,
                         RetryTimeout = TimeSpan.FromSeconds(10) // Timeout afterr 10 seconds if no success.
                         
                    },
                    inputForStep1);

                if (!ctx.IsReplaying) log.Info("starting activity 2");
                var outputOfStep2 = await ctx.CallActivityAsync<List<object>>("A_Step_2", outputOfStep1);

                return new
                {
                    Status = "Success",
                    OutputFromStep1 = outputOfStep1,
                    OutputFromStep2 = outputOfStep2
                };
            }
            catch (Exception error)
            {
                return new
                {
                    Status = "Error",
                    error.Message
                };
            }
        }
    }
}
