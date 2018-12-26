﻿namespace DurableFunctionsSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public static class Activities
    {
        [FunctionName("A_Step_1")]
        public static async Task<List<object>> Step1(
            [ActivityTrigger] string process,
            TraceWriter log
            )
        {
            log.Info("running step1");
            await Task.Delay(1000);
            return new List<object> { "Khurram", "Mughal", process };
        }


        [FunctionName("A_Step_2")]
        public static async Task<List<object>> Step2(
            [ActivityTrigger] List<object> outputFromStep1,
            TraceWriter log
            )
        {
            log.Info("running step1");
            await Task.Delay(1000);
            outputFromStep1.Reverse();
            return outputFromStep1;
        }

    }
}