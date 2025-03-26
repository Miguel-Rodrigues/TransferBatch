namespace TransferBatch.IntegrationTests
{
    public class ConsoleOutput : IDisposable
    {
        public StringWriter Writer { get; }
        private TextWriter originalOutput;

        public ConsoleOutput()
        {
            Writer = new StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(Writer);
        }

        public string GetOuput()
        {
            return Writer.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            Writer.Dispose();
        }
    }
}
