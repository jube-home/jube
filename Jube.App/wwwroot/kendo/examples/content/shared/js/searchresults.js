var searchParam = $("[name=q]").val();

var ds = new kendo.data.DataSource({
    transport: {
        parameterMap: function (data) {
            var startParam = 1 + data.skip;
            return {
                // GSE API allows maximum start index 100.
                start: ((startParam >= 100) ? 91 : startParam),
                num: data.pageSize,
                cx: GSC_INSTANCE,
                key: GSC_KEY,
                q: searchParam,
            };
        },
        read: {
            url: GSC_URL // ?q=test&cx=013254944667501211873:hhalkng-fiw&start=1&num=10&key={API_KEY}
        }
    },
    change: function () {
        var that = this;
        var resultsPresent = this.data().length > 0;
        $("#search-container").toggle(resultsPresent);
        $("#no-results").toggle(!resultsPresent);

        setTimeout(function () {
            $(".site-pager").find("[title='More pages']").hide();
            $(".site-pager").find(".k-pager-nav[title='Previous'] .k-icon").text("Previous");
            $(".site-pager").find(".k-pager-nav[title='Next'] .k-icon").text("Next");

            // GSE API allows maximum start index 100.
            if (that.page() >= 10) {
                $(".site-pager").find(".k-pager-nav[title='Next']").addClass("k-state-disabled");
            }
        }, 0);
    },
    requestStart: function(ev) {
        if (!ev.sender.data().length) {
            $(".site-pager").toggle(false);
        }
    },
    requestEnd: function (ev) {
        var response = ev.response;

        if (response) {
            var searchInformation = response.searchInformation;
            var spelling = response.spelling;
            var template = $("#results-message-template").html();

            if (searchInformation && searchInformation.totalResults != "0") {
                $("#results-msg").html(kendo.template(template)({
                    searchInformation: searchInformation,
                    spelling: spelling
                }));

                $(".site-pager").toggle(true);
            }
        }
    },
    error: function () {
        $("#no-results").show();
    },
    serverPaging: true,
    pageSize: 10,
    schema: {
        type: "json",
        data: function (data) {
            if (parseInt(data.searchInformation.totalResults) === 0) {
                return [];
            }

            return data.items.map(function(item) {
                return {
                    title: item.htmlTitle,
                    url: item.link,
                    excerpt: item.htmlSnippet
                };
            });
        },
        total: function (data) {
            return data.searchInformation.totalResults;
        }
    }
});

$("#results").kendoListView({
    dataSource: ds,
    template: $("#results-template").html(),
    dataBound: function () {
        window.scrollTo(0, 0);
    }
});

$(".site-pager").kendoPager({
    dataSource: ds,
    buttonCount: 10,
    messages: {
        previous: "Previous",
        next: "Next",
        display: "",
        empty: ""
    }
});
