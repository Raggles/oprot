using System.Windows.Controls;

namespace oprot.plot.wpf
{
    public class TimeSettingValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                if (d <= 1.0 && d >= 0.1)
                    return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Please enter a value between 0.1 and 1.0.");
        }
    }
}
