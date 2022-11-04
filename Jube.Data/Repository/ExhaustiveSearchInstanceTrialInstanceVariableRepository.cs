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

using System.Collections.Generic;
using System.Linq;
using Jube.Data.Context;
using Jube.Data.Poco;
using LinqToDB;

namespace Jube.Data.Repository
{
    public class ExhaustiveSearchInstanceTrialInstanceVariableRepository
    {
        private readonly DbContext _dbContext;

        public ExhaustiveSearchInstanceTrialInstanceVariableRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ExhaustiveSearchInstanceTrialInstanceVariable Insert(ExhaustiveSearchInstanceTrialInstanceVariable model)
        {
            model.Id = _dbContext.InsertWithInt32Identity(model);
            return model;
        }

        public void DeleteAllByExhaustiveSearchInstanceTrialInstanceId(int id)
        {
            _dbContext.ExhaustiveSearchInstanceTrialInstanceVariable
                .Where(w => w.ExhaustiveSearchInstanceTrialInstanceId == id)
                .Delete();
        }

        public void UpdateAsRemovedByExhaustiveSearchInstanceVariableId(int exhaustiveSearchInstanceVariableId,int exhaustiveSearchInstanceTrialInstanceId)
        {
            var records = _dbContext.ExhaustiveSearchInstanceTrialInstanceVariable
                .Where(u =>
                    u.ExhaustiveSearchInstanceVariableId == exhaustiveSearchInstanceVariableId 
                    && u.ExhaustiveSearchInstanceTrialInstanceId == exhaustiveSearchInstanceTrialInstanceId)
                .Set(s => s.Removed, 1)
                .Update();

            if (records == 0) throw new KeyNotFoundException();
        }
    }
}