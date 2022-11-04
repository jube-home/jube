/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using Jube.Data.Extension;
using log4net;
using Npgsql;

namespace Jube.Data.Cache
{
    public class CacheAbstractionRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;

        public CacheAbstractionRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public void Delete(long id)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "delete \"CacheAbstraction\"" +
                          " where \"Id\" = (@Id);";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("Id", id);
                command.Prepare();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void Insert(int entityAnalysisModelId, string searchKey, string searchValue, string name, double value)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "insert into\"CacheAbstraction\"(\"EntityAnalysisModelId\",\"SearchKey\"," +
                          "\"SearchValue\",\"Name\",\"Value\",\"CreatedDate\")" +
                          " values((@entityAnalysisModelId),(@searchKey),(@searchValue)," +
                          "(@name),(@value),(@createdDate)); ";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("searchKey", searchKey);
                command.Parameters.AddWithValue("searchValue", searchValue);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("value", value);
                command.Parameters.AddWithValue("createdDate", DateTime.Now);
                command.Prepare();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void Update(long id, double value)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "update \"CacheAbstraction\"" +
                          $" set \"Value\" = {value},\"CreatedDate\" = (@createdDate)" +
                          " where \"Id\" = (@id);";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("value", value);
                command.Parameters.AddWithValue("createdDate", DateTime.Now);
                command.Prepare();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public CacheAbstractionIdValueDto GetByNameSearchNameSearchValue(int entityAnalysisModelId, string name,
            string searchKey, string searchValue)
        {
            var connection = new NpgsqlConnection(_connectionString);
            CacheAbstractionIdValueDto value = null;
            try
            {
                connection.Open();

                var sql = "select \"Id\",\"Value\" from \"CacheAbstraction\"" +
                          " where \"Name\" = (@name) and \"SearchKey\" = (@searchKey)" +
                          " and \"SearchValue\" = (@searchValue)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("searchKey", searchKey);
                command.Parameters.AddWithValue("searchValue", searchValue);
                command.Parameters.AddWithValue("name", name);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    value = new CacheAbstractionIdValueDto
                    {
                        Id = (long) reader.GetValue(0),
                        Value = (double) reader.GetValue(1)
                    };
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }

        public double GetByNameSearchNameSearchValueReturnValueOnly(int entityAnalysisModelId, string name,
            string searchKey, string searchValue)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = 0d;
            try
            {
                connection.Open();

                var sql = "select \"Value\" from \"CacheAbstraction\"" +
                          " where \"Name\" = (@name) and \"SearchKey\" = (@searchKey)" +
                          " and \"SearchValue\" = (@searchValue)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId);";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("searchKey", searchKey);
                command.Parameters.AddWithValue("searchValue", searchValue);
                command.Parameters.AddWithValue("name", name);
                command.Prepare();

                var returnScalarValue = command.ExecuteScalar();
                if (returnScalarValue != null) value = returnScalarValue.AsDouble();
            }
            catch (Exception ex)
            {
                _log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }

        public class CacheAbstractionIdValueDto
        {
            public long Id { get; set; }
            public double Value { get; set; }
        }
    }
}