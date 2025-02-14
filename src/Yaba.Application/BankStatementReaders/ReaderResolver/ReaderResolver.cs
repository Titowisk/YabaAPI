using System;
using Yaba.Domain.Models.BankAccounts.Enumerations;

namespace Yaba.Application.BankStatementReaders.ReaderResolver
{
    public class ReaderResolver : IReaderResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ReaderResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IBankEstatementReader GetBankEstatementReader(BankCode bank)
        {
            if (bank.Name == BankCode.BRADESCO.Name)
                return (IBankEstatementReader)_serviceProvider.GetService(typeof(BradescoReader));

            if (bank.Name == BankCode.NUBANK.Name)
                return (IBankEstatementReader)_serviceProvider.GetService(typeof(NuBankReader));
            //if (readerName.Name == BankCode.ITAU.Name)
            //    return (IBankEstatementReader)_serviceProvider.GetService(typeof(ItauReader));

            //if (readerName.Name == BankCode.BANCODOBRASIL.Name)
            //    return (IBankEstatementReader)_serviceProvider.GetService(typeof(BancoDoBrasilReader));

            throw new ArgumentException($"{bank.Name} is not supported yet.");
        }

        /*NOTES:
            - Using factory method is possible to use one abstraction create different implementations of it
            - In the Dependency Injection the factory method should be injected along with each implementation needed
        https://stackoverflow.com/questions/39072001/dependency-injection-resolving-by-name
        */
    }
}
