$(document).on("kendoReady", function () {
    $(window).on("resize", function () {
        if (window.kendo) {
            kendo.resize($(".k-chart"));
        }
    });
});

$(document).ready(function () {
    var runDemoBtn = $("#runDemo");

    if (runDemoBtn.length && isMobile()) {
        $(".demo-button-wrapper").show();
        $("#runDemo").on("click", loadDemo);
    } else {
        loadDemo();
    }

    $(".tabstrip .tabstrip-tab").on("click", tabStripSelect);
    $(".try-kendo").click(openInDojo);

    $(document).on("click", function (ev) {
        var target = $(ev.target);
        if (target.parents(".dropdown-wrapper").length || target.is(".dropdown-toggle")) {
            return;
        }

        $(".dropdown.active").removeClass("active");
        $(".dropdown-wrapper.active").removeClass("active");
    });

    $(".tabstrip .dropdown-toggle").on("click", toggleDropdown);
    $(".theme-chooser-dropdown .kd-theme").on("click", themeChooserChange);
    $(".type-chooser").on("click", themeTypeChange)
    $(".kd-example-console .clear").on("click", clearConsole);
});

//Dropdown toggle
function toggleDropdown(e) {
    var dropdown = $(e.target).parents(".dropdown-wrapper");
    $(".dropdown-wrapper .dropdown").removeClass("active");
    dropdown.find(".dropdown").eq(0).toggleClass("active");
}
//End

// Open in Dojo
function openInDojo(e) {
    e.preventDefault();
    if (!window.dojo) {
        var scripts = $("#dojo-js").toArray();
        var dfd = $.Deferred();
        loadScripts(scripts, dfd);
    }

    postToDojo(e);
}

function postToDojo(e) {
    var button = $(e.target).closest(".try-kendo");

    $.get(button.data("url")).done(function (data) {
        window.dojo.postSnippet($(data).text(), window.location.href);
    });
}
// End

// Run to click logic
function loadStyles() {
    $("link[data-href]").each(function (index, link) {
        $(link).attr("href", $(link).attr("data-href"));
    });
}

function loadDemo() {
    $(".demo-button-wrapper").hide();

    loadStyles();
    var scripts = $("script[data-src]").toArray();
    var dfd = $.Deferred();

    dfd.done(function () {
        $(".kd-loader-wrap").hide();

        if (IS_ANGULARJS_EXAMPLE) {
            return;
        }

        resetThemableOptions();

        $("#demo-runner").html($("#demoCode").text());
        $(document).trigger("kendoReady");
    });

    loadScripts(scripts, dfd);
}

function loadScripts(scripts, dfd) {
    if (!scripts.length) {
        return;
    }

    var script = scripts.shift();
    $(script).on("load", function (e) {
        // Configure DOJO
        if ($(script).is("#dojo-js")) {
            dojo.configuration = {
                url: DOJO_ROOT,
                cdnRoot: CDN_ROOT
            };
        }

        if (scripts.length == 0) {
            dfd.resolve();
        }
        loadScripts(scripts, dfd);
    });
    $(script).attr("src", $(script).attr("data-src"));
    $(script).removeAttr("data-src");
}

function resetThemableOptions() {
    var themeName = window.selectedTheme;
    var themable = ["Chart", "TreeMap", "Diagram", "StockChart", "Sparkline", "RadialGauge", "LinearGauge", "ArcGauge"];

    if (kendo.dataviz && themeName) {
        var isSass = themeName === "default-v2" || themeName === "bootstrap-v4" || themeName === "material-v2";

        if (isSass) {
            kendo.dataviz.autoTheme(true);
        }

        for (var i = 0; i < themable.length; i++) {
            var widget = kendo.dataviz.ui[themable[i]];

            if (widget) {
                widget.fn.options.theme = isSass ? "sass" : themeName;
            }
        }
    }
}
// End

// Tabstrip logic
function tabStripSelect(ev) {
    var tab = $(ev.target).closest(".tabstrip-item");
    var tabstrip = tab.closest(".tabstrip");
    var panes = tabstrip.siblings(".tabstrip-pane");
    var targetPane = $( "#" + tab.data("container"));

    if (tab.is(".active")) {
        return;
    }

    var activeElements = tabstrip.find(".tabstrip-item.active").add($.grep(panes, function (item) {
        return $(item).is(".active");
    }));

    activeElements.removeClass("active");

    tab.add(targetPane).addClass("active");

    if (targetPane.is("#source-code-pane") && !targetPane.find(".active").length) {
        loadFirstSource(targetPane);
        return;
    }

    if (tab.is("[data-url]")) {
        loadSource(tab, targetPane);
    }
}

function loadFirstSource(sourcePane) {
    var srcTabstrip = sourcePane.find(".tabstrip");
    var firstTab = srcTabstrip.find("li.tabstrip-item").eq(0);
    var firstPane = $("#" + firstTab.data("container"));

    firstTab.add(firstPane).addClass("active");

    loadSource(firstTab, firstPane);
}

function loadSource(tab, pane) {
    var url = tab.data("url");
    var isMainSource = tab.index() === 0;

    if (!window.prettyPrint) {
        loadPrettify();
    }

    $.get(url).done(function (data) {
        tab.removeAttr("data-url");
        pane.html(data);

        if (window.prettyPrint) {
            prettyPrint();
        }
    });
}

function loadPrettify() {
    var scripts = $("#prettify-js").toArray();
    var dfd = $.Deferred();
    loadScripts(scripts, dfd);

    dfd.done(function () {
        prettyPrint();
    });
}
// End

// ThemeChooser logic
function toggleThemeChooser() {
    $(".theme-chooser-dropdown").toggleClass("active");
}

function themeChooserChange(ev) {
    var choosenTheme = $(ev.target).closest("li[data-val]").data("val");
    window.location.search = "autoRun=true&theme=" + choosenTheme;
}

function themeTypeChange(ev) {
    var target = $(ev.target),
        list = $(".theme-chooser-dropdown .themes-list"),
        tabs = $(".type-chooser .theme-type"),
        chosenType = target.data().select;

    if (target.hasClass("active")) {
        return;
    }

    list.hide();
    tabs.toggleClass("active");
    $("." + chosenType).show();
}
// End


// Console
function clearConsole() {
    kendoConsole.clear();
}