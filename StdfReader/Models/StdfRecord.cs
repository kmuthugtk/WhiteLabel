namespace StdfReader.Models;

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
}
