(function() {
    function cookie(key, value, end, path, domain, secure) {
        if (arguments.length === 1) {
            return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(key).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
        }

        if (!key || /^(?:expires|max\-age|path|domain|secure)$/i.test(key)) { return false; }
        var expires = "";
        if (end) {
          switch (end.constructor) {
            case Number:
              expires = end === Infinity ? "; expires=Fri, 31 Dec 9999 23:59:59 GMT" : "; max-age=" + end;
              break;
            case String:
              expires = "; expires=" + end;
              break;
            case Date:
              expires = "; expires=" + end.toUTCString();
              break;
          }
        }
        document.cookie = encodeURIComponent(key) + "=" + encodeURIComponent(value) + expires + (domain ? "; domain=" + domain : "") + (path ? "; path=" + path : "") + (secure ? "; secure" : "");
        return true;
    }

    $(document).ready(function() {
        ThemeChooser.publishTheme(window.kendoTheme);
    });

    var doc = document,
        extend = $.extend,
        proxy = $.proxy,
        kendo = window.kendo,
        animation = {
            show: {
                effects: "fadeIn",
                duration: 300
            },
            hide: {
                effects: "fadeOut",
                duration: 300
            }
        },
        skinRegex = /kendo\.[\w\-]+(\.min)?\.css/i;

    var Details = kendo.ui.Widget.extend({
        init: function(element, options) {
            kendo.ui.Widget.fn.init.call(this, element, options);

            this._summary = this.element.find(".tc-activator")
                .on("click", proxy(this.toggle, this));

            this._container = kendo.wrap(this._summary.next(), true);

            this._container.css("display", "none");
        },
        options: {
            name: "Details"
        },
        toggle: function() {
            var show = this._container.is(":visible");
            var animation = kendo.fx(this._container).expand("vertical");

            animation.stop()[show ? "reverse" : "play"]();

            this._summary.toggleClass("tc-active", !show);
        }
    });

    kendo.ui.plugin(Details);

    var ThemeChooser = kendo.ui.ListView.extend({
        init: function(element, options) {
            kendo.ui.ListView.fn.init.call(this, element, options);

            this.bind("change", this._updateTheme);
        },
        options: {
            name: "ThemeChooser",
            template: "",
            selectable: true,
            value: "",
            bordered: false
        },
        dataItem: function(element) {
            var uid = $(element).closest("[data-uid]").attr("data-uid");
            return this.dataSource.getByUid(uid);
        },
        _isSassTheme: function(themeName){
            return /default-v2/i.test(themeName) || /bootstrap-v4/i.test(themeName) || /material-v2/i.test(themeName);
        },
        _shouldReload: function (themeName) {
            return this._isSassTheme(window.kendoTheme) || this._isSassTheme(themeName);
        },
        _updateTheme: function(e) {
            var selection = this.select();

            if (!selection.length) {
              return;
            }

            // make the item available to event listeners
            e.item = this.dataItem(selection);

            // change theme
            var themeName = e.item.value;
            var commonFile = ThemeChooser.getCommonUrl();
            var shouldReload = this._shouldReload(themeName);

            if (this._isSassTheme(themeName)) {
                // hack: SASS themes do not need a common file, it will be skipped
                commonFile = "common-empty";
            } else if (/material/i.test(themeName) && !/material/i.test(commonFile)) {
                commonFile = "common-material";
            } else if (/bootstrap/i.test(themeName) && !/bootstrap/i.test(commonFile)) {
                commonFile = "common-" + themeName;
            } else if (/fiori/i.test(themeName) && !/fiori/i.test(commonFile)) {
                commonFile = "common-fiori";
            } else if (/office365/i.test(themeName) && !/office365/i.test(commonFile)) {
                commonFile = "common-office365";
            } else if (/nova/i.test(themeName) && !/nova/i.test(commonFile)) {
                commonFile = "common-nova";
            } else if (!/material|bootstrap|fiori|nova/i.test(themeName)) {
                commonFile = "common";
            }

            this.element.siblings("[data-role='themechooser']").each(function() {
                var instance = $(this).data("kendoThemeChooser");
                if (instance) {
                    instance.clearSelection();
                }
            });

            ThemeChooser.changeThemePair(themeName, commonFile, shouldReload, true)
                .then(proxy(this.trigger, this, "transition"));
        },
        value: function(value) {
            if (!arguments.length) {
                var dataItem = this.dataItem(this.select());
                return dataItem && dataItem.name;
            }

            var data = this.dataSource.data();

            for (var i = 0; i < data.length; i++) {
                if (data[i].value == value) {
                    this.select(this.element.find("[data-uid='" + data[i].uid + "']"));
                    break;
                }
            }
        }
    });

    var ThemeChooserViewModel = kendo.observable({
        sassThemes: [
            { value: "default-v2", name: "Default v2", colors: [ "#ff6358", "#eb5b51", "#fff" ]  },
            { value: "bootstrap-v4", name: "Bootstrap v4", colors: [ "#0275d8", "#67afe9", "#fff" ]  },
            { value: "material-v2", name: "Material", colors: [ "#3f51b5", "#283593", "#fff"] }
        ],
        themes: [
            { value: "black", name: "Black", colors: [ "#0167cc", "#4698e9", "#272727" ]  },
            { value: "blueopal", name: "Blue Opal", colors: [ "#076186", "#7ed3f6", "#94c0d2" ]  },
            { value: "bootstrap", name: "Bootstrap v3", colors: [ "#3276b1", "#67afe9", "#fff" ]  },
            { value: "default", name: "Default", colors: [ "#ef6f1c", "#e24b17", "#5a4b43" ]  },
            { value: "fiori", name: "Fiori", colors: ["#007cc0", "#e6f2f9", "#f0f0f0"] },
            { value: "flat", name: "Flat", colors: [ "#363940", "#2eb3a6", "#fff" ]  },
            { value: "highcontrast", name: "High Contrast", colors: [ "#b11e9c", "#880275", "#1b141a" ]  },
            { value: "material", name: "Material", colors: [ "#3f51b5", "#283593", "#fff" ]  },
            { value: "materialblack", name: "Material Black", colors: ["#3f51b5", "#1c1c1c", "#4d4d4d"] },
            { value: "metro", name: "Metro", colors: [ "#8ebc00", "#787878", "#fff" ]  },
            { value: "metroblack", name: "Metro Black", colors: [ "#00aba9", "#0e0e0e", "#565656" ]  },
            { value: "moonlight", name: "Moonlight", colors: [ "#ee9f05", "#40444f", "#212a33" ]  },
            { value: "nova", name: "Nova", colors: ["#ff4350", "#00acc1", "#303553"] },
            { value: "office365", name: "Office 365", colors: ["#0072c6", "#cde6f7", "#fff"] },
            { value: "silver", name: "Silver", colors: [ "#298bc8", "#515967", "#eaeaec" ]  },
            { value: "uniform", name: "Uniform", colors: [ "#666", "#ccc", "#fff" ]  }
        ],

        selectedTheme: window.kendoTheme,
        selectedSassTheme: window.kendoTheme,
        selectedSize: window.kendoCommonFile
    });

    kendo.ui.plugin(ThemeChooser);
    window.ThemeChooserViewModel = ThemeChooserViewModel;

    $(document).ready(function() {
        kendo.bind($("[data-rel=themechooser]"), ThemeChooserViewModel);
    });

    extend(ThemeChooser, {
        targetDocument: window.document,
        preloadStylesheet: function (file, callback) {
            if (/common-empty/i.test(file)) {
              return $.Deferred().resolve().promise();
            }

            var deferred = $.Deferred();
            var element = $("<link rel='stylesheet' media='print' href='" + file + "' />");
            element.appendTo( $(ThemeChooser.targetDocument).find("head") );
            deferred.then(callback);

            setTimeout(function () {
                deferred.resolve();
                element.remove();
            }, 100);

            return deferred.promise();
        },

        getCurrentCommonLink: function () {
            return $(ThemeChooser.targetDocument).find("link").filter(function () {
                return (/kendo\.common/gi).test(this.href);
            });
        },

        getCurrentThemeLink: function () {
            return $(ThemeChooser.targetDocument).find("link").filter(function () {
                return (/kendo\./gi).test(this.href) && !(/common|rtl|dataviz|mobile/gi).test(this.href);
            });
        },

        getCommonUrl: function (common) {
            var currentCommonUrl = ThemeChooser.getCurrentCommonLink().attr("href");

            return currentCommonUrl.replace(skinRegex, "kendo." + common + "$1.css");
        },

        getThemeUrl: function (themeName) {
            var currentThemeUrl = ThemeChooser.getCurrentThemeLink().attr("href");

            return currentThemeUrl.replace(skinRegex, "kendo." + themeName + "$1.css");
        },

        replaceCommon: function(commonName) {
            var newCommonUrl = ThemeChooser.getCommonUrl(commonName),
                themeLink = ThemeChooser.getCurrentCommonLink();

            if (kendo.ThemeBuilder && ThemeChooser.targetDocument == window.document ) {
                return;
            }

            ThemeChooser.updateLink(themeLink, newCommonUrl);
            cookie("commonFile", commonName, Infinity, "/");
        },

        updateLink: function(link, url) {
            var exampleElement = $("#example");
            var RTL = function () { return (/kendo\.rtl/gi).test(this.href); };
            var rel = link.eq(0).attr("rel").replace(/-disabled/i, "");
            var rtlStylesheet = $("link").filter(RTL).eq(0);

            // hack: disable old styles in SASS themes
            if (/common-empty/.test(url)) {
                rel += "-disabled";
                rtlStylesheet.attr("rel", rel);
            } else if (/common/.test(url)) {
                rtlStylesheet.attr("rel", rel);
            }

            link.eq(0).attr("rel", rel).attr("href", url);

            if (exampleElement.length) {
                exampleElement[0].style.cssText = exampleElement[0].style.cssText;
            }
        },

        replaceTheme: function(themeName) {
            var newThemeUrl = ThemeChooser.getThemeUrl(themeName),
                oldThemeName = $(ThemeChooser.targetDocument).data("kendoSkin"),
                themeLink = ThemeChooser.getCurrentThemeLink();

            if (kendo.ThemeBuilder && ThemeChooser.targetDocument == window.document ) {
                return;
            }

            ThemeChooser.updateLink(themeLink, newThemeUrl);
            $(ThemeChooser.targetDocument.documentElement).removeClass("k-" + oldThemeName).addClass("k-" + themeName);

            setTimeout(function() {
                // Wait for the CSS files to load
                ThemeChooser.publishTheme(themeName);
            }, 100);

            cookie("theme", themeName, Infinity, "/");
        },

        publishTheme: function (themeName) {
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

            if (themeName) {
                $(doc).data("kendoSkin", themeName);
            }

            $("#example").trigger("kendo:skinChange");
        },

        currentlyUsing: function(href) {
            if (/common-empty/.test(href)) {
                return true;
            }

            if (/common/.test(href)) {
                return ThemeChooser.getCurrentCommonLink().attr("href") == href;
            } else {
                return ThemeChooser.getCurrentThemeLink().attr("href") == href;
            }
        },

        animateCssChange: function(options) {
            options = $.extend({ complete: $.noop, replace: $.noop }, options);

            var prefetch = options.prefetch;

            if (!$.isArray(prefetch)) {
                prefetch = [prefetch];
            }

            prefetch = $.grep(prefetch, function(x) {
                return !ThemeChooser.currentlyUsing(x);
            });

            if (!prefetch.length) {
                return;
            }

            $.when.apply($, $.map(prefetch, ThemeChooser.preloadStylesheet)).then(function() {
                var example = $("#example");

                // In themebuilder
                if (example.length === 0 && $("#tb-preview").length === 1) {
                    example = $("#tb-preview");
                }

                example.kendoStop().kendoAnimate(extend({}, animation.hide, {
                    complete: function (element) {
                        if (element[0] == example[0]) {
                            example.css("visibility", "hidden");

                            options.replace();

                            setTimeout(function () {
                                example
                                    .css("visibility", "visible")
                                    .kendoStop()
                                    .kendoAnimate(animation.show);

                                if (prefetch.join(":").indexOf("common") > -1) {
                                    kendo.resize(example, true);
                                }

                                options.complete();
                            }, 100);
                        }
                    }
                }));
            });
        },

        changeCommon: function(commonName) {
            ThemeChooser.animateCssChange({
                prefetch: ThemeChooser.getCommonUrl(commonName),
                replace: function() {
                    ThemeChooser.replaceCommon(commonName);
                }
            });
        },

        changeTheme: function(themeName, animate, complete) {
            // Set transparent background to the chart area.
            extend(kendo.dataviz.ui.themes[themeName].chart, { chartArea: { background: "transparent"} });

            if (animate) {
                ThemeChooser.animateCssChange({
                    prefetch: ThemeChooser.getThemeUrl(themeName),
                    replace: function() {
                        ThemeChooser.replaceTheme(themeName);
                    },
                    complete: complete
                });
            } else {
                ThemeChooser.replaceTheme(themeName);
            }
        },

        changeThemePair: function(themeName, commonName, shouldReload) {
            var deferred = $.Deferred();

            ThemeChooser.animateCssChange({
                prefetch: [
                    ThemeChooser.getCommonUrl(commonName),
                    ThemeChooser.getThemeUrl(themeName)
                ],
                replace: function() {
                    window.kendoTheme = themeName;
                    window.kendoCommonFile = commonName;
                    ThemeChooser.replaceCommon(commonName);
                    ThemeChooser.replaceTheme(themeName);
                },
                complete: function() {
                    if (shouldReload) {
                        location.reload();
                    }

                    deferred.resolve();
                }
            });

            return deferred.promise();
        }
    });

    window.kendoThemeChooser = ThemeChooser;
})();
