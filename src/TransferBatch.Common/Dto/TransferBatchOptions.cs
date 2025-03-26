namespace TransferBatch.Common.Dto
{
    /// <summary>
    /// Options for the transfer batch
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="ComissionRate"></param>
    /// <param name="WorkerCount"></param>
    /// <param name="Verbose"></param>
    public record TransferBatchOptions(
        string FilePath,
        decimal ComissionRate,
        int WorkerCount,
        bool Verbose)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferBatchOptions"/> class.
        /// </summary>
        public TransferBatchOptions() : this(string.Empty, 0.1m, Environment.ProcessorCount, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferBatchOptions"/> class.
        /// </summary>
        /// <param name="parameters"></param>
        public TransferBatchOptions(Dictionary<string, string> parameters) : this()
        {
            FilePath = parameters.GetValueOrDefault(
                nameof(FilePath).ToUpper(),
                parameters.GetValueOrDefault(string.Empty, FilePath));
            ComissionRate = int.TryParse(
                parameters.GetValueOrDefault(nameof(ComissionRate).ToUpper()), out var _comissionRate)
                ? _comissionRate : ComissionRate;
            WorkerCount = int.TryParse(
                parameters.GetValueOrDefault(nameof(WorkerCount).ToUpper()), out var _workerCount)
                ? _workerCount : WorkerCount;
            Verbose = bool.TryParse(
                parameters.GetValueOrDefault(nameof(Verbose).ToUpper()), out var _verbose)
                ? _verbose : Verbose;
        }
    }
}