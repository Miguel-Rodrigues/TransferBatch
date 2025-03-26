using System.Diagnostics.CodeAnalysis;

namespace TransferBatch.Common.Dto;

[ExcludeFromCodeCoverage(Justification = "Local development code, not covered")]
/// <summary>
/// Options for the sample generator
/// </summary>
/// <param name="NumberOfAccounts">Number of Possible Accounts. Defaults to 10.</param>
/// <param name="FileSizeLimit">The size limit of the generated file in bytes. Defaults to 100KB.</param>
/// <param name="NumberOfWorkers">Number of parallel workers. Defaults to the number of CPU Cores of the machine.</param>
/// <param name="OutputPath">The output path for the generated file.</param>
public record SampleGeneratorOptions(int NumberOfAccounts, long FileSizeLimit, int NumberOfWorkers, string OutputPath)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleGeneratorOptions"/> class.
    /// </summary>
    public SampleGeneratorOptions() : this(10, 100_000, Environment.ProcessorCount, "output.csv") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleGeneratorOptions"/> class.
    /// </summary>
    /// <param name="parameters">The input args from the console</param>
    public SampleGeneratorOptions(Dictionary<string, string> parameters) : this()
    {
        NumberOfAccounts = int.TryParse(
            parameters.GetValueOrDefault(nameof(NumberOfAccounts).ToUpper()), out var _numberOfAccounts)
            ? _numberOfAccounts : NumberOfAccounts;
        FileSizeLimit = long.TryParse(
            parameters.GetValueOrDefault(nameof(FileSizeLimit).ToUpper()), out var _fileSizeLimit)
            ? _fileSizeLimit : FileSizeLimit;
        NumberOfWorkers = int.TryParse(
            parameters.GetValueOrDefault(nameof(NumberOfWorkers).ToUpper()), out var _numberOfWorkers)
            ? _numberOfWorkers : NumberOfWorkers;
        OutputPath =
            parameters.GetValueOrDefault(nameof(OutputPath).ToUpper(),
            parameters.GetValueOrDefault(string.Empty, OutputPath));
    }
};
