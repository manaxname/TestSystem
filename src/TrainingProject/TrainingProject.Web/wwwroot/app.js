﻿function customRedirect(url) {
    window.location.href = window.location.origin + "/" + url;
}

function send(url) {
    $.ajax({
        type: "post",
        url: url,
        success: function (data) {
            //console.log the response
            console.log(data);
            //Send another request in 10 seconds.
            setTimeout(function () {
                send();
            }, 10000);
        }
    });
}

function startTimer(duration, display, getUrl, redirectUrl) {
    var timer = duration, minutes, seconds;
    var i = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);

        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;

        display.textContent = minutes + ":" + seconds;

        if (--timer < 0) {
            $.get(getUrl)
                .done(function () {
                    customRedirect(redirectUrl);
                });
            clearInterval(i);
        }
    }, 1000);
}