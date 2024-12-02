using System;
using System.Buffers;
using System.IO.Pipelines;
using FastStdf.Records;

namespace FastStdf.IO;

public sealed class StdfReader : IDisposable
{
    private readonly PipeReader _pipeReader;
    private readonly MemoryPool<byte> _memoryPool;
    private bool _disposed;
    private const int MinimumBufferSize = RecordHeader.Size;

    public StdfReader(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _memoryPool = MemoryPool<byte>.Shared;
        var pipeOptions = new StreamPipeReaderOptions(bufferSize: MinimumBufferSize);
        _pipeReader = PipeReader.Create(stream, pipeOptions);
    }

    public async ValueTask<StdfRecord?> ReadRecordAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var result = await _pipeReader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;
            try
            {
                if (TryReadRecord(ref buffer, out var record))
                    return record;

                if (result.IsCompleted)
                {
                    if (buffer.Length > 0)
                        throw new InvalidDataException("Incomplete STDF record at end of file");
                    return null;
                }
            }
            finally
            {
                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
    }

	private bool TryReadRecord(ref ReadOnlySequence<byte> buffer, out StdfRecord? record)
	{
		record = null;

		if (buffer.Length < MinimumBufferSize)
			return false;

		var header = new RecordHeader(buffer.Slice(0, MinimumBufferSize).ToSpan());
		var totalLength = MinimumBufferSize + header.Length;

		if (buffer.Length < totalLength)
			return false;

		record = RecordFactory.Create(header.RecordType, header.SubType);
		if (record == null)
			return false;

		var data = buffer.Slice(MinimumBufferSize, header.Length);
		try
		{
			record.Read(data.ToSpan());
		}
		catch (Exception ex)
		{
			throw new InvalidDataException(
				$"Failed to read record type {header.RecordType}:{header.SubType}", ex);
		}

		buffer = buffer.Slice(totalLength);
		return true;
	}


	//private static StdfRecord? CreateRecord(byte recordType, byte subType) =>
	//       (recordType, subType) switch
	//       {
	//           (0, 10) => new Far(),
	//           (1, 10) => new Mir(),
	//           (5, 20) => new Prr(),
	//           _ => null
	//       };

	public void Dispose()
    {
        if (_disposed)
            return;

        _pipeReader.Complete();
        _disposed = true;
    }
}