using Shouldly;
using TransferBatch.Common.Dto;
using TransferBatch.Common.Validators;

namespace TransferBatch.Common.UnitTests.Validators
{
    public class TransferBatchOptionsValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnTrue_WhenOptionsAreValid()
        {
            var file = "testFile.txt";
            File.Create(file).Dispose();
            // Arrange
            var options = new TransferBatchOptions(file, 0.2m, 2, true);
            var validator = new TransferBatchOptionsValidator();

            // Act
            var result = validator.Validate(options);

            // Assert
            result.IsValid.ShouldBeTrue();
            File.Delete(file);
        }

        [Fact]
        public void Validate_ShouldReturnFalse_WhenOptionsAreInvalid()
        {
            // Arrange
            var options = new TransferBatchOptions(new Dictionary<string, string>
            {
                { "FILEPATH", "#!@%$**)\" + _{ }\":??COM3>?<>?" },
                { "COMISSIONRATE", "-1" },
                { "WORKERCOUNT",  "-1" },
                { "VERBOSE", false.ToString() }
            });
            var validator = new TransferBatchOptionsValidator();
            var expectedErrors = new[]
            {
                "FilePath must be a valid path of a existing file.",
                "WorkerCount must be between 1 and 100.",
                "ComissionRate must be greater than 0."
            };

            // Act
            var result = validator.Validate(options);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors
                .Select(x => x.ErrorMessage)
                .ToArray()
                .ShouldBeEquivalentTo(expectedErrors);
        }
    }
}