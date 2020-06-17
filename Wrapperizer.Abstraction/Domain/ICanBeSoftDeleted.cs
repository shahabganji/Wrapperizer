namespace Wrapperizer.Abstraction.Domain
{
    public interface ICanBeSoftDeleted
    {
        bool SoftDeleted { get; set; }
    }

    public interface ICanBeAudited
    {
        
    }
}
