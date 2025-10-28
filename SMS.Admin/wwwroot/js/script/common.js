// wwwroot/js/script/common.js
const BASE_URL = window.BASE_URL || "https://localhost:7224";

$.ajaxSetup({
    cache: false,
    crossDomain: true,
    xhrFields: { withCredentials: true },
    headers: { Accept: "application/json" }
});

function showAlert(containerSel, type, message) {
    const $c = $(containerSel);
    const html = `
      <div class="alert alert-${type} alert-dismissible fade show" role="alert">
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      </div>`;
    $c.removeClass("d-none").html(html);
}
function clearAlert(sel) { $(sel).addClass("d-none").empty(); }

function apiGet(path, query = {}) {
    const url = new URL(path.startsWith('http') ? path : (BASE_URL + path));
    Object.keys(query).forEach(k => {
        const v = query[k];
        if (v !== undefined && v !== null && v !== "") url.searchParams.append(k, v);
    });
    return $.ajax({ url: url.toString(), method: "GET" });
}
function apiPost(path, data) {
    return $.ajax({
        url: BASE_URL + path,
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data || {})
    });
}
function apiPut(path, data) {
    return $.ajax({
        url: BASE_URL + path,
        method: "PUT",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data || {})
    });
}
function apiDelete(path, data) {
    const payload = (typeof data === "number") ? JSON.stringify(data) : JSON.stringify(data || {});
    return $.ajax({
        url: BASE_URL + path,
        method: "DELETE",
        contentType: "application/json; charset=utf-8",
        data: payload
    });
}
function apiUpload(path, formData) {
    return $.ajax({
        url: BASE_URL + path,
        method: "POST",
        data: formData,
        processData: false,
        contentType: false
    });
}

// Global logout
$(document).on("click", "#btnLogoutTop", function () {
    apiPost("/Auth/Logout", {}).done(() => {
        window.location.href = "/Auth";
    }).fail((xhr) => {
        showAlert("#globalAlert", "danger", "Logout failed: " + (xhr.responseJSON?.error || xhr.statusText));
    });
});