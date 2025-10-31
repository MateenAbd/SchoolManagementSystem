// wwwroot/js/script/common.js
const BASE_URL = window.BASE_URL || "https://localhost:7224";

$.ajaxSetup({
    cache: false,
    crossDomain: true,
    xhrFields: { withCredentials: true },
    headers: { Accept: "application/json" }
});

// Add this block
$.ajaxPrefilter(function (options, originalOptions) {
    // 1) Prefix BASE_URL for non-absolute URLs
    const isAbsolute = /^(?:[a-z]+:)?\/\//i.test(options.url || "");
    if (!isAbsolute) {
        const base = (BASE_URL || "").replace(/\/+$/, "");
        const path = (options.url || "").trim();
        options.url = base + (path.startsWith("/") ? path : "/" + path);
    }

    // 2) Default to expecting JSON back (if not explicitly set)
    if (!options.dataType) options.dataType = "json";

    // 3) For write methods, JSON-serialize plain objects (but leave FormData alone)
    const method = (options.type || options.method || "GET").toUpperCase();
    const isWrite = /^(POST|PUT|PATCH|DELETE)$/i.test(method);
    const isFormData = (originalOptions.data instanceof FormData) ||
        options.processData === false ||
        options.contentType === false;

    if (isWrite && !isFormData && originalOptions.data && typeof originalOptions.data !== "string") {
        options.contentType = options.contentType || "application/json; charset=UTF-8";
        options.data = JSON.stringify(originalOptions.data);
    }

    // Optional: Anti-forgery token (uncomment if you use it)
    // const token = $("input[name='__RequestVerificationToken']").val();
    // if (token) {
    //     options.headers = options.headers || {};
    //     options.headers["RequestVerificationToken"] = token;
    // }
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
function clearAlert(sel) {
    $(sel).addClass("d-none").empty();
}


// Global logout
$(document).on("click", "#btnLogoutTop", function () {
    $.ajax({
        url: "/Auth/Logout",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({}),
        success: function () {
            window.location.href = "/Auth";
        },
        error: function (xhr) {
            showAlert("#globalAlert", "danger", "Logout failed: " + (xhr.responseJSON?.error || xhr.statusText));
        }
    });
});

//function apiGet(path, query = {}) {
//    const url = new URL(path.startsWith('http') ? path : (BASE_URL + path));
//    Object.keys(query).forEach(k => {
//        const v = query[k];
//        if (v !== undefined && v !== null && v !== "") url.searchParams.append(k, v);
//    });
//    return $.ajax({ url: url.toString(), method: "GET" });
//}
//function apiPost(path, data) {
//    return $.ajax({
//        url: BASE_URL + path,
//        method: "POST",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify(data || {})
//    });
//}
//function apiPut(path, data) {
//    return $.ajax({
//        url: BASE_URL + path,
//        method: "PUT",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify(data || {})
//    });
//}
//function apiDelete(path, data) {
//    const payload = (typeof data === "number") ? JSON.stringify(data) : JSON.stringify(data || {});
//    return $.ajax({
//        url: BASE_URL + path,
//        method: "DELETE",
//        contentType: "application/json; charset=utf-8",
//        data: payload
//    });
//}
//function apiUpload(path, formData) {
//    return $.ajax({
//        url: BASE_URL + path,
//        method: "POST",
//        data: formData,
//        processData: false,
//        contentType: false
//    });
//}


