using Shouldly;
using TransferBatch.Common.Dto;
using TransferBatch.Common.Services;

namespace TransferBatch.IntegrationTests
{
    public class TransferBatchTests
    {
        [Theory]
        [InlineData(false, "sample.transfers.csv", "expectedOutput.csv")]
        [InlineData(false, "sample.transfers.100KB.csv", "expectedOutput.100KB.csv")]
        [InlineData(false, "sample.transfers.singleTransaction.csv", "expectedOutput.singleTransaction.csv")]
        [InlineData(false, "sample.transfers.invalid.csv", "expectedOutput.invalid.csv")]
        [InlineData(true, "sample.transfers.invalid.csv", "expectedOutput.invalidVerbose.txt")]
        public async Task TransferBatch_ReadsSampleFile_ShouldReturnComissions(bool verbose, string sampleFile, string expectedOutputFile)
        {
            var currentConsoleOut = Console.Out;
            var expectedOutput = File
                .ReadAllLines($"TestData/{expectedOutputFile}")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Order().ToArray();

            using (var consoleOutput = new ConsoleOutput())
            {
                var transferBatch = new TransferBatchService();
                var options = new TransferBatchOptions()
                {
                    FilePath = $"TestData/{sampleFile}",
                    Verbose = verbose
                };

                await transferBatch.ProcessTransfers(options);
                var actualOuptut = consoleOutput
                    .GetOuput().Split(consoleOutput.Writer.NewLine)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Order().ToArray();

                expectedOutput.ShouldBeSubsetOf(actualOuptut);
            }
        }
    }
}