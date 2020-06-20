using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YabaAPI.Abstracts
{
	public abstract class Enumeration : IComparable
	{
		public short Value { get; set; }
		public string Name { get; set; }

		protected Enumeration(short value, string name)
		{
			Value = value;
			Name = name;
		}

		public override string ToString() => Name;

		public static IEnumerable<T> GetAll<T>() where T : Enumeration
		{
			var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

			return fields.Select(f => f.GetValue(null)).Cast<T>();
		}

		public int CompareTo(object other)
		{
			return Value.CompareTo(((Enumeration)other).Value);
		}

		public override bool Equals(object? obj)
		{
			var otherValue = obj as Enumeration;

			if (otherValue == null)
				return false;

			var typeMatches = GetType().Equals(otherValue.GetType());
			var valueMatches = Value.Equals(otherValue.Value);

			return typeMatches && valueMatches;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static T FromValue<T>(short value) where T : Enumeration
		{
			var matchingItem = Parse<T, short>(value, "value", item => item.Value == value);
			return matchingItem;
		}

		public static T FromDisplayName<T>(string displayName) where T : Enumeration
		{
			var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
			return matchingItem;
		}

		#region Priv Methods
		private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
		{
			var matchingItem = GetAll<T>().FirstOrDefault(predicate);

			if (matchingItem == null)
				throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

			return matchingItem;
		}
		#endregion
	}
}
