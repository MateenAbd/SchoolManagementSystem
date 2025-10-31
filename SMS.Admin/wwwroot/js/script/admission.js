// wwwroot/js/script/admission.js
// Complete: Inquiries + Applications (CRUD/Status) + Fees + Docs + Shortlist/Merit + Confirm
// Requires common.js (withCredentials via $.ajaxSetup, BASE_URL, showAlert/clearAlert)

$(function () {
    if ($("#admissionPage").length === 0) return;

    const isAdmin = ($("body").attr("data-roles") || "").includes("Admin");
    if (!isAdmin) return;

    // Modals
    const inquiryModal = new bootstrap.Modal(document.getElementById("inquiryModal"), { backdrop: "static" });
    const inqStatusModal = new bootstrap.Modal(document.getElementById("inqStatusModal"), { backdrop: "static" });
    const appModal = new bootstrap.Modal(document.getElementById("appModal"), { backdrop: "static" });
    const appStatusModal = new bootstrap.Modal(document.getElementById("appStatusModal"), { backdrop: "static" });
    const feesModal = new bootstrap.Modal(document.getElementById("feesModal"), { backdrop: "static" });
    const appDocsModal = new bootstrap.Modal(document.getElementById("appDocsModal"), { backdrop: "static" });
    const confirmModal = new bootstrap.Modal(document.getElementById("confirmModal"), { backdrop: "static" });

    // State
    let inquiriesCache = [];
    let applicationsCache = [];
    let currentAppId = 0;
    let currentAppMeta = { year: "", cls: "", applicant: "" };
    const currentUserId = parseInt($("body").attr("data-user-id") || "0", 10) || null;

    // Helpers
    function parseError(xhr) {
        if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
        if (xhr.responseJSON?.error) return xhr.responseJSON.error;
        return `${xhr.status} ${xhr.statusText}`;
    }
    function filter(list, q, props) {
        if (!q) return list || [];
        const s = q.toLowerCase();
        return (list || []).filter(x => props.some(p => (x[p] || "").toString().toLowerCase().includes(s)));
    }
    function toLocalDateStr(d) {
        if (!d) return "-";
        const dt = new Date(d);
        return isNaN(dt) ? "-" : dt.toLocaleDateString();
    }
    function toLocalDateTimeStr(d) {
        if (!d) return "-";
        const dt = new Date(d);
        return isNaN(dt) ? "-" : dt.toLocaleString();
    }
    function getDateValue(sel) {
        const v = $(sel).val();
        return v ? v : null; // "YYYY-MM-DD"
    }
    function getDateTimeLocalValue(sel) {
        const v = $(sel).val(); // "YYYY-MM-DDTHH:mm"
        return v ? v : null;
    }

    // ----------------- Inquiries -----------------
    function loadInquiries() {
        $("#inqTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-muted">Loading...</td></tr>`);
        const academicYear = $("#inqYear").val().trim();
        const interestedClass = $("#inqClass").val().trim();
        const leadStatus = $("#inqStatus").val() || "";

        $.ajax({
            url: "/Admission/GetInquiryList",
            type: "GET",
            dataType: "json",
            data: { academicYear, interestedClass, leadStatus }
        })
            .done(list => {
                inquiriesCache = Array.isArray(list) ? list : [];
                renderInquiries();
            })
            .fail(xhr => {
                showAlert("#admissionGlobalMsg", "danger", "Failed to load inquiries: " + parseError(xhr));
                $("#inqTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-danger">Error loading</td></tr>`);
            });
    }

    function renderInquiries() {
        const q = $("#inqSearch").val();
        const filtered = filter(inquiriesCache, q, ["applicantName", "phone", "email", "academicYear", "interestedClass", "leadStatus", "source"]);
        const $tb = $("#inqTable tbody");
        if (filtered.length === 0) {
            $tb.html(`<tr><td colspan="8" class="p-4 text-center text-muted">No inquiries found.</td></tr>`);
            $("#inqCount").text("");
            return;
        }
        const rows = filtered.map(i => {
            const id = i.inquiryId;
            return `
                <tr data-id="${id}">
                    <td>${id}</td>
                    <td>${i.applicantName || "-"}</td>
                    <td>${i.phone || "-"}</td>
                    <td>${i.email || "-"}</td>
                    <td>${i.academicYear || "-"}</td>
                    <td>${i.interestedClass || "-"}</td>
                    <td><span class="badge bg-info text-dark">${i.leadStatus || "New"}</span></td>
                    <td>
                        <div class="btn-group btn-group-sm" role="group">
                            <button class="btn btn-outline-primary btn-inq-edit">Edit</button>
                            <button class="btn btn-outline-secondary btn-inq-status">Status</button>
                            <button class="btn btn-outline-success btn-inq-create-app">Create App</button>
                        </div>
                    </td>
                </tr>
            `;
        }).join("");
        $tb.html(rows);
        $("#inqCount").text(`${filtered.length} inquiry(s)`);
    }

    function openInquiryModal(inquiryId) {
        clearAlert("#inqFormMsg");
        $("#inquiryId").val(inquiryId || 0);
        $("#inquiryModalLabel").text(inquiryId ? "Edit Inquiry" : "New Inquiry");
        $("#inqApplicantName, #inqPhone, #inqEmail, #inqAcademicYear, #inqInterestedClass, #inqSource, #inqNotes").val("");
        $("#inqLeadStatus").val("New");
        $("#inqFollowUpDate").val("");

        if (inquiryId) {
            $.ajax({
                url: "/Admission/GetInquiryById",
                type: "GET",
                dataType: "json",
                data: { inquiryId }
            })
                .done(i => {
                    $("#inquiryId").val(i.inquiryId);
                    $("#inqApplicantName").val(i.applicantName || "");
                    $("#inqPhone").val(i.phone || "");
                    $("#inqEmail").val(i.email || "");
                    $("#inqAcademicYear").val(i.academicYear || "");
                    $("#inqInterestedClass").val(i.interestedClass || "");
                    $("#inqLeadStatus").val(i.leadStatus || "New");
                    $("#inqSource").val(i.source || "");
                    $("#inqNotes").val(i.notes || "");
                    if (i.followUpDate) {
                        const d = new Date(i.followUpDate);
                        if (!isNaN(d)) $("#inqFollowUpDate").val(d.toISOString().substring(0, 10));
                    }
                })
                .fail(xhr => showAlert("#inqFormMsg", "danger", parseError(xhr)))
                .always(() => inquiryModal.show());
        } else {
            inquiryModal.show();
        }
    }

    function saveInquiry() {
        clearAlert("#inqFormMsg");
        const dto = {
            inquiryId: parseInt($("#inquiryId").val(), 10) || 0,
            source: $("#inqSource").val().trim() || "Online",
            leadStatus: $("#inqLeadStatus").val() || "New",
            applicantName: $("#inqApplicantName").val().trim(),
            email: $("#inqEmail").val().trim() || null,
            phone: $("#inqPhone").val().trim() || null,
            interestedClass: $("#inqInterestedClass").val().trim(),
            academicYear: $("#inqAcademicYear").val().trim(),
            notes: $("#inqNotes").val().trim() || null,
            followUpDate: getDateValue("#inqFollowUpDate")
        };
        const $btn = $("#btnSaveInquiry").prop("disabled", true).text("Saving...");

        const req = dto.inquiryId > 0
            ? $.ajax({
                url: "/Admission/UpdateInquiry",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
            : $.ajax({
                url: "/Admission/CreateInquiry",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            });

        req.done(res => {
            if (res?.success) {
                inquiryModal.hide();
                showAlert("#admissionGlobalMsg", "success", "Inquiry saved.");
                loadInquiries();
            } else {
                showAlert("#inqFormMsg", "warning", "Save failed.");
            }
        }).fail(xhr => showAlert("#inqFormMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Save"));
    }

    function openInquiryStatusModal(inquiryId, currentStatus) {
        $("#inqStatusInquiryId").val(inquiryId);
        $("#inqStatusValue").val(currentStatus || "New");
        inqStatusModal.show();
    }

    function updateInquiryStatus() {
        const payload = {
            inquiryId: parseInt($("#inqStatusInquiryId").val(), 10),
            leadStatus: $("#inqStatusValue").val()
        };
        $("#btnUpdateInquiryStatus").prop("disabled", true).text("Updating...");

        $.ajax({
            url: "/Admission/UpdateInquiryStatus",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    inqStatusModal.hide();
                    showAlert("#admissionGlobalMsg", "success", "Inquiry status updated.");
                    loadInquiries();
                } else {
                    showAlert("#admissionGlobalMsg", "warning", "Status update failed.");
                }
            })
            .fail(xhr => showAlert("#admissionGlobalMsg", "danger", parseError(xhr)))
            .always(() => $("#btnUpdateInquiryStatus").prop("disabled", false).text("Update"));
    }

    function createAppFromInquiry(inquiryId) {
        clearAlert("#appFormMsg");
        $("#applicationId").val(0);
        $("#appModalLabel").text("New Application");
        // reset
        $("#appApplicationNo, #appAcademicYear, #appClassAppliedFor, #appApplicantName, #appDob, #appGender, #appEmail, #appPhone, #appPreviousSchool, #appParentName, #appParentPhone, #appParentEmail, #appAddress, #appCategory, #appTotalMarks, #appEntranceScore, #appInquiryId").val("");
        $("#appDocumentsVerified").val("false");
        // default ApplicationDate today
        const today = new Date();
        $("#appApplicationDate").val(today.toISOString().substring(0, 10));

        if (inquiryId) {
            $.ajax({
                url: "/Admission/GetInquiryById",
                type: "GET",
                dataType: "json",
                data: { inquiryId }
            })
                .done(i => {
                    $("#appInquiryId").val(i.inquiryId || "");
                    $("#appAcademicYear").val(i.academicYear || "");
                    $("#appClassAppliedFor").val(i.interestedClass || "");
                    $("#appApplicantName").val(i.applicantName || "");
                    $("#appEmail").val(i.email || "");
                    $("#appPhone").val(i.phone || "");
                })
                .always(() => appModal.show());
        } else {
            appModal.show();
        }
    }

    // ----------------- Applications -----------------
    function loadApplications() {
        $("#appTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-muted">Loading...</td></tr>`);
        const academicYear = $("#appYear").val().trim();
        const classAppliedFor = $("#appClass").val().trim();
        const status = $("#appStatus").val() || "";

        $.ajax({
            url: "/Admission/GetApplicationList",
            type: "GET",
            dataType: "json",
            data: { academicYear, classAppliedFor, status }
        })
            .done(list => {
                applicationsCache = Array.isArray(list) ? list : [];
                renderApplications();
            })
            .fail(xhr => {
                showAlert("#admissionGlobalMsg", "danger", "Failed to load applications: " + parseError(xhr));
                $("#appTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-danger">Error loading</td></tr>`);
            });
    }

    function renderApplications() {
        const q = $("#appSearch").val();
        const filtered = filter(applicationsCache, q, ["applicantName", "phone", "email", "applicationNo", "academicYear", "classAppliedFor", "status"]);
        const $tb = $("#appTable tbody");
        if (filtered.length === 0) {
            $tb.html(`<tr><td colspan="8" class="p-4 text-center text-muted">No applications found.</td></tr>`);
            $("#appCount").text("");
            return;
        }
        const rows = filtered.map(a => {
            const id = a.applicationId;
            const date = a.applicationDate ? toLocalDateStr(a.applicationDate) : "-";
            return `
                <tr data-id="${id}">
                    <td>${id}</td>
                    <td>${a.applicationNo || "-"}</td>
                    <td>${a.applicantName || "-"}</td>
                    <td>${a.academicYear || "-"}</td>
                    <td>${a.classAppliedFor || "-"}</td>
                    <td><span class="badge bg-info text-dark">${a.status || "Submitted"}</span></td>
                    <td>${date}</td>
                    <td>
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary btn-app-edit">Edit</button>
                            <button class="btn btn-outline-secondary btn-app-status">Status</button>
                            <button class="btn btn-outline-success btn-app-fees">Fees</button>
                            <button class="btn btn-outline-info btn-app-docs">Docs</button>
                            <button class="btn btn-outline-dark btn-app-confirm">Confirm</button>
                        </div>
                    </td>
                </tr>
            `;
        }).join("");
        $tb.html(rows);
        $("#appCount").text(`${filtered.length} application(s)`);
    }

    function openAppModal(applicationId) {
        clearAlert("#appFormMsg");
        $("#applicationId").val(applicationId || 0);
        $("#appModalLabel").text(applicationId ? "Edit Application" : "New Application");

        // reset
        $("#appApplicationNo, #appAcademicYear, #appClassAppliedFor, #appApplicantName, #appDob, #appGender, #appEmail, #appPhone, #appPreviousSchool, #appParentName, #appParentPhone, #appParentEmail, #appAddress, #appCategory, #appTotalMarks, #appEntranceScore, #appInquiryId").val("");
        $("#appDocumentsVerified").val("false");
        const today = new Date();
        $("#appApplicationDate").val(today.toISOString().substring(0, 10));

        if (applicationId) {
            $.ajax({
                url: "/Admission/GetApplicationById",
                type: "GET",
                dataType: "json",
                data: { applicationId }
            })
                .done(a => {
                    $("#applicationId").val(a.applicationId || applicationId);
                    $("#appApplicationNo").val(a.applicationNo || "");
                    $("#appAcademicYear").val(a.academicYear || "");
                    $("#appClassAppliedFor").val(a.classAppliedFor || "");
                    $("#appApplicantName").val(a.applicantName || "");

                    if (a.dateOfBirth) {
                        const d = new Date(a.dateOfBirth);
                        if (!isNaN(d)) $("#appDob").val(d.toISOString().substring(0, 10));
                    } else { $("#appDob").val(""); }

                    $("#appGender").val(a.gender || "");
                    $("#appEmail").val(a.email || "");
                    $("#appPhone").val(a.phone || "");
                    $("#appPreviousSchool").val(a.previousSchool || "");
                    $("#appParentName").val(a.parentName || "");
                    $("#appParentPhone").val(a.parentPhone || "");
                    $("#appParentEmail").val(a.parentEmail || "");
                    $("#appAddress").val(a.address || "");
                    $("#appCategory").val(a.category || "");
                    $("#appTotalMarks").val(a.totalMarks ?? "");
                    $("#appEntranceScore").val(a.entranceScore ?? "");
                    $("#appInquiryId").val(a.inquiryId ?? "");
                    $("#appDocumentsVerified").val(a.documentsVerified ? "true" : "false");
                    $("#appStatusValue").val(a.status || "Submitted");

                    if (a.applicationDate) {
                        const ad = new Date(a.applicationDate);
                        if (!isNaN(ad)) $("#appApplicationDate").val(ad.toISOString().substring(0, 10));
                    }
                })
                .fail(xhr => showAlert("#appFormMsg", "danger", parseError(xhr)))
                .always(() => appModal.show());
        } else {
            appModal.show();
        }
    }

    function saveApplication() {
        clearAlert("#appFormMsg");
        const dto = {
            applicationId: parseInt($("#applicationId").val(), 10) || 0,
            inquiryId: $("#appInquiryId").val() ? parseInt($("#appInquiryId").val(), 10) : null,
            applicationNo: $("#appApplicationNo").val().trim(),
            applicantName: $("#appApplicantName").val().trim(),
            dateOfBirth: getDateValue("#appDob"),
            gender: $("#appGender").val(),
            email: $("#appEmail").val().trim() || null,
            phone: $("#appPhone").val().trim() || null,
            address: $("#appAddress").val().trim() || null,
            parentName: $("#appParentName").val().trim() || null,
            parentPhone: $("#appParentPhone").val().trim() || null,
            parentEmail: $("#appParentEmail").val().trim() || null,
            previousSchool: $("#appPreviousSchool").val().trim() || null,
            classAppliedFor: $("#appClassAppliedFor").val().trim(),
            academicYear: $("#appAcademicYear").val().trim(),
            status: $("#appStatusValue").val(),
            totalMarks: $("#appTotalMarks").val() ? parseFloat($("#appTotalMarks").val()) : null,
            entranceScore: $("#appEntranceScore").val() ? parseFloat($("#appEntranceScore").val()) : null,
            category: $("#appCategory").val().trim() || null,
            documentsVerified: $("#appDocumentsVerified").val() === "true",
            applicationDate: getDateValue("#appApplicationDate")
        };

        const $btn = $("#btnSaveApplication").prop("disabled", true).text("Saving...");
        const req = dto.applicationId > 0
            ? $.ajax({
                url: "/Admission/UpdateApplication",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
            : $.ajax({
                url: "/Admission/CreateApplication",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            });

        req.done(res => {
            if (res?.success) {
                appModal.hide();
                showAlert("#admissionGlobalMsg", "success", "Application saved.");
                loadApplications();
            } else {
                showAlert("#appFormMsg", "warning", "Save failed.");
            }
        }).fail(xhr => showAlert("#appFormMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Save"));
    }

    function openAppStatusModal(applicationId, currentStatus) {
        $("#appStatusAppId").val(applicationId);
        $("#appStatusNew").val(currentStatus || "Submitted");
        appStatusModal.show();
    }

    function updateAppStatus() {
        const payload = {
            applicationId: parseInt($("#appStatusAppId").val(), 10),
            status: $("#appStatusNew").val()
        };
        $("#btnUpdateAppStatus").prop("disabled", true).text("Updating...");

        $.ajax({
            url: "/Admission/UpdateApplicationStatus",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    appStatusModal.hide();
                    showAlert("#admissionGlobalMsg", "success", "Application status updated.");
                    loadApplications();
                } else {
                    showAlert("#admissionGlobalMsg", "warning", "Status update failed.");
                }
            })
            .fail(xhr => showAlert("#admissionGlobalMsg", "danger", parseError(xhr)))
            .always(() => $("#btnUpdateAppStatus").prop("disabled", false).text("Update"));
    }

    // ----------------- Fees -----------------
    function openFeesModal(appId) {
        currentAppId = appId;
        clearAlert("#feesMsg");
        $("#feesAppId").val(appId);
        $("#feeAmount, #feeRef, #feeRemarks").val("");
        $("#feeCurrency").val("INR");
        $("#feeMode").val("Cash");
        const now = new Date();
        const pad = n => String(n).padStart(2, "0");
        const dtLocal = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours())}:${pad(now.getMinutes())}`;
        $("#feePaymentDate").val(dtLocal);
        $("#feesAppLabel").text("ID " + appId);

        $.ajax({
            url: "/Admission/GetApplicationById",
            type: "GET",
            dataType: "json",
            data: { applicationId: appId }
        })
            .done(a => {
                currentAppMeta = { year: a.academicYear || "", cls: a.classAppliedFor || "", applicant: a.applicantName || "" };
                $("#feesAppLabel").text(`${currentAppMeta.applicant} (ID ${appId})`);
                loadFees(appId);
                if (currentAppMeta.year && currentAppMeta.cls) {
                    loadFeeSummary(currentAppMeta.year, currentAppMeta.cls);
                } else {
                    $("#feeSummaryBox").text("Select AY/Class to view summary.");
                }
            })
            .fail(() => { loadFees(appId); });

        feesModal.show();
    }

    function loadFees(applicationId) {
        const $tb = $("#feesTable tbody");
        $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);

        $.ajax({
            url: "/Admission/GetApplicationFees",
            type: "GET",
            dataType: "json",
            data: { applicationId }
        })
            .done(list => {
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No payments.</td></tr>`);
                    return;
                }
                const rows = list.map(p => {
                    const on = p.paymentDate || p.paidOn || p.createdAtUtc;
                    return `
                    <tr>
                        <td>${p.paymentId || "-"}</td>
                        <td>${p.amount ?? "-"}</td>
                        <td>${p.paymentMode || p.mode || "-"}</td>
                        <td>${toLocalDateTimeStr(on)}</td>
                        <td>${p.referenceNo || "-"}</td>
                        <td>${p.remarks || ""}</td>
                    </tr>
                `;
                }).join("");
                $tb.html(rows);
            })
            .fail(xhr => showAlert("#feesMsg", "danger", "Failed to load fees: " + parseError(xhr)));
    }

    function collectFee() {
        clearAlert("#feesMsg");
        const applicationId = parseInt($("#feesAppId").val(), 10);
        const payload = {
            applicationId,
            amount: parseFloat($("#feeAmount").val() || "0"),
            currency: $("#feeCurrency").val(),
            paymentMode: $("#feeMode").val(),
            referenceNo: $("#feeRef").val().trim() || null,
            remarks: $("#feeRemarks").val().trim() || null,
            collectedByUserId: currentUserId,
            paymentDate: getDateTimeLocalValue("#feePaymentDate")
        };
        if (!payload.amount || payload.amount <= 0) {
            showAlert("#feesMsg", "warning", "Enter a valid amount.");
            return;
        }
        const $btn = $("#btnCollectFee").prop("disabled", true).text("Saving...");

        $.ajax({
            url: "/Admission/CollectApplicationFee",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#feesMsg", "success", "Payment recorded.");
                    loadFees(applicationId);
                    if (currentAppMeta.year && currentAppMeta.cls) {
                        loadFeeSummary(currentAppMeta.year, currentAppMeta.cls);
                    }
                } else {
                    showAlert("#feesMsg", "warning", "Failed to record payment.");
                }
            })
            .fail(xhr => showAlert("#feesMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Collect"));
    }

    function loadFeeSummary(academicYear, classAppliedFor) {
        $("#feeSummaryBox").text("Loading summary...");
        $.ajax({
            url: "/Admission/GetApplicationFeeSummary",
            type: "GET",
            dataType: "json",
            data: { academicYear, classAppliedFor }
        })
            .done(dto => {
                $("#feeSummaryBox").text(JSON.stringify(dto || {}, null, 2));
            })
            .fail(() => $("#feeSummaryBox").text("Failed to load summary."));
    }

    // ----------------- Application Documents -----------------
    function openAppDocsModal(appId) {
        currentAppId = appId;
        clearAlert("#appDocsMsg");
        $("#docsAppId").val(appId);
        $("#docsAppLabel").text("ID " + appId);
        $("#appDocFile").val(""); $("#appDocDesc").val("");

        $.ajax({
            url: "/Admission/GetApplicationById",
            type: "GET",
            dataType: "json",
            data: { applicationId: appId }
        })
            .done(a => $("#docsAppLabel").text(`${a.applicantName || ""} (ID ${appId})`))
            .always(() => {
                loadAppDocuments(appId);
                appDocsModal.show();
            });
    }

    function loadAppDocuments(applicationId) {
        const $tb = $("#appDocsTable tbody");
        $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);

        $.ajax({
            url: "/Admission/GetApplicationDocuments",
            type: "GET",
            dataType: "json",
            data: { applicationId }
        })
            .done(list => {
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No documents.</td></tr>`);
                    return;
                }
                const rows = list.map(d => {
                    const fileName = d.fileName || (d.filePath ? d.filePath.split('/').pop() : "-");
                    const path = d.filePath || "#";
                    const verified = (d.isVerified ?? d.verified ?? d.documentsVerified) ? "Yes" : "No";
                    const uploaded = d.uploadedOn || d.createdAtUtc;
                    return `
                    <tr data-doc-id="${d.documentId}">
                        <td>${d.documentId}</td>
                        <td><a href="${path}" target="_blank" rel="noopener">${fileName}</a></td>
                        <td>${d.description || ""}</td>
                        <td>${toLocalDateTimeStr(uploaded)}</td>
                        <td>${verified}</td>
                        <td>
                            <div class="btn-group btn-group-sm">
                                <button class="btn btn-outline-success btn-verify-doc">Verify</button>
                            </div>
                        </td>
                    </tr>
                `;
                }).join("");
                $tb.html(rows);
            })
            .fail(xhr => showAlert("#appDocsMsg", "danger", "Failed to load documents: " + parseError(xhr)));
    }

    function uploadAppDoc() {
        clearAlert("#appDocsMsg");
        const appId = parseInt($("#docsAppId").val(), 10);
        const file = $("#appDocFile")[0].files[0];
        if (!file) return showAlert("#appDocsMsg", "warning", "Choose a file.");

        const fd = new FormData();
        fd.append("applicationId", appId);
        fd.append("description", $("#appDocDesc").val());
        fd.append("file", file);

        const $btn = $("#btnUploadAppDoc").prop("disabled", true).text("Uploading...");
        $.ajax({
            url: "/Admission/UploadApplicationDocument",
            type: "POST",
            dataType: "json",
            data: fd,
            processData: false,
            contentType: false
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#appDocsMsg", "success", "Document uploaded.");
                    $("#appDocFile").val(""); $("#appDocDesc").val("");
                    loadAppDocuments(appId);
                } else {
                    showAlert("#appDocsMsg", "warning", "Upload failed.");
                }
            })
            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Upload"));
    }

    function verifySingleDoc(docId) {
        const payload = { documentId: docId, verifiedByUserId: currentUserId, verified: true };

        $.ajax({
            url: "/Admission/VerifyApplicationDocument",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#appDocsMsg", "success", "Document verified.");
                    loadAppDocuments(parseInt($("#docsAppId").val(), 10));
                } else {
                    showAlert("#appDocsMsg", "warning", "Verification failed.");
                }
            })
            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)));
    }

    function markAllDocsVerified() {
        const payload = { applicationId: parseInt($("#docsAppId").val(), 10), documentsVerified: true };

        $.ajax({
            url: "/Admission/SetApplicationDocumentsVerified",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#appDocsMsg", "success", "All documents marked verified.");
                    loadAppDocuments(payload.applicationId);
                } else {
                    showAlert("#appDocsMsg", "warning", "Update failed.");
                }
            })
            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)));
    }

    // ----------------- Confirm Admission -----------------
    function openConfirmModal(appId) {
        clearAlert("#confirmMsg");
        $("#confirmAppId").val(appId);
        $("#confirmSection").val("");
        const today = new Date();
        $("#confirmEnrollDate").val(today.toISOString().substring(0, 10));
        $("#confirmAppLabel").text("ID " + appId);

        $.ajax({
            url: "/Admission/GetApplicationById",
            type: "GET",
            dataType: "json",
            data: { applicationId: appId }
        })
            .done(a => $("#confirmAppLabel").text(`${a.applicantName || ""} (ID ${appId})`));

        confirmModal.show();
    }

    function confirmAdmission() {
        clearAlert("#confirmMsg");
        const payload = {
            applicationId: parseInt($("#confirmAppId").val(), 10),
            section: $("#confirmSection").val().trim() || null,
            enrollmentDate: getDateValue("#confirmEnrollDate")
        };
        const $btn = $("#btnConfirmAdmission").prop("disabled", true).text("Confirming...");

        $.ajax({
            url: "/Admission/ConfirmAdmission",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#confirmMsg", "success", `Admission confirmed. Student ID = ${res.studentId}`);
                    setTimeout(() => { confirmModal.hide(); loadApplications(); }, 1200);
                } else {
                    showAlert("#confirmMsg", "warning", "Confirmation failed");
                }
            })
            .fail(xhr => showAlert("#confirmMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Confirm"));
    }

    // ----------------- Shortlist & Merit -----------------
    function generateShortlist() {
        clearAlert("#shortMsg");
        const payload = {
            academicYear: $("#shortYear").val().trim(),
            classAppliedFor: $("#shortClass").val().trim(),
            minEntranceScore: $("#shortMinScore").val() ? parseFloat($("#shortMinScore").val()) : null
        };

        $.ajax({
            url: "/Admission/GenerateShortlist",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#shortMsg", "success", `Shortlist generated. Count = ${res.count}`);
                    loadShortlist(payload.academicYear, payload.classAppliedFor);
                } else {
                    showAlert("#shortMsg", "warning", "Shortlist generation failed.");
                }
            })
            .fail(xhr => showAlert("#shortMsg", "danger", parseError(xhr)));
    }

    function loadShortlist(academicYear, classAppliedFor) {
        $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-muted">Loading...</td></tr>`);
        $.ajax({
            url: "/Admission/GetShortlist",
            type: "GET",
            dataType: "json",
            data: { academicYear, classAppliedFor }
        })
            .done(list => {
                if (!Array.isArray(list) || list.length === 0) {
                    $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-muted">No data.</td></tr>`);
                    return;
                }
                const rows = list.map(x => {
                    const appId = x.applicationId ?? x.appId ?? "-";
                    const name = x.applicantName ?? x.name ?? "-";
                    const score = (x.entranceScore ?? x.score ?? "-");
                    return `<tr><td>${appId}</td><td>${name}</td><td>${score}</td></tr>`;
                }).join("");
                $("#shortlistTable tbody").html(rows);
            })
            .fail(xhr => $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
    }

    function generateMerit() {
        clearAlert("#meritMsg");
        const payload = {
            academicYear: $("#meritYear").val().trim(),
            classAppliedFor: $("#meritClass").val().trim(),
            topN: parseInt($("#meritTopN").val(), 10) || 0
        };

        $.ajax({
            url: "/Admission/GenerateMeritList",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=UTF-8",
            data: JSON.stringify(payload)
        })
            .done(res => {
                if (res?.success) {
                    showAlert("#meritMsg", "success", `Merit list generated. Count = ${res.count}`);
                    loadMerit(payload.academicYear, payload.classAppliedFor);
                } else {
                    showAlert("#meritMsg", "warning", "Merit list generation failed.");
                }
            })
            .fail(xhr => showAlert("#meritMsg", "danger", parseError(xhr)));
    }

    function loadMerit(academicYear, classAppliedFor) {
        $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">Loading...</td></tr>`);
        $.ajax({
            url: "/Admission/GetMeritList",
            type: "GET",
            dataType: "json",
            data: { academicYear, classAppliedFor }
        })
            .done(list => {
                if (!Array.isArray(list) || list.length === 0) {
                    $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">No data.</td></tr>`);
                    return;
                }
                const rows = list.map((x, idx) => {
                    const rank = x.rank ?? (idx + 1);
                    const appId = x.applicationId ?? x.appId ?? "-";
                    const name = x.applicantName ?? x.name ?? "-";
                    const score = (x.entranceScore ?? x.score ?? "-");
                    return `<tr><td>${rank}</td><td>${appId}</td><td>${name}</td><td>${score}</td></tr>`;
                }).join("");
                $("#meritTable tbody").html(rows);
            })
            .fail(xhr => $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
    }

    // ----------------- Events -----------------
    // Inquiries
    $("#btnInqRefresh").on("click", loadInquiries);
    $("#btnNewInquiry").on("click", () => openInquiryModal(0));
    $("#inqSearch").on("input", renderInquiries);
    $(document).on("click", ".btn-inq-edit", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openInquiryModal(id);
    });
    $(document).on("click", ".btn-inq-status", function () {
        const $tr = $(this).closest("tr");
        const id = parseInt($tr.data("id"), 10);
        const status = $tr.find("td:nth-child(7) .badge").text().trim();
        if (id) openInquiryStatusModal(id, status);
    });
    $(document).on("click", ".btn-inq-create-app", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        createAppFromInquiry(id);
        $("#tab-app").click();
    });
    $("#btnSaveInquiry").on("click", saveInquiry);
    $("#btnUpdateInquiryStatus").on("click", updateInquiryStatus);

    // Applications
    $("#btnAppRefresh").on("click", loadApplications);
    $("#btnNewApplication").on("click", () => createAppFromInquiry(null));
    $("#appSearch").on("input", renderApplications);
    $(document).on("click", ".btn-app-edit", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openAppModal(id);
    });
    $(document).on("click", ".btn-app-status", function () {
        const $tr = $(this).closest("tr");
        const id = parseInt($tr.data("id"), 10);
        const status = $tr.find("td:nth-child(6) .badge").text().trim();
        if (id) openAppStatusModal(id, status);
    });
    $(document).on("click", ".btn-app-fees", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openFeesModal(id);
    });
    $(document).on("click", ".btn-app-docs", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openAppDocsModal(id);
    });
    $(document).on("click", ".btn-app-confirm", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openConfirmModal(id);
    });
    $("#btnSaveApplication").on("click", saveApplication);
    $("#btnUpdateAppStatus").on("click", updateAppStatus);

    // Fees
    $("#btnCollectFee").on("click", collectFee);

    // Docs
    $("#btnUploadAppDoc").on("click", uploadAppDoc);
    $(document).on("click", ".btn-verify-doc", function () {
        const docId = parseInt($(this).closest("tr").data("doc-id"), 10);
        if (docId) verifySingleDoc(docId);
    });
    $("#btnMarkAllDocsVerified").on("click", markAllDocsVerified);

    // Confirm
    $("#btnConfirmAdmission").on("click", confirmAdmission);

    // Shortlist/Merit
    $("#btnGenerateShortlist").on("click", generateShortlist);
    $("#btnLoadShortlist").on("click", function () {
        loadShortlist($("#shortYear").val().trim(), $("#shortClass").val().trim());
    });
    $("#btnGenerateMerit").on("click", generateMerit);
    $("#btnLoadMerit").on("click", function () {
        loadMerit($("#meritYear").val().trim(), $("#meritClass").val().trim());
    });

    // Initial load
    loadInquiries();
    loadApplications();
});




//// wwwroot/js/script/admission.js
//// Complete: Inquiries + Applications (CRUD/Status) + Fees + Docs + Shortlist/Merit + Confirm
//// Requires common.js (withCredentials, apiGet/apiPost/apiUpload, showAlert/clearAlert)

//$(function () {
//    if ($("#admissionPage").length === 0) return;

//    const isAdmin = ($("body").attr("data-roles") || "").includes("Admin");
//    if (!isAdmin) return;

//    // Modals
//    const inquiryModal = new bootstrap.Modal(document.getElementById("inquiryModal"), { backdrop: "static" });
//    const inqStatusModal = new bootstrap.Modal(document.getElementById("inqStatusModal"), { backdrop: "static" });
//    const appModal = new bootstrap.Modal(document.getElementById("appModal"), { backdrop: "static" });
//    const appStatusModal = new bootstrap.Modal(document.getElementById("appStatusModal"), { backdrop: "static" });
//    const feesModal = new bootstrap.Modal(document.getElementById("feesModal"), { backdrop: "static" });
//    const appDocsModal = new bootstrap.Modal(document.getElementById("appDocsModal"), { backdrop: "static" });
//    const confirmModal = new bootstrap.Modal(document.getElementById("confirmModal"), { backdrop: "static" });

//    // State
//    let inquiriesCache = [];
//    let applicationsCache = [];
//    let currentAppId = 0;
//    let currentAppMeta = { year: "", cls: "", applicant: "" };
//    const currentUserId = parseInt($("body").attr("data-user-id") || "0", 10) || null;

//    // Helpers
//    function parseError(xhr) {
//        if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
//        if (xhr.responseJSON?.error) return xhr.responseJSON.error;
//        return `${xhr.status} ${xhr.statusText}`;
//    }
//    function filter(list, q, props) {
//        if (!q) return list || [];
//        const s = q.toLowerCase();
//        return (list || []).filter(x => props.some(p => (x[p] || "").toString().toLowerCase().includes(s)));
//    }
//    function toLocalDateStr(d) {
//        if (!d) return "-";
//        const dt = new Date(d);
//        return isNaN(dt) ? "-" : dt.toLocaleDateString();
//    }
//    function toLocalDateTimeStr(d) {
//        if (!d) return "-";
//        const dt = new Date(d);
//        return isNaN(dt) ? "-" : dt.toLocaleString();
//    }
//    function getDateValue(sel) {
//        const v = $(sel).val();
//        return v ? v : null; // "YYYY-MM-DD"
//    }
//    function getDateTimeLocalValue(sel) {
//        // read "YYYY-MM-DDTHH:mm" and return the same string (no 'Z') to avoid timezone conversion
//        const v = $(sel).val();
//        return v ? v : null;
//    }

//    // ----------------- Inquiries -----------------
//    function loadInquiries() {
//        $("#inqTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-muted">Loading...</td></tr>`);
//        const academicYear = $("#inqYear").val().trim();
//        const interestedClass = $("#inqClass").val().trim();
//        const leadStatus = $("#inqStatus").val() || "";

//        apiGet("/Admission/GetInquiryList", { academicYear, interestedClass, leadStatus })
//            .done(list => {
//                inquiriesCache = Array.isArray(list) ? list : [];
//                renderInquiries();
//            })
//            .fail(xhr => {
//                showAlert("#admissionGlobalMsg", "danger", "Failed to load inquiries: " + parseError(xhr));
//                $("#inqTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-danger">Error loading</td></tr>`);
//            });
//    }

//    function renderInquiries() {
//        const q = $("#inqSearch").val();
//        const filtered = filter(inquiriesCache, q, ["applicantName", "phone", "email", "academicYear", "interestedClass", "leadStatus", "source"]);
//        const $tb = $("#inqTable tbody");
//        if (filtered.length === 0) {
//            $tb.html(`<tr><td colspan="8" class="p-4 text-center text-muted">No inquiries found.</td></tr>`);
//            $("#inqCount").text("");
//            return;
//        }
//        const rows = filtered.map(i => {
//            const id = i.inquiryId;
//            return `
//                <tr data-id="${id}">
//                    <td>${id}</td>
//                    <td>${i.applicantName || "-"}</td>
//                    <td>${i.phone || "-"}</td>
//                    <td>${i.email || "-"}</td>
//                    <td>${i.academicYear || "-"}</td>
//                    <td>${i.interestedClass || "-"}</td>
//                    <td><span class="badge bg-info text-dark">${i.leadStatus || "New"}</span></td>
//                    <td>
//                        <div class="btn-group btn-group-sm" role="group">
//                            <button class="btn btn-outline-primary btn-inq-edit">Edit</button>
//                            <button class="btn btn-outline-secondary btn-inq-status">Status</button>
//                            <button class="btn btn-outline-success btn-inq-create-app">Create App</button>
//                        </div>
//                    </td>
//                </tr>
//            `;
//        }).join("");
//        $tb.html(rows);
//        $("#inqCount").text(`${filtered.length} inquiry(s)`);
//    }

//    function openInquiryModal(inquiryId) {
//        clearAlert("#inqFormMsg");
//        $("#inquiryId").val(inquiryId || 0);
//        $("#inquiryModalLabel").text(inquiryId ? "Edit Inquiry" : "New Inquiry");
//        $("#inqApplicantName, #inqPhone, #inqEmail, #inqAcademicYear, #inqInterestedClass, #inqSource, #inqNotes").val("");
//        $("#inqLeadStatus").val("New");
//        $("#inqFollowUpDate").val("");

//        if (inquiryId) {
//            apiGet("/Admission/GetInquiryById", { inquiryId })
//                .done(i => {
//                    $("#inquiryId").val(i.inquiryId);
//                    $("#inqApplicantName").val(i.applicantName || "");
//                    $("#inqPhone").val(i.phone || "");
//                    $("#inqEmail").val(i.email || "");
//                    $("#inqAcademicYear").val(i.academicYear || "");
//                    $("#inqInterestedClass").val(i.interestedClass || "");
//                    $("#inqLeadStatus").val(i.leadStatus || "New");
//                    $("#inqSource").val(i.source || "");
//                    $("#inqNotes").val(i.notes || "");
//                    if (i.followUpDate) {
//                        const d = new Date(i.followUpDate);
//                        if (!isNaN(d)) $("#inqFollowUpDate").val(d.toISOString().substring(0, 10));
//                    }
//                })
//                .fail(xhr => showAlert("#inqFormMsg", "danger", parseError(xhr)))
//                .always(() => inquiryModal.show());
//        } else {
//            inquiryModal.show();
//        }
//    }

//    function saveInquiry() {
//        clearAlert("#inqFormMsg");
//        const dto = {
//            inquiryId: parseInt($("#inquiryId").val(), 10) || 0,
//            source: $("#inqSource").val().trim() || "Online",
//            leadStatus: $("#inqLeadStatus").val() || "New",
//            applicantName: $("#inqApplicantName").val().trim(),
//            email: $("#inqEmail").val().trim() || null,
//            phone: $("#inqPhone").val().trim() || null,
//            interestedClass: $("#inqInterestedClass").val().trim(),
//            academicYear: $("#inqAcademicYear").val().trim(),
//            notes: $("#inqNotes").val().trim() || null,
//            followUpDate: getDateValue("#inqFollowUpDate")
//        };
//        const $btn = $("#btnSaveInquiry").prop("disabled", true).text("Saving...");
//        const req = dto.inquiryId > 0 ? apiPost("/Admission/UpdateInquiry", dto)
//            : apiPost("/Admission/CreateInquiry", dto);
//        req.done(res => {
//            if (res?.success) {
//                inquiryModal.hide();
//                showAlert("#admissionGlobalMsg", "success", "Inquiry saved.");
//                loadInquiries();
//            } else {
//                showAlert("#inqFormMsg", "warning", "Save failed.");
//            }
//        }).fail(xhr => showAlert("#inqFormMsg", "danger", parseError(xhr)))
//            .always(() => $btn.prop("disabled", false).text("Save"));
//    }

//    function openInquiryStatusModal(inquiryId, currentStatus) {
//        $("#inqStatusInquiryId").val(inquiryId);
//        $("#inqStatusValue").val(currentStatus || "New");
//        inqStatusModal.show();
//    }

//    function updateInquiryStatus() {
//        const payload = {
//            inquiryId: parseInt($("#inqStatusInquiryId").val(), 10),
//            leadStatus: $("#inqStatusValue").val()
//        };
//        $("#btnUpdateInquiryStatus").prop("disabled", true).text("Updating...");
//        apiPost("/Admission/UpdateInquiryStatus", payload)
//            .done(res => {
//                if (res?.success) {
//                    inqStatusModal.hide();
//                    showAlert("#admissionGlobalMsg", "success", "Inquiry status updated.");
//                    loadInquiries();
//                } else {
//                    showAlert("#admissionGlobalMsg", "warning", "Status update failed.");
//                }
//            })
//            .fail(xhr => showAlert("#admissionGlobalMsg", "danger", parseError(xhr)))
//            .always(() => $("#btnUpdateInquiryStatus").prop("disabled", false).text("Update"));
//    }

//    function createAppFromInquiry(inquiryId) {
//        clearAlert("#appFormMsg");
//        $("#applicationId").val(0);
//        $("#appModalLabel").text("New Application");
//        // reset
//        $("#appApplicationNo, #appAcademicYear, #appClassAppliedFor, #appApplicantName, #appDob, #appGender, #appEmail, #appPhone, #appPreviousSchool, #appParentName, #appParentPhone, #appParentEmail, #appAddress, #appCategory, #appTotalMarks, #appEntranceScore, #appInquiryId").val("");
//        $("#appDocumentsVerified").val("false");
//        // default ApplicationDate today (no timezone shift)
//        const today = new Date();
//        $("#appApplicationDate").val(today.toISOString().substring(0, 10));

//        if (inquiryId) {
//            apiGet("/Admission/GetInquiryById", { inquiryId })
//                .done(i => {
//                    $("#appInquiryId").val(i.inquiryId || "");
//                    $("#appAcademicYear").val(i.academicYear || "");
//                    $("#appClassAppliedFor").val(i.interestedClass || "");
//                    $("#appApplicantName").val(i.applicantName || "");
//                    $("#appEmail").val(i.email || "");
//                    $("#appPhone").val(i.phone || "");
//                })
//                .always(() => appModal.show());
//        } else {
//            appModal.show();
//        }
//    }

//    // ----------------- Applications -----------------
//    function loadApplications() {
//        $("#appTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-muted">Loading...</td></tr>`);
//        const academicYear = $("#appYear").val().trim();
//        const classAppliedFor = $("#appClass").val().trim();
//        const status = $("#appStatus").val() || "";

//        apiGet("/Admission/GetApplicationList", { academicYear, classAppliedFor, status })
//            .done(list => {
//                applicationsCache = Array.isArray(list) ? list : [];
//                renderApplications();
//            })
//            .fail(xhr => {
//                showAlert("#admissionGlobalMsg", "danger", "Failed to load applications: " + parseError(xhr));
//                $("#appTable tbody").html(`<tr><td colspan="8" class="p-4 text-center text-danger">Error loading</td></tr>`);
//            });
//    }

//    function renderApplications() {
//        const q = $("#appSearch").val();
//        const filtered = filter(applicationsCache, q, ["applicantName", "phone", "email", "applicationNo", "academicYear", "classAppliedFor", "status"]);
//        const $tb = $("#appTable tbody");
//        if (filtered.length === 0) {
//            $tb.html(`<tr><td colspan="8" class="p-4 text-center text-muted">No applications found.</td></tr>`);
//            $("#appCount").text("");
//            return;
//        }
//        const rows = filtered.map(a => {
//            const id = a.applicationId;
//            const date = a.applicationDate ? toLocalDateStr(a.applicationDate) : "-";
//            return `
//                <tr data-id="${id}">
//                    <td>${id}</td>
//                    <td>${a.applicationNo || "-"}</td>
//                    <td>${a.applicantName || "-"}</td>
//                    <td>${a.academicYear || "-"}</td>
//                    <td>${a.classAppliedFor || "-"}</td>
//                    <td><span class="badge bg-info text-dark">${a.status || "Submitted"}</span></td>
//                    <td>${date}</td>
//                    <td>
//                        <div class="btn-group btn-group-sm">
//                            <button class="btn btn-outline-primary btn-app-edit">Edit</button>
//                            <button class="btn btn-outline-secondary btn-app-status">Status</button>
//                            <button class="btn btn-outline-success btn-app-fees">Fees</button>
//                            <button class="btn btn-outline-info btn-app-docs">Docs</button>
//                            <button class="btn btn-outline-dark btn-app-confirm">Confirm</button>
//                        </div>
//                    </td>
//                </tr>
//            `;
//        }).join("");
//        $tb.html(rows);
//        $("#appCount").text(`${filtered.length} application(s)`);
//    }

//    function openAppModal(applicationId) {
//        clearAlert("#appFormMsg");
//        $("#applicationId").val(applicationId || 0);
//        $("#appModalLabel").text(applicationId ? "Edit Application" : "New Application");

//        // reset
//        $("#appApplicationNo, #appAcademicYear, #appClassAppliedFor, #appApplicantName, #appDob, #appGender, #appEmail, #appPhone, #appPreviousSchool, #appParentName, #appParentPhone, #appParentEmail, #appAddress, #appCategory, #appTotalMarks, #appEntranceScore, #appInquiryId").val("");
//        $("#appDocumentsVerified").val("false");
//        const today = new Date();
//        $("#appApplicationDate").val(today.toISOString().substring(0, 10));

//        if (applicationId) {
//            apiGet("/Admission/GetApplicationById", { applicationId })
//                .done(a => {
//                    $("#applicationId").val(a.applicationId || applicationId);
//                    $("#appApplicationNo").val(a.applicationNo || "");
//                    $("#appAcademicYear").val(a.academicYear || "");
//                    $("#appClassAppliedFor").val(a.classAppliedFor || "");
//                    $("#appApplicantName").val(a.applicantName || "");

//                    if (a.dateOfBirth) {
//                        const d = new Date(a.dateOfBirth);
//                        if (!isNaN(d)) $("#appDob").val(d.toISOString().substring(0, 10));
//                    } else { $("#appDob").val(""); }

//                    $("#appGender").val(a.gender || "");
//                    $("#appEmail").val(a.email || "");
//                    $("#appPhone").val(a.phone || "");
//                    $("#appPreviousSchool").val(a.previousSchool || "");
//                    $("#appParentName").val(a.parentName || "");
//                    $("#appParentPhone").val(a.parentPhone || "");
//                    $("#appParentEmail").val(a.parentEmail || "");
//                    $("#appAddress").val(a.address || "");
//                    $("#appCategory").val(a.category || "");
//                    $("#appTotalMarks").val(a.totalMarks ?? "");
//                    $("#appEntranceScore").val(a.entranceScore ?? "");
//                    $("#appInquiryId").val(a.inquiryId ?? "");
//                    $("#appDocumentsVerified").val(a.documentsVerified ? "true" : "false");
//                    $("#appStatusValue").val(a.status || "Submitted");

//                    if (a.applicationDate) {
//                        const ad = new Date(a.applicationDate);
//                        if (!isNaN(ad)) $("#appApplicationDate").val(ad.toISOString().substring(0, 10));
//                    }
//                })
//                .fail(xhr => showAlert("#appFormMsg", "danger", parseError(xhr)))
//                .always(() => appModal.show());
//        } else {
//            appModal.show();
//        }
//    }

//    function saveApplication() {
//        clearAlert("#appFormMsg");
//        const dto = {
//            applicationId: parseInt($("#applicationId").val(), 10) || 0,
//            inquiryId: $("#appInquiryId").val() ? parseInt($("#appInquiryId").val(), 10) : null,
//            applicationNo: $("#appApplicationNo").val().trim(),
//            applicantName: $("#appApplicantName").val().trim(),
//            dateOfBirth: getDateValue("#appDob"),
//            gender: $("#appGender").val(),
//            email: $("#appEmail").val().trim() || null,
//            phone: $("#appPhone").val().trim() || null,
//            address: $("#appAddress").val().trim() || null,
//            parentName: $("#appParentName").val().trim() || null,
//            parentPhone: $("#appParentPhone").val().trim() || null,
//            parentEmail: $("#appParentEmail").val().trim() || null,
//            previousSchool: $("#appPreviousSchool").val().trim() || null,
//            classAppliedFor: $("#appClassAppliedFor").val().trim(),
//            academicYear: $("#appAcademicYear").val().trim(),
//            status: $("#appStatusValue").val(),
//            totalMarks: $("#appTotalMarks").val() ? parseFloat($("#appTotalMarks").val()) : null,
//            entranceScore: $("#appEntranceScore").val() ? parseFloat($("#appEntranceScore").val()) : null,
//            category: $("#appCategory").val().trim() || null,
//            documentsVerified: $("#appDocumentsVerified").val() === "true",
//            applicationDate: getDateValue("#appApplicationDate")
//        };

//        const $btn = $("#btnSaveApplication").prop("disabled", true).text("Saving...");
//        const req = dto.applicationId > 0 ? apiPost("/Admission/UpdateApplication", dto)
//            : apiPost("/Admission/CreateApplication", dto);
//        req.done(res => {
//            if (res?.success) {
//                appModal.hide();
//                showAlert("#admissionGlobalMsg", "success", "Application saved.");
//                loadApplications();
//            } else {
//                showAlert("#appFormMsg", "warning", "Save failed.");
//            }
//        }).fail(xhr => showAlert("#appFormMsg", "danger", parseError(xhr)))
//            .always(() => $btn.prop("disabled", false).text("Save"));
//    }

//    function openAppStatusModal(applicationId, currentStatus) {
//        $("#appStatusAppId").val(applicationId);
//        $("#appStatusNew").val(currentStatus || "Submitted");
//        appStatusModal.show();
//    }

//    function updateAppStatus() {
//        const payload = {
//            applicationId: parseInt($("#appStatusAppId").val(), 10),
//            status: $("#appStatusNew").val()
//        };
//        $("#btnUpdateAppStatus").prop("disabled", true).text("Updating...");
//        apiPost("/Admission/UpdateApplicationStatus", payload)
//            .done(res => {
//                if (res?.success) {
//                    appStatusModal.hide();
//                    showAlert("#admissionGlobalMsg", "success", "Application status updated.");
//                    loadApplications();
//                } else {
//                    showAlert("#admissionGlobalMsg", "warning", "Status update failed.");
//                }
//            })
//            .fail(xhr => showAlert("#admissionGlobalMsg", "danger", parseError(xhr)))
//            .always(() => $("#btnUpdateAppStatus").prop("disabled", false).text("Update"));
//    }

//    // ----------------- Fees -----------------
//    function openFeesModal(appId) {
//        currentAppId = appId;
//        clearAlert("#feesMsg");
//        $("#feesAppId").val(appId);
//        $("#feeAmount, #feeRef, #feeRemarks").val("");
//        $("#feeCurrency").val("INR");
//        $("#feeMode").val("Cash");
//        const now = new Date();
//        const pad = n => String(n).padStart(2, "0");
//        const dtLocal = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours())}:${pad(now.getMinutes())}`;
//        $("#feePaymentDate").val(dtLocal);
//        $("#feesAppLabel").text("ID " + appId);

//        apiGet("/Admission/GetApplicationById", { applicationId: appId })
//            .done(a => {
//                currentAppMeta = { year: a.academicYear || "", cls: a.classAppliedFor || "", applicant: a.applicantName || "" };
//                $("#feesAppLabel").text(`${currentAppMeta.applicant} (ID ${appId})`);
//                loadFees(appId);
//                if (currentAppMeta.year && currentAppMeta.cls) {
//                    loadFeeSummary(currentAppMeta.year, currentAppMeta.cls);
//                } else {
//                    $("#feeSummaryBox").text("Select AY/Class to view summary.");
//                }
//            })
//            .fail(() => { loadFees(appId); });
//        feesModal.show();
//    }

//    function loadFees(applicationId) {
//        const $tb = $("#feesTable tbody");
//        $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//        apiGet("/Admission/GetApplicationFees", { applicationId })
//            .done(list => {
//                if (!Array.isArray(list) || list.length === 0) {
//                    $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No payments.</td></tr>`);
//                    return;
//                }
//                const rows = list.map(p => {
//                    const on = p.paymentDate || p.paidOn || p.createdAtUtc;
//                    return `
//                        <tr>
//                            <td>${p.paymentId || "-"}</td>
//                            <td>${p.amount ?? "-"}</td>
//                            <td>${p.paymentMode || p.mode || "-"}</td>
//                            <td>${toLocalDateTimeStr(on)}</td>
//                            <td>${p.referenceNo || "-"}</td>
//                            <td>${p.remarks || ""}</td>
//                        </tr>
//                    `;
//                }).join("");
//                $tb.html(rows);
//            })
//            .fail(xhr => showAlert("#feesMsg", "danger", "Failed to load fees: " + parseError(xhr)));
//    }

//    function collectFee() {
//        clearAlert("#feesMsg");
//        const applicationId = parseInt($("#feesAppId").val(), 10);
//        const payload = {
//            applicationId,
//            amount: parseFloat($("#feeAmount").val() || "0"),
//            currency: $("#feeCurrency").val(),
//            paymentMode: $("#feeMode").val(),
//            referenceNo: $("#feeRef").val().trim() || null,
//            remarks: $("#feeRemarks").val().trim() || null,
//            collectedByUserId: currentUserId,
//            paymentDate: getDateTimeLocalValue("#feePaymentDate")
//        };
//        if (!payload.amount || payload.amount <= 0) {
//            showAlert("#feesMsg", "warning", "Enter a valid amount.");
//            return;
//        }
//        const $btn = $("#btnCollectFee").prop("disabled", true).text("Saving...");
//        apiPost("/Admission/CollectApplicationFee", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#feesMsg", "success", "Payment recorded.");
//                    loadFees(applicationId);
//                    if (currentAppMeta.year && currentAppMeta.cls) {
//                        loadFeeSummary(currentAppMeta.year, currentAppMeta.cls);
//                    }
//                } else {
//                    showAlert("#feesMsg", "warning", "Failed to record payment.");
//                }
//            })
//            .fail(xhr => showAlert("#feesMsg", "danger", parseError(xhr)))
//            .always(() => $btn.prop("disabled", false).text("Collect"));
//    }

//    function loadFeeSummary(academicYear, classAppliedFor) {
//        $("#feeSummaryBox").text("Loading summary...");
//        apiGet("/Admission/GetApplicationFeeSummary", { academicYear, classAppliedFor })
//            .done(dto => {
//                $("#feeSummaryBox").text(JSON.stringify(dto || {}, null, 2));
//            })
//            .fail(() => $("#feeSummaryBox").text("Failed to load summary."));
//    }

//    // ----------------- Application Documents -----------------
//    function openAppDocsModal(appId) {
//        currentAppId = appId;
//        clearAlert("#appDocsMsg");
//        $("#docsAppId").val(appId);
//        $("#docsAppLabel").text("ID " + appId);
//        $("#appDocFile").val(""); $("#appDocDesc").val("");

//        apiGet("/Admission/GetApplicationById", { applicationId: appId })
//            .done(a => $("#docsAppLabel").text(`${a.applicantName || ""} (ID ${appId})`))
//            .always(() => {
//                loadAppDocuments(appId);
//                appDocsModal.show();
//            });
//    }

//    function loadAppDocuments(applicationId) {
//        const $tb = $("#appDocsTable tbody");
//        $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//        apiGet("/Admission/GetApplicationDocuments", { applicationId })
//            .done(list => {
//                if (!Array.isArray(list) || list.length === 0) {
//                    $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No documents.</td></tr>`);
//                    return;
//                }
//                const rows = list.map(d => {
//                    const fileName = d.fileName || (d.filePath ? d.filePath.split('/').pop() : "-");
//                    const path = d.filePath || "#";
//                    const verified = (d.isVerified ?? d.verified ?? d.documentsVerified) ? "Yes" : "No";
//                    const uploaded = d.uploadedOn || d.createdAtUtc;
//                    return `
//                        <tr data-doc-id="${d.documentId}">
//                            <td>${d.documentId}</td>
//                            <td><a href="${path}" target="_blank" rel="noopener">${fileName}</a></td>
//                            <td>${d.description || ""}</td>
//                            <td>${toLocalDateTimeStr(uploaded)}</td>
//                            <td>${verified}</td>
//                            <td>
//                                <div class="btn-group btn-group-sm">
//                                    <button class="btn btn-outline-success btn-verify-doc">Verify</button>
//                                </div>
//                            </td>
//                        </tr>
//                    `;
//                }).join("");
//                $tb.html(rows);
//            })
//            .fail(xhr => showAlert("#appDocsMsg", "danger", "Failed to load documents: " + parseError(xhr)));
//    }

//    function uploadAppDoc() {
//        clearAlert("#appDocsMsg");
//        const appId = parseInt($("#docsAppId").val(), 10);
//        const file = $("#appDocFile")[0].files[0];
//        if (!file) return showAlert("#appDocsMsg", "warning", "Choose a file.");
//        const fd = new FormData();
//        fd.append("applicationId", appId);
//        fd.append("description", $("#appDocDesc").val());
//        fd.append("file", file);

//        const $btn = $("#btnUploadAppDoc").prop("disabled", true).text("Uploading...");
//        apiUpload("/Admission/UploadApplicationDocument", fd)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#appDocsMsg", "success", "Document uploaded.");
//                    $("#appDocFile").val(""); $("#appDocDesc").val("");
//                    loadAppDocuments(appId);
//                } else {
//                    showAlert("#appDocsMsg", "warning", "Upload failed.");
//                }
//            })
//            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)))
//            .always(() => $btn.prop("disabled", false).text("Upload"));
//    }

//    function verifySingleDoc(docId) {
//        const payload = { documentId: docId, verifiedByUserId: currentUserId, verified: true };
//        apiPost("/Admission/VerifyApplicationDocument", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#appDocsMsg", "success", "Document verified.");
//                    loadAppDocuments(parseInt($("#docsAppId").val(), 10));
//                } else {
//                    showAlert("#appDocsMsg", "warning", "Verification failed.");
//                }
//            })
//            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)));
//    }

//    function markAllDocsVerified() {
//        const payload = { applicationId: parseInt($("#docsAppId").val(), 10), documentsVerified: true };
//        apiPost("/Admission/SetApplicationDocumentsVerified", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#appDocsMsg", "success", "All documents marked verified.");
//                    loadAppDocuments(payload.applicationId);
//                } else {
//                    showAlert("#appDocsMsg", "warning", "Update failed.");
//                }
//            })
//            .fail(xhr => showAlert("#appDocsMsg", "danger", parseError(xhr)));
//    }

//    // ----------------- Confirm Admission -----------------
//    function openConfirmModal(appId) {
//        clearAlert("#confirmMsg");
//        $("#confirmAppId").val(appId);
//        $("#confirmSection").val("");
//        const today = new Date();
//        $("#confirmEnrollDate").val(today.toISOString().substring(0, 10));
//        $("#confirmAppLabel").text("ID " + appId);

//        apiGet("/Admission/GetApplicationById", { applicationId: appId })
//            .done(a => $("#confirmAppLabel").text(`${a.applicantName || ""} (ID ${appId})`));
//        confirmModal.show();
//    }

//    function confirmAdmission() {
//        clearAlert("#confirmMsg");
//        const payload = {
//            applicationId: parseInt($("#confirmAppId").val(), 10),
//            section: $("#confirmSection").val().trim() || null,
//            enrollmentDate: getDateValue("#confirmEnrollDate")
//        };
//        const $btn = $("#btnConfirmAdmission").prop("disabled", true).text("Confirming...");
//        apiPost("/Admission/ConfirmAdmission", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#confirmMsg", "success", `Admission confirmed. Student ID = ${res.studentId}`);
//                    setTimeout(() => { confirmModal.hide(); loadApplications(); }, 1200);
//                } else {
//                    showAlert("#confirmMsg", "warning", "Confirmation failed");
//                }
//            })
//            .fail(xhr => showAlert("#confirmMsg", "danger", parseError(xhr)))
//            .always(() => $btn.prop("disabled", false).text("Confirm"));
//    }

//    // ----------------- Shortlist & Merit -----------------
//    function generateShortlist() {
//        clearAlert("#shortMsg");
//        const payload = {
//            academicYear: $("#shortYear").val().trim(),
//            classAppliedFor: $("#shortClass").val().trim(),
//            minEntranceScore: $("#shortMinScore").val() ? parseFloat($("#shortMinScore").val()) : null
//        };
//        apiPost("/Admission/GenerateShortlist", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#shortMsg", "success", `Shortlist generated. Count = ${res.count}`);
//                    loadShortlist(payload.academicYear, payload.classAppliedFor);
//                } else {
//                    showAlert("#shortMsg", "warning", "Shortlist generation failed.");
//                }
//            })
//            .fail(xhr => showAlert("#shortMsg", "danger", parseError(xhr)));
//    }
//    function loadShortlist(academicYear, classAppliedFor) {
//        $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-muted">Loading...</td></tr>`);
//        apiGet("/Admission/GetShortlist", { academicYear, classAppliedFor })
//            .done(list => {
//                if (!Array.isArray(list) || list.length === 0) {
//                    $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-muted">No data.</td></tr>`);
//                    return;
//                }
//                const rows = list.map(x => {
//                    const appId = x.applicationId ?? x.appId ?? "-";
//                    const name = x.applicantName ?? x.name ?? "-";
//                    const score = (x.entranceScore ?? x.score ?? "-");
//                    return `<tr><td>${appId}</td><td>${name}</td><td>${score}</td></tr>`;
//                }).join("");
//                $("#shortlistTable tbody").html(rows);
//            })
//            .fail(xhr => $("#shortlistTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//    }

//    function generateMerit() {
//        clearAlert("#meritMsg");
//        const payload = {
//            academicYear: $("#meritYear").val().trim(),
//            classAppliedFor: $("#meritClass").val().trim(),
//            topN: parseInt($("#meritTopN").val(), 10) || 0
//        };
//        apiPost("/Admission/GenerateMeritList", payload)
//            .done(res => {
//                if (res?.success) {
//                    showAlert("#meritMsg", "success", `Merit list generated. Count = ${res.count}`);
//                    loadMerit(payload.academicYear, payload.classAppliedFor);
//                } else {
//                    showAlert("#meritMsg", "warning", "Merit list generation failed.");
//                }
//            })
//            .fail(xhr => showAlert("#meritMsg", "danger", parseError(xhr)));
//    }
//    function loadMerit(academicYear, classAppliedFor) {
//        $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">Loading...</td></tr>`);
//        apiGet("/Admission/GetMeritList", { academicYear, classAppliedFor })
//            .done(list => {
//                if (!Array.isArray(list) || list.length === 0) {
//                    $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">No data.</td></tr>`);
//                    return;
//                }
//                const rows = list.map((x, idx) => {
//                    const rank = x.rank ?? (idx + 1);
//                    const appId = x.applicationId ?? x.appId ?? "-";
//                    const name = x.applicantName ?? x.name ?? "-";
//                    const score = (x.entranceScore ?? x.score ?? "-");
//                    return `<tr><td>${rank}</td><td>${appId}</td><td>${name}</td><td>${score}</td></tr>`;
//                }).join("");
//                $("#meritTable tbody").html(rows);
//            })
//            .fail(xhr => $("#meritTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//    }

//    // ----------------- Events -----------------
//    // Inquiries
//    $("#btnInqRefresh").on("click", loadInquiries);
//    $("#btnNewInquiry").on("click", () => openInquiryModal(0));
//    $("#inqSearch").on("input", renderInquiries);
//    $(document).on("click", ".btn-inq-edit", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        if (id) openInquiryModal(id);
//    });
//    $(document).on("click", ".btn-inq-status", function () {
//        const $tr = $(this).closest("tr");
//        const id = parseInt($tr.data("id"), 10);
//        const status = $tr.find("td:nth-child(7) .badge").text().trim();
//        if (id) openInquiryStatusModal(id, status);
//    });
//    $(document).on("click", ".btn-inq-create-app", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        createAppFromInquiry(id);
//        $("#tab-app").click();
//    });
//    $("#btnSaveInquiry").on("click", saveInquiry);
//    $("#btnUpdateInquiryStatus").on("click", updateInquiryStatus);

//    // Applications
//    $("#btnAppRefresh").on("click", loadApplications);
//    $("#btnNewApplication").on("click", () => createAppFromInquiry(null));
//    $("#appSearch").on("input", renderApplications);
//    $(document).on("click", ".btn-app-edit", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        if (id) openAppModal(id);
//    });
//    $(document).on("click", ".btn-app-status", function () {
//        const $tr = $(this).closest("tr");
//        const id = parseInt($tr.data("id"), 10);
//        const status = $tr.find("td:nth-child(6) .badge").text().trim();
//        if (id) openAppStatusModal(id, status);
//    });
//    $(document).on("click", ".btn-app-fees", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        if (id) openFeesModal(id);
//    });
//    $(document).on("click", ".btn-app-docs", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        if (id) openAppDocsModal(id);
//    });
//    $(document).on("click", ".btn-app-confirm", function () {
//        const id = parseInt($(this).closest("tr").data("id"), 10);
//        if (id) openConfirmModal(id);
//    });
//    $("#btnSaveApplication").on("click", saveApplication);
//    $("#btnUpdateAppStatus").on("click", updateAppStatus);

//    // Fees
//    $("#btnCollectFee").on("click", collectFee);

//    // Docs
//    $("#btnUploadAppDoc").on("click", uploadAppDoc);
//    $(document).on("click", ".btn-verify-doc", function () {
//        const docId = parseInt($(this).closest("tr").data("doc-id"), 10);
//        if (docId) verifySingleDoc(docId);
//    });
//    $("#btnMarkAllDocsVerified").on("click", markAllDocsVerified);

//    // Confirm
//    $("#btnConfirmAdmission").on("click", confirmAdmission);

//    // Shortlist/Merit
//    $("#btnGenerateShortlist").on("click", generateShortlist);
//    $("#btnLoadShortlist").on("click", function () {
//        loadShortlist($("#shortYear").val().trim(), $("#shortClass").val().trim());
//    });
//    $("#btnGenerateMerit").on("click", generateMerit);
//    $("#btnLoadMerit").on("click", function () {
//        loadMerit($("#meritYear").val().trim(), $("#meritClass").val().trim());
//    });

//    // Initial load
//    loadInquiries();
//    loadApplications();
//});