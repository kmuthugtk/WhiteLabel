using StdfReader.IO;
using StdfReader.Models;
using StdfReader.Models.Records;

namespace StdfReader;

public sealed class StdfReaders : IDisposable
{
	private readonly EndianBinaryReader _reader;
	private bool _disposed;

	public StdfReaders(Stream stream, bool isLittleEndian = true)
	{
		_reader = new EndianBinaryReader(stream, isLittleEndian);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_reader.Dispose();
			_disposed = true;
		}
	}

	public IEnumerable<StdfRecord> ReadRecords()
	{

		while (true)
		{
			StdfRecord? record;
			try
			{
				record = ReadNextRecord();
			}
			catch (EndOfStreamException)
			{
				yield break;
			}
			if (record != null)
				yield return record;
		}
	}

	private StdfRecord? ReadNextRecord()
	{
		byte recordType = _reader.ReadByte();
		byte subType = _reader.ReadByte();
		uint length = _reader.ReadUInt32();

		Console.WriteLine($"Record Type: {recordType}, SubType: {subType}, Length: {length}");

		// Skip records with unreasonably large lengths
		if (length > int.MaxValue) // Example: If length exceeds 2 GB, log and skip
		{
			Console.WriteLine($"Warning: Length {length} exceeds reasonable bounds. Skipping record.");
			SkipRecord(length);
			return null;
		}

		return (recordType, subType) switch
		{
			(0, 10) => ReadFar(),
			(1, 10) => ReadMir(),
			(5, 20) => ReadPrr(),
			_ => SkipRecord(length)
		};
	}


	private Far ReadFar()
	{
		return new Far
		{
			CpuType = _reader.ReadByte(),
			StdfVersion = _reader.ReadByte()
		};
	}

	private Mir ReadMir()
	{
		return new Mir
		{
			SetupTime = DateTime.UnixEpoch.AddSeconds(_reader.ReadUInt32()),
			StartTime = DateTime.UnixEpoch.AddSeconds(_reader.ReadUInt32()),
			StationNumber = _reader.ReadByte(),
			ModeCode = _reader.ReadString(_reader.ReadByte()),
			RetestCode = _reader.ReadString(_reader.ReadByte()),
			ProtectionCode = _reader.ReadString(_reader.ReadByte()),
			BurnInTime = _reader.ReadByte(),
			CommandModeCode = _reader.ReadString(_reader.ReadByte()),
			LotId = _reader.ReadString(_reader.ReadByte()),
			PartType = _reader.ReadString(_reader.ReadByte()),
			NodeName = _reader.ReadString(_reader.ReadByte()),
			TesterType = _reader.ReadString(_reader.ReadByte()),
			JobName = _reader.ReadString(_reader.ReadByte()),
			JobRevision = _reader.ReadString(_reader.ReadByte()),
			SublotId = _reader.ReadString(_reader.ReadByte()),
			OperatorName = _reader.ReadString(_reader.ReadByte()),
			ExecType = _reader.ReadString(_reader.ReadByte()),
			ExecVersion = _reader.ReadString(_reader.ReadByte()),
			TestCode = _reader.ReadString(_reader.ReadByte()),
			TestTemperature = _reader.ReadString(_reader.ReadByte()),
			UserText = _reader.ReadString(_reader.ReadByte()),
			AuxiliaryFile = _reader.ReadString(_reader.ReadByte()),
			PackageType = _reader.ReadString(_reader.ReadByte()),
			FamilyId = _reader.ReadString(_reader.ReadByte()),
			DateCode = _reader.ReadString(_reader.ReadByte()),
			FacilityId = _reader.ReadString(_reader.ReadByte()),
			FloorId = _reader.ReadString(_reader.ReadByte()),
			ProcessId = _reader.ReadString(_reader.ReadByte()),
			OperationFrequency = _reader.ReadString(_reader.ReadByte()),
			SpecificationName = _reader.ReadString(_reader.ReadByte()),
			SpecificationVersion = _reader.ReadString(_reader.ReadByte()),
			FlowId = _reader.ReadString(_reader.ReadByte()),
			SetupId = _reader.ReadString(_reader.ReadByte()),
			DesignRevision = _reader.ReadString(_reader.ReadByte()),
			EngineeringId = _reader.ReadString(_reader.ReadByte()),
			RomCode = _reader.ReadString(_reader.ReadByte()),
			SerialNumber = _reader.ReadString(_reader.ReadByte()),
			SupervisorName = _reader.ReadString(_reader.ReadByte()),
		};
	}

	private Prr ReadPrr()
	{
		return new Prr
		{
			HeadNumber = _reader.ReadByte(),
			SiteNumber = _reader.ReadByte(),
			PartFlag = _reader.ReadByte(),
			TestCount = _reader.ReadUInt16(),
			HardBin = _reader.ReadUInt16(),
			SoftBin = _reader.ReadUInt16(),
			XCoordinate = (short?)_reader.ReadUInt16(),
			YCoordinate = (short?)_reader.ReadUInt16(),
			TestTime = _reader.ReadUInt16(),
			PartId = _reader.ReadString(_reader.ReadByte()),
			PartText = _reader.ReadString(_reader.ReadByte()),
		};
	}

	//private StdfRecord? SkipRecord(uint length)
	//{
	//	//_reader.BaseStream.Seek(length, SeekOrigin.Current);
	//	_reader.SkipBytes((int)length);
	//	return null;
	//}

	private StdfRecord? SkipRecord(uint length)
	{
		Console.WriteLine($"Skipping record with length: {length} bytes");

		if (_reader.BaseStream.CanSeek)
		{
			const int maxSeekChunk = int.MaxValue; // 2 GB chunks
			long remaining = length;

			while (remaining > 0)
			{
				long toSeek = Math.Min(maxSeekChunk, remaining);
				_reader.BaseStream.Seek(toSeek, SeekOrigin.Current);
				remaining -= toSeek;
			}
		}
		else
		{
			// For non-seekable streams, use buffered reads to skip
			const int bufferSize = 4096;
			byte[] buffer = new byte[bufferSize];
			long remaining = length;

			while (remaining > 0)
			{
				int toRead = (int)Math.Min(bufferSize, remaining);
				int bytesRead = _reader.BaseStream.Read(buffer, 0, toRead);

				if (bytesRead == 0) // End of stream reached unexpectedly
					throw new EndOfStreamException("Unable to skip bytes, end of stream reached unexpectedly.");

				remaining -= bytesRead;
			}
		}

		return null;
	}

}
