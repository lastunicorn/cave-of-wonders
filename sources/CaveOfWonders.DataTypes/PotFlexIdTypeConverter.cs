using System.ComponentModel;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.DataTypes;

/// <summary>
/// Allows infrastructure that converts values based on <see cref="TypeDescriptor"/> (CLI parameter parsing,
/// ASP.NET model binding, configuration binding) to build a <see cref="PotFlexId"/> from a string.
/// The implicit conversion operators defined on <see cref="PotFlexId"/> are invisible to such infrastructure.
/// </summary>
public class PotFlexIdTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string) || sourceType == typeof(Guid) || base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		return value switch
		{
			null => PotFlexId.Empty,
			string s => new PotFlexId(s),
			Guid g => new PotFlexId(g),
			_ => base.ConvertFrom(context, culture, value)
		};
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
			return value is PotFlexId potFlexId ? potFlexId.ToString() : null;

		return base.ConvertTo(context, culture, value, destinationType);
	}
}
