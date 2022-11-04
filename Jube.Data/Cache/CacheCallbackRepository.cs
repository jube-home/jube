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
using System.Collections.Concurrent;
using log4net;
using Npgsql;

namespace Jube.Data.Cache
{
    public class CacheCallbackRepository
    {
        private readonly string _connectionString;
        private readonly ILog _log;
        private readonly ConcurrentDictionary<Guid, Callback> _concurrentDictionary;

        public CacheCallbackRepository(string connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }
        
        public CacheCallbackRepository(string connectionString, ILog log,ConcurrentDictionary<Guid, Callback> concurrentDictionary)
        {
            _concurrentDictionary = concurrentDictionary;
            _connectionString = connectionString;
            _log = log;
        }

        private static void ManageDictionary(ConcurrentDictionary<Guid, Callback> concurrentDictionary, string value)
        {
            var splits = value.Split(",",2);
            
            if (splits.Length > 1)
            {
                var callback = new Callback
                {
                    CreatedDate = DateTime.Now,
                    Payload = splits[1]
                };

                concurrentDictionary.TryAdd(Guid.Parse(splits[0]), callback);
            }
            else
            {
                concurrentDictionary.TryRemove(Guid.Parse(splits[0]), out _);
            }
        }

        public void ListenForCallbacks()
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();

                connection.Notification += (_, e)
                    => ManageDictionary(_concurrentDictionary, e.Payload);

                using (var cmd = new NpgsqlCommand("LISTEN callback", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                while (true)
                {
                    connection.Wait();
                }
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
        
        public void Insert(byte[] json, Guid entityAnalysisModelInstanceEntryGuid)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();
                
                var sqlNotify = $"NOTIFY callback, '{entityAnalysisModelInstanceEntryGuid},{System.Text.Encoding.UTF8.GetString(json)}'";
                
                var commandNotify = new NpgsqlCommand(sqlNotify);
                commandNotify.Connection = connection;
                commandNotify.ExecuteNonQuery();
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
        
        public void Delete(Guid entityAnalysisModelInstanceEntryGuid)
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();
                
                var sqlNotify = $"NOTIFY callback, '{entityAnalysisModelInstanceEntryGuid}'";
                
                var commandNotify = new NpgsqlCommand(sqlNotify);
                commandNotify.Connection = connection;
                commandNotify.ExecuteNonQuery();
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