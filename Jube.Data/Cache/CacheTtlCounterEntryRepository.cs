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
using System.Threading.Tasks;
using log4net;
using Npgsql;

namespace Jube.Data.Cache
{
    public class CacheTtlCounterEntryRepository(string connectionString, ILog log)
    {
        public async Task DeleteAfterDecrementedAsync(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName, DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(connectionString);
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
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }

        public async Task<Dictionary<string, int>> GetExpiredTtlCounterCacheCountsAsync(int entityAnalysisModelId, int entityAnalysisModelTtlCounterId,
            string dataName, DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(connectionString);
            var value = new Dictionary<string, int>();
            try
            {
                await connection.OpenAsync();

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
                await command.PrepareAsync();

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString() ?? string.Empty,
                            (int) reader.GetValue(1));
                
                await reader.CloseAsync();
                await reader.DisposeAsync();
                await command.DisposeAsync();
            }
            catch (Exception ex)
            {
                log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }

            return value;
        }

        public async Task<Dictionary<string, int>> GetByNameDataNameDataValueAsync(int entityAnalysisModelId,
            GetByNameDataNameDataValueParams[] getByNameDataNameDataValueParams)
        {
            var connection = new NpgsqlConnection(connectionString);
            var documents = new Dictionary<string, int>();
            try
            {
                await connection.OpenAsync();

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
                await command.PrepareAsync();

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    documents.Add(reader.GetValue(0).ToString() ?? string.Empty, (int) reader.GetValue(1));
                
                await reader.CloseAsync();
                await reader.DisposeAsync();
                await command.DisposeAsync();
            }
            catch (Exception ex)
            {
                log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }

            return documents;
        }

        public async Task InsertAsync(int entityAnalysisModelId, string dataName, string dataValue, int entityAnalysisModelTtlCounterId,
            DateTime referenceDate)
        {
            var connection = new NpgsqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();

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
                
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                await connection.CloseAsync();
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