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
    public class CacheTtlCounterRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;

        public CacheTtlCounterRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public void DecrementTtlCounterCache(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName, string dataValue, int decrement, DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "update \"CacheTtlCounter\"" +
                          " set \"Value\" = \"Value\" - (@decrement)," +
                          " \"ReferenceDate\" = (@referenceDate)" +
                          " where \"EntityAnalysisModelTtlCounterId\" = (@entityAnalysisModelTtlCounterId)" +
                          " and \"DataName\" = (@dataName)" +
                          " and \"DataValue\" = (@dataValue)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId);";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("EntityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Parameters.AddWithValue("dataValue", dataValue);
                command.Parameters.AddWithValue("decrement", decrement);
                command.Parameters.AddWithValue("referenceDate", referenceDate);
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

        public DateTime? GetMostRecentFromTtlCounterCache(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName)
        {
            var connection = new NpgsqlConnection(_connectionString);
            DateTime? value = null;
            try
            {
                connection.Open();

                var sql = "select \"ReferenceDate\" from \"CacheTtlCounter\"" +
                          " where \"EntityAnalysisModelTtlCounterId\" = (@entityAnalysisModelTtlCounterId) and \"DataName\" = (@dataName)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " order by \"ReferenceDate\" desc limit 1;";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Prepare();

                value = Convert.ToDateTime(command.ExecuteScalar());
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

        public int GetByNameDataNameDataValue(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId, string dataName, string dataValue)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = 0;
            try
            {
                connection.Open();

                var sql = "select \"Value\" from \"CacheTtlCounter\"" +
                          " where \"EntityAnalysisModelTtlCounterId\" = (@entityAnalysisModelTtlCounterId) and \"DataName\" = (@dataName) " +
                          "and \"DataValue\" = (@dataValue) and \"EntityAnalysisModelId\" = (@entityAnalysisModelId);";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Parameters.AddWithValue("dataValue", dataValue);
                command.Prepare();

                var scalarReturnValue = command.ExecuteScalar();
                if (scalarReturnValue != null) value = scalarReturnValue.AsInt();
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

        public void Upsert(int entityAnalysisModelId, string dataName, string dataValue, int entityAnalysisModelTtlCounterId,
            DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "insert into \"CacheTtlCounter\"(\"EntityAnalysisModelId\",\"DataName\",\"DataValue\"," +
                          "\"EntityAnalysisModelTtlCounterId\",\"Value\",\"ReferenceDate\",\"UpdatedDate\")" +
                          " values((@entityAnalysisModelId),(@dataName),(@dataValue)," +
                          "(@entityAnalysisModelTtlCounterId),1,(@referenceDate),(@updatedDate)) " +
                          " ON CONFLICT (\"EntityAnalysisModelId\",\"EntityAnalysisModelTtlCounterId\",\"DataName\",\"DataValue\") " +
                          " DO UPDATE set \"Value\" = \"CacheTtlCounter\".\"Value\" + 1";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Parameters.AddWithValue("dataValue", dataValue);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("referenceDate", referenceDate);
                command.Parameters.AddWithValue("updatedDate", DateTime.Now);
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
            }
        }
    }
}