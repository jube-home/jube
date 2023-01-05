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
using FluentMigrator;

namespace Jube.Migrations.Projects;

[Migration(202305010847)]
public class ModelWizard4 : Migration
{
    public override void Up()
    {
        Insert.IntoTable("PermissionSpecification").Row(new {Id = 38, Name = "Read Write Model Wizard"});
        
        Insert.IntoTable("RoleRegistryPermission").Row(new
        {
            RoleRegistryId = 1,
            PermissionSpecificationId = 38,
            Active = 1,
            CreatedDate = DateTime.Now,
            CreatedUser = "Administrator",
            Version = 1
        });
    }

    public override void Down()
    {
        Delete.FromTable("PermissionSpecification").Row(new {Id = 38, Name = "Read Write Model Wizard"});
        
        Delete.FromTable("RoleRegistryPermission").Row(new
        {
            RoleRegistryId = 1,
            PermissionSpecificationId = 38,
            Active = 1,
            CreatedUser = "Administrator",
            Version = 1
        });
    }
}