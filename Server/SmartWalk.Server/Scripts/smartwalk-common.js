function collapsableText(config) {
    var $descrText = $(config.targetSelector).first();
    var relativeHeight = config.relativeSelector != null
        ? $(config.relativeSelector).first().height()
        : 0;
    var collapsedHeight = Math.max(relativeHeight, 100);

    if ($descrText.height() > collapsedHeight) {
        $descrText.data("originalHeight", $descrText.height());
        $descrText.addClass(config.gradientClass);
        $descrText.css({ height: collapsedHeight + "px", cursor: "pointer" });

        $descrText.click(function () {
            if ($(this).height() > collapsedHeight) {
                $(this).animate({ height: collapsedHeight }, 600, null, function () {
                    $(this).addClass(config.gradientClass);
                });
            } else {
                $(this).removeClass(config.gradientClass);
                $(this).animate({ height: $(this).data("originalHeight") }, 600);
            }
        });
    }
}