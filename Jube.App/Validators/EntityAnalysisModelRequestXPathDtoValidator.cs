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
using FluentValidation;
using Jube.App.Dto;

namespace Jube.App.Validators
{
    public class EntityAnalysisModelRequestXPathDtoValidator : AbstractValidator<EntityAnalysisModelRequestXPathDto>
    {
        public EntityAnalysisModelRequestXPathDtoValidator()
        {
            RuleFor(p => p.EntityAnalysisModelId).GreaterThan(0);
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            RuleFor(p => p.EnableSuppression).NotNull();
            RuleFor(p => p.Name).NotEmpty();

            var dataTypes = new List<int> {1, 2, 3, 4, 5, 6, 7};
            RuleFor(p => p.DataTypeId).Must(m => dataTypes.Contains(m));
            RuleFor(p => p.XPath).NotEmpty();
            RuleFor(p => p.SearchKey).NotNull();
            RuleFor(p => p.SearchKeyCache).NotNull();

            var intervalTypes = new List<string> {"s", "n", "h", "d"};
            RuleFor(p => p.SearchKeyCacheInterval).Must(m => intervalTypes.Contains(m));
            RuleFor(p => p.SearchKeyCacheTtlInterval).Must(m => intervalTypes.Contains(m));
            RuleFor(p => p.SearchKeyCacheValue).GreaterThanOrEqualTo(0);
            RuleFor(p => p.SearchKeyCacheSample).NotNull();
            RuleFor(p => p.SearchKeyCacheFetchLimit).GreaterThanOrEqualTo(0);
            RuleFor(p => p.SearchKeyCacheTtlValue).GreaterThanOrEqualTo(0);
            RuleFor(p => p.ReportTable).NotNull();
            RuleFor(p => p.ResponsePayload).NotNull();
        }
    }
}