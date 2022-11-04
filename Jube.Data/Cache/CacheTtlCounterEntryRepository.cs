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
using System.Collections.Generic;
using log4net;
using Npgsql;

namespace Jube.Data.Cache
{
    public class CacheTtlCounterEntryRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;

        public CacheTtlCounterEntryRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public void DeleteAfterDecremented(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName, DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "delete from \"CacheTtlCounterEntry\"" +
                          " where \"EntityAnalysisModelTtlCounterId\" = (@entityAnalysisModelTtlCounterId)" +
                          " and \"DataName\" = (@dataName)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " and \"CreatedDate\" <= (@referenceDate)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("dataName", dataName);
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

        public Dictionary<string, int> GetExpiredTtlCounterCacheCounts(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName, DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = new Dictionary<string, int>();
            try
            {
                connection.Open();

                var sql = "select \"DataValue\",count(*)::int Count" +
                          " from \"CacheTtlCounterEntry\"" +
                          " where \"EntityAnalysisModelTtlCounterId\" = (@entityAnalysisModelTtlCounterId)" +
                          " and \"DataName\" = (@dataName)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " and \"CreatedDate\" <= (@referenceDate)" +
                          " group by \"DataValue\";";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Parameters.AddWithValue("referenceDate", referenceDate);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString() ?? string.Empty,
                            (int) reader.GetValue(1));
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

        public Dictionary<string, int> GetByNameDataNameDataValue(int entityAnalysisModelId,
            GetByNameDataNameDataValueParams[] getByNameDataNameDataValueParams)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var documents = new Dictionary<string, int>();
            try
            {
                connection.Open();

                var sql =
                    "select c.\"DataName\",count(c.\"DataName\")::int from \"CacheTtlCounterEntry\" c where \"EntityAnalysisModelId\" = (@entityAnalysisModelId)";

                var command = new NpgsqlCommand();

                for (var i = 0; i < getByNameDataNameDataValueParams.Length; i++)
                {
                    sql += $" and (\"DataName\" = (@dataName_{i}) " +
                           $" and \"DataValue\" = (@dataValue_{i}) " +
                           $" and \"ReferenceDate\" > (@referenceDateFrom_{i}) " +
                           $" and \"ReferenceDate\" <= (@referenceDateTo_{i}))";

                    command.Parameters.AddWithValue($"dataName_{i}", getByNameDataNameDataValueParams[i].DataName);
                    command.Parameters.AddWithValue($"dataValue_{i}", getByNameDataNameDataValueParams[i].DataValue);
                    command.Parameters.AddWithValue($"referenceDateFrom_{i}",
                        getByNameDataNameDataValueParams[i].ReferenceDateFrom);
                    command.Parameters.AddWithValue($"referenceDateTo_{i}",
                        getByNameDataNameDataValueParams[i].ReferenceDateTo);
                }

                sql += " group by c.\"DataName\"";
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.CommandText = sql;
                command.Connection = connection;
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    documents.Add(reader.GetValue(0).ToString() ?? string.Empty, (int) reader.GetValue(1));
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

            return documents;
        }

        public void Insert(int entityAnalysisModelId, string dataName, string dataValue, int entityAnalysisModelTtlCounterId,
            DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "insert into\"CacheTtlCounterEntry\"(\"EntityAnalysisModelId\",\"DataName\",\"DataValue\"," +
                          "\"EntityAnalysisModelTtlCounterId\",\"ReferenceDate\",\"CreatedDate\")" +
                          " values((@entityAnalysisModelId),(@DataName),(@DataValue),(@entityAnalysisModelTtlCounterId)," +
                          "(@ReferenceDate),(@CreatedDate))";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("dataName", dataName);
                command.Parameters.AddWithValue("dataValue", dataValue);
                command.Parameters.AddWithValue("entityAnalysisModelTtlCounterId", entityAnalysisModelTtlCounterId);
                command.Parameters.AddWithValue("referenceDate", referenceDate);
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
            }
        }

        public class GetByNameDataNameDataValueParams
        {
            public string DataName { get; set; }
            public string DataValue { get; set; }
            public DateTime ReferenceDateFrom { get; set; }
            public DateTime ReferenceDateTo { get; set; }
        }
    }
}