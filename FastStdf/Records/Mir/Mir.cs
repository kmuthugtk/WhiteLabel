using FastStdf.Records.Mir.Models;

namespace FastStdf.Records.Mir;

/// <summary>
/// Master Information Record (MIR)
/// </summary>
public sealed class Mir : StdfRecord
{
	private readonly Models.MirData _data = new();

	public DateTime? SetupTime => _data.SetupTime;
	public DateTime? StartTime => _data.StartTime;
	public byte StationNumber => _data.StationNumber;
	public string? ModeCode => _data.ModeCode;
	public string? RetestCode => _data.RetestCode;
	public string? ProtectionCode => _data.ProtectionCode;
	public string? CommandModeCode => _data.CommandModeCode;
	public string? LotId => _data.LotId;
	public string? PartType => _data.PartType;
	public string? NodeName => _data.NodeName;
	public string? TesterType => _data.TesterType;
	public string? JobName => _data.JobName;
	public string? JobRevision => _data.JobRevision;
	public string? SublotId => _data.SublotId;
	public string? OperatorName => _data.OperatorName;
	public string? ExecType => _data.ExecType;
	public string? ExecVersion => _data.ExecVersion;
	public string? TestCode => _data.TestCode;
	public string? TestTemperature => _data.TestTemperature;
	public string? UserText => _data.UserText;
	public string? AuxiliaryFile => _data.AuxiliaryFile;
	public string? PackageType => _data.PackageType;
	public string? FamilyId => _data.FamilyId;
	public string? DateCode => _data.DateCode;
	public string? FacilityId => _data.FacilityId;
	public string? FloorId => _data.FloorId;
	public string? ProcessId => _data.ProcessId;
	public string? OperationFrequency => _data.OperationFrequency;
	public string? SpecificationName => _data.SpecificationName;
	public string? SpecificationVersion => _data.SpecificationVersion;
	public string? FlowId => _data.FlowId;
	public string? SetupId => _data.SetupId;
	public string? DesignRevision => _data.DesignRevision;
	public string? EngineeringId => _data.EngineeringId;
	public string? RomCode => _data.RomCode;
	public string? SerialNumber => _data.SerialNumber;
	public string? SupervisorName => _data.SupervisorName;

	public Mir() : base(1, 10) { }

	public override void Read(ReadOnlySpan<byte> buffer)
	{
		var reader = new MirReader(_data);
		reader.Read(buffer);
	}

	public override int GetExpectedLength() => throw new NotImplementedException("Variable length record");
}
