// Add to the host page after blazor.*.js:  <script src="js/new-tabs.js"></script>
window.implTool = {
    // Moves focus to the first invalid field on Submit, and back to a row button after an action.
    focus: function (id) {
        var el = document.getElementById(id);
        if (!el) return;
        el.focus();
        if (el.scrollIntoView) el.scrollIntoView({ block: 'center' });
    },
    scrollTop: function () { window.scrollTo(0, 0); }
};
