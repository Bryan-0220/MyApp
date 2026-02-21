namespace Domain.Common
{

    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class DuplicateException : DomainException
    {
        public DuplicateException(string message) : base(message) { }
    }

    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
