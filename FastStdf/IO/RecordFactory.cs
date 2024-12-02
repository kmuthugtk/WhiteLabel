using FastStdf.Records;
using FastStdf.Records.Mir;

namespace FastStdf.IO;

/// <summary>
/// Factory class for creating STDF record instances
/// </summary>
internal static class RecordFactory
{
	public static StdfRecord? Create(byte recordType, byte subType) =>
		(recordType, subType) switch
		{
			(0, 10) => new Far(),
			(1, 10) => new Mir(),
			(5, 20) => new Prr(),
			_ => null
		};
}
