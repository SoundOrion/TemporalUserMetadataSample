using System.Text;
using System.Text.Json;
using Temporalio.Client;
using Temporalio.Api.Common.V1;
using Temporalio.Api.WorkflowService.V1;

namespace TemporalUserMetadataConsole;

public static class TemporalUserMetadataReader
{
    public static async Task<(string? Summary, string? Details)> GetAsync(
        ITemporalClient client,
        string workflowId,
        string? runId = null)
    {
        var res =
            await client.Connection.WorkflowService
                .DescribeWorkflowExecutionAsync(
                    new DescribeWorkflowExecutionRequest
                    {
                        Namespace = client.Options.Namespace,
                        Execution = new Temporalio.Api.Common.V1.WorkflowExecution
                        {
                            WorkflowId = workflowId,
                            RunId = runId ?? ""
                        }
                    });

        var meta = res.ExecutionConfig?.UserMetadata;

        return (
            Decode(meta?.Summary),
            Decode(meta?.Details)
        );
    }

    private static string? Decode(Payload? payload)
    {
        if (payload == null || payload.Data.IsEmpty)
            return null;

        var json = Encoding.UTF8.GetString(payload.Data.ToByteArray());

        try
        {
            return JsonSerializer.Deserialize<string>(json);
        }
        catch
        {
            return json;
        }
    }
}
