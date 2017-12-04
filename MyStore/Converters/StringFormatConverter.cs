using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyStore.Converters
{
    /// <summary>
    /// Formats a string using StringFormat property or "parameter" paramater
    /// </summary>
    /// <example>
    /// <Page.Resources>
    ///     <local:StringFormatConverter x:Key="StringFormatConverter"/>
    ///     <local:StringFormatConverter x:Key="DateStringFormatConverter" StringFormat="\{0:dd/MM/yyyy\}"/>
    /// </Page.Resources>
    /// <TextBlock Text="{x:Bind Date, Converter={StaticResource StringFormatConverter}}, ConverterParameter='\{0:dd/MM/yyyy\}'"/>
    /// <TextBlock Text="{x:Bind Date, Converter={StaticResource DateStringFormatConverter}}"/>
    /// </example>
    public sealed class StringFormatConverter : IValueConverter
    {
        public string StringFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = (string.IsNullOrEmpty(StringFormat)) ? parameter as string : StringFormat;

            if (value == null || string.IsNullOrEmpty(format))
                return value;

            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
