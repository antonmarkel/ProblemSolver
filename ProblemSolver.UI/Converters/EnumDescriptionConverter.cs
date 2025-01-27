using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace ProblemSolver.UI.Converters;

// Default value converter for WPF, I used it when you need to show instances of enum,
// especially I added for every instance of enum attribute - Description, so this converter shows
// only this attribute 
public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return string.Empty;
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
            return null;
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
