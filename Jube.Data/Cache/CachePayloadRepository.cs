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
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Jube.Data.Cache
{
    public class CachePayloadRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;

        public CachePayloadRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public void CreateIndex(string name, string date, string expression, int entityAnalysisModelId)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "create index \"" + name + "\" on \"CachePayload\"" +
                          " (\"EntityAnalysisModelId\"," + date + " DESC," + expression + ")" +
                          " where \"EntityAnalysisModelId\" = " + entityAnalysisModelId;

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
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

        public List<string> GetIndexes()
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = new List<string>();
            try
            {
                connection.Open();

                var sql = "select indexname" +
                          " from pg_indexes " +
                          " where tablename =  'CachePayload';";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString());
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

        public void Insert(int entityAnalysisModelId, Dictionary<string, object> payload,
            DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "insert into\"CachePayload\"(\"EntityAnalysisModelId\",\"Json\",\"ReferenceDate\"," +
                          "\"CreatedDate\",\"EntityAnalysisModelInstanceEntryGuid\")" +
                          " values((@entityAnalysisModelId),(@json),(@referenceDate),(@createdDate)," +
                          "(@entityAnalysisModelInstanceEntryGuid))";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("json", NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(payload));
                command.Parameters.AddWithValue("referenceDate", referenceDate);
                command.Parameters.AddWithValue("createdDate", DateTime.Now);
                command.Parameters.AddWithValue("entityAnalysisModelInstanceEntryGuid",
                    entityAnalysisModelInstanceEntryGuid);
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
        
        public void Upsert(int entityAnalysisModelId, Dictionary<string, object> payload,
            DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();
                
                var sql = "insert into\"CachePayload\"(\"EntityAnalysisModelId\",\"Json\",\"ReferenceDate\"," +
                          "\"CreatedDate\",\"EntityAnalysisModelInstanceEntryGuid\")" +
                          " values((@entityAnalysisModelId),(@json),(@referenceDate),(@createdDate)," +
                          "(@entityAnalysisModelInstanceEntryGuid)) " +
                          "ON CONFLICT (\"EntityAnalysisModelInstanceEntryGuid\") " + 
                          " DO UPDATE set \"Json\" = (@json), \"UpdatedDate\" = (@updatedDate)";
                
                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("json", NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(payload));
                command.Parameters.AddWithValue("referenceDate", referenceDate);
                command.Parameters.AddWithValue("createdDate", DateTime.Now);
                command.Parameters.AddWithValue("updatedDate", DateTime.Now);
                command.Parameters.AddWithValue("entityAnalysisModelInstanceEntryGuid",
                    entityAnalysisModelInstanceEntryGuid);
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

        public List<string> GetDistinctKeys(int entityAnalysisModelId, string key, DateTime dateFrom, DateTime dateTo)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = new List<string>();
            try
            {
                connection.Open();

                var sql = "select distinct \"Json\" ->> (@key)" +
                          " from \"CachePayload\"" +
                          " where \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " and \"CreatedDate\" > (@dateFrom)" +
                          " and \"CreatedDate\" <= (@dateTo)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("key", key);
                command.Parameters.AddWithValue("dateFrom", dateFrom);
                command.Parameters.AddWithValue("dateTo", dateTo);
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString());
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

        public List<string> GetDistinctKeys(int entityAnalysisModelId, string key, DateTime dateBefore)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = new List<string>();
            try
            {
                connection.Open();

                var sql = "select distinct \"Json\" ->> (@key)" +
                          " from \"CachePayload\"" +
                          " where \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " and \"CreatedDate\" < (@dateBefore)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("key", key);
                command.Parameters.AddWithValue("dateBefore", dateBefore);
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString());
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


        public List<string> GetDistinctKeys(int entityAnalysisModelId, string key)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var value = new List<string>();
            try
            {
                connection.Open();

                var sql = "select distinct \"Json\" ->> (@key)" +
                          " from \"CachePayload\"" +
                          " where \"EntityAnalysisModelId\" = (@entityAnalysisModelId)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("key", key);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        value.Add(reader.GetValue(0).ToString());
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

        public List<Dictionary<string, object>> GetSqlByKeyValueLimit(string sql,
            string key, string value, string order, int limit)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var documents = new List<Dictionary<string, object>>();
            try
            {
                connection.Open();

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("key", key);
                command.Parameters.AddWithValue("value", value);
                command.Parameters.AddWithValue("order", order);
                command.Parameters.AddWithValue("limit", limit);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var document = new Dictionary<string, object>();
                    for (var index = 0; index < reader.FieldCount; index++)
                        if (!reader.IsDBNull(index))
                        {
                            if (!document.ContainsKey(reader.GetName(index)))
                            {
                                document.Add(reader.GetName(index), reader.GetValue(index));       
                            }
                        }
                    documents.Add(document);
                }
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
        
        public List<Dictionary<string, object>> GetInitialCounts(int entityAnalysisModelId)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var documents = new List<Dictionary<string, object>>();
            try
            {
                connection.Open();

                var sql = "select count(*) as \"Count\",max(\"ReferenceDate\") as \"Max\"," +
                        "min(\"ReferenceDate\") as \"Min\" from \"CachePayload\" " +
                        "where \"EntityAnalysisModelId\" = (@entityAnalysisModelId)";
                
                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var document = new Dictionary<string, object>();
                    for (var index = 0; index < reader.FieldCount; index++)
                        if (!reader.IsDBNull(index))
                        {
                            if (!document.ContainsKey(reader.GetName(index)))
                            {
                                document.Add(reader.GetName(index), reader.GetValue(index));       
                            }
                        }
                    documents.Add(document);
                }
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
    }
}