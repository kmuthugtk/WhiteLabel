using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastStdf.IO;

internal sealed class BufferManager
{
	private readonly byte[] _buffer;
	private int _position;

	public BufferManager(byte[] buffer)
	{
		_buffer = buffer;
		_position = 0;
	}

	public bool HasRemaining => _position < _buffer.Length;

	public ReadOnlySpan<byte> RemainingSpan => _buffer.AsSpan(_position);

	public void Advance(int count)
	{
		EnsureAvailable(count);
		_position += count;
	}

	public byte ReadByte()
	{
		EnsureAvailable(1);
		return _buffer[_position++];
	}

	public ushort ReadUInt16()
	{
		EnsureAvailable(sizeof(ushort));
		var value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.AsSpan(_position));
		_position += sizeof(ushort);
		return value;
	}

	public uint ReadUInt32()
	{
		EnsureAvailable(sizeof(uint));
		var value = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.AsSpan(_position));
		_position += sizeof(uint);
		return value;
	}

	public string ReadString(int length)
	{
		if (length == 0)
			return string.Empty;

		EnsureAvailable(length);
		var str = Encoding.ASCII.GetString(_buffer.AsSpan(_position, length));
		_position += length;
		return str;
	}

	private void EnsureAvailable(int count)
	{
		if (_position + count > _buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(count), "Not enough bytes remaining in buffer");
	}
}