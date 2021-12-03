// Settings for the form in the "Edit" page on a library item, depending on the selected library item type
const EDIT_FORM_TYPE = {
  PLAYABLE: {
    enable: { field: "RunTimeMinutes", group: "runTimeMinutesInput" },
    disable: { field: "Pages", group: "pagesInput" },
  },
  BOOK: {
    enable: { field: "Pages", group: "pagesInput" },
    disable: { field: "RunTimeMinutes", group: "runTimeMinutesInput" },
  },
};

/**
 * Enables the <input> DOM element with id `enable.field` and sets the attribute "hidden = false" for (what should be) the parent <div> with id `enable.group`
 * It does the opposite for <input> with id disable.field and it's parent <div> disable.group
 * @@typedef { { enable: { field: string, group: string }, disable: { field: string, group: string } } } FormItem
 * @@param { { enable: FormItem, disable: FormItem } } state - the <input>s and <div>s to enable and disable
 * @@param { number } runTimeOrPagesValue - the default value to set the enabled <input> to
 * @@param { boolean } borrowable - the value to set the <input> to, which sets the IsBorrowable in the Database
 */
const setEditFormType = (state, runTimeOrPagesValue, borrowable) => {
  let enableElement = document.getElementById(state.enable.field);
  let enableGroup = document.getElementById(state.enable.group);
  enableGroup.hidden = false;
  enableElement.value = runTimeOrPagesValue;
  enableElement.disabled = false;
  enableElement.textContent = runTimeOrPagesValue;

  let disableElement = document.getElementById(state.disable.field);
  let disableGroup = document.getElementById(state.disable.group);
  disableElement.value = null;
  disableElement.textContent = null;
  disableElement.disabled = true;
  disableGroup.hidden = true;

  document.getElementById("IsBorrowable").value = borrowable;
};

/**
 * Disables all elements with id's in `elementNames`, clears their contents and hides them
 * @@param { string[] } elementNames
 */
const disableElements = (elementNames) => {
  for (const name of elementNames) {
    let disableElement = document.getElementById(name);
    disableElement.textContent = null;
    disableElement.value = null;
    disableElement.disabled = true;
    disableElement.hidden = true;
  }
};

/**
 *  Enables all elements with id's in `elementNames`
 */
const enableElements = (elementNames) => {
  for (const n of elementNames) {
    let el = document.getElementById(n);
    el.disabled = false;
    el.hidden = false;
  }
};

/**
 * Event handler for the "Type" selection list, updates the form on the Edit page of a library item
 * @param {any} runTimeOrPagesValue
 */
function updateEditFields(runTimeOrPagesValue) {
  const selectedTypeValue = document.getElementById("Type").value;
  switch (selectedTypeValue) {
    case "reference book":
      // setInputFormType(FORM_LIBRARY_TYPE.BOOK_NON_BORROW);
      // setLibraryItemFormType(FORM_LIBRARY_ITEM_TYPE.BOOK_NON_BORROW);
      setEditFormType(EDIT_FORM_TYPE.BOOK, runTimeOrPagesValue, false);
      break;
    case "book":
      //setInputFormType(FORM_LIBRARY_TYPE.BOOK);
      // setLibraryItemFormType(FORM_LIBRARY_ITEM_TYPE.BOOK);
      setEditFormType(EDIT_FORM_TYPE.BOOK, runTimeOrPagesValue, true);
      break;
    case "dvd":
    case "audio book":
      // setInputFormType(FORM_LIBRARY_TYPE.PLAYABLE);
      // setLibraryItemFormType(FORM_LIBRARY_ITEM_TYPE.PLAYABLE);
      setEditFormType(EDIT_FORM_TYPE.PLAYABLE, runTimeOrPagesValue, true);
      break;
    default:
      alert(
        `You've tried altering some hidden state. This technical assessment did not involve security related issues: ${selectedTypeValue}`
      );
  }
}

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

  // Show the current tab, and add an "active" class to the button that opened the tab
  document.getElementById(tab).style.display = "block";
  tabButton.classList.toggle("active", true);
}

/// Populates <select> DOM element with id `elementId` with list of categories, retrieved from the route /LibraryItem/GetCategories
/// in an asynchronous javascript call.
async function populateCategoriesList(elementId = "categoryNameSelect") {
  let e = document.getElementById(elementId);
  if (e) {
    return fetch("/Category/GetCategories")
      .then((res) => res.json())
      .then((result) => {
        for (let cat of result) {
          let option = new Option(cat.categoryName, cat.categoryId);
          e.appendChild(option);
        }
      });
  } else {
    logElementNotFound(populateCategoriesList, elementId);
  }
}
