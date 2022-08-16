(function ($) {
    'use strict';

    $.extend(true, $.trumbowyg, {

        plugins: {
            lastname: {
                init: function (trumbowyg) {
                    var btnDef = {
                        fn: function () {
                            trumbowyg.saveRange();
                            trumbowyg.execCmd('insertHTML', '[[LastName]]');
                        }
                        , tag: 'lastname'
                        , ico: 'returnquote'
                        , title: 'Insert Patient\'s Last Name'
                        , text: 'Insert Patient\'s Last Name'
                    };

                    trumbowyg.addBtnDef('lastname', btnDef);
                }
            }
        }
    });

})(jQuery);
