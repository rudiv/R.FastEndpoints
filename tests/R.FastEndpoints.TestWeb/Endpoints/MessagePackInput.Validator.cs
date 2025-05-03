using FastEndpoints;
using FluentValidation;

namespace R.FastEndpoints.TestWeb.Endpoints;

public class MessagePackInputValidator : Validator<MessagePackInputRequest>
{
    public MessagePackInputValidator()
    {
        RuleFor(x => x.Test).NotEmpty();
    }
}