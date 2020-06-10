using Wrapperizer.Sample.Domain.Exceptions;
using Xunit;

namespace Wrapperizer.Sample.Domain.Test.Common
{
    public class NationalCode
    {
        [Fact]
        public void Should_throw_exception_if_invalid()
        {
            const string nationalCodeString = "1234";

            Assert.Throws<InvalidNationalCodeException>(() =>
            {
                var nationalCode = Domain.Common.NationalCode.From(nationalCodeString);
            });
        }
        
        [Fact]
        public void Should_cast_valid_string_to_national_code()
        {
            const string nationalCodeString = "1234567890";
            const string nationalCode = nationalCodeString;
            
            Assert.True(nationalCodeString == nationalCode);
        }
    }
}
