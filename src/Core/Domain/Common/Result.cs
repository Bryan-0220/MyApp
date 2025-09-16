namespace Domain.Results
{
    public class Result<T>
    {
        public bool Success { get; }
        public string? Message { get; }
        public T? Value { get; }

        private Result(bool success, T? value, string? message)
        {
            Success = success;
            Value = value;
            Message = message;
        }

        public static Result<T> Ok(T value, string? message = null) => new(true, value, message);
        public static Result<T> Fail(string message) => new(false, default, message);
    }
}
