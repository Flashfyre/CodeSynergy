var githubFileContents = "";
var plugins = [
        "advlist autolink lists link image charmap github charcount",
        "searchreplace code fullscreen preview autosave",
        "insertdatetime media table contextmenu paste imagetools emoticons codesample"
];
var toolbar = "undo redo | bold italic underline strikethrough | bullist numlist outdent indent | blockquote link image emoticons | codesample github | preview";
var css = [
    '//www.tinymce.com/css/codepen.min.css',
    '/lib/font-awesome/css/font-awesome.css',
    '/css/prism.css'
];
var languages = [
        { text: 'ActionScript', value: 'actionscript' },
        { text: 'AppleScript', value: 'applescript' },
        { text: 'ASP.NET', value: 'aspnet' },
        { text: 'AutoHotkey', value: 'autohotkey' },
        { text: 'Bash', value: 'bash' },
        { text: 'F#', value: 'fsharp' },
        { text: 'Git', value: 'git' },
        { text: 'C', value: 'c' },
        { text: 'C#', value: 'csharp' },
        { text: 'C++', value: 'cpp' },
        { text: 'CSS', value: 'css' },
        { text: 'HTML/XML', value: 'markup' },
        { text: 'Java', value: 'java' },
        { text: 'JavaScript', value: 'javascript' },
        { text: 'JSON', value: 'json' },
        { text: 'Less', value: 'less' },
        { text: 'Objective C', value: 'objectivec' },
        { text: 'Pascal', value: 'pascal' },
        { text: 'Perl', value: 'perl' },
        { text: 'PHP', value: 'php' },
        { text: 'Powershell', value: 'powershell' },
        { text: 'Python', value: 'python' },
        { text: 'Ruby', value: 'ruby' },
        { text: 'Scala', value: 'scala' },
        { text: 'SQL', value: 'sql' },
        { text: 'Swift', value: 'swift' }
];

function initEditors() {
    tinymce.init({
        selector: 'textarea:not(#modal textarea)',
        height: 128,
        menubar: false,
        plugins: plugins,
        toolbar: toolbar,
        skin: "codesynergy",
        content_css: css,
        codesample_languages: languages,
        color_picker_callback: function (callback, value) {
            callback('#FF00FF');
        },
        plugin_preview_width: $(".container").innerWidth(),
        plugin_preview_height: window.innerHeight * 0.8,
        setup: function (editor) {
            editor.on("OpenWindow", function (e) {
                if (e.win.features.title == "Preview") {
                    // Hacky method of adding PrismJS syntax highlighting to the Preview window
                    e.win.$el.html(e.win.$el.html().replace("prism.css%22%3E", "prism.css%22%3E%3Cscript%20src%3D%22%2Fjs%2Fprism.js%22%3E%3C%2Fscript%3E"));
                } else if (e.win.features.title == "GitHub") {
                    $(e.win.$el).prepend('<script type="text/javascript" src="/js/initgithubplugin.js"></script>');
                }
            });
        },
        init_instance_callback: function (editor) {
            // Load a script using a unique instance of the script loader
            var scriptLoader = new tinymce.dom.ScriptLoader();

            scriptLoader.load('/js/prism.js');
            $(editor.contentDocument.head).append('<script type="text/javascript" src="/js/prism.js"></script>');
        }
    });
}

function initModalEditors() {
    tinymce.init({
        selector: '#modal textarea',
        height: 240,
        width: 466,
        menubar: false,
        plugins: plugins,
        toolbar: toolbar,
        skin: "codesynergy",
        content_css: css,
        codesample_languages: languages,
        color_picker_callback: function (callback, value) {
            callback('#FF00FF');
        },
        plugin_preview_width: $(".container").innerWidth(),
        plugin_preview_height: window.innerHeight * 0.8,
        setup: function (editor) {
            editor.on("OpenWindow", function (e) {
                if (e.win.features.title == "Preview") {
                    // Hacky method of adding PrismJS syntax highlighting to the Preview window
                    e.win.$el.html(e.win.$el.html().replace("prism.css%22%3E", "prism.css%22%3E%3Cscript%20src%3D%22%2Fjs%2Fprism.js%22%3E%3C%2Fscript%3E"));
                } else if (e.win.features.title == "GitHub") {
                    $(e.win.$el).prepend('<script type="text/javascript" src="/js/initgithubplugin.js"></script>');
                }
            });
        },
        init_instance_callback: function (editor) {
            // Load a script using a unique instance of the script loader
            var scriptLoader = new tinymce.dom.ScriptLoader();

            scriptLoader.load('/js/prism.js');
            $(editor.contentDocument.head).append('<script type="text/javascript" src="/js/prism.js"></script>');
        }
    });
}

$(document).ready(function () {
    initEditors();
});