$('.form').find('input, textarea').on('keyup blur focus', function (e) {

    var $this = $(this),
        label = $this.prev('label');
    if ($this.val() !== '') {
        label.addClass('active highlight');
    }
    if (e.type === 'keyup') {
        if ($this.val() === '') {
            label.removeClass('active highlight');
        } else {
            label.addClass('active highlight');
        }
    } else if (e.type === 'blur') {
        if ($this.val() === '') {
            label.removeClass('active highlight');
        } else {
            label.removeClass('highlight');
        }
    } else if (e.type === 'focus') {

        if ($this.val() === '') {
            label.removeClass('active highlight');
        }
        else {
            label.addClass('highlight');
        }
    }

});

$('.tab a').on('click', function (e) {

    e.preventDefault();

    $(this).parent().addClass('active');
    $(this).parent().siblings().removeClass('active');

    target = $(this).attr('href');

    $('.tab-content > div').not(target).hide();

    $(target).fadeIn(600);

});

$(document).ready(function () {
    $("#loginLink").click(function (event) {
        event.preventDefault();
        $(".login-wrap").fadeToggle("fast");
        $(".cms-index-index").css("overflow-y", "hidden");
        event.stopPropagation();
    });
    $(".form").click(function (event) {
        event.stopPropagation();
    });
    $(document).keyup(function (e) {
        if (e.keyCode == 27 && $(".login-wrap").css("display") != "none") {
            event.preventDefault();
            $(".login-wrap").fadeToggle("fast");
        }
    });

    $(".login-wrap").click(function (e) {
        if ($(".login-wrap").css("display") != "none") {
            $(".login-wrap").fadeToggle("fast");
            $(".cms-index-index").css("overflow-y", "scroll");
            //$(".login-wrap").css("overflow-y", "hidden");
        }
    });
});
