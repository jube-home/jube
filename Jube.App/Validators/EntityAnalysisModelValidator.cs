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
    public class EntityAnalysisModelsDtoValidator : AbstractValidator<EntityAnalysisModelDto>
    {
        public EntityAnalysisModelsDtoValidator()
        {
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            RuleFor(p => p.Name).NotEmpty();

            RuleFor(p => p.EntryXPath).NotEmpty();
            RuleFor(p => p.ReferenceDateXPath).NotEmpty();
            RuleFor(p => p.EntryName).NotEmpty();
            RuleFor(p => p.ReferenceDateName).NotEmpty();

            var typesPayloadLocation = new List<int> {1, 3};

            RuleFor(p => p.ReferenceDatePayloadLocationTypeId).Must(m => typesPayloadLocation.Contains(m));
            RuleFor(p => p.ReferenceDateName).NotEmpty();
            RuleFor(p => p.ReferenceDatePayloadLocationTypeId).Must(m => typesPayloadLocation.Contains(m));
            RuleFor(p => p.ReferenceDateXPath).NotEmpty();
            RuleFor(p => p.CacheFetchLimit).GreaterThanOrEqualTo(0);
            RuleFor(p => p.MaxResponseElevation).GreaterThanOrEqualTo(0);

            var typesInterval = new List<char> {'s', 'n', 'h', 'd'};

            RuleFor(p => p.CacheTtlInterval).Must(m => typesInterval.Contains(m));
            RuleFor(p => p.CacheTtlIntervalValue).GreaterThanOrEqualTo(0);

            RuleFor(p => p.MaxResponseElevationInterval).Must(m => typesInterval.Contains(m));
            RuleFor(p => p.MaxResponseElevationValue).GreaterThanOrEqualTo(0);
            RuleFor(p => p.MaxResponseElevationThreshold).GreaterThanOrEqualTo(0);

            RuleFor(p => p.MaxActivationWatcherInterval).Must(m => typesInterval.Contains(m));
            RuleFor(p => p.MaxActivationWatcherValue).GreaterThanOrEqualTo(0);
            RuleFor(p => p.MaxActivationWatcherThreshold).GreaterThanOrEqualTo(0);
            RuleFor(p => p.ActivationWatcherSample).InclusiveBetween(0, 1);

            RuleFor(p => p.EnableCache).NotNull();
            RuleFor(p => p.EnableTtlCounter).NotNull();
            RuleFor(p => p.EnableSanctionCache).NotNull();
            RuleFor(p => p.EnableRdbmsArchive).NotNull();
            RuleFor(p => p.EnableActivationWatcher).NotNull();
        }
    }
}