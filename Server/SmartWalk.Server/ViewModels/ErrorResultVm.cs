using System.Collections.Generic;
using System.Linq;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class ErrorResultVm
    {
        [UsedImplicitly]
        public ErrorResultVm()
        {
        }

        public ErrorResultVm(IList<ValidationError> errors)
        {
            ValidationErrors = errors;
        }

        [UsedImplicitly]
        public IList<ValidationError> ValidationErrors { get; set; } 
    }

    public class ValidationError
    {
        [UsedImplicitly]
        public ValidationError()
        {   
        }

        public ValidationError(string property, string error)
        {
            Property = property;
            Error = error;
        }

        [UsedImplicitly]
        public string Property { get; set; }

        [UsedImplicitly]
        public string Error { get; set; }
    }

    public static class ResultVmExtensions
    {
        public static bool ContainsPropertyError(this IEnumerable<ValidationError> errors, string property)
        {
            var result = PropertyErrors(errors, property).Any();
            return result;
        }

        public static IList<ValidationError> PropertyErrors(
            this IEnumerable<ValidationError> errors, string property)
        {
            var result = errors.Where(er => er.Property.Contains(property)).ToArray();
            return result;
        }
    }
}