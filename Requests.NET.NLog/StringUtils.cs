using System;
using JetBrains.Annotations;

namespace RequestsNET.NLog
{
  [PublicAPI]
  public static class StringUtils
  {
    private static readonly Random _stringRandom = new Random();

    public static string GenerateRandomString(
        int length,
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
      var stringChars = new char[length];

      lock (_stringRandom) {
        for (var i = 0; i < stringChars.Length; i++)
          stringChars[i] = chars[_stringRandom.Next(chars.Length)];
      }

      return new string(stringChars);
    }
  }
}