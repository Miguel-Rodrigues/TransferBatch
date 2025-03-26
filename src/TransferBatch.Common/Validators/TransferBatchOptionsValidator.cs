using FluentValidation;
using TransferBatch.Common.Dto;

namespace TransferBatch.Common.Validators
{
    public class TransferBatchOptionsValidator : AbstractValidator<TransferBatchOptions>
    {
        public TransferBatchOptionsValidator()
        {
            RuleFor(x => x.FilePath)
                .NotEmpty()
                .Must(path => 
                    !Path.GetInvalidPathChars().Any(path.Contains) &&
                    File.Exists(path))
                .WithMessage("FilePath must be a valid path of a existing file.");
            RuleFor(x => x.WorkerCount)
                .InclusiveBetween(1, 100)
                .WithMessage("WorkerCount must be between 1 and 100.");
            RuleFor(x => x.ComissionRate)
                .GreaterThan(0)
                .WithMessage("ComissionRate must be greater than 0.");
        }
    }
}