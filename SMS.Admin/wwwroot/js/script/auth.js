// wwwroot/js/script/auth.js
$(function () {
    function redirectAfterLogin(roles) {
        roles = roles || [];
        if (roles.includes("Admin")) {
            window.location.href = "/Auth";
        } else if (roles.includes("Student") || roles.includes("Teacher")) {
            window.location.href = "/Attendance";
        } else {
            window.location.href = "/";
        }
    }

    // Login (button click, not form submit)
    $("#btnLogin").on("click", function () {
        clearAlert("#globalAlert"); clearAlert("#loginMsg");
        const $btn = $(this).prop("disabled", true).text("Signing in...");

        const payload = {
            userNameOrEmail: $("#loginUser").val().trim(),
            password: $("#loginPass").val(),
            rememberMe: $("#rememberMe").is(":checked")
        };

        $.ajax({
            url: "/Auth/Login",
            type: "POST",
            contentType: "application/json; charset=UTF-8",
            dataType: "json",
            data: JSON.stringify(payload),
            cache: false
            // headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() } // if needed
        })
            .done(res => {
                if (res?.success) {
                    redirectAfterLogin(res.roles || []);
                } else {
                    showAlert("#loginMsg", "warning", "Login failed.");
                }
            })
            .fail(xhr => {
                const msg = xhr.responseJSON?.error || "Invalid credentials";
                showAlert("#loginMsg", "danger", msg);
            })
            .always(() => $btn.prop("disabled", false).text("Login"));
    });

    // Register (Admin)
    $("#btnRegister").on("click", function () {
        clearAlert("#registerMsg");
        const $btn = $(this).prop("disabled", true).text("Saving...");

        const payload = {
            userName: $("#regUserName").val().trim(),
            email: $("#regEmail").val().trim(),
            phoneNumber: $("#regPhone").val().trim(),
            password: $("#regPassword").val(),
            roleName: $("#regRole").val()
        };

        $.ajax({
            url: "/Auth/Register",
            type: "POST",
            contentType: "application/json; charset=UTF-8",
            dataType: "json",
            data: JSON.stringify(payload),
            cache: false
            // headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() } // if needed
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#registerMsg", "success", `User created. Id = ${res.userId}`);
                    $("#regUserName, #regEmail, #regPhone, #regPassword").val("");
                    $("#regRole").val("");
                } else {
                    showAlert("#registerMsg", "warning", "Registration failed.");
                }
            })
            .fail(xhr => {
                const msg = (xhr.responseJSON?.errors && xhr.responseJSON.errors.join("<br/>"))
                    || xhr.responseJSON?.error || xhr.statusText;
                showAlert("#registerMsg", "danger", msg);
            })
            .always(() => $btn.prop("disabled", false).text("Register"));
    });

    // Link Student (Admin)
    $("#btnLink").on("click", function () {
        clearAlert("#linkMsg");
        const $btn = $(this).prop("disabled", true).text("Linking...");

        const payload = {
            userId: parseInt($("#linkUserId").val(), 10),
            studentId: parseInt($("#linkStudentId").val(), 10)
        };

        $.ajax({
            url: "/Auth/LinkStudent",
            type: "POST",
            contentType: "application/json; charset=UTF-8",
            dataType: "json",
            data: JSON.stringify(payload),
            cache: false
            // headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() } // if needed
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#linkMsg", "success", `Linked successfully. Link Id = ${res.id}`);
                    $("#linkUserId, #linkStudentId").val("");
                } else {
                    showAlert("#linkMsg", "warning", "Link failed.");
                }
            })
            .fail(xhr => {
                const msg = xhr.responseJSON?.error || xhr.statusText;
                showAlert("#linkMsg", "danger", msg);
            })
            .always(() => $btn.prop("disabled", false).text("Link"));
    });
});













//// wwwroot/js/script/auth.js
//$(function () {
//    function redirectAfterLogin(roles) {
//        roles = roles || [];
//        if (roles.includes("Admin")) {
//            window.location.href = "/Auth";
//        } else if (roles.includes("Student") || roles.includes("Teacher")) {
//            window.location.href = "/Attendance";
//        } else {
//            window.location.href = "/";
//        }
//    }

//    // Login (button click, not form submit)
//    $("#btnLogin").on("click", function () {
//        clearAlert("#globalAlert"); clearAlert("#loginMsg");
//        const $btn = $(this).prop("disabled", true).text("Signing in...");

//        const payload = {
//            userNameOrEmail: $("#loginUser").val().trim(),
//            password: $("#loginPass").val(),
//            rememberMe: $("#rememberMe").is(":checked")
//        };

//        apiPost("/Auth/Login", payload)
//            .done(res => {
//                if (res?.success) {
//                    redirectAfterLogin(res.roles || []);
//                } else {
//                    showAlert("#loginMsg", "warning", "Login failed.");
//                }
//            })
//            .fail(xhr => {
//                const msg = xhr.responseJSON?.error || "Invalid credentials";
//                showAlert("#loginMsg", "danger", msg);
//            })
//            .always(() => $btn.prop("disabled", false).text("Login"));
//    });

//    // Register (Admin)
//    $("#btnRegister").on("click", function () {
//        clearAlert("#registerMsg");
//        const $btn = $(this).prop("disabled", true).text("Saving...");

//        const payload = {
//            userName: $("#regUserName").val().trim(),
//            email: $("#regEmail").val().trim(),
//            phoneNumber: $("#regPhone").val().trim(),
//            password: $("#regPassword").val(),
//            roleName: $("#regRole").val()
//        };

//        apiPost("/Auth/Register", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#registerMsg", "success", `User created. Id = ${res.userId}`);
//                    $("#regUserName, #regEmail, #regPhone, #regPassword").val("");
//                    $("#regRole").val("");
//                } else {
//                    showAlert("#registerMsg", "warning", "Registration failed.");
//                }
//            })
//            .fail(xhr => {
//                const msg = (xhr.responseJSON?.errors && xhr.responseJSON.errors.join("<br/>"))
//                    || xhr.responseJSON?.error || xhr.statusText;
//                showAlert("#registerMsg", "danger", msg);
//            })
//            .always(() => $btn.prop("disabled", false).text("Register"));
//    });

//    // Link Student (Admin)
//    $("#btnLink").on("click", function () {
//        clearAlert("#linkMsg");
//        const $btn = $(this).prop("disabled", true).text("Linking...");

//        const payload = {
//            userId: parseInt($("#linkUserId").val(), 10),
//            studentId: parseInt($("#linkStudentId").val(), 10)
//        };

//        apiPost("/Auth/LinkStudent", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#linkMsg", "success", `Linked successfully. Link Id = ${res.id}`);
//                    $("#linkUserId, #linkStudentId").val("");
//                } else {
//                    showAlert("#linkMsg", "warning", "Link failed.");
//                }
//            })
//            .fail(xhr => {
//                const msg = xhr.responseJSON?.error || xhr.statusText;
//                showAlert("#linkMsg", "danger", msg);
//            })
//            .always(() => $btn.prop("disabled", false).text("Link"));
//    });
//});