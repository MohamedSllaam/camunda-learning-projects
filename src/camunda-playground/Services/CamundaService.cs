using camunda_playground.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Zeebe.Client;
using camunda_playground.Config;
namespace camunda_playground.Services;


internal class CamundaService(IZeebeClient zeebeClient, ILogger<CamundaService> logger) : ICamundaService
{
    public async Task<long> StartInstance<T>(string bpmnProcessId, T? variablesObject)
    {
        var @event = zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(bpmnProcessId)
            .LatestVersion();

        string? variablesJson = null;
        if (variablesObject is not null)
        {
            variablesJson = JsonSerializer.Serialize(variablesObject, SerializationOptions.IgnoreNull);
            @event = @event.Variables(variablesJson);
        }

        var response = await @event.Send();

        logger.LogInformation("Stated camunda workflow instance of bpmn {bpmnProcessId}, process instance key = {processInstanceKey}, varaiables = {variables}", bpmnProcessId, response.ProcessInstanceKey, variablesJson);
        return response.ProcessInstanceKey;
    }

    public async Task PublishMessage<T>(string message, string correlationKey, T? variablesObject)
    {
        var @event = zeebeClient.NewPublishMessageCommand()
             .MessageName(message)
             .CorrelationKey(correlationKey);

        string? variablesJson = null;
        if (variablesObject is not null)
        {
            variablesJson = JsonSerializer.Serialize(variablesObject, SerializationOptions.IgnoreNull);
            @event = @event.Variables(variablesJson);
        }

        await @event.Send();

        logger.LogInformation("Published camunda message {message}, correlationKey = {correlationKey}, varaiables = {variables}", message, correlationKey, variablesJson);
    }
}

