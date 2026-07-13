using System.Security.Cryptography;

namespace YouG.Application.Common;

/// <summary>Generates group invite-link codes (docs/03-DATABASE.md GroupInviteLinks.Code, varchar(12)).</summary>
public static class InviteCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O/1/I

    public static string Generate()
    {
        Span<char> code = stackalloc char[8];
        for (var i = 0; i < code.Length; i++)
        {
            code[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(code);
    }
}
