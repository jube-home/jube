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
    public class VisualisationRegistryDatasourceValidator : AbstractValidator<VisualisationRegistryDatasourceDto>
    {
        public VisualisationRegistryDatasourceValidator()
        {
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Priority).GreaterThanOrEqualTo(0);
            RuleFor(p => p.ColumnSpan).GreaterThanOrEqualTo(0);
            RuleFor(p => p.RowSpan).GreaterThanOrEqualTo(0);
            RuleFor(p => p.Command).NotEmpty();
            RuleFor(p => p.IncludeGrid).NotNull();
            RuleFor(p => p.IncludeDisplay).NotNull();

            var visualisationTypes = new List<int> {1, 2, 3};
            RuleFor(p => p.VisualisationTypeId)
                .Must(m => visualisationTypes.Contains(m))
                .When(w => w.IncludeDisplay);

            RuleFor(p => p.VisualisationText)
                .NotEmpty()
                .When(w => w.IncludeDisplay);
        }
    }
}