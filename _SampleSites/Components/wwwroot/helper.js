// This script uses for checking the namespace conflict has happened or not within the E2E test.
var Toolbelt;
(function (Toolbelt) {
    var Blazor;
    (function (Blazor) {
        function add(a, b) {
            return a + b;
        }
        Blazor.add = add;
    })(Blazor = Toolbelt.Blazor || (Toolbelt.Blazor = {}));
})(Toolbelt || (Toolbelt = {}));

