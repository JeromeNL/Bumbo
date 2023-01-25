"use strict";

// Class Definition
var KTAppInboxLib = function() {
    // Private properties
    var _asideEl;
    var _listEl;
    var _viewEl;
    var _composeEl;
    var _replyEl;

    // Private Methods
    var _initEditor = function(form, editorId) {
        if (!form) {
            return;
        }
        
        // init editor
        var options = {
            modules: {
                toolbar: {}
            },
            placeholder: 'Type message...',
            theme: 'snow'
        };

        // Init editor
        var editor = new Quill('#' + editorId, options);

        // Customize editor
        var toolbar = form.querySelector('.ql-toolbar');
        var editor = form.querySelector('.ql-editor');

        if (toolbar) {
            KTUtil.addClass(toolbar, 'px-5 border-top-0 border-start-0 border-end-0');
        }

        if (editor) {
            KTUtil.addClass(editor, 'px-8');
        }
    }

    var _initForm = function(form, editorId) {
        if (!form) {
            return;
        }

        // Init autocompletes
        var to = form.querySelector('[name=compose_to]');

        var tagifyTo = new Tagify(to, {
            delimiters: ", ", // add new tags when a comma or a space character is entered
            maxTags: 10,
            blacklist: ["fuck", "shit", "pussy"],
            keepInvalidTags: true, // do not remove invalid tags (but keep them marked as invalid)
            whitelist: [{
                value: 'Chris Muller',
                email: 'chris.muller@wix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-1.jpg',
                class: 'tagify__tag--primary'
            }, {
                value: 'Nick Bold',
                email: 'nick.seo@gmail.com',
                initials: 'SS',
                initialsState: 'warning',
                pic: ''
            }, {
                value: 'Alon Silko',
                email: 'alon@keenthemes.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-5.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Sara Loran',
                email: 'sara.loran@tilda.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-6.jpg'
            }, {
                value: 'Eric Davok',
                email: 'davok@mix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-12.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Lina Nilson',
                email: 'lina.nilson@loop.com',
                initials: 'LN',
                initialsState: 'danger',
                pic: './assets/media/avatars/300-21.jpg'
            }],
            templates: {
                dropdownItem: function(tagData) {
                    try {
                        var html = '';

                        html += '<div class="tagify__dropdown__item">';
                        html += '   <div class="d-flex align-items-center">';
                        html += '       <span class="symbol sumbol-' + (tagData.initialsState ? tagData.initialsState : '') + ' me-2">';
                        html += '           <span class="symbol-label" style="background-image: url(\'' + (tagData.pic ? tagData.pic : '') + '\')">' + (tagData.initials ? tagData.initials : '') + '</span>';
                        html += '       </span>';
                        html += '       <div class="d-flex flex-column">';
                        html += '           <a href="#" class="text-gray-800 text-hover-primary fw-bold">' + (tagData.value ? tagData.value : '') + '</a>';
                        html += '           <span class="text-muted fw-bold">' + (tagData.email ? tagData.email : '') + '</span>';
                        html += '       </div>';
                        html += '   </div>';
                        html += '</div>';

                        return html;
                    } catch (err) {}
                }
            },
            transformTag: function(tagData) {
                tagData.class = 'tagify__tag tagify__tag--primary';
            },
            dropdown: {
                classname: "color-blue",
                enabled: 1,
                maxItems: 5
            }
        });

        var cc = form.querySelector('[name=compose_cc]');
        var tagifyCc = new Tagify(cc, {
            delimiters: ", ", // add new tags when a comma or a space character is entered
            maxTags: 10,
            blacklist: ["fuck", "shit", "pussy"],
            keepInvalidTags: true, // do not remove invalid tags (but keep them marked as invalid)
            whitelist: [{
                value: 'Chris Muller',
                email: 'chris.muller@wix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-1.jpg',
                class: 'tagify__tag--primary'
            }, {
                value: 'Nick Bold',
                email: 'nick.seo@gmail.com',
                initials: 'SS',
                initialsState: 'warning',
                pic: ''
            }, {
                value: 'Alon Silko',
                email: 'alon@keenthemes.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-5.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Sara Loran',
                email: 'sara.loran@tilda.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-6.jpg'
            }, {
                value: 'Eric Davok',
                email: 'davok@mix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-12.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Lina Nilson',
                email: 'lina.nilson@loop.com',
                initials: 'LN',
                initialsState: 'danger',
                pic: './assets/media/avatars/300-21.jpg'
            }],
            templates: {
                dropdownItem: function(tagData) {
                    try {
                        var html = '';

                        html += '<div class="tagify__dropdown__item">';
                        html += '   <div class="d-flex align-items-center">';
                        html += '       <span class="symbol sumbol-' + (tagData.initialsState ? tagData.initialsState : '') + ' me-2">';
                        html += '           <span class="symbol-label" style="background-image: url(\'' + (tagData.pic ? tagData.pic : '') + '\')">' + (tagData.initials ? tagData.initials : '') + '</span>';
                        html += '       </span>';
                        html += '       <div class="d-flex flex-column">';
                        html += '           <a href="#" class="text-gray-800 text-hover-primary fw-bold">' + (tagData.value ? tagData.value : '') + '</a>';
                        html += '           <span class="text-muted fw-bold">' + (tagData.email ? tagData.email : '') + '</span>';
                        html += '       </div>';
                        html += '   </div>';
                        html += '</div>';

                        return html;
                    } catch (err) {}
                }
            },
            transformTag: function(tagData) {
                tagData.class = 'tagify__tag tagify__tag--primary';
            },
            dropdown: {
                classname: "color-blue",
                enabled: 1,
                maxItems: 5
            }
        });

        var bcc = form.querySelector('[name=compose_bcc]');
        var tagifyBcc = new Tagify(bcc, {
            delimiters: ", ", // add new tags when a comma or a space character is entered
            maxTags: 10,
            blacklist: ["fuck", "shit", "pussy"],
            keepInvalidTags: true, // do not remove invalid tags (but keep them marked as invalid)
            whitelist: [{
                value: 'Chris Muller',
                email: 'chris.muller@wix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-1.jpg',
                class: 'tagify__tag--primary'
            }, {
                value: 'Nick Bold',
                email: 'nick.seo@gmail.com',
                initials: 'SS',
                initialsState: 'warning',
                pic: ''
            }, {
                value: 'Alon Silko',
                email: 'alon@keenthemes.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-5.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Sara Loran',
                email: 'sara.loran@tilda.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-6.jpg'
            }, {
                value: 'Eric Davok',
                email: 'davok@mix.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-12.jpg'
            }, {
                value: 'Sam Seanic',
                email: 'sam.senic@loop.com',
                initials: '',
                initialsState: '',
                pic: './assets/media/avatars/300-20.jpg'
            }, {
                value: 'Lina Nilson',
                email: 'lina.nilson@loop.com',
                initials: 'LN',
                initialsState: 'danger',
                pic: './assets/media/avatars/300-21.jpg'
            }],
            templates: {
                dropdownItem: function(tagData) {
                    try {
                        var html = '';

                        html += '<div class="tagify__dropdown__item">';
                        html += '   <div class="d-flex align-items-center">';
                        html += '       <span class="symbol sumbol-' + (tagData.initialsState ? tagData.initialsState : '') + ' me-2">';
                        html += '           <span class="symbol-label" style="background-image: url(\'' + (tagData.pic ? tagData.pic : '') + '\')">' + (tagData.initials ? tagData.initials : '') + '</span>';
                        html += '       </span>';
                        html += '       <div class="d-flex flex-column">';
                        html += '           <a href="#" class="text-gray-800 text-hover-primary fw-bold">' + (tagData.value ? tagData.value : '') + '</a>';
                        html += '           <span class="text-muted fw-bold">' + (tagData.email ? tagData.email : '') + '</span>';
                        html += '       </div>';
                        html += '   </div>';
                        html += '</div>';

                        return html;
                    } catch (err) {}
                }
            },
            transformTag: function(tagData) {
                tagData.class = 'tagify__tag tagify__tag--primary';
            },
            dropdown: {
                classname: "color-blue",
                enabled: 1,
                maxItems: 5
            }
        });

        // CC input show
        KTUtil.on(form, '[data-inbox="cc-show"]', 'click', function(e) {
            var input = form.querySelector('.inbox-to-cc');
            KTUtil.removeClass(input, 'd-none');
            KTUtil.addClass(input, 'd-flex');
            form.querySelector("[name=compose_cc]").focus();

            _updateEditorScroll(editorId);
        });

        // CC input hide
        KTUtil.on(form, '[data-inbox="cc-hide"]', 'click', function(e) {
            var input = form.querySelector('.inbox-to-cc');
            KTUtil.removeClass(input, 'd-flex');
            KTUtil.addClass(input, 'd-none');

            _updateEditorScroll(editorId);
        });

        // BCC input show
        KTUtil.on(form, '[data-inbox="bcc-show"]', 'click', function(e) {
            var input = form.querySelector('.inbox-to-bcc');
            KTUtil.removeClass(input, 'd-none');
            KTUtil.addClass(input, 'd-flex');
            form.querySelector("[name=compose_bcc]").focus();

            _updateEditorScroll(editorId);
        });

        // BCC input hide
        KTUtil.on(form, '[data-inbox="bcc-hide"]', 'click', function(e) {
            var input = form.querySelector('.inbox-to-bcc');
            KTUtil.removeClass(input, 'd-flex');
            KTUtil.addClass(input, 'd-none');

            _updateEditorScroll(editorId);
        });
    }

    var _initAttachments = function(container, editorId) {
        var id = '#' + container.getAttribute('id');
        var previewNode = $(id + " .dropzone-item");

        previewNode.id = "";
        var previewTemplate = previewNode.parent(".dropzone-items").html();
        previewNode.remove();
        
        var myDropzone = new Dropzone(id, { // Make the whole body a dropzone
            url: "https://keenthemes.com/scripts/void.php", // Set the url for your upload script location
            parallelUploads: 20,
            maxFilesize: 1, // Max filesize in MB
            previewTemplate: previewTemplate,
            previewsContainer: id + " .dropzone-items", // Define the container to display the previews
            clickable: id + "_select" // Define the element that should be used as click trigger to select files.
        });
        
        myDropzone.on("addedfile", function(file) {
            // Hookup the start button
            $(document).find( id + " .dropzone-item").css("display", "");
            _updateEditorScroll(editorId);
        });

        myDropzone.on("removedfile", function(file) {
            _updateEditorScroll(editorId);
        });

        // Update the total progress bar
        myDropzone.on("totaluploadprogress", function(progress) {
            $( id + " .progress-bar").css("width", progress + "%");
        });

        myDropzone.on("sending", function(file) {
            // Show the total progress bar when upload starts
            $( id + " .progress-bar").css("opacity", "1");
        });

        // Hide the total progress bar when nothing"s uploading anymore
        myDropzone.on("complete", function(progress) {
            var thisProgressBar = id + " .dz-complete";

            setTimeout(function(){
                $( thisProgressBar + " .progress-bar, " + thisProgressBar + " .progress").css("opacity", "0");
            }, 300)
        });
    }

    var _updateEditorScroll = function(editor) {
        var el = document.querySelector('#' + editor);
        var scroll = KTScroll.getInstance(el);
        if (scroll) {
            scroll.update();
        }
    }

    // Public methods
    return {
        // Public functions
        initEditor: function(form, editor) {
            _initEditor(form, editor);
        },

        initForm: function(form, editor) {
            _initForm(form, editor);
        },

        initAttachments: function(container, editor) {
            _initAttachments(container, editor);
        }
    };
}();

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTAppInboxLib;
}