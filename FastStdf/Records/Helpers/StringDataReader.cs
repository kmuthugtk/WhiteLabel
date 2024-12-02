using System.Text;

namespace FastStdf.Records.Helpers;

/// <summary>
/// Handles reading string data from a buffer using string length information
/// </summary>
internal ref struct StringDataReader
{
	private readonly ReadOnlySpan<byte> _data;
	private int _position;

	public StringDataReader(ReadOnlySpan<byte> data)
	{
		_data = data;
		_position = 0;
	}

	public bool HasRemaining => _position < _data.Length;

	public ReadOnlySpan<byte> RemainingSpan => _data[_position..];

	public (string Value, bool Success) ReadString(int length)
	{
		if (length == 0)
			return (string.Empty, true);

		if (_position + length > _data.Length)
			return (string.Empty, false);

		var str = Encoding.ASCII.GetString(_data.Slice(_position, length));
		_position += length;
		return (str, true);
	}

	public string ReadStrings(int length)
	{
		if (length == 0)
			return string.Empty;

		if (_position + length > _data.Length)
			return string.Empty;

		var str = Encoding.ASCII.GetString(_data.Slice(_position, length));
		_position += length;
		return str;
	}

	public bool TryReadStrings(StringLengthReader lengths, out MirStrings strings)
	{
		strings = new MirStrings();
		try
		{
			strings.ModeCode = ReadStrings(lengths.Length1);
			strings.RetestCode = ReadStrings(lengths.Length2);
			strings.ProtectionCode = ReadStrings(lengths.Length3);
			strings.CommandModeCode = ReadStrings(lengths.Length4);
			strings.LotId = ReadStrings(lengths.Length5);
			strings.PartType = ReadStrings(lengths.Length6);
			strings.NodeName = ReadStrings(lengths.Length7);
			strings.TesterType = ReadStrings(lengths.Length8);
			strings.JobName = ReadStrings(lengths.Length9);
			strings.JobRevision = ReadStrings(lengths.Length10);
			strings.SublotId = ReadStrings(lengths.Length11);
			strings.OperatorName = ReadStrings(lengths.Length12);
			strings.ExecType = ReadStrings(lengths.Length13);
			strings.ExecVersion = ReadStrings(lengths.Length14);
			strings.TestCode = ReadStrings(lengths.Length15);
			strings.TestTemperature = ReadStrings(lengths.Length16);
			strings.UserText = ReadStrings(lengths.Length17);
			strings.AuxiliaryFile = ReadStrings(lengths.Length18);
			strings.PackageType = ReadStrings(lengths.Length19);
			strings.FamilyId = ReadStrings(lengths.Length20);
			strings.DateCode = ReadStrings(lengths.Length21);
			strings.FacilityId = ReadStrings(lengths.Length22);
			strings.FloorId = ReadStrings(lengths.Length23);
			strings.ProcessId = ReadStrings(lengths.Length24);
			strings.OperationFrequency = ReadStrings(lengths.Length25);
			strings.SpecificationName = ReadStrings(lengths.Length26);
			strings.SpecificationVersion = ReadStrings(lengths.Length27);
			strings.FlowId = ReadStrings(lengths.Length28);
			strings.DesignRevision = ReadStrings(lengths.Length29);
			strings.EngineeringId = ReadStrings(lengths.Length30);
			strings.RomCode = ReadStrings(lengths.Length31);
			strings.SerialNumber = ReadStrings(lengths.Length32);
			strings.SupervisorName = ReadStrings(lengths.Length33);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}



	public bool TryReadStrings(byte[] lengths, out MirStrings strings)
	{
		strings = new MirStrings();

		var totalLength = 0;
		foreach (var length in lengths)
			totalLength += length;

		if (_data.Length - _position < totalLength)
			return false;

		var currentIndex = 0;
		try
		{
			(strings.ModeCode, _) = ReadString(lengths[currentIndex++]);
			(strings.RetestCode, _) = ReadString(lengths[currentIndex++]);
			(strings.ProtectionCode, _) = ReadString(lengths[currentIndex++]);
			(strings.CommandModeCode, _) = ReadString(lengths[currentIndex++]);
			(strings.LotId, _) = ReadString(lengths[currentIndex++]);
			(strings.PartType, _) = ReadString(lengths[currentIndex++]);
			(strings.NodeName, _) = ReadString(lengths[currentIndex++]);
			(strings.TesterType, _) = ReadString(lengths[currentIndex++]);
			(strings.JobName, _) = ReadString(lengths[currentIndex++]);
			(strings.JobRevision, _) = ReadString(lengths[currentIndex++]);
			(strings.SublotId, _) = ReadString(lengths[currentIndex++]);
			(strings.OperatorName, _) = ReadString(lengths[currentIndex++]);
			(strings.ExecType, _) = ReadString(lengths[currentIndex++]);
			(strings.ExecVersion, _) = ReadString(lengths[currentIndex++]);
			(strings.TestCode, _) = ReadString(lengths[currentIndex++]);
			(strings.TestTemperature, _) = ReadString(lengths[currentIndex++]);
			(strings.UserText, _) = ReadString(lengths[currentIndex++]);
			(strings.AuxiliaryFile, _) = ReadString(lengths[currentIndex++]);
			(strings.PackageType, _) = ReadString(lengths[currentIndex++]);
			(strings.FamilyId, _) = ReadString(lengths[currentIndex++]);
			(strings.DateCode, _) = ReadString(lengths[currentIndex++]);
			(strings.FacilityId, _) = ReadString(lengths[currentIndex++]);
			(strings.FloorId, _) = ReadString(lengths[currentIndex++]);
			(strings.ProcessId, _) = ReadString(lengths[currentIndex++]);
			(strings.OperationFrequency, _) = ReadString(lengths[currentIndex++]);
			(strings.SpecificationName, _) = ReadString(lengths[currentIndex++]);
			(strings.SpecificationVersion, _) = ReadString(lengths[currentIndex++]);
			(strings.FlowId, _) = ReadString(lengths[currentIndex++]);
			(strings.SetupId, _) = ReadString(lengths[currentIndex++]);
			(strings.DesignRevision, _) = ReadString(lengths[currentIndex++]);
			(strings.EngineeringId, _) = ReadString(lengths[currentIndex++]);
			(strings.RomCode, _) = ReadString(lengths[currentIndex++]);
			(strings.SerialNumber, _) = ReadString(lengths[currentIndex++]);
			(strings.SupervisorName, _) = ReadString(lengths[currentIndex]);

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}


	//public bool TryReadStrings(byte[] lengths, out PrrStrings strings)
	//{
	//	strings = new PrrStrings();

	//	if (lengths.Length < 2)
	//		return false;

	//	var totalLength = lengths[0] + lengths[1];
	//	if (_data.Length - _position < totalLength)
	//		return false;

	//	try
	//	{
	//		(strings.PartText, _) = ReadString(lengths[0]);
	//		(strings.PartFix, _) = ReadString(lengths[1]);
	//		return true;
	//	}
	//	catch (Exception)
	//	{
	//		return false;
	//	}
	//}
}