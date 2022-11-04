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
using System.Linq;
using Jube.Data.Context;
using Jube.Data.Validation;
using Jube.Data.Poco;
using Jube.Data.Reporting;
using LinqToDB;

namespace Jube.Data.Repository
{
    public class VisualisationRegistryDatasourceRepository
    {
        private readonly DbContext _dbContext;
        private readonly int _tenantRegistryId;
        private readonly string _userName;

        public VisualisationRegistryDatasourceRepository(DbContext dbContext, string userName)
        {
            _dbContext = dbContext;
            _userName = userName;
            _tenantRegistryId = _dbContext.UserInTenant.Where(w => w.User == _userName)
                .Select(s => s.TenantRegistryId).FirstOrDefault();
        }

        public IEnumerable<VisualisationRegistryDatasource> Get()
        {
            return _dbContext.VisualisationRegistryDatasource
                .Where(w => w.VisualisationRegistry.TenantRegistryId == _tenantRegistryId
                            && (w.Deleted == 0 || w.Deleted == null));
        }

        public IEnumerable<VisualisationRegistryDatasource> GetByVisualisationRegistryId(int visualisationRegistryId)
        {
            return _dbContext.VisualisationRegistryDatasource
                .Where(w => w.VisualisationRegistry.TenantRegistryId == _tenantRegistryId
                            && w.VisualisationRegistryId == visualisationRegistryId &&
                            (w.Deleted == 0 || w.Deleted == null));
        }

        public IEnumerable<VisualisationRegistryDatasource> GetByVisualisationRegistryIdActiveOnly(int visualisationRegistryId)
        {
            return _dbContext.VisualisationRegistryDatasource
                .Where(w => w.VisualisationRegistry.TenantRegistryId == _tenantRegistryId
                            && w.VisualisationRegistryId == visualisationRegistryId
                            && w.Active == 1 &&
                            (w.Deleted == 0 || w.Deleted == null));
        }
        
        public VisualisationRegistryDatasource GetById(int id)
        {
            return _dbContext.VisualisationRegistryDatasource.FirstOrDefault(w
                => w.VisualisationRegistry.TenantRegistryId == _tenantRegistryId
                   && w.Id == id
                   && (w.Deleted == 0 || w.Deleted == null));
        }

        public VisualisationRegistryDatasource Insert(VisualisationRegistryDatasource model)
        {
            if (model.VisualisationRegistryId != null)
            {
                Dictionary<string, string> columns;
                try
                {
                    columns = ValidateSeries(_dbContext, model.VisualisationRegistryId.Value, model.Command);
                }
                catch (Exception e)
                {
                    var sqlValidationFailed = new SqlValidationFailed(e.Message);
                    throw sqlValidationFailed;
                }

                model.CreatedUser = _userName;
                model.CreatedDate = DateTime.Now;
                model.Version = 1;
                model.Id = _dbContext.InsertWithInt32Identity(model);

                FillSeries(model.Id, columns);
            }

            return model;
        }

        public VisualisationRegistryDatasource Update(VisualisationRegistryDatasource model)
        {
            if (model.VisualisationRegistryId != null)
            {
                Dictionary<string, string> columns;
                try
                {
                    columns = ValidateSeries(_dbContext, model.VisualisationRegistryId.Value, model.Command);
                }
                catch (Exception e)
                {
                    var sqlValidationFailed = new SqlValidationFailed(e.Message);
                    throw sqlValidationFailed;
                }
                
                var existing = _dbContext.VisualisationRegistryDatasource
                    .FirstOrDefault(w => w.Id
                                         == model.Id
                                         && (w.Deleted == 0 || w.Deleted == null)
                                         && (w.Locked == 0 || w.Locked == null));

                if (existing == null) throw new KeyNotFoundException();

                model.Version = existing.Version + 1;
                model.CreatedUser = _userName;
                model.CreatedDate = DateTime.Now;
                model.InheritedId = existing.Id;

                Delete(existing.Id);

                var id = _dbContext.InsertWithInt32Identity(model);
                model.Id = id;
                
                FillSeries(model.Id, columns);
            }

            return model;
        }

        private void FillSeries(int id, Dictionary<string, string> columns)
        {
            foreach (var (key, value) in columns)
            {
                var visualisationRegistryDatasourceSeries = new VisualisationRegistryDatasourceSeries
                    {
                        VisualisationRegistryDatasourceId = id,
                        Name = key
                    };

                switch (value)
                {
                    case "integer":
                    case "bigint":
                        visualisationRegistryDatasourceSeries.DataTypeId = 2;
                        break;
                    case "double precision":
                        visualisationRegistryDatasourceSeries.DataTypeId = 3;
                        break;
                    default:
                    {
                        if (value.Contains("timestamp"))
                            visualisationRegistryDatasourceSeries.DataTypeId = 4;
                        else
                            visualisationRegistryDatasourceSeries.DataTypeId = value switch
                            {
                                "smallint" => 5,
                                "double precision[]" => 6,
                                _ => value.EndsWith("[]") ? 7 : 1
                            };

                        break;
                    }
                }

                _dbContext.Insert(visualisationRegistryDatasourceSeries);
            }
        }

        private Dictionary<string, string> ValidateSeries(DbContext dbContext, int visualisationRegistryId, string sql)
        {
            var visualisationRegistryParameterRepository = new VisualisationRegistryParameterRepository(_dbContext);
            var parameters =
                visualisationRegistryParameterRepository.GetByVisualisationRegistryId(visualisationRegistryId);

            var parametersDefaultValues = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                object defaultValue = parameter.DataTypeId switch
                {
                    1 => "",
                    2 => 0,
                    3 => 0d,
                    4 => new DateTime(),
                    5 => true,
                    _ => ""
                };

                parametersDefaultValues.Add(parameter.Name.Replace(" ", "_"), defaultValue);
            }

            var postgres = new Postgres(dbContext.ConnectionString);
            return postgres.Introspect(sql, parametersDefaultValues);
        }
        
        public void Delete(int id)
        {
            var records = _dbContext.VisualisationRegistryDatasource
                .Where(d => d.VisualisationRegistry.TenantRegistryId == _tenantRegistryId
                            && d.Id == id
                            && (d.Locked == 0 || d.Locked == null)
                            && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.Deleted, Convert.ToByte(1))
                .Set(s => s.DeletedDate, DateTime.Now)
                .Set(s => s.DeletedUser, _userName)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }
    }
}