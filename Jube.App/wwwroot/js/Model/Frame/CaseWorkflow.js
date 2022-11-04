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

const endpoint = "/api/CaseWorkflow";
const parentKeyName = "entityAnalysisModelId";
const validationFail = "There is invalid data in the form. Please check fields and correct.";

var enableVisualisation = $("#EnableVisualisation").kendoSwitch({
    change: function () {
        SetEnableVisualisation();
    }
});

var visualisationRegistryId = $("#VisualisationRegistryId").kendoDropDownList({
    dataTextField: "text",
    dataValueField: "value"
});

$.get("/api/VisualisationRegistry",
    {
        id: parentKey
    },
    function (data) {
        for (const value of data) {
            visualisationRegistryId.getKendoDropDownList().dataSource.add({
                "value": value.id,
                "text": value.name
            });
        }
    
        if (typeof id === "undefined") {
            ReadyNew();
            SetEnableVisualisation();
        } else {
            $.get(endpoint + "/" + id,
                function (data) {
                    visualisationRegistryId.data("kendoDropDownList").value(data.visualisationRegistryId);

                    if (data.enableVisualisation) {
                        enableVisualisation.data("kendoSwitch").check(true);
                    } else {
                        enableVisualisation.data("kendoSwitch").check(false);
                    }

                    SetEnableVisualisation();
                    ReadyExisting(data);
                });
        }
    });

$(function () {
    deleteButton
        .click(function () {
            if (confirm('Are you sure you want to delete?')) {
                Delete(endpoint, id);
            }
        });
});

function GetData() {
    if (enableVisualisation.data("kendoSwitch").check()) {
        return {
            enableVisualisation: enableVisualisation.data("kendoSwitch").check(),
            visualisationRegistryId: visualisationRegistryId.data("kendoDropDownList").value()
        };        
    }
    else {
        return {};
    }
}

$(function () {
    addButton
        .click(function () {
            if (validator.validate()) {
                Create(endpoint, GetData(), "id", parentKeyName);
            } else {
                $("#ErrorMessage").html(validationFail);
            }
        });
});

$(function () {
    updateButton
        .click(function () {
            if (validator.validate()) {
                Update(endpoint, GetData(), "id", parentKeyName);
            } else {
                $("#ErrorMessage").html(validationFail);
            }
        });
});

function SetEnableVisualisation() {
    if ($('#EnableVisualisation').prop('checked')) {
        $('#EnableVisualisationTable').show();
    } else {
        $('#EnableVisualisationTable').hide();
    }
}

//# sourceURL=CaseWorkflow.js