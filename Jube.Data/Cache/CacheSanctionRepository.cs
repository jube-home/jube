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
using log4net;
using Npgsql;

namespace Jube.Data.Cache
{
    public class CacheSanctionRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;

        public CacheSanctionRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public CacheSanctionDto GetByMultiPartStringDistanceThreshold(int entityAnalysisModelId, string multiPartString,
            int distanceThreshold)
        {
            var connection = new NpgsqlConnection(_connectionString);
            CacheSanctionDto value = null;
            try
            {
                connection.Open();

                var sql = "select \"Id\",\"Value\",\"CreatedDate\" from\"CacheSanction\"" +
                          " where \"MultiPartString\" = (@multiPartString)" +
                          " and \"DistanceThreshold\" = (@distanceThreshold)" +
                          " and \"EntityAnalysisModelId\" = (@entityAnalysisModelId)" +
                          " order by \"CreatedDate\" desc" +
                          " limit 1";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("multiPartString", multiPartString);
                command.Parameters.AddWithValue("distanceThreshold", distanceThreshold);
                command.Prepare();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    value = new CacheSanctionDto
                    {
                        Id = (long) reader.GetValue(0),
                        CreatedDate = Convert.ToDateTime(reader.GetValue(2))
                    };

                    if (!reader.IsDBNull(1)) value.Value = (double) reader.GetValue(1);
                }
                reader.Close();
                reader.Dispose();
                command.Dispose();

                return value;
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

        public void Insert(int entityAnalysisModelId, string multiPartString,
            int distanceThreshold, double? value)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "insert into \"CacheSanction\"(" +
                          "\"Value\",\"MultiPartString\",\"DistanceThreshold\",\"CreatedDate\"," +
                          "\"EntityAnalysisModelId\")" +
                          " values((@value),(@multiPartString),(@distanceThreshold),(@createdDate)," +
                          "(@entityAnalysisModelId))";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("value", value.HasValue ? value : DBNull.Value);
                command.Parameters.AddWithValue("multiPartString", multiPartString);
                command.Parameters.AddWithValue("distanceThreshold", distanceThreshold);
                command.Parameters.AddWithValue("createdDate", DateTime.Now);
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);

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

        public void Update(long id, double? value)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                var sql = "update \"CacheSanction\"" +
                          " set \"Value\" = (@value), " +
                          " \"CreatedDate\" = (@createdDate) " +
                          " where \"Id\" = (@Id)";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("Id", id);
                command.Parameters.AddWithValue("value", value.HasValue ? value : DBNull.Value);
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

        public class CacheSanctionDto
        {
            public long Id { get; set; }
            public DateTime CreatedDate { get; set; }
            public double? Value { get; set; }
        }
    }
}