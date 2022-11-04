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
using System.Dynamic;
using System.Linq;
using Jube.Data.Extension;
using Newtonsoft.Json;
using Npgsql;

namespace Jube.Data.Reporting
{
    public class Postgres
    {
        private readonly string _connectionString;

        public Postgres(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Dictionary<string, string> Introspect(string sql, Dictionary<string, object> parameters)
        {
            var connection = new NpgsqlConnection(_connectionString);
            var values = new Dictionary<string, string>();
            try
            {
                connection.Open();

                var tableName = "Temp_" + Guid.NewGuid().ToString("N");

                var wrapSql = $"select * into TEMPORARY TABLE {tableName} from (select * from ({sql}) b LIMIT 0) c";
                var commandTempTable = new NpgsqlCommand(wrapSql);
                commandTempTable.Connection = connection;

                foreach (var (key, value) in parameters.Where(parameter => sql.Contains("@" + parameter.Key)))
                    commandTempTable.Parameters.AddWithValue(key, value);

                commandTempTable.ExecuteNonQuery();

                var introspectionSql = "SELECT attname, format_type(atttypid, atttypmod) AS type" +
                                       " FROM pg_attribute" +
                                       $" WHERE attrelid = '{tableName}'::regclass" +
                                       " AND attnum > 0 " +
                                       " AND NOT attisdropped " +
                                       " ORDER BY attnum";


                var command = new NpgsqlCommand(introspectionSql);
                command.Connection = connection;

                var reader = command.ExecuteReader();
                while (reader.Read()) values.Add(reader.GetValue(0).AsString(), reader.GetValue(1).AsString());
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return values;
        }

        public void Prepare(string sql, List<object> parameters)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;

                for (var i = 0; i < parameters.Count; i++)
                    command.Parameters.AddWithValue("@" + (i + 1), parameters[i]);

                command.Prepare();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public List<string> ExecuteReturnOnlyJsonFromArchiveSample(int entityAnalysisModelId,
            string filterSql,
            string filterTokens,
            int limit,bool mockData)
        {
            var value = new List<string>();
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var tokens = JsonConvert.DeserializeObject<List<object>>(filterTokens);
                tokens.Add(entityAnalysisModelId);
                tokens.Add(limit);

                var tableName = mockData ? "MockArchive" : "Archive";
                
                var sql = $"select \"Json\" from \"{tableName}\" where \"EntityAnalysisModelId\" = (@{tokens.Count - 1})"
                          + " and " + filterSql
                          + $" order by \"EntityAnalysisModelInstanceEntryGuid\" limit (@{limit})";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;

                for (var i = 0; i < tokens.Count; i++) command.Parameters.AddWithValue("@" + (i + 1), tokens[i]);

                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read()) value.Add(reader.GetValue(0).AsString());
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }
        
        public List<IDictionary<string, object>> ExecuteReturnPayloadFromArchiveWithSkipLimit(string sql,
            DateTime adjustedStartDate,
            int skip,
            int limit)
        {
            var value = new List<IDictionary<string, object>>();
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var command = new NpgsqlCommand(sql);
                command.Connection = connection; 
                command.Parameters.AddWithValue("adjustedStartDate", adjustedStartDate);
                command.Parameters.AddWithValue("limit", limit);
                command.Parameters.AddWithValue("skip", skip);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var eo = new ExpandoObject() as IDictionary<string, object>;
                    for (var index = 0; index < reader.FieldCount; index++)
                        if (!eo.ContainsKey(reader.GetName(index)))
                        {
                            eo.Add(reader.GetName(index), reader.IsDBNull(index) ? null : reader.GetValue(index));   
                        }
                    value.Add(eo);
                }
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }

        public List<IDictionary<string, object>> ExecuteByNamedParameters(string sql,
            Dictionary<string, object> parameters)
        {
            var value = new List<IDictionary<string, object>>();
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;

                foreach (var (key, o) in parameters)
                    command.Parameters.AddWithValue("@" + key, o);

                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var eo = new ExpandoObject() as IDictionary<string, object>;
                    for (var index = 0; index < reader.FieldCount; index++)
                        if (!eo.ContainsKey(reader.GetName(index)))
                        {
                            eo.Add(reader.GetName(index), reader.IsDBNull(index) ? null : reader.GetValue(index));   
                        }
                    value.Add(eo);
                }
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }

        public List<IDictionary<string, object>> ExecuteByOrderedParameters(string sql, List<object> parameters)
        {
            var value = new List<IDictionary<string, object>>();
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;

                for (var i = 0; i < parameters.Count; i++)
                    command.Parameters.AddWithValue("@" + (i + 1), parameters[i]);

                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var eo = new ExpandoObject() as IDictionary<string, object>;
                    for (var index = 0; index < reader.FieldCount; index++)
                        if (!eo.ContainsKey(reader.GetName(index)))
                        {
                            eo.Add(reader.GetName(index), reader.IsDBNull(index) ? null : reader.GetValue(index));   
                        }
                    value.Add(eo);
                }
                reader.Close();
                reader.Dispose();
                command.Dispose();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return value;
        }
    }
}