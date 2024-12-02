using System.Buffers.Binary;
using System.Text;

namespace StdfReader.IO;

public sealed class EndianBinaryReader : IDisposable
{
	private readonly BinaryReader _reader;
	private bool _isLittleEndian;
	private bool _disposed;

	public EndianBinaryReader(Stream stream, bool isLittleEndian = true)
	{
		if(stream == null)
			throw new ArgumentNullException(nameof(stream));
		_reader = new BinaryReader(stream, System.Text.Encoding.ASCII, leaveOpen: false);
		_isLittleEndian = isLittleEndian;
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_reader.Dispose();
			_disposed = true;
		}
	}

	public byte ReadByte() => _reader.ReadByte();

	public unsafe float ReadSingle()
	{
		var data = _reader.ReadBytes(4);
		if (data.Length != 4) throw new EndOfStreamException();

		return _isLittleEndian
			? BitConverter.ToSingle(data, 0)
			: BitConverter.ToSingle(data.ReverseIfNeeded());
	}

	public uint ReadUInt32()
	{
		var data = _reader.ReadBytes(4);
		if (data.Length != 4) throw new EndOfStreamException();

		return _isLittleEndian
			? BinaryPrimitives.ReadUInt32LittleEndian(data)
			: BinaryPrimitives.ReadUInt32BigEndian(data);
	}

	public ushort ReadUInt16()
	{
		var data = _reader.ReadBytes(2);
		if (data.Length != 2) throw new EndOfStreamException();

		return _isLittleEndian
			? BinaryPrimitives.ReadUInt16LittleEndian(data)
			: BinaryPrimitives.ReadUInt16BigEndian(data);
	}

	public string ReadString(int length)
	{
		if (length == 0) return string.Empty;

		var data = _reader.ReadBytes(length);
		if (data.Length != length) throw new EndOfStreamException();

		return Encoding.ASCII.GetString(data);
	}

	public void SkipBytes(int count)
	{
		if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
		if (_reader.BaseStream.CanSeek)
		{
			_reader.BaseStream.Seek(count, SeekOrigin.Current);
		}
		else
		{
			var buffer = new byte[4096];
			while (count > 0)
			{
				var toRead = Math.Min(buffer.Length, count);
				var bytesRead = _reader.Read(buffer, 0, toRead);
				if (bytesRead == 0) throw new EndOfStreamException();
				count -= bytesRead;
			}
		}
	}

	public Stream BaseStream => _reader.BaseStream;
}

public static class ByteArrayExtensions
{
	public static byte[] ReverseIfNeeded(this byte[] data)
	{
		Array.Reverse(data);
		return data;
	}
}
