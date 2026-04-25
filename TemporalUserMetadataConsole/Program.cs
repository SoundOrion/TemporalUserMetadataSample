using Temporalio.Client;
using TemporalUserMetadataConsole;

var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

var workflowId = $"my-workflow-id";
var result = await TemporalUserMetadataReader.GetAsync(client, workflowId);

Console.WriteLine(result.Summary);
Console.WriteLine(result.Details);


var handle = client.GetWorkflowHandle(workflowId);
var desc = await handle.DescribeAsync();

foreach (var item in desc.Memo)
{
    string value = item.Key switch
    {
        "Amount" => (await item.Value.ToValueAsync<int>()).ToString(),
        "Tags" => string.Join(", ", await item.Value.ToValueAsync<string[]>()),
        _ => await item.Value.ToValueAsync<string>()
    };

    Console.WriteLine($"{item.Key}: {value}");
}

Console.ReadLine();