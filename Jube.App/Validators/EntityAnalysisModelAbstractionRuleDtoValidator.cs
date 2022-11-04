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
    public class EntityAnalysisModelAbstractionRuleDtoValidator : AbstractValidator<EntityAnalysisModelAbstractionRuleDto>
    {
        public EntityAnalysisModelAbstractionRuleDtoValidator()
        {
            RuleFor(p => p.EntityAnalysisModelId).GreaterThan(0);
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            
            RuleFor(p => p.SearchKey).NotEmpty()
                .When(w => w.Search);
            
            RuleFor(p => p.SearchValue).GreaterThanOrEqualTo(0)
                .When(w => w.Search);
            
            var intervalTypes = new List<string> {"s","n","h","d"};
            RuleFor(p => p.SearchInterval)
                .Must(m => intervalTypes.Contains(m))
                .When(w => w.Search);
            
            RuleFor(p => p.SearchFunctionKey).NotEmpty()
                .When(w => w.Search && w.SearchFunctionTypeId != 1);
            
            RuleFor(p => p.Offset).NotNull()
                .When(w => w.Search);
            
            var offsetTypes = new List<int> {1,2,3,4};
            RuleFor(p => p.OffsetTypeId).Must(m => offsetTypes.Contains(m))
                .When(w => w.Search && w.Offset);
            
            RuleFor(p => p.OffsetValue).GreaterThanOrEqualTo(0)
                .When(w => w.Search && w.Offset);
            
            RuleFor(p => p.BuilderRuleScript).NotNull();
            RuleFor(p => p.Json).NotEmpty();
            RuleFor(p => p.CoderRuleScript).NotNull();
            
            var ruleTypes = new List<int> {1,2};
            RuleFor(p => p.RuleScriptTypeId).Must(m => ruleTypes.Contains(m));
            
            RuleFor(p => p.ReportTable).NotNull();
            RuleFor(p => p.ResponsePayload).NotNull();
        }
    }
}