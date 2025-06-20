namespace PAYUNiSDK.Extensions;

internal static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
}
