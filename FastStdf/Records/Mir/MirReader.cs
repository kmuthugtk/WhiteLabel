using FastStdf.Records.Helpers;

namespace FastStdf.Records.Mir;

internal sealed class MirReader
{
	private readonly Models.MirData _data;

	public MirReader(Models.MirData data)
	{
		_data = data;
	}

	public void Read(ReadOnlySpan<byte> buffer)
	{
		var offset = 0;

		ReadFixedFields(buffer, ref offset);

		if (offset >= buffer.Length)
			return;

		var stringLengths = new StringLengthReader(buffer[offset..]);
		offset += StringLengthReader.Size;

		if (offset >= buffer.Length)
			return;

		ReadStringFields(buffer[offset..], stringLengths);
	}

	private void ReadFixedFields(ReadOnlySpan<byte> buffer, ref int offset)
	{
		var setupTimeValue = BitConverter.ToUInt32(buffer.Slice(offset, 4));
		offset += 4;
		var startTimeValue = BitConverter.ToUInt32(buffer.Slice(offset, 4));
		offset += 4;
		_data.StationNumber = buffer[offset++];

		_data.SetupTime = setupTimeValue > 0 ? DateTimeOffset.FromUnixTimeSeconds(setupTimeValue).DateTime : null;
		_data.StartTime = startTimeValue > 0 ? DateTimeOffset.FromUnixTimeSeconds(startTimeValue).DateTime : null;
	}

	private void ReadStringFields(ReadOnlySpan<byte> buffer, StringLengthReader lengths)
	{
		var stringData = new StringDataReader(buffer);
		if (!stringData.TryReadStrings(lengths, out var strings))
			return;

		_data.ModeCode = strings.ModeCode;
		_data.RetestCode = strings.RetestCode;
		_data.ProtectionCode = strings.ProtectionCode;
		_data.CommandModeCode = strings.CommandModeCode;
		_data.LotId = strings.LotId;
		_data.PartType = strings.PartType;
		_data.NodeName = strings.NodeName;
		_data.TesterType = strings.TesterType;
		_data.JobName = strings.JobName;
		_data.JobRevision = strings.JobRevision;
		_data.SublotId = strings.SublotId;
		_data.OperatorName = strings.OperatorName;
		_data.ExecType = strings.ExecType;
		_data.ExecVersion = strings.ExecVersion;
		_data.TestCode = strings.TestCode;
		_data.TestTemperature = strings.TestTemperature;
		_data.UserText = strings.UserText;
		_data.AuxiliaryFile = strings.AuxiliaryFile;
		_data.PackageType = strings.PackageType;
		_data.FamilyId = strings.FamilyId;
		_data.DateCode = strings.DateCode;
		_data.FacilityId = strings.FacilityId;
		_data.FloorId = strings.FloorId;
		_data.ProcessId = strings.ProcessId;
		_data.OperationFrequency = strings.OperationFrequency;
		_data.SpecificationName = strings.SpecificationName;
		_data.SpecificationVersion = strings.SpecificationVersion;
		_data.FlowId = strings.FlowId;
		_data.SetupId = strings.SetupId;
		_data.DesignRevision = strings.DesignRevision;
		_data.EngineeringId = strings.EngineeringId;
		_data.RomCode = strings.RomCode;
		_data.SerialNumber = strings.SerialNumber;
		_data.SupervisorName = strings.SupervisorName;
	}
}