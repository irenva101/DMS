using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMS.Shared.Helper
{
    public class StringEnumConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();

            // Try to match against Description attribute first
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == stringValue)
                        return (T)field.GetValue(null)!;
                }
            }

            // Fallback: match against enum name
            if (Enum.TryParse(stringValue, true, out T result))
                return result;

            // Fallback: numeric value
            if (int.TryParse(stringValue, out int intValue) && Enum.IsDefined(typeof(T), intValue))
                return (T)(object)intValue;

            throw new JsonException($"Unable to convert \"{stringValue}\" to enum {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Do not change this, it will brake login and encoding of permissions
            writer.WriteStringValue(value.ToString());
        }
    }
}
