function lockClick(url, topic_Id) {
    $.ajax({
        //beforeSend: function () {
        //    $('#' + loaderId).show();
        //},
        type: "get",
        url: url,
        success: function (data) {

            var topicId = data['topicId'];
            var isLocked = data['isLocked'];

            if (isLocked) {
                $('#buttonLock' + topicId).attr("onclick", `lockClick('/Test/LockTopic?TopicId=${topicId}&IsLocked=false', ${topic_Id})`).text("unlock");
                $('#buttonLockStatus' + topicId).text("(locked)");
            }
            else {
                $('#buttonLock' + topicId).attr("onclick", `lockClick('/Test/LockTopic?TopicId=${topicId}&IsLocked=true', ${topic_Id})`).text("lock");
                $('#buttonLockStatus' + topicId).text("(unlocked)");
            }
            //$('#' + loaderId).hide();
        }
    });
}

function filterClick() {
    $("#filter").click(function () {
        $("#spinner").css('display', 'inline-block');
    });
}

function btnPublicTopicsClick() {
    var btnPublicTopics = $("#buttonPublicTopics");
    var btnPersonalTopics = $("#buttonPersonalTopics");
    var spinner = $("#spinner");

    spinner.css('display', 'inline-block');

    if (btnPublicTopics.hasClass("btn-light")) {
        btnPublicTopics.removeClass("btn-light").addClass("btn-primary");
        btnPersonalTopics.removeClass("btn-primary").addClass("btn-light")
    }

    $("#topicTitle").remove();
    $("<h1 id='topicTitle'>Public Tests</h1>").insertAfter(btnPersonalTopics);
}

function btnPersonalTopicsClick() {
    var btnPublicTopics = $("#buttonPublicTopics");
    var btnPersonalTopics = $("#buttonPersonalTopics");
    var spinner = $("#spinner");

    spinner.css('display', 'inline-block');

    if (btnPersonalTopics.hasClass("btn-light")) {
        btnPersonalTopics.removeClass("btn-light").addClass("btn-primary");
        btnPublicTopics.removeClass("btn-primary").addClass("btn-light")
    }

    $("#topicTitle").remove();
    $("<h1 id='topicTitle'>Personal Tests</h1>").insertAfter(btnPersonalTopics);
}