namespace DurableFunctionsSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System.Collections.Generic;
    using System.Threading.Tasks;


   
    public static class Activities
    {
        [FunctionName("A_SendEmailForDocumentApproval")]
        public static async Task<string> SendFakeEmailForDocumentApproval([ActivityTrigger] string fakeDocumentName,TraceWriter log)
        {
            // simulate email
            await Task.Delay(1000);
            return "Email sent for document approval";
        }

        [FunctionName("A_Publish_Document_To_System")]
        public static async Task<string> SimulateDocumentApproved([ActivityTrigger] string fakeDocumentName, TraceWriter log)
        {
            // simulate email
            await Task.Delay(1000);
            return "File published to the site";
        }

        [FunctionName("A_Delete_Document_As_Rejected")]
        public static async Task<string> SimulateDocumentRejected([ActivityTrigger] string fakeDocumentName, TraceWriter log)
        {
            // simulate email
            await Task.Delay(1000);
            return "File removed from the cache!";
        }

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


        [FunctionName("A_Step_Fake_EnrichObject")]
        public static async Task<FakeEnrichObject> EnrichObject(
            [ActivityTrigger] FakeEnrichObject fakeEnrichObjects,
            TraceWriter log
            )
        {
            // simulate the task.
            log.Info("start enriching object");
            await Task.Delay(3000);

            return fakeEnrichObjects;
        }

        public class FakeEnrichObject
        {

        }
    }
}
