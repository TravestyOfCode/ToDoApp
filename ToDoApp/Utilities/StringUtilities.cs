namespace ToDoApp.Utilities;

public static class StringUtilities
{
    public static bool IsWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
