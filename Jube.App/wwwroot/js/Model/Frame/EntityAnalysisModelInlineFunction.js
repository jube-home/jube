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

var endpoint = "/api/EntityAnalysisModelInlineFunction";
var parentKeyName = "entityAnalysisModelId";
var validationFail = "There is invalid data in the form. Please check fields and correct.";

var returnDataTypeId = $("#ReturnDataTypeId").kendoDropDownList({
    dataTextField: "text",
    dataValueField: "value"
});

if (typeof id === "undefined") {
    initBuilderCoder(1, GetSelectedParentID());
    ReadyNew();
} else {
    $.get(endpoint + "/" + id,
        function (data) {
            returnDataTypeId.data("kendoDropDownList").value(data.returnDataTypeId);

            const builderCoderData = {
                ruleTextCoder: data.functionScript,
                ruleType: 1
            };

            initBuilderCoder(1, GetSelectedParentID(), builderCoderData);
            ReadyExisting(data);
        });
}

$(function () {
    deleteButton
        .click(function () {
            if (confirm('Are you sure you want to delete?')) {
                Delete(endpoint, id);
            }
        });
});

function GetData() {
    const builderCoder = getBuilderCoder();
    return {
        responsePayload: $("#ResponsePayload").prop("checked"),
        functionScript: builderCoder.ruleTextCoder,
        returnDataTypeId: returnDataTypeId.data("kendoDropDownList").value(),
        name: $("#Name").val()
    };
}

$(function () {
    addButton
        .click(function () {
            if (validator.validate() && validateBuilderCoder()) {
                Create(endpoint, GetData(), "id", parentKeyName);
            } else {
                $("#ErrorMessage").html(validationFail);
            }
        });
});

$(function () {
    updateButton
        .click(function () {
            if (validator.validate() && validateBuilderCoder()) {
                Update(endpoint, GetData(), "id", parentKeyName);
            } else {
                $("#ErrorMessage").html(validationFail);
            }
        });
});

//# sourceURL=EntityAnalysisModelInlineFunction.js