using System.Buffers;

namespace FastStdf.IO;

internal static class BufferExtensions
{
	public static ReadOnlySpan<byte> ToSpan(this ReadOnlySequence<byte> sequence)
	{
		if (sequence.IsSingleSegment)
			return sequence.First.Span;

		var array = new byte[sequence.Length];
		sequence.CopyTo(array);
		return array;
	}
}
