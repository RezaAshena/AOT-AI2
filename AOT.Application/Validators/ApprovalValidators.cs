using AOT.Application.Commands;

namespace AOT.Application.Validators;

public class CreateApprovalRequestCommandValidator : AbstractValidator<CreateApprovalRequestCommand>
{
    public CreateApprovalRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.RequesterId)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ExpiresAtUtc)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration must be in the future");
    }
}
