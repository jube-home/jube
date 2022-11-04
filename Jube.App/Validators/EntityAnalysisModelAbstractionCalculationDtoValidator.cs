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
    public class EntityAnalysisModelAbstractionCalculationDtoValidator : AbstractValidator<EntityAnalysisModelAbstractionCalculationDto>
    {
        public EntityAnalysisModelAbstractionCalculationDtoValidator()
        {
            RuleFor(p => p.EntityAnalysisModelId).GreaterThan(0);
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            
            RuleFor(p => p.EntityAnalysisModelAbstractionNameLeft).NotEmpty()
                .When(w => w.AbstractionCalculationTypeId != 5);
            
            RuleFor(p => p.EntityAnalysisModelAbstractionNameRight).NotEmpty()
                .When(w => w.AbstractionCalculationTypeId != 5);

            var abstractionCalculationTypes = new List<int> {1,2,3,4,5};            
            RuleFor(p => p.AbstractionCalculationTypeId)
                .Must(m => abstractionCalculationTypes.Contains(m));
            
            RuleFor(p => p.FunctionScript).NotEmpty()
            .When(w => w.AbstractionCalculationTypeId == 5);
            
            RuleFor(p => p.ReportTable).NotNull();
            RuleFor(p => p.ResponsePayload).NotNull();
        }
    }
}