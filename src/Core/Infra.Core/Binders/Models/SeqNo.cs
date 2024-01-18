namespace Infra.Core.Binders.Models;

public class SeqNo(string seqNo)
{
    public string Value { get; } = seqNo;
}
