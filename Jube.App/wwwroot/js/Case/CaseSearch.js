// noinspection ES6ConvertVarToLetConst,JSUnresolvedVariable,HtmlUnknownAttribute,JSObsoletePrivateAccessSyntax,JSUnusedLocalSymbols

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

var caseWorkflowId;
var dateFields = [];
var caseId;
var guid;
var firstBind = true;

function onChange(e) {
    var grid = e.sender;
    var currentDataItem = grid.dataItem(this.select());
    if (currentDataItem["Id"] > 0) {
        caseId = currentDataItem["Id"];
        $('#Fetch').show();
        $('#FetchSet').text('Selected Case Id = ' + caseId);
    }
}

function generateGrid(gridData) {
    var model = generateModel(gridData[0]);
    var columns = generateColumns(gridData[0]);

    if (dateFields.length > 0) {
        var parseFunction = function (response) {
            for (var i = 0; i < response.length; i++) {
                for (var fieldIndex = 0; fieldIndex < dateFields.length; fieldIndex++) {
                    var record = response[i];
                    record[dateFields[fieldIndex]] = kendo.parseDate(record[dateFields[fieldIndex]]);
                }
            }
            return response;
        };
    }
    
    $("#grid").kendoGrid({
        dataSource: {
            data: gridData,
            schema: {
                model: model
            }
        },
        columns: columns,
        groupable: true,
        toolbar: ["excel"],
        excel: {
            fileName: "Cases.xlsx",
            proxyURL: "https://proxy.jube.io",
            filterable: true,
            allPages: true
        },
        selectable: true,
        change: onChange,
        dataBound: SetColor,
        height: 500
    });
}

function formatColumnName(data) {
/*    var result = data.replace(/([A-Z])/g, " $1");
    return result.charAt(0).toUpperCase() + result.slice(1);*/
    return data;
}

function generateColumns(gridData) {
    var columns = [];
    for (var property in gridData) {
        if (Object.prototype.hasOwnProperty.call(gridData, property)) {
            var column = {};
            column["width"] = "400px;";
            column["field"] = property;
            column["title"] = formatColumnName(property);
            if (property === 'ForeColor' || property === 'BackColor') {
                column["hidden"] = true;
            }
            columns.push(column);
        }
    }
    return columns;
}

function generateModel(gridData) {
    var model = {};
    model.id = "Id";
    var fields = {};
    for (var property in gridData) {
        if (Object.prototype.hasOwnProperty.call(gridData, property)) {
            var propType = typeof gridData[property];

            if (propType === "number") {
                fields[property] = {
                    type: "number",
                    validation: {
                        required: true
                    }
                };
            } else if (propType === "boolean") {
                fields[property] = {
                    type: "boolean",
                    validation: {
                        required: true
                    }
                };
            } else if (propType === "string") {
                var parsedDate = kendo.parseDate(gridData[property]);
                if (parsedDate) {
                    fields[property] = {
                        type: "date",
                        validation: {
                            required: true
                        }
                    };
                    dateFields.push(property);
                } else {
                    fields[property] = {
                        validation: {
                            required: true
                        }
                    };
                }
            } else {
                fields[property] = {
                    validation: {
                        required: true
                    }
                };
            }

        }
    }
    model.fields = fields;

    return model;
}

function ExecuteCasesInSession() {
    var grid = $('#grid').data('kendoGrid');
    if (typeof grid !== "undefined") {
        grid.destroy();
        $("#grid").empty();
    }

    $.get("../api/SessionCaseSearchCompiledSql/ByGuid/" + guid,
        function (data) {
            generateGrid(data);
        });
}

function SetColor() {
    var grid = $('#grid').data('kendoGrid');
    var rows = grid.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = grid.dataItem(row);
        var backColor = dataItem.get("BackColor");
        var foreColor = dataItem.get("ForeColor");

        row.css("background-color", backColor);
        row.css("color", foreColor);
    }
}

function onSelect(e) {
    var kitems = $(e.node).add($(e.node).parentsUntil('.k-treeview', '.k-item'));

    var texts = $.map(kitems,
        function (kitem) {
            return $(kitem).find('>div span.k-in').text();
        });

    var treeview = $("#Tree").getKendoTreeView();
    var item = treeview.dataItem(e.node);

    if (item.caseWorkflowId) {
        caseWorkflowId = item.caseWorkflowId;

        $.get("../api/CaseWorkflowFilter/" + item.id,
            function (data) {
                const casesFilterBuilderParsed = {
                    filterJson: JSON.parse(data.filterJson),
                    selectJson: JSON.parse(data.selectJson)
                };

                initCaseFilterBuilder(item.caseWorkflowId, casesFilterBuilderParsed);

                const casesFilterBuilder = {
                    filterJson: data.filterJson,
                    selectJson: data.selectJson,
                    filterSql: data.filterSql,
                    filterTokens: data.filterTokens,
                    caseWorkflowId: caseWorkflowId
                };

                compileSqlOnServer(casesFilterBuilder, true);
            });
    } else {
        return false;
    }
}

function compileSqlOnServer(data, refreshGrid) {
    $.ajax({
        url: "../api/SessionCaseSearchCompiledSql/",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (data) {
            guid = data.guid;
            if (refreshGrid) {
                $("#Peek").show();
                $("#Skim").show();
                ExecuteCasesInSession();
            }
        }
    });
}

function onCollapse(e) {
    const dataItemCollapsed = this.dataItem(e.node);
    dataItemCollapsed.loaded(false);
}

$(document).ready(function () {
    $("#Fetch").kendoButton({
        click: function (e) {
            window.location.href = '/Case/Case?CaseId=' + caseId;
        }
    }).hide();

    $("#Skim").kendoButton({
        click: function (e) {
            window.location.href = '/Case/Case?SessionCaseSearchCompiledSqlControllerGuid=  ' + guid;
        }
    }).hide();

    $("#Peek").kendoButton({
        click: function (e) {
            var builderResult = getCasesFilter();

            const casesFilterBuilder = {
                filterJson: builderResult.filterJson,
                selectJson: builderResult.selectJson,
                filterSql: builderResult.filterSql,
                filterTokens: builderResult.filterTokens,
                caseWorkflowId: caseWorkflowId
            };

            compileSqlOnServer(casesFilterBuilder, true);
        }
    }).hide();

    const filter = {
        transport: {
            read: {
                url: '../api/CaseWorkflowFilter/ByCasesWorkflowIdActiveOnly',
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: "id",
                hasChildren: false
            }
        }
    };

    const workflow = {
        transport: {
            read: {
                url: '../api/CaseWorkflow/ByEntityAnalysisModelIdActiveOnly',
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: "id",
                hasChildren: true,
                children: filter
            }
        }
    };

    const model = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: '../api/EntityAnalysisModel',
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: 'id',
                hasChildren: true,
                children: workflow
            }
        }
    });

    $.get("../api/SessionCaseSearchCompiledSql/ByLast/",
        function (data) {
            if (typeof data !== "undefined") {
                guid = data.guid;
                caseWorkflowId = data.caseWorkflowId;

                const casesFilterBuilderParsed = {
                    filterJson: JSON.parse(data.filterJson),
                    selectJson: JSON.parse(data.selectJson)
                };

                initCaseFilterBuilder(data.caseWorkflowId, casesFilterBuilderParsed);

                $("#Peek").show();
                $("#Skim").show();

                ExecuteCasesInSession();
            }
            
            $("#Tree").kendoTreeView({
                dataSource: model,
                dataTextField: "name",
                select: onSelect,
                collapse: onCollapse,
                dataBound: function (e) {
                    var treeview = $("#Tree").getKendoTreeView();
                    treeview.expand(".k-item");

                    if (typeof e.node !== "undefined") {
                        var item = treeview.dataItem(e.node);
                        if (typeof item !== "undefined") {
                            if (typeof item.entityAnalysisModelId !== "undefined") {
                                if (typeof caseWorkflowId !== "undefined") {
                                    if (item.id === caseWorkflowId) {
                                        treeview.select(e.node);
                                    }   
                                }
                            }
                        }
                    }
                }
            });
        }
    );
});

//# sourceURL=CaseSearch.js