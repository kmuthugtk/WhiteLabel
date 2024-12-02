using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastStdf.Extensions;

public static class SpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16(this ReadOnlySpan<byte> span, ref int offset)
    {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(offset));
        offset += sizeof(ushort);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32(this ReadOnlySpan<byte> span, ref int offset)
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(offset));
        offset += sizeof(uint);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadString(this ReadOnlySpan<byte> span, ref int offset, int length)
    {
        var str = Encoding.ASCII.GetString(span.Slice(offset, length));
        offset += length;
        return str;
    }
}