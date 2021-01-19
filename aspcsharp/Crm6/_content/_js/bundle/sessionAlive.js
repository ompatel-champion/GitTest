
$(function () {
    SessionKeepAlive.start(10);
});

SessionKeepAlive =
{
    delay: 10000,
    url: "/_handlers/ping.ashx",
    run: function () {
        $.get(this.url + "?d=" + escape(new Date().getTime()), function (response) {
            //change the color of the heart indicator
            var $heart = $("heart");
            if (!$heart.hasClass("red")) $heart.css("color", "red").addClass("red"); else $heart.css("color", "green").removeClass("red");
        });

        //start again
        setTimeout("SessionKeepAlive.run()", this.delay);
    },
    start: function (delay) { 
        // Convert delay to millis
        this.delay = parseInt(delay) * 60000;
        setTimeout("SessionKeepAlive.run()", this.delay);

        //heart indicator(this is hidden)
        $("<heart style='color:#ddd;margin:0 5px;' class='hide'>&hearts;</heart>").prependTo($(".welcome-message"))
    }
};