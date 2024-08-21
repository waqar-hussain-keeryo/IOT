namespace IOT.Business.Validations
{
    public class ValidationResult
    {
        public bool IsSuccess { get; }
        public IEnumerable<string> Errors { get; }

        private ValidationResult(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static ValidationResult Success()
        {
            return new ValidationResult(true, Enumerable.Empty<string>());
        }

        public static ValidationResult Failure(IEnumerable<string> errors)
        {
            return new ValidationResult(false, errors);
        }
    }
}
