$(function () {
    $(".max-people").on('change', setButtonValidity);
    $(".date-picker").on('change', setButtonValidity);

    function setButtonValidity() {  
        $(".submit-button").prop('disabled', !isFormValid());
    }

    function isFormValid() {
        const selectedValue = $(".max-people option:selected").val();
        const date = $(".date-picker").val();
        return selectedValue !== "0" && date;
    }
});