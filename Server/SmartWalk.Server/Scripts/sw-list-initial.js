// Initial scripts to run before list rendering started.
// This is to define thumbs CSS width before browser started rendering them.

if (typeof sw === "undefined") sw = {};

sw.setCSSRule = function (selector, property, value) {
    for (var i = 0; i < document.styleSheets.length; i++) {
        var ss = document.styleSheets[i];
        var rules = (ss.cssRules || ss.rules);
        var sel = selector.toLowerCase();

        for (var j = 0, len = rules.length; j < len; j++) {
            if (rules[j].selectorText && (rules[j].selectorText.toLowerCase() == sel)) {
                if (value != null) {
                    rules[j].style[property] = value;
                    return;
                } else {
                    if (ss.deleteRule) {
                        ss.deleteRule(j);
                    } else if (ss.removeRule) {
                        ss.removeRule(j);
                    } else {
                        rules[j].style.cssText = "";
                    }
                }
            }
        }
    }
};

sw.fitThumbs = function (listId, thumbSelector, context) {
    var containerElement = document.getElementById(listId);
    if (containerElement == null) return;
    
    var containerWidth = containerElement.offsetWidth;
    if (context.previousWidth != containerWidth) {
        context.previousWidth = containerWidth;
        var itemsCount = Math.max(Math.round(containerWidth / context.defaultWidth), 1);
        sw.setCSSRule(thumbSelector, "width", (containerWidth / itemsCount) + "px");
    }
};