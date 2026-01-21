using FiscalCore.Application.DTOs.Users;
using FluentValidation;

namespace FiscalCore.Application.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("El id es obligatorio.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
            .MaximumLength(100).WithMessage("El nombre de usuario no debe exceder los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email debe ser válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe contener al menos 8 caracteres.");
    }
}
