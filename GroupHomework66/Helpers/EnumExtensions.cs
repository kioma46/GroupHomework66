using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GroupHomework66.Helpers;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.GetName() ?? value.ToString();
    }
}