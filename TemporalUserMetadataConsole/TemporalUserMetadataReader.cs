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

    public static async Task<(string? Summary, string? Details, Dictionary<string, string?> Memo)> GetAsync2(
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

        var memo = new Dictionary<string, string?>();

        if (res.WorkflowExecutionInfo?.Memo?.Fields != null)
        {
            foreach (var field in res.WorkflowExecutionInfo.Memo.Fields)
            {
                memo[field.Key] = Decode(field.Value);
            }
        }

        return (
            Decode(meta?.Summary),
            Decode(meta?.Details),
            memo
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
