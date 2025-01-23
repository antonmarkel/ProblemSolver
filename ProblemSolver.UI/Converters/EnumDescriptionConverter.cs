using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace ProblemSolver.UI.Converters;
public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return string.Empty; // Вернем пустую строку, если значение null
        }

        if (value is Enum enumValue)
        {
            return GetEnumDescription(enumValue);
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null; // Вернем null, если значение null
        }

        var enumType = targetType;
        if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            enumType = Nullable.GetUnderlyingType(enumType);
        }

        foreach (var field in enumType.GetFields())
        {
            var descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descriptionAttribute != null)
            {
                if (descriptionAttribute.Description == value.ToString())
                {
                    return Enum.Parse(enumType, field.Name);
                }
            }
            else
            {
                if (field.Name == value.ToString())
                {
                    return Enum.Parse(enumType, field.Name);
                }
            }
        }

        return value;
    }

    private string GetEnumDescription(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));

        return attribute == null ? value.ToString() : attribute.Description;
    }
}
