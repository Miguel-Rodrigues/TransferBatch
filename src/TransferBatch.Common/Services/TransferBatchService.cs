using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Text;
using TransferBatch.Common.Dto;

namespace TransferBatch.Common.Services;

/// <summary>
/// Processes a batch of transfers to calculate the comissions
/// </summary>
public class TransferBatchService
{
    private const int BLOCK_COUNT = 10;
    private const decimal TRANSFERS_FEE = 0.1m;

    private readonly ConcurrentQueue<(string AccountId, decimal Amount)> _elegibleForHighestTransfer = new();
    private readonly ConcurrentDictionary<string, decimal> _totalsPerAccount = new();
    private (string AccountId, decimal Amount) _highestTransfer;

    private long _fileSize = 0;
    private long _totalLinesRead = 0;
    private long _totalBytesRead = 0;

    /// <summary>
    /// Processes the transfers from the CSV file
    /// </summary>
    /// <returns>The asynchronous task of this call</returns>
    public async Task ProcessTransfers(TransferBatchOptions options)
    {
        using var fileStream = new FileStream(options.FilePath, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
        using var reader = TextReader.Synchronized(streamReader);
        _fileSize = fileStream.Length;

        Task monitor = LaunchWorkers(reader, options.WorkerCount, options.Verbose);

        await CheckProgress(monitor, options.Verbose);
        await OutputComissions(options.Verbose);
    }

    /// <summary>
    /// Launch worker threads for batch processing
    /// </summary>
    /// <returns>The asynchronous task of this call</returns>
    private async Task LaunchWorkers(TextReader reader, int workerCount, bool verbose)
    {
        var workers = Enumerable
            .Range(0, workerCount)
            .Select(i => Task.Run(async() => await ProcessWorker(reader, verbose)));

        var cancellationTokenSource = new CancellationTokenSource();
        var highestTransferResolver = Task.Run(() => ResolveHighestTransfer(cancellationTokenSource.Token));
        await Task.WhenAll(workers);

        cancellationTokenSource.Cancel();
        await highestTransferResolver;
    }

    /// <summary>
    /// Resolves the highest transfer of the batch
    /// </summary>
    /// <param name="token"></param>
    private void ResolveHighestTransfer(CancellationToken token)
    {
        while (!_elegibleForHighestTransfer.IsEmpty || !token.IsCancellationRequested)
       
        if (_elegibleForHighestTransfer.TryDequeue(out var next) &&
            _highestTransfer.Amount < next.Amount)
        {
            _highestTransfer = next;
        }
    }

    /// <summary>
    /// Monitor progress of the CSV batch
    /// </summary>
    /// <param name="monitor">Task aggregator with all workers tasks to monitor.</param>
    /// <returns>The asynchronous task of this call</returns>
    private async Task CheckProgress(Task monitor, bool verbose)
    {
        var sw = Stopwatch.StartNew();
        do
        {
            await Task.Delay(100);
            var progress = Math.Min((float)_totalBytesRead / _fileSize * 100, 100);
            Console.Title = $"{progress:F2}%, {(int)sw.Elapsed.TotalSeconds}s, {_totalLinesRead} lines";
            if (verbose)
            {
                Console.Write($"\rProgress: {progress:F2}%, Elapsed {(int)sw.Elapsed.TotalSeconds}s, {_totalLinesRead} lines written");
            }
        } while (!monitor.IsCompleted);
        if (verbose)
        {
            Console.WriteLine($"\rProgress: {100:F2}%, Elapsed {(int)sw.Elapsed.TotalSeconds}s, {_totalLinesRead} lines written");
        }
    }

    /// <summary>
    /// Outputs the comissions to the console
    /// </summary>
    /// <returns>The asynchronous task of this call</returns>
    private async Task OutputComissions(bool verbose)
    {
        _totalsPerAccount.AddOrUpdate(_highestTransfer.AccountId ?? string.Empty, 0, (key, oldValue) => oldValue - _highestTransfer.Amount);
        if (verbose)
        {
            Console.WriteLine();
            Console.WriteLine("== Highest Transaction ==");
            Console.WriteLine("AccountID\tTransaction");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"{_highestTransfer.AccountId}\t\t{_highestTransfer.Amount:0.######}");

            Console.WriteLine();
            Console.WriteLine("== Comissions ==");
            Console.WriteLine("AccountID\tComission");
            Console.WriteLine("--------------------------------");
            await Parallel.ForEachAsync(_totalsPerAccount, async (comission, _) =>
            {
                await Console.Out.WriteLineAsync($"{comission.Key}\t\t{comission.Value * TRANSFERS_FEE:0.######}");
            });
        }
        else
        {
            await Parallel.ForEachAsync(_totalsPerAccount, async (comission, _) =>
            {
                await Console.Out.WriteLineAsync($"{comission.Key},{comission.Value * TRANSFERS_FEE:0.######}");
            });
        }
    }

    /// <summary>
    /// Worker method to process the CSV file
    /// </summary>
    /// <returns>The asynchronous task of this call</returns>
    private async Task ProcessWorker(TextReader reader, bool verbose)
    {
        string? line = string.Empty;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            string[] cols = line.Split(',');
            if (string.IsNullOrWhiteSpace(line) ||
                cols.Length != 3 ||
                !decimal.TryParse(cols[2], out decimal amount))
            {
                if (verbose)
                {
                    Console.WriteLine($"Invalid line: \"{line}\"");
                }
                continue;
            }

            string accountId = cols[0];
            _totalsPerAccount.AddOrUpdate(accountId, amount, (key, oldValue) => oldValue + amount);

            if (amount > _highestTransfer.Amount)
            {
                _elegibleForHighestTransfer.Enqueue((accountId, amount));
            }

            _totalLinesRead++;
            _totalBytesRead += Encoding.UTF8.GetByteCount(line);
        }
    }
}
