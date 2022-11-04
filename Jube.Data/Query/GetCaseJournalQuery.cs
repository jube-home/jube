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
using System.Text.RegularExpressions;
using Jube.Data.Context;
using Jube.Data.Reporting;
using Newtonsoft.Json.Linq;

namespace Jube.Data.Query
{
    public class GetCaseJournalQuery
    {
        private readonly string _connectionString;
        private readonly DbContext _dbContext;
        private readonly string _userName;

        public GetCaseJournalQuery(DbContext dbContext, string user)
        {
            _connectionString = dbContext.ConnectionString;
            _userName = user;
            _dbContext = dbContext;
        }

        public List<Dictionary<string, object>> Execute(string key, string keyValue, int caseWorkflowId, int limit,
            int activationRuleCount, double responseElevation)
        {
            var values = new List<Dictionary<string, object>>();

            var caseWorkflowXPathByCaseWorkflowIdQuery =
                new GetCaseWorkflowXPathByCaseWorkflowIdQuery(_dbContext, _userName);

            var xPaths = caseWorkflowXPathByCaseWorkflowIdQuery
                .Execute(caseWorkflowId).ToList();

            var sql = "select '(' || \"ActivationRuleCount\" || ') ' || r.\"Name\" as \"Activation\", a.* " +
                      "from \"Archive\" a " +
                      "inner join \"EntityAnalysisModel\" m on m.\"Id\" = a.\"EntityAnalysisModelId\" " +
                      "inner join \"TenantRegistry\" t on t.\"Id\" = m.\"TenantRegistryId\" " +
                      "inner join \"UserInTenant\" u on u.\"TenantRegistryId\" = t.\"Id\" " +
                      "left join (select \"Id\", \"Name\" from \"EntityAnalysisModelActivationRule\") r on r.\"Id\" = a.\"EntityAnalysisModelActivationRuleId\" " +
                      "where u.\"User\" = (@1) " +
                      "and a.\"Json\" -> 'payload' ->> (@2) = (@3) " +
                      "and a.\"ResponseElevation\" >= (@4)" +
                      "and a.\"ActivationRuleCount\" >= (@5) " +
                      "order by a.\"Id\" desc " +
                      "limit (@6)";
            
                var tokens = new List<object>
            {
                _userName,
                key,
                keyValue,
                responseElevation,
                activationRuleCount,
                limit
            };

            var postgres = new Postgres(_connectionString);

            postgres.Prepare(sql, tokens);

            foreach (var record in postgres.ExecuteByOrderedParameters(sql, tokens))
            {
                var json = JObject.Parse(record["Json"].ToString());

                var cellFormats = new List<GetCaseJournalQueryCellFormatDto>();

                var value = new Dictionary<string, object>
                {
                    {"Id", record["Id"]},
                    {"Activation", record["Activation"]},
                    {"EntityAnalysisModelInstanceEntryGuid", record["EntityAnalysisModelInstanceEntryGuid"]},
                    {"ReferenceDate", record["ReferenceDate"]},
                    {"CreatedDate", record["CreatedDate"]},
                    {"ResponseElevation", record["ResponseElevation"]},
                    {"EntryKeyValue", record["EntryKeyValue"]}
                };

                if (json != null)
                {
                    foreach (var xPath in xPaths)
                    {
                        try
                        {
                            var jToken = json.SelectToken(xPath.XPath);
                            if (jToken != null)
                            {
                                var valueToken = jToken.Value<string>();

                                if (!value.ContainsKey(xPath.Name))
                                {
                                    value.Add(xPath.Name, valueToken);

                                    if (xPath.ConditionalRegularExpressionFormatting)
                                        try
                                        {
                                            var regex = new Regex(xPath.RegularExpression);

                                            var match = regex.Match(valueToken);

                                            if (match.Success)
                                                cellFormats.Add(new GetCaseJournalQueryCellFormatDto
                                                {
                                                    CellFormatKey = xPath.Name,
                                                    CellFormatBackColor = xPath.ConditionalFormatBackColor,
                                                    CellFormatForeColor = xPath.ConditionalFormatForeColor,
                                                    CellFormatForeRow = xPath.ForeRowColorScope,
                                                    CellFormatBackRow = xPath.BackRowColorScope
                                                });
                                        }
                                        catch
                                        {
                                            //ignored
                                        }
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        if (xPath.BoldLineMatched)
                            value.Add("BoldLine", new GetCaseJournalQueryBoldLineDto
                            {
                                BoldLineKey = xPath.Name,
                                BoldLineFormatBackColor = xPath.BoldLineFormatBackColor,
                                BoldLineFormatForeColor = xPath.BoldLineFormatForeColor
                            });
                    }

                    if (cellFormats.Count > 0) value.Add("CellFormat", cellFormats);

                    values.Add(value);
                }
            }

            return values;
        }

        public class GetCaseJournalQueryBoldLineDto
        {
            public string BoldLineKey { get; set; }
            public string BoldLineFormatForeColor { get; set; }
            public string BoldLineFormatBackColor { get; set; }
        }

        public class GetCaseJournalQueryCellFormatDto
        {
            public string CellFormatKey { get; set; }
            public string CellFormatBackColor { get; set; }
            public string CellFormatForeColor { get; set; }
            public bool CellFormatForeRow { get; set; }
            public bool CellFormatBackRow { get; set; }
        }
    }
}