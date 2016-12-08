$(function () {

    const modalTransitionSpeed = 500; // Fade transition speed of modal window in ms
    const linkGridMap = {
        "/": ["questionGrid"],
        "Question/RelatedQuestions": ["relatedQuestionGrid"],
        "Question/SimilarQuestions": ["similarQuestionGrid"],
        "/Starred": ["starredQuestionGrid"],
        "/User*": ["userPostGrid", "userTagGrid"],
        "/Ranking*": ["rankingGrid"],
        "Admin/Moderation": ["userBanGrid", "untrustedURLPatternGrid"],
        "Admin/UserRoles": ["userRoleGrid"]
    }; // Associative array of URLs to their pages' containing grids

    var loggedIn; // Whether the user is logged in
    var timeoutTimer; // Timer to use for the modal window's loading timeout
    var navbarHeight; // Height of the navbar
    var lastWidth = window.innerWidth; // Last window width
    var mainHeight; // Window height without the navbar
    var onReportClicked, onBanClicked; // Click events for report buttons and ban buttons

    $(document).ready(function () {
        loggedIn = $("#login-button").length === 0; // The user is logged in if there is no login button present

        var linkGridUrls = Object.keys(linkGridMap); // Get the URLs that contain grids
        var gridData = JSON.parse(localStorage.getItem("gridData")); // Get the grid data object from local storage

        // Go through each grid URL
        for (var k = 0; k < linkGridUrls.length; k++) {
            var key = linkGridUrls[k];
            var gridNames = linkGridMap[key];
            var $links = $("a[href" + (key[key.length - 1] == "*" ? "^='" + (key = key.slice(0, -1)) : "='" + key) + "'], a[data-path^='" + key + "']"); // Get the links on the page that go to the current URL
            if ($links.length != 0) { // If there is one or more links for the URL
                var linkGridData = {}; // Object to contain data to add to the link URL's
                // Go through the grid names and add the grid data for the current grid to the object
                for (var g = 0; g < gridNames.length; g++) {
                    var gridName = gridNames[g];
                    if (gridData[gridName] != null) {
                        linkGridData[gridName] = gridData[gridName];
                    }
                }

                if (Object.keys(linkGridData).length != 0) { // If there is any data
                    var separator; // Separator (? if there is no query string and & if there is already a query string)
                    // Modify the link's URL (or data-path attribute for certain modal opening buttons) to add the grid data as a query string so the grids load correctly sorted without requiring a reload
                    if ($links.attr("data-path") == null) {
                        separator = $links.attr("href").indexOf("?") > -1 ? "&" : "?";
                        $links.attr("href", $links.attr("href") + separator + $.param(linkGridData));
                    } else {
                        separator = $links.attr("data-path").indexOf("?") > -1 ? "&" : "?";
                        $links.attr("data-path", $links.attr("data-path") + separator + $.param(linkGridData));
                    }
                }
            }
        }

        $(window).on("resize", function () { // When the window is resized
            if (loggedIn) // Initialize the menu if the user is logged in
                initMenu(false);
            centerModal(); // Center the modal window
            // Set the main container's height appropriately according to the navbar
            $("#main").css("height", (mainHeight = ((window.innerHeight - $(".navbar")[0].offsetHeight) - (window.innerWidth <= 768 && $(".navbar-collapse").hasClass("in") && $("#main-menu").hasClass("open") ? $("#main-menu").children()[0].offsetHeight : 0))) + "px");
            // Whether the resize has changed the navbar display mode
            var switchedModes = window.innerWidth > 768 ? lastWidth <= 768 : lastWidth > 768;
            // Width of the scrollbar
            var scrollbarWidth = getScrollbarWidth();
            // If the user is logged in, correctly position the menu
            if (loggedIn) {
                var menuLeft = window.innerWidth <= 768 ? 0 : $("#menu-button").offset().left - ($("#main-menu")[0].offsetWidth >> 1);
                $("#main-menu").css({ "left": menuLeft + "px", "margin-left": menuLeft > 0 ? Math.min(window.innerWidth - menuLeft - $("#main-menu")[0].offsetWidth - scrollbarWidth, 20) + "px" : "0px" });
            }
            // Set the navbar's max width according to the window width and scrollbar width (not necessary in condensed mode)
            $(".navbar-collapse").css("max-width", window.innerWidth <= 768 ? null : (window.innerWidth - scrollbarWidth) + "px");
            if (window.innerWidth <= 768 && navbarHeight == null) // Set the navbar height variable to the total navbar height
                navbarHeight = (parseInt($(".navbar-collapse").css("padding-bottom")) + parseInt($(".navbar-collapse").css("height")) + parseInt($(".navbar-collapse").css("border-width")));
            if (switchedModes) { // If the navbar display mode has switched
                if (innerWidth > 768) // If the mode is expanded mode, adjust the navbar appropriately
                    $(".navbar-collapse").trigger("hide.bs.collapse").removeClass("in");
                if (innerWidth <= 768 && $("#main-menu").hasClass("open")) // If the mode is condensed mode and the menu is open, close the menu
                    $("#menu-button").trigger("click");
                else // If the menu is closed or the mode is expanded mode, set the main menu's top to adjust to the new navbar height
                    $("#main-menu").css("top", getMainMenuTop() + "px");
                if (loggedIn) // If the user is logged in, set the navbar header's min width so that the user profile link has enough space to go to the far right before the menu items
                    $(".navbar-header").css("min-width", "calc(100% - " + Math.max($("ul.navbar-nav")[0].offsetWidth, 163) + "px)");
            }
            lastWidth = window.innerWidth; // Set lastWidth to the new width for the next resize
        });

        $(".navbar-collapse").on("show.bs.collapse", function (e) { // When the navbar menu in condensed mode is opened, move the page content down so it isn't overlapped
            mainHeight -= navbarHeight; // Change the mainHeight to reflect the new size
            $("#main").css({ "margin-top": (navbarHeight + 11) + "px", "height": mainHeight + "px" });
            if (loggedIn) // If the user is logged in, adjust the main menu's top coordinate appropriately
                $("#main-menu").css("top", getMainMenuTop() + "px");
        }).on("hide.bs.collapse", function (e) { // When the navbar menu in condensed mode is closed, move the page content back to its normal vertical placement
            if (loggedIn) // If the user is logged in, close the main menu if it's open (since the navbar menu containing its button is being hidden) and adjust its top coordinate
                $("#main-menu").removeClass("open").css("top", (getMainMenuTop() - navbarHeight) + "px");
            mainHeight += navbarHeight; // Change the mainHeight to reflect the new size
            $("#main").css({ "margin-top": "11px", "height": mainHeight + "px"});
        });
        
        if (loggedIn) { // If the user is logged in
            // Set the navbar header min width so that the user profile link moves to the furthest right that it can without pushing the menu items to a new line
            $(".navbar-header").css("min-width", "calc(100% - " + Math.max($("ul.navbar-nav")[0].offsetWidth, 163) + "px)");

            $("#logout-button").on("click", function () { // Logout when the logout button is clicked
                $("#logout-form").submit();
                $(this).off("click");
                return false;
            });

            $("#mailbox-button").on("click", function () { // Open the mailbox when the mailbox button is clicked and add the return URL (the current page URL)
                loadModal("Mailbox/0?returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
            });

            $("#menu-button").on("click", function () { // When the menu button is clicked, open the menu
                var isOpen = $menu.hasClass("open");
                $menu = $("#main-menu");
                $menu.toggleClass("open");
                if ($(".navbar-collapse").hasClass("in") && window.innerWidth <= 768) { // When in condensed mode
                    var menuHeight = $menu.children()[0].offsetHeight; // Set the new menu height
                    var fullMenuHeight = menuHeight + navbarHeight; // Set the full menu height (menu height + navbar height)
                    if (isOpen) { // If the menu is being closed, adjust the page content top and height appropriately to move it up
                        mainHeight += menuHeight;
                        $("#main").css({ "margin-top": (navbarHeight + 11) + "px", "height": mainHeight + "px" });
                    } else { // If the menu is being opened, adjust the page content top and height appropriately to move it down
                        mainHeight -= menuHeight;
                        $("#main").css({ "margin-top": (fullMenuHeight + 11) + "px", "height": mainHeight + "px" });
                    }
                }
                initMenu(false); // Initialize the menu
            });

            $(".btn-private-message").on("click", function () { // When a private message button is clicked, open the mailbox with a new private message to the user the button is for
                loadModal("Mailbox/0?recipient=" + $(this).attr("data-display-name") + "&returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
            });

            initMenu(true); // Initialize the menu
            initModalLinks(); // Initialize the modal links
        } else {
            $("#login-button").on("click", function () { // When the login button is clicked, open the login page in the modal window
                loadModal("Account/Login?returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
            });
        }

        if ($("#modal").data("init")) { // If the modal is being initialized through a query string, load the page for the URL contained in the init data attribute
            loadModal($("#modal").data("init"), true);
        } else { // If the modal is not being initialized, trigger a resize the for the window 
            $(window).trigger("resize");
        }

        $("#modal").removeData("init"); // Remove the modal's init data attribute if there is one since it's no longer necessary

        onReportClicked = function () { // When a report button is clicked, open the modal window with the report page
            loadModal("Mailbox/ReportItem?reportTypeID=" + $(this).attr("data-report-type-id") + "&reportedItemID=" + $(this).attr("data-reportable-item-id") + "&returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
        };
        onBanClicked = function () { // If a ban button is clicked, open the modal window with the moderation page
            loadModal("Admin/Moderation?displayName=" + $(this).attr("data-display-name"), false);
        };

        $(".btn-report").click(onReportClicked); // Set the click event for report buttons
        $(".btn-ban").click(onBanClicked); // Set the click event for ban buttons
    });

    // Initialize the main menu
    function initMenu(initLinks) {
        // Set the top coordinate to where it should go and ensure the visibility
        $("#main-menu").css({ "top": getMainMenuTop() +
            "px", "visibility": "inherit"
        });

        if (initLinks) { // If main menu modal links should be initialized, set their click events and path data attributes
            $(".main-menu-item a").click(function () {
                loadModal($(this).attr("data-path"), false);
            });
        }
    }

    // Initialize the modal links on the page
    function initModalLinks() {
        $("a.modal-link").click(function () {
            loadModal($(this).attr("data-path"), false);
        });
    }

    // Get the appropriate main menu top coordinate according to the navbar display mode and whether the menu is open
    function getMainMenuTop() {
        $menu = $("#main-menu");
        return $menu.hasClass("open") ? $(".navbar")[0].offsetHeight : -($menu.height() - ($(".navbar")[0].offsetHeight - ($(".navbar").height()))) + (window.innerWidth > 768 || !$(".navbar-collapse").hasClass("in") ? 0 : navbarHeight + $(".navbar-header")[0].offsetHeight);
    }

    // Load a page in the modal window
    function loadModal(path, init) {
        if (path !== null && ($("#modal").attr("unsaved") == null || confirm("Are you sure you want to leave this page?"))) { // Ensure the path is not null and show a confirm dialog if necessary
            if (path[0] !== "_") { // If the page being loaded is not a timeout page
                console.log("Set timeout in 30 seconds");
                timeoutTimer = setTimeout(function () { // Set the timeout timer to load the timeout page in 30s
                    loadModal("_" + path, false);
                }, 30000);
            }
            // Show the modal and add loading elements
            $("#modal").modal("show").attr("unsaved", null).html('<div class="loading-container"><img class="loading" src="/images/Load.gif" width="96px" height="96px" /></div>');
            $("#modal").off("click"); // Remove the modal's click event that would close it if clicked
            path = "/" + path; // Add a forward slash before the path
            var checkPath = path; // String to use to differentiate which page is being loaded according to the URL
            if (checkPath.indexOf("?") > -1) // Remove any query string
                checkPath = checkPath.slice(0, checkPath.indexOf("?"));
            if (checkPath.lastIndexOf("/") > 0 && checkPath.lastIndexOf("/") != checkPath.indexOf("/", 1)) // Remove any unnecessary parts of the URL from checkPath
                checkPath = checkPath.slice(0, checkPath.lastIndexOf("/"));
            // Load a page according to the path with the page's intended modal size
            switch (checkPath) {
                case "/Account/Login":
                    resizeModal(480, 600, init);
                    $("#modal").load(path, onLoginLoad);
                    break;
                case "/Account/Register":
                    resizeModal(800, 600, init);
                    $("#modal").load(path, onRegisterLoad);
                    break;
                case "/Account/Settings":
                    resizeModal(800, 600, init);
                    $("#modal").load(path, onSettingsLoad);
                    break;
                case "/Question/RelatedQuestions":
                    resizeModal(1280, 720, init);
                    $("#modal").load(path, onRelatedQuestionsLoad);
                    break;
                case "/Question/SimilarQuestions":
                    resizeModal(1280, 720, init);
                    $("#modal").load(path, onSimilarQuestionsLoad);
                    break;
                case "/Admin/Moderation":
                    resizeModal(1280, 720, init);
                    $("#modal").load(path, onModerationLoad);
                    break;
                case "/Admin/UserRoles":
                    resizeModal(960, 720, init);
                    $("#modal").load(path, onUserRolesLoad);
                    break;
                default:
                    if (checkPath.startsWith("/Mailbox")) { // If the page is for the Mailbox
                        if (checkPath != "/Mailbox/ReportItem") // If not reporting an item, resize to 1280x720
                            resizeModal(1280, 720, init);
                        else // If reporting an item, resize to 800x600
                            resizeModal(800, 600, init);
                        $("#modal").load(path, onMailboxLoad);
                    } else { // If the page is a timeout, load the timeout page and set the path to load when the refresh page is clicked
                        resizeModal(400, 240, init);
                        $("#modal").load("/Home/Timeout", onTimeoutLoad).attr("data-refresh-path", path.slice(path.indexOf("/") + 1));
                    }
            }
        }
    }

    // Initialize the modal window
    function initModal(successful) {
        console.log("Cancel timeout");
        window.clearTimeout(timeoutTimer);
        if ($("#modal > div#modal-content").children().length > 0) {
            if (successful)
                $("#modal").attr("data-refresh-path", null);
            $('<span class="close-modal" style="float: right"><i class="fa fa-3x fa-times-circle blue hover-green" data-toggle="modal" data-target="#modal"></i></span>')
                .appendTo("#modal > div#modal-header > div");
            $("#modal").off("click");
            updateModalHeight();
        }
    }

    // Update the height of the modal to contain the content correctly
    function updateModalHeight() {
        if ($("#modal > div#modal-content").children().length > 0) {
            var modalHeight = ($("#modal").attr("toHeight") != null ? $("#modal").attr("toHeight") : $("#modal")[0].offsetHeight) - (parseInt($("#modal").css("padding-top")) + parseInt($("#modal").css("padding-bottom")) + $("#modal > div#modal-header")[0].offsetHeight);
            if ($("#modal > div#modal-content").children().length > 0 && modalHeight < ($("#modal").attr("toHeight") != null ? $("#modal").attr("toHeight") : $("#modal")[0].offsetHeight)) {
                $("#modal > div#modal-content").css("height", modalHeight + "px");
            }
        }
    }
    
    // Resize the modal window to a given width and height in pixels
    function resizeModal(width, height, init) {
        var $dialog = $("#modal").attr("toHeight", height);
        var offset = ((window.innerHeight - $dialog.attr("toHeight")) >> 1) + $(".navbar-fixed-top").height(),
        bottomMargin = parseInt($dialog.css('margin-bottom'));

        if (offset < bottomMargin)
            offset = bottomMargin;

        if (!init) {
            $dialog.animate({ "max-width": width + "px", "height": height + "px", "margin-top": offset + "px" }, modalTransitionSpeed, "swing", function () { $(this).attr("toHeight", null); updateModalHeight(); });
        } else {
            $dialog.css({ "max-width": width + "px", "height": height + "px", "margin-top": offset + "px" }).attr("toHeight", null);
        }
    }

    // When the login page is loaded
    function onLoginLoad() {
        initModal(true);
        $("#register-link").on("click", function () {
            loadModal("Account/Register?returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
        });
    }

    // When the registration page is loaded
    function onRegisterLoad() {
        initModal(true);

        $("#region-form-group").hide();
        $("#city-form-group").hide();

        $("#CountryID").on("change", onCountryIDChange);

        $("#RegionID").on("change", onRegionIDChange);

        $("#login-link").on("click", function () {
            loadModal("Account/Login?returnurl=" + window.location.pathname.replace(/%20/g, "_"), false);
        });
    }

    // When the settings page is loaded
    function onSettingsLoad() {
        initModal(true);

        $("#CountryID").on("change", onCountryIDChange);

        $("#RegionID").on("change", onRegionIDChange);
    }

    // When the mailbox is loaded
    function onMailboxLoad() {
        initModal(true);

        // Adjust the height of the list table containers so that they correctly allow scrolling when items exceed the bottom (delay so CSS has time to load)
        window.setTimeout(function() {
            var controlsHeight = $(".mailbox-controls")[0].offsetHeight;
            $(".mailbox-list-table-container").css("height", "calc(100% - " + controlsHeight + "px)");
        }, 10);

        // When the new message button is clicked
        $(".btn-new-message").each(function () {
            $(this).on("click", function () {
                $(".mailbox-item-row.open").trigger("click");
                $(this).prop("disabled", true);
                $("#modal .mce-tinymce").remove();
                $("#mailbox-content-container").addClass("loading").html('<div class="loading-container"><img class="loading" src="/images/Load.gif" width="96px" height="96px" /></div>').load("/Mailbox/NewMessage?returnUrl=" + $(this).parent().parent().parent().attr("data-return-url"), onMailboxContentLoad);
            });
        });

        // When a mailbox item row is clicked
        $(".mailbox-item-row:not(.mailbox-item-row-empty) > td > div.mailbox-item").on("click", function () {
            var isContainerLoading = $("#mailbox-content-container").hasClass("loading");
            if (!isContainerLoading)
                $("#modal .mce-tinymce").remove();
            var $row = $(this).parent().parent();
            if (!$row.hasClass("open")) {
                $(".mailbox-item-row.open").removeClass("open");
                if ($row.attr("data-private-message-id") != null)
                    $(".mailbox-item-row[data-private-message-id=" + $row.attr("data-private-message-id") + "]").addClass("open");
                else
                    $row.addClass("open");
                if (!isContainerLoading) {
                    $("#mailbox-content-container").addClass("loading").html('<div class="loading-container"><img class="loading" src="/images/Load.gif" width="96px" height="96px" /></div>').load("/Mailbox/" + $row.attr("data-mailbox-type-id") + "/" + $row.attr("data-mailbox-item-id")
                        + "/MailboxItem?returnUrl=" + $row.parentsUntil("#mailbox-list-container", "div.tab-pane").attr("data-return-url"), onMailboxContentLoad);
                    $(".btn-new-message").prop("disabled", true);
                }
            } else {
                if ($row.attr("data-private-message-id") != null)
                    $(".mailbox-item-row[data-private-message-id=" + $row.attr("data-private-message-id") + "]").removeClass("open");
                else
                    $row.removeClass("open");
                if (!isContainerLoading)
                    $("#mailbox-content-container").html("");
            }
        });
    }

    // When a page is loaded inside the mailbox
    function onMailboxContentLoad() {
        $("#mailbox-content-container").removeClass("loading");
        $(".btn-new-message").prop("disabled", null);
        $("#mailbox-content-container .btn-report").click(onReportClicked);
        $("#mailbox-content-container .btn-ban").click(onBanClicked);
        $(".private-message-link").on("click", function () {
            $(".mailbox-item-row.open").removeClass("open");
            $("#mailbox-content-container").addClass("loading").html('<div class="loading-container"><img class="loading" src="/images/Load.gif" width="96px" height="96px" /></div>').load("/Mailbox"
                + "/PrivateMessage?privateMessageID=" + $(this).attr("data-private-message-id") + "&returnUrl=" + $(this).parentsUntil("div.scroll-div", "#mailbox-content-container").attr("data-return-url"), onMailboxContentLoad);
            $(".btn-new-message").prop("disabled", true);
        });
    }

    // When the related questions page is loaded
    function onRelatedQuestionsLoad() {
        initModal(true);
    }

    // When the similar questions page is loaded
    function onSimilarQuestionsLoad() {
        initModal(true);
    }

    // When the user roles page is loaded
    function onUserRolesLoad() {
        initModal(true);
    }

    // When the moderation page is loaded
    function onModerationLoad() {
        initModal(true);

        $("#modal .toggle-section").append("&nbsp;").append($(document.createElement("i")).addClass("fa").addClass("fa-compress")).on("click", function () {
            var $section = $(this).parent().find("#" + $(this).attr("data-section-id"));
            $section.toggleClass("section-collapse");
            if ($section.hasClass("section-collapse")) {
                $section.hide(500);
                $(this).children("i").removeClass("fa-compress").addClass("fa-expand");
            } else {
                $section.show(500);
                $(this).children("i").removeClass("fa-expand").addClass("fa-compress");
            }
        });
    }

    // When the timeout page is loaded
    function onTimeoutLoad() {
        initModal(false);
        $("#refreshLink").click(function () {
            loadModal($("#modal").attr("data-refresh-path"), false);
        });
    }

    // Center the modal window
    function centerModal() {
        var $dialog = $("#modal");
        var offset = ((window.innerHeight - parseInt($dialog.css("height"))) >> 1) + $(".navbar-fixed-top").height(),
            bottomMargin = parseInt($dialog.css('margin-bottom'));

        if (offset < bottomMargin)
            offset = bottomMargin;

        $dialog.animate({ "margin-top": offset }, modalTransitionSpeed, "swing");
    }

    // When a Country ID select value is changed
    function onCountryIDChange() {
        $sel = $("#CountryID :selected");
        $("#RegionID option:not([value=''])").remove();
        if ($sel.is("[value='']")) {
            $("#region-form-group").hide().children("div").children("select").children(":first-child").prop("selected", true);
            $("#city-form-group").hide().children("div").children("input").val("");
        } else {
            $.ajax({
                type: "POST",
                url: 'Account/GetRegions',
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: {
                    countryID: $sel.val(),
                    __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
                },
                dataType: "json",
                success: function (data) {
                    for (var r in data)
                        $("#RegionID").append('<option value="' + data[r][0] + '">' + data[r][1] + '</option>');
                    if (data.length !== 0) {
                        $("#region-form-group").show();
                        $("#city-form-group").hide();
                    } else {
                        $("#region-form-group").hide();
                        $("#city-form-group").show();
                    }
                },
                error: function () { alert("Error: Something went wrong retrieving the region list. Please reselect your country to try again"); }
            });
        }
    }

    // When a Region ID select value is changed
    function onRegionIDChange() {
        if ($("#RegionID :selected").is("[value='']"))
            $("#city-form-group").hide();
        else
            $("#city-form-group").show();
    }

    // Get the page's scrollbar's width by creating a hidden container to retrieve it
    // This function is from http://stackoverflow.com/questions/13382516/getting-scroll-bar-width-using-javascript
    function getScrollbarWidth() {
        var outer = document.createElement("div");
        outer.style.visibility = "hidden";
        outer.style.width = "100px";
        outer.style.msOverflowStyle = "scrollbar"; // needed for WinJS apps

        document.body.appendChild(outer);

        var widthNoScroll = outer.offsetWidth;
        // force scrollbars
        outer.style.overflow = "scroll";

        // add innerdiv
        var inner = document.createElement("div");
        inner.style.width = "100%";
        outer.appendChild(inner);        

        var widthWithScroll = inner.offsetWidth;

        // remove divs
        outer.parentNode.removeChild(outer);

        return widthNoScroll - widthWithScroll;
    }
});
