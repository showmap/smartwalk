// ==========================================================================
// Project:     Text Collapse
// Description: Collapses the long texts and adds the gradient at the bottom.
// Copyright:   ©2014 Ievgen Tiutiunnyk
// License:     Licensed under the MIT license
// Version:     1.0
// ==========================================================================

!function ($) {

    $.fn.textCollapse = function(height) {
        var collapsedHeight = Math.max($.fn.textCollapse.defaults.collapsedHeight, height || 0);

        return this.each(function() {
            var $this = $(this);

            if ($this.height() > collapsedHeight) {
                $this.data("originalHeight", $this.height());
                $this.addClass("collapsed-text");
                $this.css({ height: collapsedHeight + "px", cursor: "pointer", overflow: "hidden" });

                $this.click(function() {
                    var $this = $(this);

                    if ($this.height() > collapsedHeight) {
                        $this.animate({ height: collapsedHeight }, 600, null, function() {
                            $this.addClass("collapsed-text");
                        });
                    } else {
                        $this.removeClass("collapsed-text");
                        $this.animate({ height: $this.data("originalHeight") }, 600);
                    }
                });
            }
        });
    };

    $.fn.textCollapse.defaults = {
        collapsedHeight: 100
    };
}(window.jQuery);