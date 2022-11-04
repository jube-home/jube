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
using Jube.Data.Poco;
using LinqToDB;

namespace Jube.Data.Repository
{
    public class ExhaustiveSearchInstanceRepository
    {
        private readonly DbContext _dbContext;
        private readonly int? _tenantRegistryId;
        private readonly string _userName;

        public ExhaustiveSearchInstanceRepository(DbContext dbContext, string userName)
        {
            _dbContext = dbContext;
            _userName = userName;
            _tenantRegistryId = _dbContext.UserInTenant.Where(w => w.User == _userName)
                .Select(s => s.TenantRegistryId).FirstOrDefault();
        }

        public ExhaustiveSearchInstanceRepository(DbContext dbContext, int tenantRegistryId)
        {
            _dbContext = dbContext;
            _tenantRegistryId = tenantRegistryId;
        }

        public ExhaustiveSearchInstanceRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ExhaustiveSearchInstance> Get()
        {
            return _dbContext.ExhaustiveSearchInstance
                .Where(w => w.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId);
        }

        public IEnumerable<ExhaustiveSearchInstance> GetByEntityAnalysisModelId(int entityAnalysisModelId)
        {
            return _dbContext.ExhaustiveSearchInstance.Where(w =>
                (w.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                && w.EntityAnalysisModelId == entityAnalysisModelId
                && (w.Deleted == 0 || w.Deleted == null));
        }

        public ExhaustiveSearchInstance GetById(int id)
        {
            return _dbContext.ExhaustiveSearchInstance.FirstOrDefault(w =>
                (w.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                && w.Id == id && (w.Deleted == 0 || w.Deleted == null));
        }

        public ExhaustiveSearchInstance Insert(ExhaustiveSearchInstance model)
        {
            model.CreatedUser = _userName;
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;
            model.Version = 1;
            model.Guid = Guid.NewGuid();
            model.Id = _dbContext.InsertWithInt32Identity(model);
            return model;
        }

        public ExhaustiveSearchInstance Update(ExhaustiveSearchInstance model)
        {
            var existing = _dbContext.ExhaustiveSearchInstance
                .FirstOrDefault(w => w.Id
                                     == model.Id
                                     && (w.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId ||
                                         !_tenantRegistryId
                                             .HasValue)
                                     && (w.StatusId == 0 || w.StatusId == null)
                                     && (w.Deleted == 0 || w.Deleted == null)
                                     && (w.Locked == 0 || w.Locked == null));

            if (existing == null) throw new KeyNotFoundException();

            model.Version = existing.Version + 1;
            model.Guid = existing.Guid;
            model.UpdatedDate = DateTime.Now;
            model.CreatedUser = _userName;
            model.CreatedDate = DateTime.Now;
            model.Id = existing.Id;

            Delete(existing.Id);

            var id = _dbContext.InsertWithInt32Identity(model);

            model.Id = id;

            return model;
        }

        public void UpdateStatus(int id, byte statusId)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                    && d.Id == id
                    && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Set(s => s.StatusId, statusId)
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }
        
        public void UpdateCompleted(int id)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                    && d.Id == id
                    && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Set(s => s.CompletedDate, DateTime.Now)
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }

        public void UpdateModels(int id, int models)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                    && d.Id == id
                    && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.Models, models)
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }

        public void UpdateModelsSinceBest(int id, int modelsSinceBest)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                    && d.Id == id
                    && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.ModelsSinceBest, modelsSinceBest)
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }
        
        public void UpdateBestScore(int id, double score,int topologyComplexity)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
                    && d.Id == id
                    && (d.Deleted == 0 || d.Deleted == null))
                .Set(s => s.Score, score)
                .Set(s => s.TopologyComplexity, topologyComplexity)
                .Set(s => s.UpdatedDate, DateTime.Now)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }

        public void Delete(int id)
        {
            var records = _dbContext.ExhaustiveSearchInstance
                .Where(d =>
                    (d.EntityAnalysisModel.TenantRegistryId == _tenantRegistryId || !_tenantRegistryId.HasValue)
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