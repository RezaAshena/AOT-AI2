using AOT.Application.Commands;

namespace AOT.Application.Validators;

public class CreateWorkflowCommandValidator : AbstractValidator<CreateWorkflowCommand>
{
    public CreateWorkflowCommandValidator()
    {
        RuleFor(x => x.WorkflowName)
            .NotEmpty().WithMessage("Workflow name is required")
            .MaximumLength(200).WithMessage("Workflow name must not exceed 200 characters");

        RuleFor(x => x.WorkflowDefinitionId)
            .NotEmpty().WithMessage("Workflow definition ID is required");
    }
}

public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(x => x.ApprovalRequestId)
            .NotEmpty().WithMessage("Approval request ID is required");

        RuleFor(x => x.ApproverId)
            .NotEmpty().WithMessage("Approver ID is required");

        RuleFor(x => x.Comments)
            .MaximumLength(1000).WithMessage("Comments must not exceed 1000 characters");
    }
}
