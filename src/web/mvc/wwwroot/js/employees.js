async function getManagers() {
    return fetch("/Employees/GetManagers").then(r => r.json());
}