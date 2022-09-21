namespace RequestsNET.NLog
{
  internal static class Utils
  {
    public static string LimitText(string text, int maxLength)
    {
      if (text.Length > maxLength - 3)
        text = text.Substring(0, maxLength - 3) + "...";
      return text;
    }
  }
}