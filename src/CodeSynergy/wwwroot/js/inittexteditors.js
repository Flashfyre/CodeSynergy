$(document).ready(function() {
    tinymce.init({
        selector: 'textarea',
        height: 128,
        plugins: [
                "advlist autolink lists link image charmap",
                "searchreplace visualblocks code fullscreen",
                "insertdatetime media table contextmenu paste imagetools"
        ],
        toolbar: "undo redo | styleselect | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        content_css: [
            '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css',
            '//www.tinymce.com/css/codepen.min.css'
        ],
        color_picker_callback: function(callback, value) {
            callback('#FF00FF');
        },
        setup: function (editor) {
            editor.addButton('github', {
                title: 'GitHub',
                icon: 'icon-github',
                onclick: function () {

                }
            });
        },
    });
    $("textarea button i").addClass("blue");
});