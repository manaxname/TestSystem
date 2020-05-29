$(function () {
    $("#filter").click(function () {
        $("#spinner").css('display', 'inline-block');;
    });
});

function sendAjax(url, loaderId) {
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
                $('#buttonLock' + topicId).attr("onclick", `sendAjax('/Test/LockTopic?TopicId=${topicId}&IsLocked=false', ${loaderId})`).text("UnLock");
            }
            else {
                $('#buttonLock' + topicId).attr("onclick", `sendAjax('/Test/LockTopic?TopicId=${topicId}&IsLocked=true', ${loaderId})`).text("Lock");
            }
            //$('#' + loaderId).hide();
        }
    });
}