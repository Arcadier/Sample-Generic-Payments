
jQuery("#agree").click(function (e) {
    e.preventDefault();
    var data = {};
    data["CashKey"] = $("#cashkey").val();
    data["Note"] = $("#note").val();

    $.ajax({
        url: '/cash/agreetopay',
        data: data,
        type: 'POST',
        success: function(response) {
            if (response.result !== undefined) {
                window.location = response.result;
            } else {
                toastr.error('Something went wrong! Please try again later.', 'Error');
            }
        },
        error: function() {
            toastr.error('Failed!', 'Error');
        }
    });
});

jQuery("#cancel").click(function (e) {
    e.preventDefault();
    var data = {};
    data["CashKey"] = $("#cashkey").val();

    $.ajax({
        url: '/cash/canceltopay',
        data: data,
        type: 'POST',
        success: function (response) {
            if (response.result !== undefined) {
                window.location = response.result;
            } else {
                toastr.error('Something went wrong! Please try again later.', 'Error');
            }
        },
        error: function () {
            toastr.error('Failed!', 'Error');
        }
    });
});