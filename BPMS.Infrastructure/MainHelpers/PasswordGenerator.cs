using System.Security.Cryptography;
using System.Text;

namespace BPMS.Infrastructure.MainHelpers;

public static class PasswordGenerator
{
    public static bool ExcludeSymbols { get; set; }
    public static bool RepeatCharacters { get; set; }
    public static bool ConsecutiveCharacters { get; set; }
    private const int UBoundDigit = 61;
    private static RNGCryptoServiceProvider _rng;
    private static int _minSize;
    private static int _maxSize;
    private static string Exclusions { get; set; }
    public static int Minimum
    {
        get { return _minSize; }
        set
        {
            _minSize = value;
            if (Minimum > _minSize)
            {
                _minSize = Minimum;
            }
        }
    }
    public static int Maximum
    {
        get { return _maxSize; }
        set
        {
            _maxSize = value;
            if (_minSize >= _maxSize)
            {
                _maxSize = Maximum;
            }
        }
    }
    public enum PasswordMode
    {
        Number, Letters, Both
    }
    private static char[] PwdCharArray;

    private static int GetCryptographicRandomNumber(int lBound, int uBound)
    {
        uint urndnum;
        var rndnum = new Byte[4];
        if (lBound == uBound - 1)
        {
            return lBound;
        }

        var xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));

        do
        {
            _rng.GetBytes(rndnum);
            urndnum = System.BitConverter.ToUInt32(rndnum, 0);
        } while (urndnum >= xcludeRndBase);

        return (int)(urndnum % (uBound - lBound)) + lBound;
    }

    private static char GetRandomCharacter()
    {
        var upperBound = PwdCharArray.GetUpperBound(0);

        if (ExcludeSymbols == true)
            upperBound = UBoundDigit;

        var randomCharPosition = GetCryptographicRandomNumber(PwdCharArray.GetLowerBound(0), upperBound);

        var randomChar = PwdCharArray[randomCharPosition];

        return randomChar;
    }

    public static string Generate(int minimum = 6, int maximum = 7, PasswordMode mode = PasswordMode.Both)
    {
        switch (mode)
        {
            case PasswordMode.Number:
                PwdCharArray = "0123456789".ToCharArray();
                break;
            case PasswordMode.Letters:
                PwdCharArray = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
                break;
            default:
                PwdCharArray = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
                break;
        }

        ConsecutiveCharacters = false;
        RepeatCharacters = true;
        ExcludeSymbols = false;
        Minimum = minimum;
        Maximum = maximum;
        Exclusions = null;
        _rng = new RNGCryptoServiceProvider();
        var pwdLength = GetCryptographicRandomNumber(Minimum, Maximum);

        var pwdBuffer = new StringBuilder { Capacity = Maximum };

        char nextCharacter;

        var lastCharacter = nextCharacter = '\n';

        for (int i = 0; i < pwdLength; i++)
        {
            nextCharacter = GetRandomCharacter();

            if (ConsecutiveCharacters == false)
            {
                while (lastCharacter == nextCharacter)
                {
                    nextCharacter = GetRandomCharacter();
                }
            }

            if (RepeatCharacters == false)
            {
                var temp = pwdBuffer.ToString();
                var duplicateIndex = temp.IndexOf(nextCharacter);
                while (-1 != duplicateIndex)
                {
                    nextCharacter = GetRandomCharacter();
                    duplicateIndex = temp.IndexOf(nextCharacter);
                }
            }

            if ((Exclusions != null))
            {
                while (-1 != Exclusions.IndexOf(nextCharacter))
                {
                    nextCharacter = GetRandomCharacter();
                }
            }

            pwdBuffer.Append(nextCharacter);
            lastCharacter = nextCharacter;
        }

        {
            return pwdBuffer.ToString();
        }
    }

}