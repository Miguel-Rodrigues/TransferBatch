using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using TransferBatch.Common.Dto;

namespace TransferBatch.Common.Services;

[ExcludeFromCodeCoverage(Justification = "Local development code, not covered")]
/// <summary>
/// Generates a CSV file with random data
/// </summary>
public class SampleGeneratorService
{
    private long _totalBytesWritten = 0;
    private long _totalLinesWritten = 0;
    private bool _stopFlag = false;
    private ulong _transferId = 1;

    /// <summary>
    /// Generates a CSV file with random data
    /// </summary>
    /// <param name="args">The input args from the console</param>
    /// <returns></returns>
    public async Task Generate(SampleGeneratorOptions options)
    {
        Task monitor = LaunchWorkers(options);
        await CheckProgress(options, monitor);
    }

    /// <summary>
    /// Launch worker threads for CSV generation
    /// </summary>
    /// <param name="options">A list of intput parameters</param>
    /// <returns>The asynchronous task of this call</returns>
    private async Task LaunchWorkers(SampleGeneratorOptions options)
    {
        Console.WriteLine($"Starting CSV generation with {options.NumberOfWorkers} worker threads...");

        File.Create(options.OutputPath).Dispose();
        using FileStream fileStream = new(options.OutputPath, FileMode.Open, FileAccess.ReadWrite);
        using StreamWriter stream = new(fileStream, Encoding.UTF8);
        using TextWriter writer = TextWriter.Synchronized(stream);

        // Start worker tasks
        Task[] workers = Enumerable
            .Range(0, options.NumberOfWorkers)
            .Select(x =>
            {
                var isMaster = x == 0;
                return Task.Run(() => ProcessWorker(options, writer));
            })
            .ToArray();

        await Task
            .WhenAll(workers)
            .ContinueWith(async _ => await writer.FlushAsync());
    }

    /// <summary>
    /// Monitor progress of the CSV generation
    /// </summary>
    /// <param name="options">A list of intput parameters</param>
    /// <param name="monitor">Task aggregator with all workers tasks to monitor.</param>
    /// <returns>The asynchronous task of this call</returns>
    private async Task CheckProgress(SampleGeneratorOptions options, Task monitor)
    {
        // Monitor progress  
        var sw = Stopwatch.StartNew();
        do
        {
            await Task.Delay(100);
            var progress = Math.Min((float)_totalBytesWritten / options.FileSizeLimit * 100, 100);
            Console.Write($"\rProgress: {progress:F2}%, Elapsed: {(int)sw.Elapsed.TotalSeconds}s, {_totalLinesWritten} lines written");
            Console.Title = $"{progress:F2}%, {(int)sw.Elapsed.TotalSeconds}s, {_totalLinesWritten} lines";
        } while (!monitor.IsCompleted);

        Console.WriteLine("\nCSV file generation completed.");
    }

    /// <summary>
    /// Worker call for random data generation
    /// </summary>
    /// <param name="options">A list of intput parameters</param>
    /// <param name="writer">The synchronous text writer</param>
    /// <returns></returns>
    private async Task ProcessWorker(SampleGeneratorOptions options, TextWriter writer)
    {
        Random rand = new Random();
        string[] accountIDs = new string[options.NumberOfAccounts];
        for (int i = 0; i < options.NumberOfAccounts; i++)
            accountIDs[i] = "A" + (i + 1);

        string line = "";
        StringBuilder buffer = new StringBuilder();
        while (!_stopFlag)
        {
            await writer.WriteAsync(line);

            string accountID = accountIDs[rand.Next(options.NumberOfAccounts)];
            double totalTransferAmount = Math.Round(rand.NextDouble() * 100000, 6);

            buffer.Append(accountID);
            buffer.Append(",T");
            buffer.Append(Interlocked.Increment(ref _transferId)); //NOTE: the '++' operator is not threadsafe apparently...
            buffer.Append(",");
            buffer.AppendLine(totalTransferAmount.ToString(CultureInfo.InvariantCulture));

            line = buffer.ToString();
            buffer.Clear();

            _totalLinesWritten++;
            _totalBytesWritten += Encoding.UTF8.GetByteCount(line);
            _stopFlag = _totalBytesWritten >= options.FileSizeLimit;
        }
    }
}