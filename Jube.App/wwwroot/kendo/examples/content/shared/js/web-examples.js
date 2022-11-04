$(function () {
    populateSearchDataSource(desktopExamples);

    var regex = /\/(kendo-ui|staging-kendo-ui|aspnet-mvc|aspnet-core|staging-mvc|staging-core|production-staging-mvc|production-staging-core|php-ui|jsp-ui)/i;
    var match = regex.exec(window.location.href);

    var basePathName = match ? match[0] + "/" : "/";

    $("#example-search").kendoExampleSearch({
        product: product,
        minLength: 3,
        template: '<a href="' + basePathName + '#: url #"> #: text # </a>',
        dataTextField: "text",
        select: function (e) {
            location.pathname = e.item.find("a").attr("href");
        },
        height: 300
    });

    $("#example-sidebar").kendoResponsivePanel({
        breakpoint: 1200,
        orientation: "left",
        toggleButton: "#sidebar-toggle"
    });

    $("#sidebar-toggle").click(function() {
        $("#example-search").focus();
    });
});
