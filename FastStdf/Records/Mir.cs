using System;
using System.Buffers.Binary;
using FastStdf.Extensions;
using FastStdf.IO;
using FastStdf.Records.Helpers;

namespace FastStdf.Records;

/// <summary>
/// Master Information Record (MIR)
/// </summary>
public sealed class Mir : StdfRecord
{
	// Properties remain the same...
	public DateTime? SetupTime { get; private set; }
	public DateTime? StartTime { get; private set; }
	public byte StationNumber { get; private set; }
	public string? ModeCode { get; private set; }
	public string? RetestCode { get; private set; }
	public string? ProtectionCode { get; private set; }
	public ushort? BurnInTime { get; private set; }
	public string? CommandModeCode { get; private set; }
	public string LotId { get; private set; } = string.Empty;
	public string PartType { get; private set; } = string.Empty;
	public string NodeName { get; private set; } = string.Empty;
	public string TesterType { get; private set; } = string.Empty;
	public string? JobName { get; private set; }
	public string? JobRevision { get; private set; }
	public string? SublotId { get; private set; }
	public string? OperatorName { get; private set; }
	public string? ExecType { get; private set; }
	public string? ExecVersion { get; private set; }
	public string? TestCode { get; private set; }
	public string? TestTemperature { get; private set; }
	public string? UserText { get; private set; }
	public string? AuxiliaryFile { get; private set; }
	public string? PackageType { get; private set; }
	public string? FamilyId { get; private set; }
	public string? DateCode { get; private set; }
	public string? FacilityId { get; private set; }
	public string? FloorId { get; private set; }
	public string? ProcessId { get; private set; }
	public string? OperationFrequency { get; private set; }
	public string? SpecificationName { get; private set; }
	public string? SpecificationVersion { get; private set; }
	public string? FlowId { get; private set; }
	public string? SetupId { get; private set; }
	public string? DesignRevision { get; private set; }
	public string? EngineeringId { get; private set; }
	public string? RomCode { get; private set; }
	public string? SerialNumber { get; private set; }
	public string? SupervisorName { get; private set; }

	public Mir() : base(1, 10) { }

	public override void Read(ReadOnlySpan<byte> buffer)
	{
		try
		{
			//var reader = new BufferReader(buffer);

			var offset = 0;
			// Read fixed-Length fields
			var setupTimeBytes = buffer.Slice(offset, 4); // Extract 4 bytes
			long setupTimeSeconds = BinaryPrimitives.ReadInt32LittleEndian(setupTimeBytes); // Convert to long
			SetupTime = DateTimeOffset.FromUnixTimeSeconds(setupTimeSeconds).DateTime;
			offset += 4;

			var startTimeBytes = buffer.Slice(offset, 4); // Extract 4 bytes
			long startTimeSeconds = BinaryPrimitives.ReadInt32LittleEndian(startTimeBytes); // Convert to long
			StartTime = DateTimeOffset.FromUnixTimeSeconds(setupTimeSeconds).DateTime;
			offset += 4;
			StationNumber = buffer[offset++];

			if (offset >= buffer.Length)
				return;

			// Read string lengths
			var stringLengths = new StringLengthReader(buffer[offset..]);
			offset += StringLengthReader.Size;

			if (offset >= buffer.Length)
				return;

			// Read strings
			var stringData = new StringDataReader(buffer[offset..]);
			if (stringData.TryReadStrings(stringLengths, out var strings))
			{
				ModeCode = strings.ModeCode;
				RetestCode = strings.RetestCode;
				ProtectionCode = strings.ProtectionCode;
				CommandModeCode = strings.CommandModeCode;
				LotId = strings.LotId;
				PartType = strings.PartType;
				NodeName = strings.NodeName;
				TesterType = strings.TesterType;
				JobName = strings.JobName;
				JobRevision = strings.JobRevision;
				SublotId = strings.SublotId;
				OperatorName = strings.OperatorName;
				ExecType = strings.ExecType;
				ExecVersion = strings.ExecVersion;
				TestCode = strings.TestCode;
				TestTemperature = strings.TestTemperature;
				UserText = strings.UserText;
				AuxiliaryFile = strings.AuxiliaryFile;
				PackageType = strings.PackageType;
				FamilyId = strings.FamilyId;
				DateCode = strings.DateCode;
				FacilityId = strings.FacilityId;
				FloorId = strings.FloorId;
				ProcessId = strings.ProcessId;
				OperationFrequency = strings.OperationFrequency;
				SpecificationName = strings.SpecificationName;
				SpecificationVersion = strings.SpecificationVersion;
				FlowId = strings.FlowId;
				SetupId = strings.SetupId;
				DesignRevision = strings.DesignRevision;
				EngineeringId = strings.EngineeringId;
				RomCode = strings.RomCode;
				SerialNumber = strings.SerialNumber;
				SupervisorName = strings.SupervisorName;
			}
		}
		catch (ArgumentOutOfRangeException ex)
		{
			throw new InvalidDataException($"Failed to read MIR record: {ex.Message}", ex);
		}
	}

	public override int GetExpectedLength() => throw new NotImplementedException("Variable length record");
}