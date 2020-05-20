namespace Wrapperizer.Core.Abstraction
{
    public class Result<T>
    {
        public bool Succeeded => this.Errors == null;
        public bool Failed => this.Errors != null;

        public T Data { get; }
        public DomainError[] Errors { get; }

        protected Result(T data)
        {
            this.Data = data;
            this.Errors = null;
        }
        protected Result(params DomainError[] errors)
        {
            this.Data = default;
            this.Errors = errors;
        }
        
        public static implicit operator Result<T>( T data )
            => new Result<T>(data);
        public static implicit operator Result<T>( DomainError error )
            => new Result<T>(error);
        public static implicit operator Result<T>( DomainError[] errors )
            => new Result<T>(errors);
    }
}
