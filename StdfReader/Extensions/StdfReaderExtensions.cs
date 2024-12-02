using StdfReader.Models;

namespace StdfReader.Extensions;

public static class StdfReaderExtensions
{
	public static IEnumerable<T> ReadRecordsOfType<T>(this StdfReaders reader) where T : StdfRecord
	{
		return reader.ReadRecords().OfType<T>();
	}

	public static async Task ProcessRecordsParallelAsync(
		this StdfReaders reader,
		Action<StdfRecord> processRecord,
		int maxDegreeOfParallelism = -1)
	{
		var options = new ParallelOptions
		{
			MaxDegreeOfParallelism = maxDegreeOfParallelism == -1
				? Environment.ProcessorCount
				: maxDegreeOfParallelism
		};

		await Parallel.ForEachAsync(
			reader.ReadRecords(),
			options,
			async (record, token) =>
			{
				await Task.Run(() => processRecord(record), token);
			});
	}
}
