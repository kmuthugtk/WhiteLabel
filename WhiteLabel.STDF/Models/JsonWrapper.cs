using LinqToStdf.Records.V4;
using WhiteLabel.STDF.Models;

namespace WhiteLabel.STDF.Models;

public class JsonWrapper
{
	public MirRecord Mir { get; set; }
	public List<PrrRecord> Prr { get; set; }
}
