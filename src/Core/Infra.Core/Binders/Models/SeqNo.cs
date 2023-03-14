namespace Infra.Core.Binders.Models;

public class SeqNo
{
    public string Value { get; }

    public SeqNo(string seqNo) => Value = seqNo;
}
