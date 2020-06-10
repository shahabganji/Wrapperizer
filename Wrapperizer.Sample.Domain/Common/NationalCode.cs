using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Sample.Domain.Exceptions;

namespace Wrapperizer.Sample.Domain.Common
{
    /// <summary>
    /// Represents a national code in Iran
    /// </summary>
    public sealed class NationalCode : ValueObject<NationalCode>
    {
        private readonly string _nationalCode;

        private NationalCode(string nationalCode)
        {
            _nationalCode = nationalCode;
        }

        /// <summary>
        /// Returns a human readable string of national code 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _nationalCode;

        /// <summary>
        /// Create a new instance of <see cref="NationalCode"/> from the specified string.
        /// </summary>
        /// <param name="nationalCode"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNationalCodeException">If the specified string does not match the validation rules </exception>
        public static NationalCode From(string nationalCode)
        {
            if (!IsNationalCodeValid(nationalCode))
                throw new InvalidNationalCodeException($"National code, {nationalCode} ,is not valid");

            return new NationalCode(nationalCode);
        }

        public static bool IsNationalCodeValid(string nationalCode) => nationalCode.Length == 10;
        

        public static implicit operator string(NationalCode nationalCode) => nationalCode.ToString();
        public static explicit operator NationalCode(string nationalCode) => From(nationalCode);
    }
}
