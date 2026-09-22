using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DMS.Shared.Helper
{
    public static class PostgresEnumGenerator
    {
        public static void RegisterEnums<T>(this ModelBuilder modelBuilder, List<Type>? entityExclusions = default)
            where T : DbContext
        {
            if (entityExclusions == null)
                entityExclusions = [];

            var contextType = typeof(T);

            // Finds all entities defined as DbSet<T>
            var entityTypes = contextType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop =>
                    prop.PropertyType.IsGenericType
                    && prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                )
                .Where(prop => !entityExclusions.Contains(prop.PropertyType.GetGenericArguments()[0])) // Exclude specified entities
                .Select(prop => prop.PropertyType.GetGenericArguments()[0]) // Retrieves the entity type T from DbSet<T>
                .ToList();

            // Iterates through all entities in a DbSet<>
            foreach (var entityType in entityTypes)
            {
                var properties = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop =>
                    prop.PropertyType.IsEnum ||
                    (Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum ?? false)
                );

                foreach (var property in properties)
                {
                    var enumType = property.PropertyType.IsEnum
                    ? property.PropertyType
                    : Nullable.GetUnderlyingType(property.PropertyType)!;

                    // first value from enum
                    var firstEnumValue = Enum.GetValues(enumType).GetValue(0)!;

                    // Applying enum -> string conversion and limiting the length
                    var builder = modelBuilder
                        .Entity(entityType)
                        .Property(property.Name)
                        .HasConversion<string>() // Enum is stored as a string
                        .HasColumnType("varchar") // We set the type to varchar
                        .HasMaxLength(40); // We limit the length to 40 characters.

                    if (!property.PropertyType.IsGenericType ||
                        property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                    {
                        if (firstEnumValue is not null)
                        {
                            builder.HasDefaultValue(firstEnumValue);
                        }
                    }
                }
            }
        }
    }
}
