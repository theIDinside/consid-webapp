function dateValidator(dateInputField) {
    // the checking of value.length == 7, is due to some weird, arcane, inexplicable reason, for 
    // having the JS interpreters, interpreting for instance "2012-11" as a valid date (it becomes 2012-11-01). 
    // however, the same does *not* work for 2012-
    if (dateInputField.value == "" || dateInputField.value == null && dateInputField.value.length == 7)
        return false;
    else {
        try {
            // parse date input
            let d = Date.parse(dateInputField.value);
            if (isNaN(d)) throw new Error("Date format error");
            d = new Date(d);
            console.log(`date: ${d.toLocaleDateString()}`);
        } catch (err) {
            console.log(`date parse error: ${err}`);
            return false;
        }
    }
    return true;
};

/**
 * 
 * @param {any} elementToValidateID - Element to which the error message will be displayed for, if `validator` fails.
 * @param {string} errorMessage - Error message to be displayed in created "message box"
 * @param {any} event - An event to preventDefault on, if `validator` returns false.
 * @param { Function } validator - a closure (that encapsulates it's own context). If this closure returns false, it means validation failed. 
 *                                 Validation logic is provided by the caller.
 */
function validateInputOnSubmit(elementToValidateID, errorMessage, event, validator) {
    if (!validator(elementToValidateID)) {
        let elementValidateDetails = document.getElementById(`${elementToValidateID.id}ValidateSpan`);
        if (!elementValidateDetails) {
            elementValidateDetails = document.createElement("span");
            elementValidateDetails.id = `${elementToValidateID.id}ValidateSpan`;
            elementToValidateID.parentElement.appendChild(elementValidateDetails);
        }
        elementToValidateID.classList.add("invalidInputContent");
        elementValidateDetails.classList.toggle("text-danger", true);
        elementValidateDetails.classList.toggle("field-validation-valid", true);
        elementValidateDetails.classList.toggle("field-validation-error", false);
        while (elementValidateDetails.firstChild) {
            elementValidateDetails.removeChild(elementValidateDetails.firstChild);
        }
        let validateErrMsg = document.createElement("span");
        validateErrMsg.innerText = errorMessage;
        elementValidateDetails.appendChild(validateErrMsg);
        if (event)
            event.preventDefault();
        return false;
    } else {
        let elementValidateDetails = document.getElementById(`${elementToValidateID.id}ValidateSpan`);
        if (elementValidateDetails) {
            while (elementValidateDetails.firstChild) {
                elementValidateDetails.removeChild(elementValidateDetails.firstChild);
            }
        }
        elementToValidateID.classList.toggle("invalidInputContent", false);
        return true;
    }
}