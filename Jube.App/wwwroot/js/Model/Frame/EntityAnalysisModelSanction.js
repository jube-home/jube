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

const endpoint = "/api/EntityAnalysisModelSanction";
const parentKeyName = "entityAnalysisModelId";
const validationFail = "There is invalid data in the form. Please check fields and correct.";

const multipartStringDataName = $("#MultipartStringDataName").kendoDropDownList({
    dataTextField: "text",
    dataValueField: "value"
});

const distance = $("#Distance").kendoSlider({
    increaseButtonTitle: "Right",
    decreaseButtonTitle: "Left",
    value: 2,
    min: 0,
    max: 5,
    smallStep: 1,
    largeStep: 1,
    tickPlacement: "none"
});

const cacheValue = $("#CacheValue").kendoNumericTextBox({
    format: "#",
    decimals: 0
});

$.get("/api/GetEntityAnalysisPotentialMultiPartStringNames" + "/" + parentKey,
    function (data) {
        for (const value of data) {
            multipartStringDataName.getKendoDropDownList().dataSource.add({
                "value": value,
                "text": value
            });
        }

        if (typeof id === "undefined") {
            ReadyNew();
        } else {
            $.get(endpoint + "/" + id,
                function (data) {
                    multipartStringDataName.data("kendoDropDownList")
                        .value(data.multipartStringDataName);

                    distance.data("kendoSlider").value(data.distance);

                    $("input[name=CacheInterval][value=" + data.cacheInterval + "]")
                        .prop('checked', true);

                    cacheValue.data("kendoNumericTextBox").value(data.cacheValue);

                    ReadyExisting(data);
                });
        }
    }
);

function GetData() {
    return {
        multipartStringDataName: multipartStringDataName.data("kendoDropDownList").value(),
        distance: distance.data("kendoSlider").value(),
        cacheInterval: $('input[name=CacheInterval]:checked').val(),
        cacheValue: cacheValue.val()
    };
}

$(function () {
    deleteButton
        .click(function () {
            if (confirm('Are you sure you want to delete?')) {
                Delete(endpoint, id);
            }
        });
});

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

//# sourceURL=EntityAnalysisModelSanction.js