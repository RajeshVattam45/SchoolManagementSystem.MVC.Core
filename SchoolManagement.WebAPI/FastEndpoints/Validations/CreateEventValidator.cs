using FastEndpoints;
using FluentValidation;
using global::FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using System.Text.RegularExpressions;

namespace SchoolManagement.WebAPI.FastEndpoints.Validations
{
    public class CreateEventValidator : Validator<SchoolManagement.Core.Entites.Models.Events>
    {
        public CreateEventValidator ( )
        {
            RuleFor ( e => e.OrganizedBy )
                .NotEmpty ().WithMessage ( "Organiser name is required." )
                .Must ( name =>
                {
                    var isValid = Regex.IsMatch ( name ?? "", "^[a-zA-Z ]+$" );
                    if (!isValid)
                    {
                        Console.WriteLine ( "Validation failed: OrganizedBy contains invalid characters." );
                    }
                    return isValid;
                } )
                .WithMessage ( "Organiser name must contain only letters and spaces." );

            RuleFor ( e => e.EventDate ).Custom ( ( date, context ) =>
            {
                if (date.Date < DateTime.Today)
                {
                    Console.WriteLine ( $"Validation Failed: Event date {date} not be past" );
                    context.AddFailure ( "EventDate", "Event date must be today or in the future." );
                }
            } );
        }
    }
}
