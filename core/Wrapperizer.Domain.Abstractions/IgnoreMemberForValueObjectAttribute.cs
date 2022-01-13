using System;

namespace Wrapperizer.Domain.Abstractions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMemberForValueObjectAttribute : Attribute
    {
    }
}
