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
using FluentMigrator.Runner;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Reporting;
using Jube.Data.Repository;

namespace Jube.Data.Query
{
    public class GetByVisualisationRegistryDatasourceCommandExecutionQuery
    {
        private readonly DbContext _dbContext;
        private readonly string _userName;

        public GetByVisualisationRegistryDatasourceCommandExecutionQuery(DbContext dbContext, string user)
        {
            _dbContext = dbContext;
            _userName = user;
        }

        public dynamic Execute(int id, Dictionary<int, object> parametersById)
        {
            var values = new List<IDictionary<string, object>>();
            var visualisationRegistryDatasourceRepository =
                new VisualisationRegistryDatasourceRepository(_dbContext, _userName);
            var visualisationRegistryDatasource = visualisationRegistryDatasourceRepository.GetById(id);

            var visualisationRegistryParameterRepository =
                new VisualisationRegistryParameterRepository(_dbContext, _userName);
            if (visualisationRegistryDatasource.VisualisationRegistryId != null)
            {
                var visualisationRegistryParameter
                    = visualisationRegistryParameterRepository
                        .GetByVisualisationRegistryId(visualisationRegistryDatasource.VisualisationRegistryId.Value);

                var parametersByName = visualisationRegistryParameter.ToDictionary(
                    parameter => parameter.Name.Replace(" ", "_"), parameter => parametersById.ContainsKey(parameter.Id)
                        ? parametersById[parameter.Id]
                        : parameter.DefaultValue);

                var sw = new StopWatch();
                sw.Start();

                string error = default;
                try
                {
                    var postgres = new Postgres(_dbContext.ConnectionString);
                    values = postgres.ExecuteByNamedParameters(visualisationRegistryDatasource.Command,
                        parametersByName);
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }

                sw.Stop();

                var visualisationRegistryDatasourceExecutionLog =
                    new VisualisationRegistryDatasourceExecutionLog
                    {
                        Records = values.Count,
                        Error = error,
                        ResponseTime = sw.ElapsedTime().Milliseconds,
                        VisualisationRegistryDatasourceId = visualisationRegistryDatasource.Id,
                        CreatedDate = DateTime.Now,
                        CreatedUser = _userName
                    };

                var visualisationRegistryDatasourceExecutionLogRepository
                    = new VisualisationRegistryDatasourceExecutionLogRepository(_dbContext);

                visualisationRegistryDatasourceExecutionLog =
                    visualisationRegistryDatasourceExecutionLogRepository.Insert(
                        visualisationRegistryDatasourceExecutionLog);

                var visualisationRegistryDatasourceExecutionLogParameterRepository
                    = new VisualisationRegistryDatasourceExecutionLogParameterRepository(_dbContext);

                foreach (var visualisationRegistryDatasourceExecutionLogParameter in parametersById.Select(parameter =>
                    new VisualisationRegistryDatasourceExecutionLogParameter
                    {
                        Value = parameter.Value.ToString(),
                        VisualisationRegistryDatasourceExecutionLogId = visualisationRegistryDatasourceExecutionLog.Id,
                        VisualisationRegistryParameterId = parameter.Key
                    }))
                    visualisationRegistryDatasourceExecutionLogParameterRepository
                        .Insert(visualisationRegistryDatasourceExecutionLogParameter);
            }

            return values;
        }
    }
}