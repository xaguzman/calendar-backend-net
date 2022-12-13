using FluentValidation;

namespace CalendarBackend.Models;

public class NewUserValidator : AbstractValidator<UserModel>{
    
    public NewUserValidator(){
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es obligatorio");
        RuleFor(x => x.Email).EmailAddress().WithMessage("El email es obligatorio");
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("El password debe ser de al menos 6 caracteres");
    }

}