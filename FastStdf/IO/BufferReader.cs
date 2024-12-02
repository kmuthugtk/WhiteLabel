using System.Buffers.Binary;
using System.Text;
using System;

namespace FastStdf.IO;

internal ref struct BufferReader
{
	private readonly ReadOnlySpan<byte> _buffer;
	private readonly int _position;

	public BufferReader(ReadOnlySpan<byte> buffer, int position = 0)
	{
		_buffer = buffer;
		_position = position;
	}

	public bool HasRemaining => _position < _buffer.Length;

	public ReadOnlySpan<byte> RemainingSpan => _buffer[_position..];

	public BufferReader Advance(int count)
	{
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative");

		if (count == 0)
			return this;

		EnsureAvailable(count);
		return new BufferReader(_buffer, _position + count);
	}

	public byte ReadByte()
	{
		EnsureAvailable(1);
		return _buffer[_position];
	}

	public ushort ReadUInt16()
	{
		EnsureAvailable(sizeof(ushort));
		return BinaryPrimitives.ReadUInt16LittleEndian(_buffer[_position..]);
	}

	public uint ReadUInt32()
	{
		EnsureAvailable(sizeof(uint));
		return BinaryPrimitives.ReadUInt32LittleEndian(_buffer[_position..]);
	}

	public string ReadString(int length)
	{
		if (length <= 0)
			return string.Empty;

		EnsureAvailable(length);
		return Encoding.ASCII.GetString(_buffer.Slice(_position, length));
	}

	private void EnsureAvailable(int count)
	{
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative");

		if (_position + count > _buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(count),
				$"Not enough bytes remaining in buffer. Required: {count}, Available: {_buffer.Length - _position}");
	}
}