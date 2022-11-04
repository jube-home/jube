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

using FluentValidation;
using Jube.App.Dto;

namespace Jube.App.Validators
{
    public class ExhaustiveSearchInstanceDtoValidator : AbstractValidator<ExhaustiveSearchInstanceDto>
    {
        public ExhaustiveSearchInstanceDtoValidator()
        {
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Anomaly).NotNull();
            RuleFor(p => p.Filter).NotNull();
            RuleFor(p => p.AnomalyProbability).GreaterThanOrEqualTo(0).When(w => w.Anomaly);
            RuleFor(p => p.Filter).NotEmpty().When(w => w.Filter);
            RuleFor(p => p.FilterJson).NotEmpty().When(w => w.Filter);
            RuleFor(p => p.FilterTokens).NotEmpty().When(w => w.Filter);
            RuleFor(p => p.FilterSql).NotEmpty().When(w => w.Filter);
            RuleFor(p => p.ReportTable).NotNull();
            RuleFor(p => p.ResponsePayload).NotNull();
        }
    }
}