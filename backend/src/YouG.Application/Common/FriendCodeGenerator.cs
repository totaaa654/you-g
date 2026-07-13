using System.Security.Cryptography;

namespace YouG.Application.Common;

/// <summary>Generates the human-shareable "YG-XXXXXX" codes used for friend-adding (docs/03-DATABASE.md Users.FriendCode).</summary>
public static class FriendCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O/1/I — avoids visual ambiguity

    public static string Generate()
    {
        Span<char> code = stackalloc char[6];
        for (var i = 0; i < code.Length; i++)
        {
            code[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return $"YG-{new string(code)}";
    }
}
