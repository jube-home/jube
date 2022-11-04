(function () {
    $(document).on("kendoReady", function () {
        setTimeout(setup, 500);
    });

    function setup() {
        var selectedYear = 2011;

        stocksDataSource.bind('change', function () {
            $("[name=chart-type][value=area]").prop("checked", true);

            var view = this.view(),
                index = $("#company-filtering-tabs").data("kendoTabStrip").select().index();

            // populate detailed stock prices
            populateStockPrices(view[index], index);
        });

        var defaultSeriesColors = ["#70b5dd", "#1083c7", "#1c638d"];

        function populateStockPrices(data, companyIndex) {
            var container = $(".company-info"),
                yearlyStockValues = data.items,
                highest = yearlyStockValues[0].High,
                lowest = yearlyStockValues[0].Low,
                volume = 0,
                metric = "",
                format = function (number) {
                    return kendo.toString(number, "n");
                }

            $.each(yearlyStockValues, function () {
                highest = this.High > highest ? this.High : highest;
                lowest = this.Low < lowest ? this.Low : lowest;
                volume += this.Volume;
            });

            if (volume > 999999999) {
                volume /= 1000000000;
                metric = "billions stocks";
            } else if (volume > 999999) {
                volume /= 1000000;
                metric = "millions stocks";
            }
            function yearlyRelativeValue(stockValues) {
                return stockValues[stockValues.length - 1].Close / stockValues[0].Open * 100;
            }

            var relativeValues = $.map(yearlyStockValues, function (item, index) {
                var value = 100;

                if (index > 0) {
                    value = item.Close * 100 / yearlyStockValues[index - 1].Open;
                }

                return { value: value };
            });

            var companyRelativeGain = $.map(stocksDataSource.view(), function (data, index) {
                return {
                    value: yearlyRelativeValue(data.items) - 100
                };
            });
            var highChart = $("#highest-sparkline").data("kendoSparkline");
            var lowChart = $("#lowest-sparkline").data("kendoSparkline");
            var relativeChart = $("#relative-value-sparkline").data("kendoSparkline");
            var relativePie = $("#relative-value-pie").data("kendoChart");
            var volumeChart = $("#volume-chart").data("kendoChart");

            highChart.setDataSource(yearlyStockValues);
            lowChart.setDataSource(yearlyStockValues);
            relativeChart.setOptions({ dataSource: relativeValues, series: [{ field: "value", type: "line", color: "#4da3d5" }] });
            relativePie.setOptions({ dataSource: companyRelativeGain, series: [{ field: "value", type: "pie" }] });
            volumeChart.setOptions({ dataSource: yearlyStockValues });

            container
                .find(".eoy-closing").text(format(yearlyStockValues[yearlyStockValues.length - 1].Close)).end()
                .find(".highest").text(format(highest)).end()
                .find(".lowest").text(format(lowest)).end()
                .find(".first dt .metric").eq(1).text(metric).end().end()
                .find(".volume").text(format(volume)).end()
                .find(".relative-value").text(format(yearlyRelativeValue(yearlyStockValues) - 100) + "%").end();
        }

        $("[name=chart-type]").on("click", function () {
            var chart = $("#yearly-stock-prices").data("kendoChart"),
                allSeries = chart.options.series,
                newSeriesType = $(this).val();

            chart.options.seriesDefaults.type = newSeriesType;

            for (var series in allSeries) {
                allSeries[series].type = newSeriesType;
                allSeries[series].opacity = newSeriesType == "area" ? .8 : 1;
            }

            chart.redraw();
        });

        var companyInfoTemplate = kendo.template($("#company-info-template").html());

        $(".company-info").each(function () {
            var panel = $(this);
            panel.html(companyInfoTemplate({ name: panel.attr("id") }));
        });

        var yearTabs = $("#year-filtering-tabs").data("kendoTabStrip");
        yearTabs.bind('change', function (e) {
            selectedYear = this.value();

            stocksDataSource.options.transport.read.url = "../" + parentDataFolder + "/dataviz/dashboards/stock-data-" + selectedYear + ".json";

            $(".selected-year").text(selectedYear);

            stocksDataSource.read();
        });
        yearTabs.trigger("change");

        var companyTabs = $("#company-filtering-tabs").data("kendoTabStrip");
        companyTabs.bind('change', function (e) {
            var company = this.value().toLowerCase(),
                index = this.select().index(),
                view = stocksDataSource.view();

            if (view.length) {
                $(".company-info").html(companyInfoTemplate({ name: company }));

                populateStockPrices(view[index], index);
            }
        });

        $(companyTabs.element).find(".k-item").each(function (index) {
            var color = defaultSeriesColors[index];

            $(this).css({
                color: color,
                borderColor: color
            });
        }).end();
    }
})()