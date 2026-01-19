namespace camunda_playground.Interfaces;
public interface ICamundaService
{
    Task<long> StartInstance<T>(string bpmnProcessId, T? variablesObject);
    Task PublishMessage<T>(string message, string correlationKey, T? variablesObject);
}