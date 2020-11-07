$(document).on('click', '.addQuestion', function () {
    $.ajax({
        url: this.href,
        cache: false,
        success: function (html) {
            $('.questionsRows').append(html);
        }
    });
    return false;
});

$(document).on('click', '.addAnswer', function () {
    var element = this;
    $.ajax({
        url: this.href,
        cache: false,
        success: function (html) {
            $(element).parents('.questionRow').find('.answersRows').append(html);
        }
    });
    return false;
});

$(document).on('click', '.deleteQuestionRow', function () {
    $(this).parents('.questionRow').remove();
    return false;
});

$(document).on('click', '.deleteAnswerRow', function () {
    $(this).parents('.answerRow').remove();
    return false;
});