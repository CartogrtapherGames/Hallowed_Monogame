using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hallowed.Utilities;

/// <summary>
/// A generic SmartEnum that supports any type of value (string, int, etc.)
/// </summary>
/// <typeparam name="TEnum">The specific Enum class (e.g. ScreenName)</typeparam>
/// <typeparam name="TValue">The type of the value (e.g. string, int)</typeparam>
public abstract class SmartEnum<TEnum, TValue>
  where TEnum : SmartEnum<TEnum, TValue>
  where TValue : IEquatable<TValue>
{
  public TValue Value { get; }

  protected SmartEnum(TValue value)
  {
    Value = value;
  }

  // Look up an item by its value
  public static TEnum FromValue(TValue value)
  {
    if (EnumCache.Map.TryGetValue(value, out var result))
    {
      return result;
    }
    throw new ArgumentException($"Value '{value}' not found in {typeof(TEnum).Name}");
  }

  // Safe check if value exists
  public static bool TryFromValue(TValue value, out TEnum result)
  {
    return EnumCache.Map.TryGetValue(value, out result);
  }

  // Cache logic (same as before, but generic)
  private static class EnumCache
  {
    public static readonly Dictionary<TValue, TEnum> Map = typeof(TEnum)
      .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
      .Where(f => f.FieldType == typeof(TEnum))
      .Select(f => f.GetValue(null))
      .Cast<TEnum>()
      .ToDictionary(x => x.Value);
  }

  // Standard Object overrides for easy comparison
  public override string ToString() => Value.ToString();

  public override bool Equals(object obj) => obj is TEnum other && Value.Equals(other.Value);

  public override int GetHashCode() => Value.GetHashCode();
}
