using TransferBatch.Common;
using TransferBatch.Common.Dto;
using TransferBatch.Common.Services;
using TransferBatch.Common.Validators;

namespace TransferBatch;

internal class Program
{
    static async Task Main(string[] args)
    {
        var parameters = args.ToParameters("verbose");
        var options = new TransferBatchOptions(parameters);

        var validatorResult = await (new TransferBatchOptionsValidator().ValidateAsync(options));
        if (!validatorResult.IsValid)
        {
            Console.Error.WriteLine("There are some issues:");
            foreach (var error in validatorResult.Errors)
            {
                Console.Error.WriteLine($"\t{error.ErrorMessage}");
            }
            Console.WriteLine("Usage: TransferBatch [-verbose] [-ComissionRate <0.1>] [-WorkerCount <num_of_CPUs>] <FilePath>");
            Environment.Exit(1);
            return;
        }

        await new TransferBatchService().ProcessTransfers(options);
    }
}