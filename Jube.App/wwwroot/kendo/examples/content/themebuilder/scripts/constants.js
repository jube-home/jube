kendo.ThemeBuilder.getConstants = function(context) {
    context = context || window.parent;
    var kendo = window.kendo,
        constant = function(property, target, values){
            return {
                target: target,
                property: property,
                values: values
            };
        },
        gradientConstant = function(target) {
            return {
                property: "background-image",
                editor: "ktb-gradient",
                infer: function() {
                    var background = cssPropertyFrom(target.slice(1), "background-image"),
                        match = /linear-gradient\((.*)\)$/i.exec(background);

                    return match ? match[1] : "none";
                }
            };
        },
        toProtocolRelative = function(url) {
            return url.replace(/^http(s?):\/\//i, "//");
        },
        cdnRoot = (function() {
            var scripts = document.getElementsByTagName("script"),
                script, path, i;

            for (i = 0; i < scripts.length; i++) {
                script = scripts[i];

                if (script.src.indexOf("kendo.all.min") > 0) {
                    break;
                }
            }

            path = script.src.split('?')[0];

            return toProtocolRelative(path.split("/").slice(0,-2).join("/") + "/");
        })(),
        COLOR = "color",
        cssPropertyFrom = function(cssClass, property) {
            var dummy = $("<div class='" + cssClass + "' />"), result;

            dummy.css("display", "none").appendTo(context.document.body);

            result = dummy.css(property);

            dummy.remove();

            return result;
        },
        webConstants = {
            "@image-folder": {
                readonly: true,
                infer: function() {
                    var result = cssPropertyFrom("k-i-loading", "background-image")
                            .replace(/url\(["']?(.*?)\/loading\.gif["']?\)$/i, "\"$1\""),
                        cdnRootRe = /cdn\.kendostatic\.com|da7xgjtj801h2\.cloudfront\.net/i;

                    result = result.replace(cdnRootRe, "kendo.cdn.telerik.com");

                    return toProtocolRelative(result);
                }
            },

            "@fallback-texture":                { readonly: true, value: "none" },

            "@texture":                         {
                property: "background-image",
                target: ".k-header",
                values: [ { text: "flat", value: "none" } ].concat(
                    [
                        "highlight", "glass", "brushed-metal", "noise",
                        "dots1", "dots2", "dots3", "dots4", "dots5",
                        "dots6", "dots7", "dots8", "dots9", "dots10",
                        "dots11", "dots12", "dots13", "leather1", "leather2",
                        "stripe1", "stripe2", "stripe3", "stripe4", "stripe5", "stripe6"
                    ].map(function(x) {
                        return { text: x, value: "url('" + cdnRoot + "styles/textures/" + x + ".png')" };
                    }
                )),
                infer: function() {
                    var background = cssPropertyFrom("k-header", "background-image"),
                        match = /^(.*),\s*[\-\w]*linear-gradient\(/i.exec(background);

                    return match ? match[1] : "none";
                }
            },

            "@theme-type": {
                readonly: true,
                type: "file-import",
                values: [
                    { text: "Bootstrap", value: "type-bootstrap.less" },
                    { text: "Default", value: "type-default.less" },
                    { text: "Fiori", value: "type-fiori.less" },
                    { text: "Flat", value: "type-flat.less" },
                    { text: "High Contrast", value: "type-highcontrast.less" },
                    { text: "Material", value: "type-material.less" },
                    { text: "Metro", value: "type-metro.less" },
                    { text: "Nova", value: "type-nova.less" },
                    { text: "Office 365", value: "type-office365.less" }
                ],
                infer: function() {
                    var values = this.values;
                    var i, name, prop;
                    var result = "type-default.less";

                    for (i = 0; i < this.values.length; i++) {
                        name = values[i].text.replace(/\s/g, "").toLowerCase();
                        prop = cssPropertyFrom("ktb-theme-id-" + name, "opacity");

                        if (prop === "0") {
                            result = "type-" + name + ".less";
                            break;
                        }
                    }

                    return result;
                }
            },

            "@accent":              constant(COLOR, ".ktb-var-accent"),
            "@base":                constant(COLOR, ".ktb-var-base"),
            "@background":          constant(COLOR, ".ktb-var-background"),

            "@border-radius":       constant("border-radius", ".ktb-var-border-radius"),

            "@normal-background":   constant(COLOR, ".ktb-var-normal-background"),
            "@normal-gradient":     gradientConstant(".ktb-var-normal-gradient"),
            "@normal-text-color":   constant(COLOR, ".ktb-var-normal-text-color"),

            "@hover-background":    constant(COLOR, ".ktb-var-hover-background"),
            "@hover-gradient":      gradientConstant(".ktb-var-hover-gradient"),
            "@hover-text-color":    constant(COLOR, ".ktb-var-hover-text-color"),

            "@selected-background": constant(COLOR, ".ktb-var-selected-background"),
            "@selected-gradient":   gradientConstant(".ktb-var-selected-gradient"),
            "@selected-text-color": constant(COLOR, ".ktb-var-selected-text-color"),

            "@is-dark-theme":       {
                readonly: true,
                infer: function() {
                    var prop =  cssPropertyFrom("ktb-var-is-dark-theme ", "opacity");

                    if (prop === "1") {
                        return true;
                    }

                    return false;
                }
            },

            "@theme-colors": {
                readonly: true,
                values: [
                    "primary", "secondary", "tertiary", "info", "success",
                    "warning", "error", "dark", "light", "inverse"
                    ].map(function(constant) {
                        var color = kendo.parseColor(cssPropertyFrom("ktb-var-" + constant, COLOR)).toCss();
                        return { text: constant, value: color };
                    }
                ),
                infer: function() {
                    var values = this.values,
                        result = "{\n",
                        constant;

                    for (var i = 0; i < this.values.length; i++) {
                        constant = values[i];

                        result += "    " + constant.text + ": " + constant.value + "; \n";
                    }

                    result += "}";

                    return result;
                }
            },

            "@primary":             constant(COLOR, ".ktb-var-primary"),
            "@secondary":           constant(COLOR, ".ktb-var-secondary"),
            "@tertiary":            constant(COLOR, ".ktb-var-tertiary"),
            "@error":               constant(COLOR, ".ktb-var-error"),
            "@warning":             constant(COLOR, ".ktb-var-warning"),
            "@success":             constant(COLOR, ".ktb-var-success"),
            "@info":                constant(COLOR, ".ktb-var-info"),
            '@dark':                constant(COLOR, ".ktb-var-dark"),
            '@light':               constant(COLOR, ".ktb-var-light"),
            '@inverse':             constant(COLOR, ".ktb-var-inverse"),

            "@series-a":            constant(COLOR, ".ktb-var-series-a"),
            "@series-b":            constant(COLOR, ".ktb-var-series-b"),
            "@series-c":            constant(COLOR, ".ktb-var-series-c"),
            "@series-d":            constant(COLOR, ".ktb-var-series-d"),
            "@series-e":            constant(COLOR, ".ktb-var-series-e"),
            "@series-f":            constant(COLOR, ".ktb-var-series-f")
        },
        datavizConstants = {
            "chart.title.color":                          constant(COLOR),
            "chart.legend.labels.color":                  constant(COLOR),
            "chart.chartArea.background":                 constant(COLOR),
            "chart.seriesDefaults.labels.color":          constant(COLOR),
            "chart.axisDefaults.line.color":              constant(COLOR),
            "chart.axisDefaults.labels.color":            constant(COLOR),
            "chart.axisDefaults.minorGridLines.color":    constant(COLOR),
            "chart.axisDefaults.majorGridLines.color":    constant(COLOR),
            "chart.axisDefaults.title.color":             constant(COLOR),
            "chart.seriesColors[0]":                      constant(COLOR),
            "chart.seriesColors[1]":                      constant(COLOR),
            "chart.seriesColors[2]":                      constant(COLOR),
            "chart.seriesColors[3]":                      constant(COLOR),
            "chart.seriesColors[4]":                      constant(COLOR),
            "chart.seriesColors[5]":                      constant(COLOR),
            "gauge.pointer.color":                        constant(COLOR),
            "gauge.scale.rangePlaceholderColor":          constant(COLOR),
            "gauge.scale.labels.color":                   constant(COLOR),
            "gauge.scale.minorTicks.color":               constant(COLOR),
            "gauge.scale.majorTicks.color":               constant(COLOR),
            "gauge.scale.line.color":                     constant(COLOR)
        },
        webConstantsHierarchy = {
            "Widgets": {
                "@theme-type":          "Theme type",
                "@accent":              "Accent color",
                "@base":                "Widget base",
                "@background":          "Widget background",
                "@border-radius":       "Border radius",
                "@normal-background":   "Normal background",
                "@normal-text-color":   "Normal text",
                "@normal-gradient":     "Normal gradient",
                "@hover-background":    "Hovered background",
                "@hover-text-color":    "Hovered text",
                "@hover-gradient":      "Hovered gradient ",
                "@selected-background": "Selected background",
                "@selected-text-color": "Selected text",
                "@selected-gradient":   "Selected gradient",
                "@error":               "Error",
                "@warning":             "Warning",
                "@success":             "Success",
                "@info":                "Info",
                "@series-a":            "Series A",
                "@series-b":            "Series B",
                "@series-c":            "Series C",
                "@series-d":            "Series D",
                "@series-e":            "Series E",
                "@series-f":            "Series F",
                "@texture":             "Texture"
            }
        },
        datavizConstantsHierarchy = {
            "Title, legend & charting area": {
                "chart.title.color":                       "Title color",
                "chart.legend.labels.color":               "Legend text color",
                "chart.chartArea.background":              "Charting area"
            },

            "Axes": {
                "chart.seriesDefaults.labels.color":       "Series text color",
                "chart.axisDefaults.line.color":           "Axis line color",
                "chart.axisDefaults.labels.color":         "Axis labels color",
                "chart.axisDefaults.minorGridLines.color": "Minor grid lines color",
                "chart.axisDefaults.majorGridLines.color": "Major grid lines color",
                "chart.axisDefaults.title.color":          "Axis title color"
            },

            "Series colors": {
                "chart.seriesColors[0]":                   "Color #1",
                "chart.seriesColors[1]":                   "Color #2",
                "chart.seriesColors[2]":                   "Color #3",
                "chart.seriesColors[3]":                   "Color #4",
                "chart.seriesColors[4]":                   "Color #5",
                "chart.seriesColors[5]":                   "Color #6"
            },

            "Gauge": {
                "gauge.pointer.color":                     "Pointer color",
                "gauge.scale.rangePlaceholderColor":       "Range placeholder color",
                "gauge.scale.labels.color":                "Scale labels text color",
                "gauge.scale.minorTicks.color":            "Minor ticks color",
                "gauge.scale.majorTicks.color":            "Major ticks color",
                "gauge.scale.line.color":                  "Scale line color"
            }
        };

    return {
        webConstants: new kendo.LessTheme({
            less: window.less,
            constants: webConstants
        }),
        datavizConstants: new kendo.JsonConstants({
            constants: datavizConstants
        }),
        webConstantsHierarchy: webConstantsHierarchy,
        datavizConstantsHierarchy: datavizConstantsHierarchy
    };
};
