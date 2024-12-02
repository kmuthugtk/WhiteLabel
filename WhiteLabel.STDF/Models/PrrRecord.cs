namespace WhiteLabel.STDF.Models;

public class PrrRecord
{
	public byte? HeadNumber { get; set; }
	public byte? SiteNumber { get; set; }
	public byte PartFlag { get; set; }
	public ushort TestCount { get; set; }
	public ushort HardBin {  get; set; }
	public ushort? SoftBin { get; set; }
	public short? XCoordinate { get; set; }
	public short? YCoordinate { get; set; }
	public uint? TestTime { get; set; }
	public string PartId { get; set; }
	public string PartText { get; set; }
	public byte[] PartFix { get; set; }
	public bool AbnormalTest { get; set; }
	public bool? Failed { get; set; }
	public DateTime LastModified { get; set; }
}
