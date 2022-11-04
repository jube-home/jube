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

const endpoint = "/api/UserRegistry";
const parentKeyName = "roleRegistryId";
const validationFail = "There is invalid data in the form. Please check fields and correct.";

$("#Reset").kendoButton({
    click: function () {
        $("#Processing").show();
        $("#Reset").data("kendoButton").enable(false);
        $("#PasswordTable").hide();
        GetPasswordAndShow();
    }
}).hide();

var passwordLocked = $("#PasswordLocked").kendoSwitch();
passwordLocked.data("kendoSwitch").enable(false);

var roleRegistryId = $("#RoleRegistryId").kendoDropDownList({
    dataTextField: "text",
    dataValueField: "value"
});

function GetPasswordAndShow() {
    $.get("/api/UserRegistry/SetPassword/" + id,
        function (data) {
            $("#Processing").hide();
            $("#Password").val(data.password);
            $("#PasswordExpiry").html(new Date(data.passwordExpiryDate).toLocaleString());
            $("#PasswordTable").show();
            $("#Reset").data("kendoButton").enable(true);
            passwordLocked.data("kendoSwitch").value(false);
        }
    );
}

$("#PasswordTable").hide();
$("#Processing").hide();

$.get("/api/RoleRegistry",
    function (data) {
        for (const value of data) {
            roleRegistryId.getKendoDropDownList().dataSource.add({
                "value": value.id,
                "text": value.name
            });
        }

        if (typeof id === "undefined") {
            roleRegistryId.data("kendoDropDownList").value(parentKey);
            passwordLocked.data("kendoSwitch").value(true);
            ReadyNew();
        } else {
            $.get(endpoint + "/" + id,
                function (data) {
                    $("#Email").val(data.email);
                    roleRegistryId.data("kendoDropDownList").value(data.roleRegistryId);

                    if (data.passwordLocked) {
                        passwordLocked.data("kendoSwitch").value(true);
                    } else {
                        passwordLocked.data("kendoSwitch").value(false);
                    }

                    $("#Reset").show();

                    ReadyExisting(data);
                });
        }
    }
);

$(function () {
    deleteButton
        .click(function () {
            if (confirm('Are you sure you want to delete?')) {
                Delete(endpoint, id);
            }
        });
});

function GetData() {
    return {
        name: $("#Name").val(),
        email: $("#Email").val(),
        roleRegistryId: roleRegistryId.data("kendoDropDownList").value()
    };
}

$(function () {
    addButton
        .click(function () {
            if (validator.validate()) {
                Create(endpoint, GetData(), "id", parentKeyName);

                $("#Reset").show();
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

                $("#Reset").show();
            } else {
                $("#ErrorMessage").html(validationFail);
            }
        });
});

//# sourceURL=UserRegistry.js