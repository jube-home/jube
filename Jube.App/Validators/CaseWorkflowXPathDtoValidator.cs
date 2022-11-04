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
    public class CaseWorkflowXPathDtoValidator : AbstractValidator<CaseWorkflowXPathDto>
    {
        public CaseWorkflowXPathDtoValidator()
        {
            RuleFor(p => p.CaseWorkflowId).GreaterThan(0);
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Active).NotNull();
            RuleFor(p => p.Locked).NotNull();
            RuleFor(p => p.XPath).NotEmpty();
            RuleFor(p => p.BoldLineMatched).NotNull();
            
            RuleFor(p => p.BoldLineFormatForeColor)
                .NotEmpty().When(w=> w.BoldLineMatched);
            
            RuleFor(p => p.BoldLineFormatBackColor)
                .NotEmpty().When(w=> w.BoldLineMatched);
                
            RuleFor(p => p.ConditionalRegularExpressionFormatting).NotNull();
            
            RuleFor(p => p.ConditionalFormatForeColor)
                .NotEmpty().When(w=> w.ConditionalRegularExpressionFormatting);
            
            RuleFor(p => p.ConditionalFormatBackColor)
                .NotEmpty().When(w=> w.ConditionalRegularExpressionFormatting);
            
            RuleFor(p => p.ConditionalFormatBackColor)
                .NotEmpty().When(w=> w.ConditionalRegularExpressionFormatting);
                
            RuleFor(p => p.RegularExpression)
                .NotEmpty().When(w=> w.ConditionalRegularExpressionFormatting);
                
            RuleFor(p => p.ForeRowColorScope)
                .NotNull().When(w=> w.ConditionalRegularExpressionFormatting);
            
            RuleFor(p => p.BackRowColorScope)
                .NotNull().When(w=> w.ConditionalRegularExpressionFormatting);
        }
    }
}