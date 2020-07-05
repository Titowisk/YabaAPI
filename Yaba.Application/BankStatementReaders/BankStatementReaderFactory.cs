using System.Collections.Generic;

namespace Yaba.Application.BankStatementReaders
{
    public static class BankStatementReaderFactory
    {
        private static Dictionary<short, IBankEstatementReader> ReaderOptions { get; } = new Dictionary<short, IBankEstatementReader>()
            {
                {237, new BradescoReader() }
            };

        public static IBankEstatementReader GetReader(short bankCode)
        {
            return ReaderOptions[bankCode];
        }
    }
}
