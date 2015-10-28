$(function() {
    $("#IsRepeatable").on("click", function() {
        if ($(this).is(":checked"))
            $(".repeatableTransaction").slideDown(); //show
        else
            $(".repeatableTransaction").slideUp(); //hide
    });
});