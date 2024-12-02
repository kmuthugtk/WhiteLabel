using FastStdf.IO;
using System;

namespace FastStdf.Records;

/// <summary>
/// Reads and stores string lengths for STDF records
/// </summary>
internal readonly struct StringLengthReader
{
	public byte Length1 { get; }
	public byte Length2 { get; }
	public byte Length3 { get; }
	public byte Length4 { get; }
	public byte Length5 { get; }
	public byte Length6 { get; }
	public byte Length7 { get; }
	public byte Length8 { get; }
	public byte Length9 { get; }
	public byte Length10 { get; }
	public byte Length11 { get; }
	public byte Length12 { get; }
	public byte Length13 { get; }
	public byte Length14 { get; }
	public byte Length15 { get; }
	public byte Length16 { get; }
	public byte Length17 { get; }
	public byte Length18 { get; }
	public byte Length19 { get; }
	public byte Length20 { get; }
	public byte Length21 { get; }
	public byte Length22 { get; }
	public byte Length23 { get; }
	public byte Length24 { get; }
	public byte Length25 { get; }
	public byte Length26 { get; }
	public byte Length27 { get; }
	public byte Length28 { get; }
	public byte Length29 { get; }
	public byte Length30 { get; }
	public byte Length31 { get; }
	public byte Length32 { get; }
	public byte Length33 { get; }

	public const int Size = 33;

	public StringLengthReader(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < Size)
			throw new ArgumentException($"Buffer must be at least {Size} bytes", nameof(buffer));

		Length1 = buffer[0];
		Length2 = buffer[1];
		Length3 = buffer[2];
		Length4 = buffer[3];
		Length5 = buffer[4];
		Length6 = buffer[5];
		Length7 = buffer[6];
		Length8 = buffer[7];
		Length9 = buffer[8];
		Length10 = buffer[9];
		Length11 = buffer[10];
		Length12 = buffer[11];
		Length13 = buffer[12];
		Length14 = buffer[13];
		Length15 = buffer[14];
		Length16 = buffer[15];
		Length17 = buffer[16];
		Length18 = buffer[17];
		Length19 = buffer[18];
		Length20 = buffer[19];
		Length21 = buffer[20];
		Length22 = buffer[21];
		Length23 = buffer[22];
		Length24 = buffer[23];
		Length25 = buffer[24];
		Length26 = buffer[25];
		Length27 = buffer[26];
		Length28 = buffer[27];
		Length29 = buffer[28];
		Length30 = buffer[29];
		Length31 = buffer[30];
		Length32 = buffer[31];
		Length33 = buffer[32];
	}

	//public static StringLengthReader Read(BufferReader reader)
	//{
	//	var lengths = new byte[Size];
	//	for (var i = 0; i < Size && reader.HasRemaining; i++)
	//	{
	//		lengths[i] = reader.ReadByte();
	//		reader = reader.Advance(1);
	//	}
	//	return new StringLengthReader(lengths);
	//}

	public static StringLengthReader Read(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < Size)
			throw new ArgumentException($"Buffer must be at least {Size} bytes", nameof(buffer));

		return new StringLengthReader(buffer);
	}

	public int GetTotalLength() =>
		Length1 + Length2 + Length3 + Length4 + Length5 +
		Length6 + Length7 + Length8 + Length9 + Length10 +
		Length11 + Length12 + Length13 + Length14 + Length15 +
		Length16 + Length17 + Length18 + Length19 + Length20 +
		Length21 + Length22 + Length23 + Length24 + Length25 +
		Length26 + Length27 + Length28 + Length29 + Length30 +
		Length31 + Length32 + Length33;

	public bool HasData => GetTotalLength() > 0;

	public override string ToString() =>
		$"Total Length: {GetTotalLength()}, Individual Lengths: [{Length1},{Length2},{Length3},...,{Length33}]";
}