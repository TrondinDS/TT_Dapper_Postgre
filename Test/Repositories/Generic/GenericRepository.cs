using Dapper;
using System.Collections.Generic;
using System.Reflection;
using Test1.DB.DbConnectionFactory;
using static Dapper.SqlMapper;

namespace Test.Repositories.Generic
{

    /// <summary>
    /// Абстрактный базовый класс репозитория для сущностей с целочисленным ключом.
    /// Использует Dapper для простых CRUD‑операций.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// Имя таблицы в базе данных (без схемы).
        /// </summary>
        protected abstract string TableName { get; }

        /// <summary>
        /// Название поля первичного ключа.
        /// </summary>
        protected abstract string KeyName { get; }

        public GenericRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Получить все записи из таблицы.
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {TableName}";
            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.QueryAsync<TEntity>(sql);
        }

        /// <summary>
        /// Получить запись по ключу.
        /// </summary>
        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM {TableName} WHERE {KeyName} = @Id";
            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.QuerySingleOrDefaultAsync<TEntity>(sql, new { Id = id });
        }

        /// <summary>
        /// Добавить новую запись. Возвращает сгенерированный ключ (Id).
        /// </summary>
        public virtual async Task<int> AddAsync(TEntity entity)
        {
            // Собираем имена колонок и параметров через рефлексию, исключая ключ
            var props = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !string.Equals(p.Name, KeyName, StringComparison.OrdinalIgnoreCase))
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToArray();

            var columns = string.Join(", ", props.Select(p => p.Name));
            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $@"
                INSERT INTO {TableName} ({columns})
                VALUES ({parameters})
                RETURNING {KeyName};";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.ExecuteScalarAsync<int>(sql, entity);
        }

        /// <summary>
        /// Обновить существующую запись. Возвращает true, если найдена и обновлена.
        /// Использует COALESCE для мягкого обновления (если значение параметра null, колонка не меняется).
        /// </summary>
        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            // Собираем пары "Column = COALESCE(@Column, Column)", исключая ключ
            var props = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !string.Equals(p.Name, KeyName, StringComparison.OrdinalIgnoreCase))
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToArray();

            var setClause = string.Join(", ", props.Select(p =>
                $"{p.Name} = COALESCE(@{p.Name}, {p.Name})"));

            var sql = $@"
                UPDATE {TableName}
                SET {setClause}
                WHERE {KeyName} = @{KeyName}; ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var affected = await conn.ExecuteAsync(sql, entity);
            return affected > 0;
        }

        /// <summary>
        /// Удалить запись по ключу. Возвращает true, если запись была удалена.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM {TableName} WHERE {KeyName} = @Id;";
            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var affected = await conn.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
