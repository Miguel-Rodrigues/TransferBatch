using TransferBatch.Common;
using TransferBatch.Common.Dto;
using TransferBatch.Common.Services;
using TransferBatch.Common.Validators;

namespace TransferBatch.TestDataGenerator;

internal class Program
{
    static async Task Main(string[] args)
    {
        var parameters = args.ToParameters();
        var options = new SampleGeneratorOptions(parameters);
        var validatorResult = await (new SampleGeneratorOptionsValidator().ValidateAsync(options));

        if (!validatorResult.IsValid)
        {
            Console.Error.WriteLine("There are some issues:");
            foreach (var error in validatorResult.Errors)
            {
                Console.Error.WriteLine($"\t{error.ErrorMessage}");
            }
            Console.WriteLine();
            Console.WriteLine("Usage: TestDataGenerator [-verbose] [-NumberOfAccounts 10] [-FileSizeLimit 10000] [-NumberOfWorkers <Nr_of_CPUs>] [<OutputPath>]");
            Environment.Exit(1);
            return;
        }
        await new SampleGeneratorService().Generate(options);
    }
}