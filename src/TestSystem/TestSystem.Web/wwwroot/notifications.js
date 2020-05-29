$(function () {

    var dropmenu = document.getElementById("drop-menu");
    var bell = document.getElementById("bell");
    var bellClickCount = 0;
    var hideDuration = 1000;

    //i'm using "click" but it works with any event
    document.addEventListener('click', function (event) {
        //var isClickInsideDropMenu = dropmenu.contains(event.target);
        var isClickInsideBell = bell == null ? false : bell.contains(event.target);

        if (isClickInsideBell && bellClickCount % 2 == 0) {
            $(dropmenu).slideToggle(hideDuration);
            bellClickCount++;
        }
        else if (isClickInsideBell && bellClickCount % 2 == 1/* || !isClickInsideDropMenu && !isClickInsideBell*/) {
            $(dropmenu).hide(hideDuration);
            bellClickCount++;
        }

    });
});