using System.Security.Cryptography;

namespace Epoche.Shared;
public static class ECDsaHelper
{
    public static ECParameters CreatePrivateECParameters(byte[] combinedKey, bool validateKey = true) =>
        combinedKey is null
        ? throw new ArgumentNullException(nameof(combinedKey))
        : CreatePrivateECParameters((ReadOnlySpan<byte>)combinedKey, validateKey);

    public static ECParameters CreatePrivateECParameters(ReadOnlySpan<byte> combinedKey, bool validateKey = true)
    {
        if (combinedKey.Length != 96)
        {
            throw new ArgumentNullException(nameof(combinedKey));
        }

        return CreatePrivateECParameters(combinedKey[..32], combinedKey[32..], validateKey);
    }

    public static ECParameters CreatePrivateECParameters(byte[] privateKey, byte[] publicKey, bool validateKey = true) =>
        privateKey is null
        ? throw new ArgumentNullException(nameof(privateKey))
        : publicKey is null
        ? throw new ArgumentNullException(nameof(publicKey))
        : CreatePrivateECParameters((ReadOnlySpan<byte>)privateKey, (ReadOnlySpan<byte>)publicKey, validateKey);

    public static ECParameters CreatePrivateECParameters(ReadOnlySpan<byte> privateKey, ReadOnlySpan<byte> publicKey, bool validateKey = true)
    {
        if (privateKey.Length != 32)
        {
            throw new ArgumentOutOfRangeException(nameof(privateKey));
        }

        var p = CreatePublicECParameters(publicKey);
        p.D = privateKey.ToArray();
        p.Validate();
        if (validateKey)
        {
            using var ec = ECDsa.Create(p);
            try
            {
                Span<byte> testhash = stackalloc byte[32];
                Span<byte> testsig = stackalloc byte[64];
                RandomHelper.GetRandomBytes(testhash);
                if (!ec.TrySignHash(testhash, testsig, out var size) ||
                    size != 64 ||
                    !ec.VerifyHash(testhash, testsig))
                {
                    throw new ArgumentOutOfRangeException(nameof(publicKey));
                }
            }
            catch (Exception e)
            {
                throw new ArgumentOutOfRangeException(nameof(publicKey), e);
            }
        }

        return p;
    }

    public static ECParameters CreatePublicECParameters(byte[] publicKey) =>
        publicKey is null
        ? throw new ArgumentNullException(nameof(publicKey))
        : CreatePublicECParameters((ReadOnlySpan<byte>)publicKey);

    public static ECParameters CreatePublicECParameters(ReadOnlySpan<byte> publicKey)
    {
        if (publicKey.Length != 64)
        {
            throw new ArgumentOutOfRangeException(nameof(publicKey));
        }

        var p = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q =
                {
                    X = publicKey[..32].ToArray(),
                    Y = publicKey[32..].ToArray()
                }
        };
        p.Validate();
        return p;
    }
}