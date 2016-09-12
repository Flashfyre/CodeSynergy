$(function () {

    const modalTransitionSpeed = 500;

    var loggedIn;
    var timeoutTimer;

    $(document).ready(function () {
        loggedIn = $("#login-button").length === 0;

        $(window).on("resize", function () {
            if (loggedIn)
                initMenu(false);
            centerModal();
            $("#main").css("height", (window.innerHeight - $(".navbar")[0].offsetHeight) + "px");
            if (loggedIn)
                $("#main-menu").css("left", window.innerWidth <= 768 ? 0 : $("#menu-button").offset().left - ($("#main-menu")[0].offsetWidth >> 1) + "px");
        });

        $(window).trigger("resize");
        
        if (loggedIn) {

            $("#logout-button").on("click", function () {
                $("#logout-form").submit();
                return false;
            });

            $("#menu-button").on("click", function () {
                $menu = $("#main-menu");
                $menu.toggleClass("open");
                initMenu(false);
            });

            initMenu(true);
        } else {
            $("#login-button").on("click", function () {
                loadModal("Account/Login");
            });
        }

        $('.mvc-grid').mvcgrid();
    });

    function initMenu(initLinks) {
        $menu = $("#main-menu");
        $menu.css({ "top": $menu.hasClass("open") ? $(".navbar")[0].offsetHeight + "px" :
            -($menu.height() - ($(".navbar")[0].offsetHeight - ($(".navbar").height()))) +
            "px", "visibility": "inherit"
        });

        if (initLinks) {
            $(".main-menu-item a").click(function () {
                loadModal($(this).attr("data-path"));
            });
        }
    }

    function loadModal(path) {
        if (path !== null && ($("#modal").attr("unsaved") == null || confirm("Are you sure you want to leave this page?"))) {
            if (path[0] !== "_") {
                console.log("Set timeout in 30 seconds");
                timeoutTimer = setTimeout(function () {
                    loadModal("_" + path);
                }, 30000);
            }
            $("#modal").modal("show").attr("unsaved", null).html('<div class="loading-container"><img class="loading" src="../images/Load.gif" width="96px" height="96px" /></div>');
            $("#modal").off("click");
            path = "../" + path;
            switch (path) {
                case "../Account/Login":
                    resizeModal(440, 600);
                    $("#modal").load(path, onLoginLoad);
                    break;
                case "../Account/Register":
                    resizeModal(800, 600);
                    $("#modal").load(path, onRegisterLoad);
                    break;
                case "../Admin/Moderation":
                    resizeModal(1280, 720);
                    $("#modal").load(path, onModerationLoad);
                    break;
                case "../Admin/UserRoles":
                    resizeModal(800, 600);
                    $("#modal").load(path, onUserRolesLoad);
                    break;
                default:
                    resizeModal(400, 200);
                    $("#modal").load("../Home/Timeout", onTimeoutLoad).attr("data-refresh-path", path.slice(path[0] == '_' ? 4 : 3));
            }
        }
    }

    function initModal(successful) {
        console.log("Cancel timeout");
        clearTimeout(timeoutTimer);
        if (successful) {
            $("#modal").attr("data-refresh-path", null);
        }
        $('<span class="close-modal" style="float: right"><i class="fa fa-3x fa-times-circle blue hover-green" data-toggle="modal" data-target="#modal"></i></span>')
            .insertAfter("#modal table td h2:first-child");
        $("#modal").off("click");
        var modalHeight = ($("#modal").attr("toHeight") != null ? $("#modal").attr("toHeight") : $("#modal")[0].offsetHeight) - (parseInt($("#modal").css("padding-top")) + parseInt($("#modal").css("padding-bottom")) + $("#modal table tr:first-child td")[0].offsetHeight);
        console.log("height: " + modalHeight + ", toHeight: " + $("#modal").attr("toHeight"));
        if ($("#modal table").length === 1 && modalHeight < ($("#modal").attr("toHeight") != null ? $("#modal").attr("toHeight") : $("#modal")[0].offsetHeight)) {
            $("#modal table tr:nth-child(2) td").css("height", modalHeight + "px");
        }
    }

    function resizeModal(width, height) {
        var $dialog = $("#modal").attr("toHeight", height);
        var offset = (window.innerHeight - $dialog.attr("toHeight")) >> 1,
        bottomMargin = parseInt($dialog.css('margin-bottom'), 10);

        if (offset < bottomMargin)
            offset = bottomMargin;

        $dialog.animate({ "width": width + "px", "height": height + "px", "margin-top": offset + "px" }, modalTransitionSpeed, "swing", function () { $(this).attr("toHeight", null) });
    }

    function onRegisterLoad() {
        initModal(true);

        $("#region-form-group").hide();
        $("#city-form-group").hide();

        $("#CountryID").on("change", function () {
            $sel = $("#CountryID :selected");
            $("#RegionID option:not([value=NULL])").remove();
            if ($sel.is("[value=NULL]")) {
                $("#region-form-group").hide();
                $("#city-form-group").hide();
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
        });

        $("#RegionID").on("change", function () {
            if ($("#RegionID :selected").is("[value=NULL]"))
                $("#city-form-group").hide();
            else
                $("#city-form-group").show();
        });

        $("#login-link").on("click", function () {
            loadModal("Account/Login");
        });
    }

    function onLoginLoad() {
        initModal(true);
        $("#register-link").on("click", function () {
            loadModal("Account/Register");
        });
    }

    function onUserRolesLoad() {
        initModal(true);
    }

    function onModerationLoad() {
        initModal(true);
    }

    function onTimeoutLoad() {
        initModal(false);
        $("#refreshLink").click(function () {
            loadModal($("#modal").attr("data-refresh-path"));
        });
    }

    function centerModal() {
        var $dialog = $("#modal");
        var offset = (window.innerHeight - $dialog.height()) >> 1,
            bottomMargin = parseInt($dialog.css('margin-bottom'), 10);

        if (offset < bottomMargin)
            offset = bottomMargin;

        $dialog.animate({ "margin-top": offset }, modalTransitionSpeed, "swing");
    }
});
