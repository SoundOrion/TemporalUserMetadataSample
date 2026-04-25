using Temporalio.Client;
using Worker;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

//var workflowId = $"my-workflow-id-{Guid.NewGuid()}";
var workflowId = $"my-workflow-id";

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (SayHelloWorkflow wf) => wf.RunAsync("Temporal"),
        new WorkflowOptions
        {
            Id = workflowId,
            TaskQueue = "my-task-queue",

            StaticSummary = "サマリー",
            StaticDetails = "詳細を記入します",

            Memo = new Dictionary<string, object>
            {
                ["CustomerId"] = "CUST-001",
                ["OrderNo"] = "ORD-9999",
                ["RequestedBy"] = "Tanaka",
                ["Amount"] = 1200,
                ["Tags"] = new[] { "VIP", "Web" }
            }
        });

Console.WriteLine("Workflow result: {0}", result);