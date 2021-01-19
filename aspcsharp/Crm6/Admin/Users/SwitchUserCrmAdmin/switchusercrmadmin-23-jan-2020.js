var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text());

$(function () {
    // init Select2
    $("#ddlUsers").select2({});

    // Mobile Menu Hide/Show on Trigger
    $('a.mobile-trigger').click(function (event) {
        event.stopPropagation();

        if (!$(this).hasClass('active')) {
            $('.mobile-header .mob-menu').stop(true, true).slideDown(300);
            $(this).addClass('active');
        } else {
            $('.mobile-header .mob-menu').stop(true, true).slideUp(300);
            $(this).removeClass('active');
        }
    });
});