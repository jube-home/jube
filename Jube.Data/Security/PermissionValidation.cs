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

using System.Threading.Tasks;
using Jube.Data.Context;
using Jube.Data.Extension;
using Npgsql;

namespace Jube.Data.Security
{
    public class PermissionValidation
    {
        public async Task<PermissionValidationDto> GetPermissionsAsync(string connectionString, string userName)
        {
            var connection = new NpgsqlConnection(connectionString);
            PermissionValidationDto permissionValidationDto;
            try
            {
                await connection.OpenAsync();
                permissionValidationDto = await GetPermissionsFromDatabaseAsync(connection, userName);
            }
            catch
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }

            return permissionValidationDto;
        }

        public async Task<PermissionValidationDto> GetPermissionsAsync(DbContext dbContext, string userName)
        {
            var connection = (NpgsqlConnection) dbContext.Connection;
            return await GetPermissionsFromDatabaseAsync(connection, userName);
        }

        private async Task<bool> LandlordAsync(NpgsqlConnection connection, string userName)
        {
            var landlord = false;

            const string sqlLandlord = "select tr.\"Landlord\",tr.\"Id\" " +
                                       "from \"RoleRegistry\" rr " +
                                       "inner join \"TenantRegistry\" tr on rr.\"TenantRegistryId\" = tr.\"Id\" " +
                                       "inner join \"UserRegistry\" ur on ur.\"RoleRegistryId\" = rr.\"Id\" " +
                                       "where ur.\"Name\" = @userName " +
                                       "and (ur.\"Deleted\" = 0 or ur.\"Deleted\" IS NULL) " +
                                       "and tr.\"Active\" = 1 " +
                                       "order by tr.\"Id\"";

            var commandSqlLandlord = new NpgsqlCommand(sqlLandlord);
            commandSqlLandlord.Connection = connection;
            commandSqlLandlord.Parameters.AddWithValue("userName", userName);
            await commandSqlLandlord.PrepareAsync();

            var readerLandlord = await commandSqlLandlord.ExecuteReaderAsync();
            while (await readerLandlord.ReadAsync())
            {
                if (!readerLandlord.IsDBNull(0))
                {
                    if (readerLandlord.GetValue(0).AsShort() == 1)
                    {
                        landlord = true;
                    }
                }

                break;
            }

            await readerLandlord.CloseAsync();
            await readerLandlord.DisposeAsync();
            await readerLandlord.DisposeAsync();

            return landlord;
        }

        private async Task<PermissionValidationDto> GetPermissionsFromDatabaseAsync(NpgsqlConnection connection,
            string userName)
        {
            var permissionValidationDto = new PermissionValidationDto();

            var command = new NpgsqlCommand();
            command.Connection = connection;

            permissionValidationDto.Landlord = await LandlordAsync(connection, userName);

            if (permissionValidationDto.Landlord)
            {
                command.CommandText
                    = "select \"Id\" " +
                      "from \"PermissionSpecification\"";
            }
            else
            {
                command.CommandText
                    = "select rrp.\"PermissionSpecificationId\" " +
                      "from \"RoleRegistryPermission\" rrp " +
                      "inner join \"RoleRegistry\" rr on rrp.\"RoleRegistryId\" = rr.\"Id\" " +
                      "inner join \"UserRegistry\" ur on ur.\"RoleRegistryId\" = rr.\"Id\" " +
                      "where ur.\"Active\" = 1 " +
                      "and rr.\"Active\" = 1 " +
                      "and rrp.\"Active\" = 1 " +
                      "and (ur.\"Deleted\" = 0 or ur.\"Deleted\" IS NULL) " +
                      "and (rr.\"Deleted\" = 0 or rr.\"Deleted\" IS NULL) " +
                      "and (rrp.\"Deleted\" = 0 or rrp.\"Deleted\" IS NULL) " +
                      "and (ur.\"PasswordLocked\" = 0 or ur.\"PasswordLocked\" IS NULL) " +
                      "and ur.\"Name\" = (@userName)";

                command.Parameters.AddWithValue("userName", userName);
            }

            await command.PrepareAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) permissionValidationDto.Permissions.Add(reader.GetValue(0).AsInt());

            await reader.CloseAsync();
            await reader.DisposeAsync();
            await command.DisposeAsync();

            return permissionValidationDto;
        }
    }
}