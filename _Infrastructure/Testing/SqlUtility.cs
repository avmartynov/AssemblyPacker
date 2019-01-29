using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Data.SqlClient;
using NLog;
using Twidlle.Infrastructure.CodeAnnotation;

#pragma warning disable CA2100
namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Процедуры для прямого обращения в базу 
    /// o - для получения тестовых данных, 
    /// o - для обеспечения исходных условий тестов, 
    /// o - для проверки пост-условий тестов, 
    /// etc. </summary>
    public static class SqlUtility
    {
        /// <summary>  Возвращает первое поле первой строки результирующего набора данных как строку. </summary>
        /// <param name="connectionString">Строка соединения с базой данных.</param>
        /// <param name="sqlQueryFormat">Шаблон SQL-запроса. Первая колонка результирующего набора должна иметь строковый тип.</param>
        /// <param name="arguments">Параметры форматирования SQL-скрипта.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2100: Review SQL queries for security vulnerabilities")]
        [CanBeNull]
        public static string GetStringValue([NotNull] string connectionString, [NotNull] string sqlQueryFormat, [NotNull] params object[] arguments)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var sqlQuery = string.Format(sqlQueryFormat, arguments);
                _logger.Trace(sqlQuery);
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (!dataReader.Read())
                            throw new Exception(string.Format("Empty result set. Query:'{0}'.", sqlQuery));

                        return dataReader.IsDBNull(0) ? null : dataReader.GetString(0);
                    }
                }
            }
        }


        /// <summary>  Возвращает массив значений-строк полей первой записи (строки) результирующего набора данных. </summary>
        /// <param name="connectionString">Строка соединения с базой данных.</param>
        /// <param name="sqlQueryFormat">Шаблон SQL-запроса. Первая колонка результирующего набора должна иметь строковый тип.</param>
        /// <param name="arguments">Параметры форматирования SQL-скрипта.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2100: Review SQL queries for security vulnerabilities")]
        [NotNull]
        public static string[] GetDataRow([NotNull] string connectionString, [NotNull] string sqlQueryFormat, [NotNull] params object[] arguments)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var sqlQuery = string.Format(sqlQueryFormat, arguments);
                _logger.Trace(sqlQuery);
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (!dataReader.Read())
                            throw new Exception(string.Format("Empty result set. Query:'{0}'.", sqlQuery));

                        var result = new string[dataReader.FieldCount];
                        for (var idxField = 0; idxField < dataReader.FieldCount; idxField++)
                        {
                            if (!dataReader.IsDBNull(idxField))
                                result[idxField] = dataReader.GetString(idxField);
                        }
                        return result;
                    }
                }
            }
        }


        /// <summary>  Возвращает массив значений-строк полей первой колонки результирующего набора данных. </summary>
        /// <param name="connectionString">Строка соединения с базой данных.</param>
        /// <param name="sqlQueryFormat">Шаблон SQL-запроса. Первая колонка результирующего набора должна иметь строковый тип.</param>
        /// <param name="arguments">Параметры форматирования SQL-скрипта.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2100: Review SQL queries for security vulnerabilities")]
        [NotNull]
        public static string[] GetDataColumn([NotNull] string connectionString, [NotNull] string sqlQueryFormat, [NotNull] params object[] arguments)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var sqlQuery = string.Format(sqlQueryFormat, arguments);
                _logger.Trace(sqlQuery);
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        var list = new List<string>();

                        while(dataReader.Read())
                        {
                            var value = dataReader.GetString(0);
                            list.Add(value);
                        }

                        return list.ToArray();
                    }
                }
            }
        }


        /// <summary> Выполняет произвольный SQL-скрипт. </summary>
        /// <param name="connectionString">Строка соединения с базой данных.</param>
        /// <param name="sqlTextFormat">Шаблон SQL-скрипта.</param>
        /// <param name="arguments">Параметры форматирования SQL-скрипта.</param>
        [SuppressMessage("Microsoft.Security", "CA2100: Review SQL queries for security vulnerabilities")]
        public static void Execute([NotNull] string connectionString, [NotNull] string sqlTextFormat, [NotNull] params object[] arguments)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var sqlText = string.Format(sqlTextFormat, arguments);
                _logger.Trace(sqlText);
                using (var cmd = new SqlCommand(sqlText, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    }
}