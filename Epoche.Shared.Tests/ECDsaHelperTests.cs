using System;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace Epoche.Shared;
public class ECDsaHelperTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_ArrayFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePrivateECParameters(p.D!, p.Q.X!.Concat(p.Q.Y!).ToArray(), true);
        Assert.Equal(p.D!, ecp.D);
        Assert.Equal(p.Q.X!, p.Q.X!);
        Assert.Equal(p.Q.Y!, p.Q.Y!);

        ecp = ECDsaHelper.CreatePrivateECParameters(p.D!.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray(), true);
        Assert.Equal(p.D!, ecp.D);
        Assert.Equal(p.Q.X!, p.Q.X!);
        Assert.Equal(p.Q.Y!, p.Q.Y!);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_SpanFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePrivateECParameters(p.D!.AsSpan(), p.Q.X!.Concat(p.Q.Y!).ToArray().AsSpan(), true);
        Assert.Equal(p.D!, ecp.D);
        Assert.Equal(p.Q.X!, p.Q.X!);
        Assert.Equal(p.Q.Y!, p.Q.Y!);

        ecp = ECDsaHelper.CreatePrivateECParameters(p.D!.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray().AsSpan(), true);
        Assert.Equal(p.D!, ecp.D);
        Assert.Equal(p.Q.X!, p.Q.X!);
        Assert.Equal(p.Q.Y!, p.Q.Y!);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_InvalidArrayParameters_ThrowsException()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        Assert.ThrowsAny<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d, p.Q.X!.Concat(p.Q.Y!).ToArray(), true));
        Assert.ThrowsAny<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray(), true));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_InvalidSpanParameters_ThrowsException()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        Assert.ThrowsAny<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d.AsSpan(), p.Q.X!.Concat(p.Q.Y!).ToArray().AsSpan(), true));
        Assert.ThrowsAny<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray().AsSpan(), true));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_InvalidArrayParametersWithoutValidation_DoesNotThrow()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        ECDsaHelper.CreatePrivateECParameters(d, p.Q.X!.Concat(p.Q.Y!).ToArray(), false);
        ECDsaHelper.CreatePrivateECParameters(d.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray(), false);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePrivateECParameters_InvalidSpanParametersWithoutValidation_DoesNotThrow()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        ECDsaHelper.CreatePrivateECParameters(d.AsSpan(), p.Q.X!.Concat(p.Q.Y!).ToArray().AsSpan(), false);
        ECDsaHelper.CreatePrivateECParameters(d.Concat(p.Q.X!).Concat(p.Q.Y!).ToArray().AsSpan(), false);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePublicECParameters_ArrayFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePublicECParameters(p.Q.X!.Concat(p.Q.Y!).ToArray());
        Assert.Equal(ecp.Q.X!, p.Q.X!);
        Assert.Equal(ecp.Q.Y!, p.Q.Y!);
        Assert.Null(ecp.D);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreatePublicECParameters_SpanFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePublicECParameters(p.Q.X!.Concat(p.Q.Y!).ToArray().AsSpan());
        Assert.Equal(ecp.Q.X!, p.Q.X!);
        Assert.Equal(ecp.Q.Y!, p.Q.Y!);
        Assert.Null(ecp.D);
    }
}