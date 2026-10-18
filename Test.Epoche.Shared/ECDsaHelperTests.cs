using System.Security.Cryptography;

namespace Epoche.Shared;
public class ECDsaHelperTests
{
    [Test]
    public async Task CreatePrivateECParameters_ArrayFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePrivateECParameters(p.D!, p.Q.X!.Concat(p.Q.Y!).ToArray(), true);
        await Assert.That(ecp.D).IsEquivalentTo(p.D!);
        await Assert.That(p.Q.X!).IsEquivalentTo(p.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(p.Q.Y!);

        ecp = ECDsaHelper.CreatePrivateECParameters([.. p.D!, .. p.Q.X!, .. p.Q.Y!], true);
        await Assert.That(ecp.D).IsEquivalentTo(p.D!);
        await Assert.That(p.Q.X!).IsEquivalentTo(p.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(p.Q.Y!);
    }

    [Test]
    public async Task CreatePrivateECParameters_SpanFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePrivateECParameters(p.D!.AsSpan(), [.. p.Q.X!, .. p.Q.Y!], true);
        await Assert.That(ecp.D).IsEquivalentTo(p.D!);
        await Assert.That(p.Q.X!).IsEquivalentTo(p.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(p.Q.Y!);

        ecp = ECDsaHelper.CreatePrivateECParameters([.. p.D!, .. p.Q.X!, .. p.Q.Y!], true);
        await Assert.That(ecp.D).IsEquivalentTo(p.D!);
        await Assert.That(p.Q.X!).IsEquivalentTo(p.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(p.Q.Y!);
    }

    [Test]
    public async Task CreatePrivateECParameters_InvalidArrayParameters_ThrowsException()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        Assert.Throws<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d, p.Q.X!.Concat(p.Q.Y!).ToArray(), true));
        Assert.Throws<Exception>(() => ECDsaHelper.CreatePrivateECParameters([.. d, .. p.Q.X!, .. p.Q.Y!], true));
    }

    [Test]
    public async Task CreatePrivateECParameters_InvalidSpanParameters_ThrowsException()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        Assert.Throws<Exception>(() => ECDsaHelper.CreatePrivateECParameters(d.AsSpan(), [.. p.Q.X!, .. p.Q.Y!], true));
        Assert.Throws<Exception>(() => ECDsaHelper.CreatePrivateECParameters([.. d, .. p.Q.X!, .. p.Q.Y!], true));
    }

    [Test]
    public async Task CreatePrivateECParameters_InvalidArrayParametersWithoutValidation_DoesNotThrow()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        ECDsaHelper.CreatePrivateECParameters(d, p.Q.X!.Concat(p.Q.Y!).ToArray(), false);
        ECDsaHelper.CreatePrivateECParameters([.. d, .. p.Q.X!, .. p.Q.Y!], false);
    }

    [Test]
    public async Task CreatePrivateECParameters_InvalidSpanParametersWithoutValidation_DoesNotThrow()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var d = p.D!.ToArray();
        d[0]++;
        ECDsaHelper.CreatePrivateECParameters(d.AsSpan(), [.. p.Q.X!, .. p.Q.Y!], false);
        ECDsaHelper.CreatePrivateECParameters([.. d, .. p.Q.X!, .. p.Q.Y!], false);
    }

    [Test]
    public async Task CreatePublicECParameters_ArrayFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePublicECParameters([.. p.Q.X!, .. p.Q.Y!]);
        await Assert.That(p.Q.X!).IsEquivalentTo(ecp.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(ecp.Q.Y!);
        Assert.Null(ecp.D);
    }

    [Test]
    public async Task CreatePublicECParameters_SpanFromExport_RecreatesParameters()
    {
        var e = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = e.ExportParameters(true);

        var ecp = ECDsaHelper.CreatePublicECParameters([.. p.Q.X!, .. p.Q.Y!]);
        await Assert.That(p.Q.X!).IsEquivalentTo(ecp.Q.X!);
        await Assert.That(p.Q.Y!).IsEquivalentTo(ecp.Q.Y!);
        Assert.Null(ecp.D);
    }
}