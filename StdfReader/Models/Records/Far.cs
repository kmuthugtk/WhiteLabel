namespace StdfReader.Models.Records;

public class Far : StdfRecord
{
	public byte CpuType { get; set; }
	public byte StdfVersion { get; set; }

	public Far() : base(0, 10) { }
}
