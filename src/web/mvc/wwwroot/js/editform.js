/**
 * Function that emulates a "tabbed" pane, where one can choose from different tabs
 * @param {HTMLElement} tabButton - the DOM element button corresponding to the `tab` tab
 * @param {string} tab - the ID of the "tab" DOM element
 */
function openTab(tabButton, tab) {
  // Get all elements with class="tabcontent" and hide them
  let tabcontent = document.getElementsByClassName("tabcontent");
  for (let element of tabcontent) {
    element.style.display = "none";
  }

  // Get all elements with class="tablinks" and remove the class "active"
  let tabbuttons = document.getElementsByClassName("tab-button");
  for (let i = 0; i < tabbuttons.length; i++) {
    tabbuttons[i].classList.toggle("active", false);
  }

  // Show the current tab, and add an "active" class to the button that opened the tab, to add a bit of pleasant UI highlighting
  document.getElementById(tab).style.display = "block";
  tabButton.classList.toggle("active", true);
}
