(function ($) {
    function endsWith(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }
    var banner = $("#webinar-banner");
    var bannerLink = banner.find("a");
    var trackingAttribute = "data-gtm-event";
    var uniqueLocalStorageBannerKey = "KendoUI2021R1";
    var bannerExpiresAt = Date.UTC(2021, 0, 29); //Note that month value is zero based.
    var showWebinarBanner = true;
    var suites = {
        // "suite": "trackingAttribute"
        "/kendo-demos/": "yellow-strip, KendoUI2018R1ExportWebinar, kendo-overview-page", // for localhost only
        "kendo-demos/": "yellow-strip, KendoUI2018R1ExportWebinar, kendo-demos-page", // for localhost only
        "/kendo-ui/": "yellow-strip, KendoUI2018R1ExportWebinar, kendo-overview-page", // Kendo UI Demos Overview page only
        "/aspnet-mvc/": "yellow-strip, KendoUI2018R1ExportWebinar, mvc-overview-page",
        "/aspnet-core/": "yellow-strip, KendoUI2018R1ExportWebinar, core-overview-page",
        "/php-ui/": "yellow-strip, KendoUI2018R1ExportWebinar, php-overview-page",
        "/jsp-ui/": "yellow-strip, KendoUI2018R1ExportWebinar, jsp-overview-page"
    };

    for (var j in suites) {
        var isOverviewKey = (j === "/kendo-ui/" || j === "/kendo-demos/" || j === "/php-ui/" || j === "/jsp-ui/" || j === "/aspnet-mvc/" || j === "/aspnet-core/");
        var containsKey = location.href.indexOf(j) > 0;
        var endsWithKey = endsWith(location.href, j);

        if (containsKey && (isOverviewKey && endsWithKey || !isOverviewKey && !endsWithKey)) {
            try {
                showWebinarBanner = (typeof bannerExpiresAt == "undefined" || bannerExpiresAt > (new Date()).getTime()) && ("1" !== localStorage.getItem(uniqueLocalStorageBannerKey));
            } catch (e) {
            }

            if (showWebinarBanner) {
                bannerLink.attr(trackingAttribute, suites[j]);
                banner.css("display", "flex").find(".close").on("click", function () {
                    try {
                        localStorage.setItem(uniqueLocalStorageBannerKey, 1);
                        banner.animate({ height: 0 }, function () {
                            banner.hide();
							banner.remove();
                            $(window).trigger("resize");
                        });
                        return true;
                    } catch (e) {
                    }
                });
            }
            break;
        }
    }


})($);