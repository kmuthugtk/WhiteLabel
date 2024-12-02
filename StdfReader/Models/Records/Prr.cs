namespace StdfReader.Models.Records;

public class Prr : StdfRecord
{
	public byte? HeadNumber { get; set; }
	public byte? SiteNumber { get; set; }
	public byte PartFlag { get; set; }
	public ushort TestCount { get; set; }
	/// <summary>
	/// While ushort, valid bins must be 0 - 32,767
	/// </summary>
	public ushort HardBin { get; set; }
	/// <summary>
	/// While ushort, valid bins must be 0 - 32,767
	/// </summary>
	public ushort? SoftBin { get; set; }
	public short? XCoordinate { get; set; }
	public short? YCoordinate { get; set; }
	public uint? TestTime { get; set; }
	public string PartId { get; set; }
	public string PartText { get; set; }
	public byte[] PartFix { get; set; }
	//dependency properties
	static readonly byte _SupersedesPartIdMask = 0x01;
	static readonly byte _SupersedesCoordsMask = 0x02;
	static readonly byte _AbnormalTestMask = 0x04;
	static readonly byte _FailedMask = 0x08;
	static readonly byte _FailFlagInvalidMask = 0x10;

	public bool SupersedesPartId
	{
		get { return (PartFlag & _SupersedesPartIdMask) != 0; }
		set
		{
			if (value) PartFlag |= _SupersedesPartIdMask;
			else PartFlag &= (byte)~_SupersedesPartIdMask;
		}
	}

	public bool SupersedesCoords
	{
		get { return (PartFlag & _SupersedesCoordsMask) != 0; }
		set
		{
			if (value) PartFlag |= _SupersedesCoordsMask;
			else PartFlag &= (byte)~_SupersedesCoordsMask;
		}
	}

	public bool AbnormalTest
	{
		get { return (PartFlag & _AbnormalTestMask) != 0; }
		set
		{
			if (value) PartFlag |= _AbnormalTestMask;
			else PartFlag &= (byte)~_AbnormalTestMask;
		}
	}

	public bool? Failed
	{
		get { return (PartFlag & _FailFlagInvalidMask) != 0 ? (bool?)null : (PartFlag & _FailedMask) != 0; }
		set
		{
			if (value == null)
			{
				PartFlag &= (byte)~_FailedMask;
				PartFlag |= _FailFlagInvalidMask;
			}
			else
			{
				PartFlag &= (byte)~_FailFlagInvalidMask;
				if ((bool)value) PartFlag |= _FailedMask;
				else PartFlag &= (byte)~_FailedMask;
			}
		}
	}

	public Prr() : base(5, 20) { }
}
