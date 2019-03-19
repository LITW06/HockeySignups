//disable button approach
$(function () {

    $("#firstName, #lastName, #email").on('keyup', function() {
        $("#submit-button").prop('disabled', !isFormValid());
    });
    
    function isFormValid() {
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        var email = $('#email').val();

        if (!firstName || !lastName || !email) {
            return false;
        }

        return isValidEmail(email);
    }

    function isValidEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
});


//$(function () {

//    $('#dismiss-alert').click(function () {
//        $('#error-alert').hide();
//    });

//    $("form").on('submit', function () {
//        if (!isFormValid()) {
//            showError('Form has errors, please fix...');
//            return false;
//        }

//        return true;
//    });

//    function showError(message) {
//        $('#error-alert').show();
//        $('#error-message').text(message);
//    }

//    function isFormValid() {
//        var firstName = $('#firstName').val();
//        var lastName = $('#lastName').val();
//        var email = $('#email').val();


//        if (!firstName || !lastName || !email) {
//            if (!firstName) {
//                $("#firstName").addClass('error-background');
//            }
//            return false;
//        }

//        $("#firstName").removeClass('error-background');


//        return isValidEmail(email);
//    }

//    function isValidEmail(email) {
//        var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
//        return re.test(email);
//    }
//});
