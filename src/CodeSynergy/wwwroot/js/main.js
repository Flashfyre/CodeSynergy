$(function () {

    $(document).ready(function () {
        $(window).on("resize load", function () {
            $('.modal:visible').each(centerModal);
        });

        $("#btnLogin").on("click", function () {
            loadModal("Account/Login");
        });
    });

    function loadModal(path) {
        $("#modal").html('<div class="loading-container blue"><i class="loading fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i>' +
            '<span class="sr-only">Loading...</span></div>');
        switch (path) {
            case "Account/Login":
                $("#modal").load(path, onLoginLoad);
                break;
            case "Account/Register":
                $("#modal").load(path, onRegisterLoad);
                break;
        }
    }

    function initModal(dialog) {
        $('<span class="btnCloseModal" style="float: right"><i class="fa fa-3x fa-times-circle blue" data-toggle="modal" data-target="#modal"></i></span>')
            .insertAfter($(dialog).children("h2:first-child"))
    }

    function onRegisterLoad() {
        initModal(this);
        centerModal(this);
        $("#linkLogin").on("click", function () {
            loadModal("Account/Login");
        });
    }

    function onLoginLoad() {
        initModal(this);
        centerModal(this);
        $("#linkRegister").on("click", function () {
            loadModal("Account/Register");
        });
    }

    function centerModal() {
        centerModal(this);
    }

    function centerModal(dialog) {
        var offset = ($(window).height() - $(dialog).height()) / 2,
       bottomMargin = parseInt($(dialog).css('marginBottom'), 10);

        // Make sure you don't hide the top part of the modal w/ a negative margin if it's longer than the screen height, and keep the margin equal to the bottom margin of the modal
        if (offset < bottomMargin)
            offset = bottomMargin;

        $(dialog).css("margin-top", offset);
    }
});
