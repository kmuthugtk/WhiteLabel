using System;
using FastStdf.Extensions;
using FastStdf.IO;

namespace FastStdf.Records;

/// <summary>
/// Part Results Record (PRR)
/// </summary>
public sealed class Prr : StdfRecord
{
	public uint HeadNumber { get; private set; }
	public uint SiteNumber { get; private set; }
	public byte PartFlag { get; private set; }
	public ushort NumTests { get; private set; }
	public byte HardBin { get; private set; }
	public byte SoftBin { get; private set; }
	public ushort PartId { get; private set; }
	public string PartText { get; private set; } = string.Empty;
	public float TestTime { get; private set; }
	public string PartFix { get; private set; } = string.Empty;

	public bool SupersedesPartId => (PartFlag & 0x01) != 0;
	public bool SupersedesTestFlags => (PartFlag & 0x02) != 0;
	public bool Failed => (PartFlag & 0x08) != 0;

	public Prr() : base(5, 20) { }

	public override void Read(ReadOnlySpan<byte> buffer)
	{
		var manager = new BufferManager(buffer.ToArray());

		HeadNumber = manager.ReadUInt32();
		SiteNumber = manager.ReadUInt32();
		PartFlag = manager.ReadByte();
		NumTests = manager.ReadUInt16();
		HardBin = manager.ReadByte();
		SoftBin = manager.ReadByte();
		PartId = manager.ReadUInt16();

		if (!manager.HasRemaining)
			return;

		var stringLengths = new byte[2];
		stringLengths[0] = manager.ReadByte(); // PartText length
		stringLengths[1] = manager.ReadByte(); // PartFix length

		if (stringLengths[0] > 0)
		{
			PartText = manager.ReadString(stringLengths[0]);
		}

		// Read TestTime as 4 bytes IEEE floating point
		if (manager.HasRemaining && manager.RemainingSpan.Length >= 4)
		{
			var testTimeBytes = manager.RemainingSpan[..4];
			TestTime = BitConverter.ToSingle(testTimeBytes);
			manager.Advance(4);
		}

		if (manager.HasRemaining && stringLengths[1] > 0)
		{
			PartFix = manager.ReadString(stringLengths[1]);
		}
	}

	public override int GetExpectedLength() => throw new NotImplementedException("Variable length record");
}