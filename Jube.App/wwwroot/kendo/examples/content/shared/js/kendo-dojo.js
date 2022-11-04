 (function($, window) {
    var dojo = {
        postSnippet: function (snippet, baseUrl) {
            snippet = dojo.fixCDNReferences(snippet);
            snippet = dojo.addBaseRedirectTag(snippet, baseUrl);
            snippet = dojo.addConsoleScript(snippet);
            snippet = dojo.fixLineEndings(snippet);
            snippet = dojo.replaceCommon(snippet, window.selectedThemeCommon);
            snippet = dojo.replaceTheme(snippet, window.selectedTheme);
            snippet = window.btoa(encodeURIComponent(snippet));

            var form = $('<form method="post" action="' + dojo.configuration.url + '" target="_blank" />').hide().appendTo(document.body);
            $("<input name='snippet'>").val(snippet).appendTo(form);

            if ($("#mobile-application-container").length) {
                $("<input name='mode'>").val("ios7").appendTo(form);
            }

            form.submit();
        },
        replaceCommon: function(code, common) {
            if (common) {
                if (/-empty/.test(common)) {
                    code = code.replace(/&#10;\s*<link[^>]*common\.min\.css[^>]*>/, "");
                } else {
                    code = code.replace(/common\.min\.css/, common + ".min.css");
                }
            }

            return code;
        },
        replaceTheme: function(code, theme) {
            if (theme) {
                if (/(default-v2|bootstrap-v4|material-v2)/.test(theme)) {
                    code = code.replace(/&#10;\s*<link[^>]*default\.mobile\.min\.css[^>]*>/, "");
                }

                code = code.replace(/default(\.mobile)?\.min\.css/g, theme + "$1.min.css");
            }

            return code;
        },
        addBaseRedirectTag: function (code, baseUrl) {
            return code.replace(
                '<head>',
                '<head>\n' +
                '    <base href="' + baseUrl + '">\n' +
                '    <style>html { font-size: 14px; font-family: Arial, Helvetica, sans-serif; }</style>'
            );
        },
        addConsoleScript: function (code) {
            if (code.indexOf("kendoConsole") !== -1) {
                var styleReference = '    <link rel="stylesheet" href="../content/shared/styles/examples-offline.css">\n';
                var scriptReference = '    <script src="../content/shared/js/console.js"></script>\n';
                code = code.replace("</head>", styleReference + scriptReference + "</head>");
            }

            return code;
        },
        fixLineEndings: function (code) {
            return code.replace(/\n/g, "&#10;");
        },
        fixCDNReferences: function (code) {
            return code.replace(/<head>[\s\S]*<\/head>/, function (match) {
                return match
                    .replace(/src="js\//g, "src=\"" + dojo.configuration.cdnRoot + "/js/")
                    .replace(/href="\/?/g, "href=\"" + dojo.configuration.cdnRoot + "/");
            });
        }
    };

    $.extend(window, {
        dojo: dojo
    });
})(jQuery, window);
