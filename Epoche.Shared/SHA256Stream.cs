using System.Security.Cryptography;

namespace Epoche.Shared;
/// <summary>
/// Wraps a read-only source stream and computes the SHA256 once the stream has been completely consumed.
/// </summary>
public sealed class SHA256Stream : Stream
{
    readonly IncrementalHash IncrementalSHA256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    readonly Stream Source;

    public SHA256Stream(Stream source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    byte[]? sha256Hash;
    /// <summary>
    /// This is populated after the stream is completely consumed.
    /// </summary>
    public byte[]? SHA256Hash => sha256Hash?.ToArray();

    public override bool CanRead => Source.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => Source.Length;
    public override long Position { get => Source.Position; set => throw new NotSupportedException(); }
    public override void Flush() => Source.Flush();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Close() => Source.Close();
    public override bool CanTimeout => Source.CanTimeout;
    protected override void Dispose(bool disposing) => Source.Dispose();
    public override ValueTask DisposeAsync() => Source.DisposeAsync();
    public override int WriteTimeout { get => Source.WriteTimeout; set => Source.WriteTimeout = value; }
    public override int ReadTimeout { get => Source.ReadTimeout; set => Source.ReadTimeout = value; }
    public override Task FlushAsync(CancellationToken cancellationToken) => Source.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count)
    {
        var r = Source.Read(buffer, offset, count);
        if (r > 0)
        {
            IncrementalSHA256.AppendData(buffer, offset, r);
        }
        else
        {
            sha256Hash ??= IncrementalSHA256.GetHashAndReset();
        }
        return r;
    }

    public override int Read(Span<byte> buffer)
    {
        var r = Source.Read(buffer);
        if (r > 0)
        {
            IncrementalSHA256.AppendData(buffer[..r]);
        }
        else
        {
            sha256Hash ??= IncrementalSHA256.GetHashAndReset();
        }
        return r;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var r = await Source.ReadAsync(buffer, offset, count, cancellationToken);
        if (r > 0)
        {
            IncrementalSHA256.AppendData(buffer, offset, r);
        }
        else
        {
            sha256Hash ??= IncrementalSHA256.GetHashAndReset();
        }
        return r;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var readTask = Source.ReadAsync(buffer, cancellationToken);
        if (readTask.IsCompletedSuccessfully)
        {
            var r = readTask.Result;
            if (r > 0)
            {
                IncrementalSHA256.AppendData(buffer.Span[..r]);
            }
            else
            {
                sha256Hash ??= IncrementalSHA256.GetHashAndReset();
            }
            return new ValueTask<int>(r);
        }
        return new ValueTask<int>(ReadCoreAsync(readTask, buffer));
    }

    async Task<int> ReadCoreAsync(ValueTask<int> pendingRead, Memory<byte> buffer)
    {
        var r = await pendingRead;
        if (r > 0)
        {
            IncrementalSHA256.AppendData(buffer.Span[..r]);
        }
        else
        {
            sha256Hash ??= IncrementalSHA256.GetHashAndReset();
        }
        return r;
    }
}