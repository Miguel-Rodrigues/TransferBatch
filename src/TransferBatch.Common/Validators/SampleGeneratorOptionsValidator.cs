using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using TransferBatch.Common.Dto;

namespace TransferBatch.Common.Validators
{
    [ExcludeFromCodeCoverage(Justification = "Local development code, not covered")]
    public class SampleGeneratorOptionsValidator : AbstractValidator<SampleGeneratorOptions>
    {
        public SampleGeneratorOptionsValidator()
        {
            RuleFor(x => x.NumberOfAccounts)
                .GreaterThan(0)
                .WithMessage("NumberOfAccounts must be greater than 0.");
            RuleFor(x => x.FileSizeLimit)
                .InclusiveBetween(100, 1_000_000_000_000)
                .WithMessage("FileSizeLimit must be between 100 bytes and 1TB.");
            RuleFor(x => x.NumberOfWorkers)
                .InclusiveBetween(1, 100)
                .WithMessage("NumberOfWorkers must be between 1 and 100.");
            RuleFor(x => x.OutputPath)
                .NotEmpty()
                .Must(path => !Path.GetInvalidPathChars().Any(path.Contains))
                .WithMessage("OutputPath must be a valid path.");
        }
    }
}