using System.Transactions;

namespace PersonnelManagement.IntegrationTest.Infrastructure
{
    public class DatabaseFixture : IDisposable
    {
        private readonly TransactionScope _transactionScope;

        public DatabaseFixture()
        {
            _transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled
            );
        }

        public virtual void Dispose()
        {
            _transactionScope?.Dispose();
        }
    }
}