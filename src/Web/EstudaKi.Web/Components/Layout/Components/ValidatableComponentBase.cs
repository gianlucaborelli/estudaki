using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Layout.Components
{
    public abstract class ValidatableComponentBase : ComponentBase
    {
        private Dictionary<string, List<string>> _validationErrors = new();

        /// <summary>
        /// Array with all validation error messages.
        /// Use for binding with @bind-Errors in MudForm.
        /// </summary>
        protected string[] Errors { get; set; } = [];

        /// <summary>
        /// Gets a validation function for a specific property.
        /// Use with @GetValidationFunc(nameof(Property)) in MudBlazor components.
        /// </summary>
        /// <param name="propertyName">Property name to be validated</param>
        /// <returns>Function that returns the validation errors for the property</returns>
        protected Func<string, IEnumerable<string>> GetValidationFunc(string propertyName)
        {
            return value => _validationErrors.TryGetValue(propertyName, out var errors) 
                ? errors 
                : Array.Empty<string>();
        }

        /// <summary>
        /// Processes FluentValidation errors and stores them in the validation dictionary.
        /// Automatically updates the Errors property.
        /// </summary>
        /// <param name="validationResult">FluentValidation result</param>
        protected void ProcessValidationErrors(ValidationResult validationResult)
        {
            _validationErrors.Clear();

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    if (!_validationErrors.ContainsKey(error.PropertyName))
                    {
                        _validationErrors[error.PropertyName] = new List<string>();
                    }
                    _validationErrors[error.PropertyName].Add(error.ErrorMessage);
                }

                Errors = _validationErrors.Values
                    .SelectMany(errors => errors)
                    .ToArray();
            }
            else
            {
                Errors = [];
            }
        }

        /// <summary>
        /// Clears all stored validation errors.
        /// </summary>
        protected void ClearValidationErrors()
        {
            _validationErrors.Clear();
            Errors = [];
        }

        /// <summary>
        /// Checks if a specific property has validation errors.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if there are errors, False otherwise</returns>
        protected bool HasValidationErrors(string propertyName)
        {
            return _validationErrors.ContainsKey(propertyName) && _validationErrors[propertyName].Any();
        }

        /// <summary>
        /// Gets the validation errors for a specific property.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>List of errors or empty list if no errors exist</returns>
        protected IEnumerable<string> GetValidationErrors(string propertyName)
        {
            return _validationErrors.TryGetValue(propertyName, out var errors) 
                ? errors 
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Returns all validation errors as a string array.
        /// </summary>
        /// <returns>Array with all error messages</returns>
        protected string[] GetAllValidationErrors()
        {
            return _validationErrors.Values
                .SelectMany(errors => errors)
                .ToArray();
        }
    }
}
