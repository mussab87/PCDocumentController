(function ($) {
    'use strict';

    $.extend(true, $.trumbowyg, {

        plugins: {
            firstname: {
                init: function (trumbowyg) {
                    var btnDef = {
                        fn: function () {
                            trumbowyg.saveRange();
                            trumbowyg.execCmd('insertHTML', '[[FirstName]]');
                        }
                        , tag: 'firstname'
                        , ico: 'blockquote'
                        , title: 'Insert Patient\'s First Name'
                        , text: 'Insert Patient\'s First Name'
                    };

                    trumbowyg.addBtnDef('firstname', btnDef);
                }
            }
        }
    });

})(jQuery);
