using FluentValidation;

namespace CalendarBackend.Models;

public class EventValidator : AbstractValidator<EventModel>{
    
    public EventValidator(){
        RuleFor(x => x.Title).NotEmpty().WithMessage("El título es obligatorio");
        RuleFor(x => x.Start).NotEmpty().WithMessage("Fecha de inicio es obligatoria");
        RuleFor(x => x.End).NotEmpty().WithMessage("Fecha de finalización es obligatoria");
        
        // RuleFor(x => x.Password).MinimumLength(6).WithMessage("El password debe ser de al menos 6 caracteres");
    }

}