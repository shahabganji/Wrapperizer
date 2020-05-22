namespace Wrapperizer.Core.Abstraction
{
    public abstract class DomainError
    {
        public static  readonly  DomainError Empty = new EmptyMessage();
        public abstract string Message { get; }
        
        
        sealed  class EmptyMessage : DomainError
        {
            public override string Message => string.Empty;
        }
    }
    
    
}
