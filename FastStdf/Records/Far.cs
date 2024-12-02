using System;

namespace FastStdf.Records;

/// <summary>
/// File Attributes Record (FAR)
/// </summary>
public sealed class Far : StdfRecord
{
    public byte CpuType { get; private set; }
    public byte StdfVersion { get; private set; }

    public Far() : base(0, 10) { }

    public override void Read(ReadOnlySpan<byte> buffer)
    {
        CpuType = buffer[0];
        StdfVersion = buffer[1];
    }

    public override int GetExpectedLength() => 2;
}