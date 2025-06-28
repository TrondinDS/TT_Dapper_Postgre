using System.Data;

namespace Test.Repositories.Generic
{
    /// <summary>
    /// Обёртка над транзакцией и соединением.
    /// Позволяет безопасно использовать using/await using.
    /// </summary>
    public class DbTransactionContext : IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }

        private bool _committedOrRolledBack = false;

        public DbTransactionContext(IDbConnection connection, IDbTransaction transaction)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Подтверждение транзакции.
        /// </summary>
        public void Commit()
        {
            if (_committedOrRolledBack) return;
            Transaction.Commit();
            _committedOrRolledBack = true;
        }

        /// <summary>
        /// Откат транзакции.
        /// </summary>
        public void Rollback()
        {
            if (_committedOrRolledBack) return;
            Transaction.Rollback();
            _committedOrRolledBack = true;
        }

        public void Dispose()
        {
            if (!_committedOrRolledBack)
            {
                try { Transaction.Rollback(); } catch { }
            }

            Transaction.Dispose();
            Connection.Dispose();
        }
    }
}
