using System;

namespace FastStdf.Records;

/// <summary>
/// Base class for all STDF records
/// </summary>
public abstract class StdfRecord
{
    public byte RecordType { get; set; }
    public byte SubType { get; set; }
    public uint Length { get; set; }
    
    protected StdfRecord(byte recordType, byte subType)
    {
        RecordType = recordType;
        SubType = subType;
    }
    
    public abstract void Read(ReadOnlySpan<byte> buffer);
    public abstract int GetExpectedLength();
}