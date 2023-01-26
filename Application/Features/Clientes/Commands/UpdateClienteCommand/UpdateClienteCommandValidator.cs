using FluentValidation;

namespace Application.Features.Clientes.Commands.UpdateClienteCommand
{
    public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
    {
        public UpdateClienteCommandValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a 0");

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(c => c.FechaNacimiento)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío");

            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("{PropertyName} debe cumplir el formato 0000-0000")
                .MaximumLength(9).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío")
                .EmailAddress().WithMessage("{PropertyName} debe ser una dirección de email valida")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(c => c.Direccion)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío")
                .MaximumLength(120).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

        }
    }
}
