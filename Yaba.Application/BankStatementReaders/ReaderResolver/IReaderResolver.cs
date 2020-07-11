using Yaba.Domain.Models.BankAccounts.Enumerations;

namespace Yaba.Application.BankStatementReaders.ReaderResolver
{
    public interface IReaderResolver
    {
        IBankEstatementReader GetBankEstatementReader(BankCode readerName);
    }
}
