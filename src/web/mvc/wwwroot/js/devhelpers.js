// this *assumes* that the function always 
// has this format: function foo(bar, baz) {
//      ...
// } 
// if any other format, the resulting output will be wrong. Used for debugging purposes. 
// It's a ok enough way to get the function signature of a function
function getFunctionSignature(fn) {
    if (typeof fn === "function") {
        let def = fn.toString().split("\n")[0];
        let a = def.lastIndexOf(" {");
        let end = a == -1 ? def.lastIndexOf("{") : a;
        return def.substring(0, end);
    } else {
        return "";
    }
}

// some developer user experience ergonomics
function logElementNotFound(fn, elementId) {
    console.log(`[error: ${getFunctionSignature(fn)}] Could not find DOM element with ID: ${elementId}`);
}

export { logElementNotFound }