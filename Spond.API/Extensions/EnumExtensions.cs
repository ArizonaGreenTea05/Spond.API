using System.Runtime.Serialization;

namespace Spond.API.Extensions;

/// <summary>
/// Extension methods for enum values.
/// </summary>
internal static class EnumExtensions
{
    /// <summary>
    /// Gets the <see cref="EnumMemberAttribute"/> value for an enum value, or falls back to
    /// the enum member name if no attribute is present.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The serialized string representation of the enum value.</returns>
    public static string ToEnumMemberValue(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name is null) return value.ToString();

        var field = type.GetField(name);
        if (field is null) return name;

        var attr = (EnumMemberAttribute?)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));
        return attr?.Value ?? name;
    }
}
