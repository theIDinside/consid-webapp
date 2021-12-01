// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const host = window.location.host;

function onDocumentLoaded(cb) {
  document.addEventListener("DOMContentLoaded", cb);
}

function getCategoryItemCount(id) {
  return fetch(`/Category/GetCategoryItemCount/${id}`).then((res) =>
    res.json()
  );
}

function getEmployeeInfo(id) {
  return fetch(`/Employees/GetEmployeeInfo/${id}`).then((res) => res.json());
}

function setHoverHighlight(element, propertyClass = "hover-active") {
  element.addEventListener("mouseover", (e) => {
    element.classList.toggle(propertyClass, true);
  });
  element.addEventListener("mouseout", (e) => {
    element.classList.toggle(propertyClass, false);
  });
}

function setClickHandler(subscriber, publisher) {
  subscriber.addEventListener("click", publisher);
}

function routeAction(el, uri) {
  setClickHandler(el, () => (window.location.href = `http://${host}/${uri}`));
}

function route(uri) {
  window.location.href = `http://${host}/${uri}`;
}
