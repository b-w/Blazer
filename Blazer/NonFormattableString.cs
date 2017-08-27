#if FEATURE_FORMATTABLE_STRING
namespace Blazer
{
    using System;

    public sealed class NonFormattableString
    {
        public string Value { get; }

        public NonFormattableString(string value)
        {
            Value = value;
        }

        public static implicit operator NonFormattableString(string value)
        {
            return new NonFormattableString(value);
        }

        public static implicit operator NonFormattableString(FormattableString value)
        {
            throw new InvalidOperationException($"Conversion from {nameof(FormattableString)} to {nameof(NonFormattableString)} is not supported.");
        }
    }
}
#endif
