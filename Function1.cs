using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using DurableFunctionsSample.DI;
using Microsoft.Extensions.DependencyInjection;

namespace DurableFunctionsSample
{
    //  "AzureWebJobsSecretStorageType": "files" add this in the local.settings.json // there is a bug -> Emulator.
    public static class Function1
    {
        private static ServiceProvider services = new ContainerBuilder().Build();

        [FunctionName("DurableFunctionSample")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            var calculator = services.GetService<ICalculator>();
            var result = calculator.Add(2, 5);

            string process = req.RequestUri.ParseQueryString().Get("process");
            if (process == null) return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            var id = await client.StartNewAsync("O_MainActivity", process);
            return client.CreateCheckStatusResponse(req, id);
        }
    }
}
