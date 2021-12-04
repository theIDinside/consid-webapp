// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const host = window.location.host;
// wrapper function, that looks cleaner
function onDocumentLoaded(cb) {
  document.addEventListener("DOMContentLoaded", cb);
}

// when we want to request how many items exists in a specific category; this route is defined in CategoryController.cs
function getCategoryItemCount(id) {
  return fetch(`/Category/GetCategoryItemCount/${id}`).then((res) =>
    res.json()
  );
}
// asynchronous call, from the edit/details page of an employee. With this, we can request from the server, at
// any time we want, the info of a particular employee, without being tied down to the architecture of ASP Core.
function getEmployeeInfo(id) {
  return fetch(`/Employees/GetEmployeeInfo/${id}`).then((res) => res.json());
}

function setClickHandler(subscriber, publisher) {
  subscriber.addEventListener("click", publisher);
}
// this is used, for creating <a> links on the fly, for instance, when we are editing an employee record
// we create an <a> HTML element with it's href attribute set to this, so that we can navigate directly to it's (possible) manager
function route(uri) {
  window.location.href = `http://${host}/${uri}`;
}

// sets the selection of <select> to a value (if it exists)
// finds the DOM element with the id `dropDownListElementId` and sets the selected value to `value
function setSelection(value, dropDownListElementId) {
  let dropdown = document.getElementById(dropDownListElementId);
  if (dropdown) {
    dropdown.value = value;
  } else {
    logElementNotFound(setSelection, dropDownListElementId);
  }
}

// Function that makes sure all inputs are trimmed. We don't want to store "    some item name ", instead we want to store "some item name"
function formInputTrimmer() {
  let forms = document.getElementsByTagName("form");
  for (let f of forms) {
    f.addEventListener("submit", (evt) => {
      let f = document.getElementById(evt.target.id);
      for (let el of f.elements) {
        if (el.type === "text") {
          console.log(`element is a text type.. trimming`);
          el.value = el.value.trim();
        }
      }
    });
  }
}
