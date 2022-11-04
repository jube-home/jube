var isAndroid = kendo.support.mobileOS.android;


var currentExample,
    currentSection;


var TITLES = {
    "actionsheet": "ActionSheet",
    "grid": "Grid",
    "scheduler": "Scheduler",
    "application": "Application",
    "mobile-buttongroup": "ButtonGroup",
    "collapsible": "Collapsilble",
    "mobile-drawer": "Drawer",
    "mobile-button": "Button",
    "mobile-forms": "Forms",
    "mobile-layout": "Layout",
    "mobile-listview": "ListView",
    "mobile-tabstrip": "TabStrip",
    "mobile-view": "View",
    "modalview": "ModalView",
    "navbar": "NavBar",
    "popover": "PopOver",
    "scroller": "Scroller",
    "mobile-scrollview": "ScrollView",
    "splitview": "SplitView",
    "mobile-styling": "Styling",
    "mobile-switch": "Switch",
    "touchevents": "Touch Events"
};

// override datasources

navDataSource = new kendo.data.DataSource({
    transport: {
        read: {
            url: NAV_JSON_URL,
            dataType: "json"
        }
    },
    change: function(e) {
        $.each(this.view(), function(index, group) {
            group.items.sort(function(a, b) {
                if (a.text === "Basic usage") {
                    return false;
                }

                if (b.text === "Basic usage") {
                    return true;
                }

                // console.log(a, b);
                return a.text > b.text;
            });
        });
    },

    schema: {
        model: {
            id: "url"
        },
        parse: function(response) {
            for (var i = 0; i < response.length; i++) {
                response[i].section = TITLES[response[i].url.split("/")[0]];
            }
            return response;
        }
    },
    group: { field: "section" }
})

searchDataSource = navDataSource;

function nullCurrentExample(e) {
    currentExample = null;
}

function removeView(e) {
    if (!e.view.element.data("persist")) {
        e.view.purge();
    }
}

function initSearch(e) {
    var searchBox = e.view.element.find("#demos-search");

    searchBox.on("input", function() {
        searchExamplesFor(searchBox.val(), product);
    });

    searchBox.on("blur", function() {
        if (searchBox.val() == "") {
            hideSearch();
        }
    });
}

var desktop = !kendo.support.mobileOS;

function showSearch() {
    $("#normal").addClass("navbar-hidden");
    $("#search").removeClass("navbar-hidden");
    if (desktop) {
       setTimeout(function() {
            $("#demos-search").focus();
       });
    } else {
        $("#demos-search").focus();
    }
}

function hideSearch() {
    $("#normal").removeClass("navbar-hidden");
    $("#search").addClass("navbar-hidden");
}

function checkSearch(e) {
    if (!searchDataSource.filter()) {
        e.preventDefault();
        this.replace([]);
        $("#search-tooltip").show();
    } else {
        $("#search-tooltip").hide();
    }
}

function showDemoLayout(e) {
    currentExample = null;
    currentSection = null;
    navDataSource.fetch(function() {
        var url = e.view.id,
            element = e.view.element;

        currentExample = navDataSource.get(url);

        if (currentExample) {
            element.find("[data-icon=source-code]").show();
        } else {
            element.find("[data-icon=source-code]").hide();
        }
    });
}

var sourceCode;

function openDojo(e) {
    window.dojo.postSnippet(sourceCode, window.location.href.replace(window.location.hash, ""));
    e.preventDefault();
}

function goToSourceCode() {
    app.navigate("#source-code-view?view=" + app.view().id);
}

function showSourceCode(e) {
    app.showLoading();
    $("#source-code-container").empty();

    $.get("../source/mobile?path=" + encodeURIComponent("~/Views/demos/" + e.view.params.view + ".cshtml")).then(function(response) {
        $("#source-code-container").html(response);
        sourceCode = $("#source-code-container").find(".prettyprint").text();
        prettyPrint();
        app.hideLoading();
    });
}

function disableInSourceCode(e) {
    if (app.view().id === "#source-code-view") {
        e.preventDefault();
    }
}

window.app = new kendo.mobile.Application($(document.body), {
    layout: "examples",
    transition: "slide",
    skin: "nova",
    icon: {
        "" : '@Url.Content("~/content/mobile/AppIcon72x72.png")',
        "72x72" : '@Url.Content("~/content/mobile/AppIcon72x72.png")',
        "76x76" : '@Url.Content("~/content/mobile/AppIcon76x76.png")',
        "114x114" : '@Url.Content("~/content/mobile/AppIcon72x72@2x.png")',
        "120x120" : '@Url.Content("~/content/mobile/AppIcon76x76@2x.png")',
        "152x152" : '@Url.Content("~/content/mobile/AppIcon76x76@2x.png")'
    }
});
