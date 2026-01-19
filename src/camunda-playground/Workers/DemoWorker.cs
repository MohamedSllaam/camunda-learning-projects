using camunda_playground.Constants.Catering;
using Zeebe.Client.Accelerator.Attributes;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;


namespace camunda_playground.Workers;

[JobType(CancelCateringAgreementConstants.Workers.Approve)]
public class DemoWorker
{
    public async Task Handle(IJobClient client, IJob job)
    {
        // do something
        await client.NewCompleteJobCommand(job.Key).Send();
    }
}