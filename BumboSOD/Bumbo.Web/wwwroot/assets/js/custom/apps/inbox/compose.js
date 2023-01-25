"use strict";

// Class definition
var KTAppInboxCompose = function() {
    // Private variables
    var _composeEl;

    // Private methods
    var _init = function() {
        KTAppInboxLib.initEditor( _composeEl, 'kt_inbox_compose_editor' );
        KTAppInboxLib.initAttachments( document.querySelector('#kt_inbox_compose_attachments'), 'kt_inbox_compose_editor' );
        KTAppInboxLib.initForm( document.querySelector('#kt_inbox_compose_form'), 'kt_inbox_compose_editor' );
    }

    // Public methods
    return {
        // Public functions
        init: function() {
            // Init variables
            _composeEl = document.querySelector('#kt_inbox_compose');

            if ( _composeEl !== null ) {
                _init();
            }
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function() {
    KTAppInboxCompose.init();
});

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTAppInboxCompose;
}