using System.Buffers.Binary;

namespace FastStdf.IO;

/// <summary>
/// Represents the header of an STDF record
/// </summary>
public readonly struct RecordHeader
{
	public ushort Length { get; }
	public byte RecordType { get; }
	public byte SubType { get; }

	public const int Size = 4;

	public RecordHeader(ReadOnlySpan<byte> headerBytes)
	{
		if (headerBytes.Length < Size)
			throw new ArgumentException("Header buffer too small", nameof(headerBytes));

		Length = BinaryPrimitives.ReadUInt16LittleEndian(headerBytes);
		RecordType = headerBytes[2];
		SubType = headerBytes[3];
	}

	public override string ToString() =>
		$"RecordType: {RecordType}, SubType: {SubType}, Length: {Length}";
}