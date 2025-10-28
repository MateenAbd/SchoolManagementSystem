// wwwroot/js/script/student.js
// Needs common.js loaded (BASE_URL, ajaxSetup, apiGet/Post/Put/Delete, apiUpload, showAlert, clearAlert)

$(function () {
    if ($("#studentPage").length === 0) return;

    const isAdmin = $("body").attr("data-roles")?.includes("Admin");
    if (!isAdmin) return;

    // Bootstrap modals
    const studentModal = new bootstrap.Modal(document.getElementById("studentModal"), { backdrop: "static" });
    const photoModal = new bootstrap.Modal(document.getElementById("photoModal"), { backdrop: "static" });
    const docsModal = new bootstrap.Modal(document.getElementById("docsModal"), { backdrop: "static" });
    const enrollModal = new bootstrap.Modal(document.getElementById("enrollModal"), { backdrop: "static" });
    const viewModal = new bootstrap.Modal(document.getElementById("viewModal"), { backdrop: "static" });

    let studentsCache = [];
    let currentStudentId = 0; // used for edit/photo/docs/enroll modals
    let viewStudentId = 0;    // used for view modal

    // Helpers
    function parseError(xhr) {
        if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
        if (xhr.responseJSON?.error) return xhr.responseJSON.error;
        return xhr.status + " " + xhr.statusText;
    }
    function toDateStr(d) {
        if (!d) return "-";
        const dt = new Date(d);
        if (isNaN(dt)) return "-";
        return dt.toLocaleDateString();
    }

    // Render table
    function renderTable(list) {
        const $tb = $("#studentTable tbody");
        if (!Array.isArray(list) || list.length === 0) {
            $tb.html(`<tr><td colspan="5" class="p-4 text-center text-muted">No students found.</td></tr>`);
            $("#studentCount").text("");
            return;
        }
        const rows = list.map(s => {
            const id = s.studentId;
            const name = [s.firstName, s.lastName].filter(Boolean).join(" ") || "-";
            const cls = s.className || "";
            const sec = s.section || "";
            return `
                <tr data-id="${id}">
                    <td>${id}</td>
                    <td>${name}</td>
                    <td>${cls}</td>
                    <td>${sec}</td>
                    <td>
                        <div class="btn-group btn-group-sm" role="group">
                            <button class="btn btn-outline-dark btn-view" title="View">View</button>
                            <button class="btn btn-outline-primary btn-edit" title="Edit">Edit</button>
                            <button class="btn btn-outline-secondary btn-photo" title="Photo">Photo</button>
                            <button class="btn btn-outline-info btn-docs" title="Documents">Documents</button>
                            <button class="btn btn-outline-success btn-enroll" title="Enrollment">Enrollment</button>
                            <button class="btn btn-outline-danger btn-delete" title="Delete">Delete</button>
                        </div>
                    </td>
                </tr>
            `;
        }).join("");
        $tb.html(rows);
        $("#studentCount").text(`${list.length} student(s)`);
    }

    function applyFilter() {
        const q = ($("#txtSearch").val() || "").toLowerCase();
        if (!q) return renderTable(studentsCache);
        const filtered = studentsCache.filter(s => {
            const name = [s.firstName || "", s.lastName || ""].join(" ").toLowerCase();
            const cls = (s.className || "").toLowerCase();
            const sec = (s.section || "").toLowerCase();
            return name.includes(q) || cls.includes(q) || sec.includes(q);
        });
        renderTable(filtered);
    }

    function loadStudents() {
        $("#studentTable tbody").html(`<tr><td colspan="5" class="p-4 text-center text-muted">Loading...</td></tr>`);
        apiGet("/Student/GetStudentList")
            .done(list => {
                studentsCache = Array.isArray(list) ? list : [];
                applyFilter();
            })
            .fail(xhr => {
                showAlert("#studentGlobalMsg", "danger", "Failed to load students: " + parseError(xhr));
                $("#studentTable tbody").html(`<tr><td colspan="5" class="p-4 text-center text-danger">Error loading students</td></tr>`);
            });
    }

    // Student form modal
    function openStudentForm(studentId) {
        clearAlert("#studentFormMsg");
        $("#studentId").val(studentId || 0);
        $("#firstName, #lastName, #className, #section, #email, #phone, #address, #guardianName, #healthInfo").val("");
        $("#dob").val("");
        $("#gender").val("");

        if (studentId && studentId > 0) {
            $("#studentModalLabel").text("Edit Student");
            apiGet("/Student/GetStudentById", { studentId })
                .done(dto => {
                    $("#studentId").val(dto.studentId || studentId);
                    $("#firstName").val(dto.firstName || "");
                    $("#lastName").val(dto.lastName || "");
                    $("#className").val(dto.className || "");
                    $("#section").val(dto.section || "");
                    $("#email").val(dto.email || "");
                    $("#phone").val(dto.phone || "");
                    $("#address").val(dto.address || "");
                    $("#guardianName").val(dto.guardianName || "");
                    $("#healthInfo").val(dto.healthInfo || "");
                    if (dto.dateOfBirth) {
                        const d = new Date(dto.dateOfBirth);
                        if (!isNaN(d)) $("#dob").val(d.toISOString().substring(0, 10));
                    }
                    $("#gender").val(dto.gender || "");
                })
                .fail(xhr => showAlert("#studentFormMsg", "danger", parseError(xhr)))
                .always(() => studentModal.show());
        } else {
            $("#studentModalLabel").text("Add Student");
            studentModal.show();
        }
    }
    function saveStudent() {
        clearAlert("#studentFormMsg");
        const studentId = parseInt($("#studentId").val(), 10) || 0;

        const dto = {
            studentId: studentId,
            firstName: $("#firstName").val().trim(),
            lastName: $("#lastName").val().trim(),
            className: $("#className").val().trim(),
            section: $("#section").val().trim(),
            dateOfBirth: $("#dob").val() || null,
            gender: $("#gender").val(),
            email: $("#email").val().trim(),
            phone: $("#phone").val().trim(),
            address: $("#address").val().trim(),
            guardianName: $("#guardianName").val().trim(),
            healthInfo: $("#healthInfo").val().trim()
        };

        const $btn = $("#btnSaveStudent").prop("disabled", true).text("Saving...");
        const req = studentId > 0 ? apiPut("/Student/UpdateStudent", dto) : apiPost("/Student/AddStudent", dto);

        req.done(res => {
            if (res?.success) {
                studentModal.hide();
                showAlert("#studentGlobalMsg", "success", (studentId > 0 ? "Updated" : "Created") + " successfully.");
                loadStudents();
            } else {
                showAlert("#studentFormMsg", "warning", "Save failed.");
            }
        }).fail(xhr => {
            showAlert("#studentFormMsg", "danger", parseError(xhr));
        }).always(() => $btn.prop("disabled", false).text("Save"));
    }

    // Photo modal
    function openPhotoModal(studentId) {
        currentStudentId = studentId;
        clearAlert("#photoMsg");
        $("#photoFile").val("");
        $("#photoPreview").hide().attr("src", "");
        $("#photoStudentLabel").text("ID " + studentId);

        apiGet("/Student/GetStudentById", { studentId })
            .done(dto => {
                const name = [dto.firstName, dto.lastName].filter(Boolean).join(" ");
                $("#photoStudentLabel").text(`${name} (ID ${studentId})`);
                if (dto.photoUrl) $("#photoPreview").attr("src", dto.photoUrl).show();
            })
            .always(() => photoModal.show());
    }
    function uploadPhoto() {
        clearAlert("#photoMsg");
        const file = $("#photoFile")[0].files[0];
        if (!file) {
            showAlert("#photoMsg", "warning", "Choose a photo file.");
            return;
        }
        const fd = new FormData();
        fd.append("studentId", currentStudentId);
        fd.append("file", file);

        const $btn = $("#btnUploadPhoto").prop("disabled", true).text("Uploading...");
        apiUpload("/Student/UploadPhoto", fd)
            .done(res => {
                if (res?.success) {
                    showAlert("#photoMsg", "success", "Photo uploaded.");
                    if (res.photoUrl) $("#photoPreview").attr("src", res.photoUrl).show();
                    loadStudents();
                } else {
                    showAlert("#photoMsg", "warning", "Upload failed.");
                }
            })
            .fail(xhr => showAlert("#photoMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Upload Photo"));
    }

    // Documents modal
    function openDocsModal(studentId) {
        currentStudentId = studentId;
        clearAlert("#docsMsg");
        $("#docFile").val("");
        $("#docDesc").val("");
        $("#docsStudentLabel").text("ID " + studentId);
        docsModal.show();

        apiGet("/Student/GetStudentById", { studentId }).done(dto => {
            const name = [dto.firstName, dto.lastName].filter(Boolean).join(" ");
            $("#docsStudentLabel").text(`${name} (ID ${studentId})`);
        });

        loadDocuments(studentId);
    }
    function loadDocuments(studentId) {
        const $tb = $("#docsTable tbody");
        $tb.html(`<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>`);
        apiGet("/Student/GetStudentDocuments", { studentId })
            .done(docs => {
                if (!Array.isArray(docs) || docs.length === 0) {
                    $tb.html(`<tr><td colspan="5" class="p-3 text-center text-muted">No documents.</td></tr>`);
                    return;
                }
                const rows = docs.map(d => {
                    const uploaded = d.uploadedOn ? new Date(d.uploadedOn).toLocaleString() : "-";
                    const fileName = d.fileName || (d.filePath ? d.filePath.split('/').pop() : "-");
                    const safePath = d.filePath || "#";
                    return `
                        <tr data-doc-id="${d.documentId}">
                            <td>${d.documentId}</td>
                            <td><a href="${safePath}" target="_blank" rel="noopener">${fileName}</a></td>
                            <td>${d.description || ""}</td>
                            <td>${uploaded}</td>
                            <td><button class="btn btn-sm btn-outline-danger btn-del-doc">Delete</button></td>
                        </tr>
                    `;
                }).join("");
                $tb.html(rows);
            })
            .fail(xhr => showAlert("#docsMsg", "danger", "Failed to load documents: " + parseError(xhr)));
    }
    function uploadDocument() {
        clearAlert("#docsMsg");
        const file = $("#docFile")[0].files[0];
        if (!file) {
            showAlert("#docsMsg", "warning", "Choose a document file.");
            return;
        }
        const desc = $("#docDesc").val();

        const fd = new FormData();
        fd.append("studentId", currentStudentId);
        fd.append("description", desc);
        fd.append("file", file);

        const $btn = $("#btnUploadDoc").prop("disabled", true).text("Uploading...");
        apiUpload("/Student/UploadDocument", fd)
            .done(res => {
                if (res?.success) {
                    showAlert("#docsMsg", "success", "Document uploaded.");
                    $("#docFile").val(""); $("#docDesc").val("");
                    loadDocuments(currentStudentId);
                } else {
                    showAlert("#docsMsg", "warning", "Upload failed.");
                }
            })
            .fail(xhr => showAlert("#docsMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Upload"));
    }
    function deleteDocument(docId) {
        apiDelete("/Student/DeleteStudentDocumentById", docId)
            .done(res => {
                if (res?.success) {
                    showAlert("#docsMsg", "success", "Document deleted.");
                    loadDocuments(currentStudentId);
                } else {
                    showAlert("#docsMsg", "warning", "Delete failed.");
                }
            })
            .fail(xhr => showAlert("#docsMsg", "danger", "Delete failed: " + parseError(xhr)));
    }

    // Enrollment modal
    function openEnrollModal(studentId) {
        currentStudentId = studentId;
        clearAlert("#enrollMsg");
        $("#enrollmentId").val(0);
        $("#enrollAcademicYear, #enrollClassName, #enrollSection, #enrollDate").val("");
        $("#enrollIsActive").val("true");
        $("#enrollStudentLabel").text("ID " + studentId);

        apiGet("/Student/GetStudentById", { studentId }).done(dto => {
            const name = [dto.firstName, dto.lastName].filter(Boolean).join(" ");
            $("#enrollStudentLabel").text(`${name} (ID ${studentId})`);
        });

        apiGet("/Student/GetEnrollmentByStudent", { studentId })
            .done(e => {
                $("#enrollmentId").val(e.enrollmentId || 0);
                $("#enrollAcademicYear").val(e.academicYear || "");
                $("#enrollClassName").val(e.className || "");
                $("#enrollSection").val(e.section || "");
                if (e.enrollmentDate) {
                    const d = new Date(e.enrollmentDate);
                    if (!isNaN(d)) $("#enrollDate").val(d.toISOString().substring(0, 10));
                }
                $("#enrollIsActive").val((e.isActive === false) ? "false" : "true");
            })
            .fail(xhr => {
                if (xhr.status !== 404) showAlert("#enrollMsg", "danger", parseError(xhr));
            })
            .always(() => enrollModal.show());
    }
    function saveEnrollment() {
        clearAlert("#enrollMsg");
        const payload = {
            enrollmentId: parseInt($("#enrollmentId").val(), 10) || 0,
            studentId: currentStudentId,
            academicYear: $("#enrollAcademicYear").val().trim(),
            className: $("#enrollClassName").val().trim(),
            section: $("#enrollSection").val().trim(),
            isActive: $("#enrollIsActive").val() === "true",
            enrollmentDate: $("#enrollDate").val() || null
        };

        const $btn = $("#btnSaveEnrollment").prop("disabled", true).text("Saving...");
        apiPost("/Student/AddOrUpdateEnrollment", payload)
            .done(res => {
                if (res?.success) {
                    $("#enrollmentId").val(res.id || payload.enrollmentId);
                    showAlert("#enrollMsg", "success", "Enrollment saved.");
                } else {
                    showAlert("#enrollMsg", "warning", "Save failed.");
                }
            })
            .fail(xhr => showAlert("#enrollMsg", "danger", parseError(xhr)))
            .always(() => $btn.prop("disabled", false).text("Save"));
    }

    // VIEW modal (consolidated)
    function openViewModal(studentId) {
        viewStudentId = studentId;

        // Reset view fields
        $("#vTitle").text("-");
        $("#vStudentId, #vFullName, #vClass, #vSection, #vGender, #vDob, #vGuardian, #vEmail, #vPhone, #vAddress, #vHealth")
            .text("-");
        $("#vPhoto, #vPhotoLarge").hide().attr("src", "");
        $("#vNoPhoto, #vNoPhoto2").show();

        // Documents table
        $("#viewDocsTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">Loading...</td></tr>`);
        // Enrollment fields
        $("#vEnrollYear, #vEnrollClass, #vEnrollSection, #vEnrollActive, #vEnrollDate").text("-");

        // Load student details
        apiGet("/Student/GetStudentById", { studentId })
            .done(dto => {
                const fullName = [dto.firstName, dto.lastName].filter(Boolean).join(" ") || "-";
                $("#vTitle").text(`${fullName} (ID ${dto.studentId})`);
                $("#vStudentId").text(dto.studentId || studentId);
                $("#vFullName").text(fullName);
                $("#vClass").text(dto.className || "-");
                $("#vSection").text(dto.section || "-");
                $("#vGender").text(dto.gender || "-");
                $("#vDob").text(toDateStr(dto.dateOfBirth));
                $("#vGuardian").text(dto.guardianName || "-");
                $("#vEmail").text(dto.email || "-");
                $("#vPhone").text(dto.phone || "-");
                $("#vAddress").text(dto.address || "-");
                $("#vHealth").text(dto.healthInfo || "-");

                if (dto.photoUrl) {
                    $("#vPhoto").attr("src", dto.photoUrl).show();
                    $("#vPhotoLarge").attr("src", dto.photoUrl).show();
                    $("#vNoPhoto, #vNoPhoto2").hide();
                }
            })
            .fail(xhr => showAlert("#vAboutMsg", "danger", parseError(xhr)));

        // Load documents
        loadViewDocuments(studentId);

        // Load enrollment
        loadViewEnrollment(studentId);

        viewModal.show();
    }

    function loadViewDocuments(studentId) {
        const $tb = $("#viewDocsTable tbody");
        apiGet("/Student/GetStudentDocuments", { studentId })
            .done(docs => {
                if (!Array.isArray(docs) || docs.length === 0) {
                    $tb.html(`<tr><td colspan="4" class="p-3 text-center text-muted">No documents.</td></tr>`);
                    return;
                }
                const rows = docs.map(d => {
                    const uploaded = d.uploadedOn ? new Date(d.uploadedOn).toLocaleString() : "-";
                    const fileName = d.fileName || (d.filePath ? d.filePath.split('/').pop() : "-");
                    const safePath = d.filePath || "#";
                    return `
                        <tr>
                            <td>${d.documentId}</td>
                            <td><a href="${safePath}" target="_blank" rel="noopener">${fileName}</a></td>
                            <td>${d.description || ""}</td>
                            <td>${uploaded}</td>
                        </tr>
                    `;
                }).join("");
                $tb.html(rows);
            })
            .fail(xhr => showAlert("#vDocsMsg", "danger", "Failed to load documents: " + parseError(xhr)));
    }

    function loadViewEnrollment(studentId) {
        apiGet("/Student/GetEnrollmentByStudent", { studentId })
            .done(e => {
                $("#vEnrollYear").text(e.academicYear || "-");
                $("#vEnrollClass").text(e.className || "-");
                $("#vEnrollSection").text(e.section || "-");
                $("#vEnrollActive").text(e.isActive === false ? "No" : "Yes");
                $("#vEnrollDate").text(toDateStr(e.enrollmentDate));
            })
            .fail(xhr => {
                if (xhr.status === 404) {
                    $("#vEnrollYear").text("—");
                    $("#vEnrollClass").text("—");
                    $("#vEnrollSection").text("—");
                    $("#vEnrollActive").text("—");
                    $("#vEnrollDate").text("—");
                } else {
                    showAlert("#vEnrollMsg", "danger", "Failed to load enrollment: " + parseError(xhr));
                }
            });
    }

    // Events: toolbar
    $("#btnRefreshStudents").on("click", loadStudents);
    $("#btnNewStudent").on("click", () => openStudentForm(0));
    $("#txtSearch").on("input", applyFilter);

    // Events: table actions
    $(document).on("click", ".btn-view", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openViewModal(id);
    });
    $(document).on("click", ".btn-edit", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openStudentForm(id);
    });
    $(document).on("click", ".btn-photo", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openPhotoModal(id);
    });
    $(document).on("click", ".btn-docs", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openDocsModal(id);
    });
    $(document).on("click", ".btn-enroll", function () {
        const id = parseInt($(this).closest("tr").data("id"), 10);
        if (id) openEnrollModal(id);
    });
    $(document).on("click", ".btn-delete", function () {
        const id = parseInt($(this).closest("tr").data["id"], 10) || parseInt($(this).closest("tr").attr("data-id"), 10);
        if (!id) return;
        if (!confirm("Delete student ID " + id + " ?")) return;

        apiDelete("/Student/DeleteStudentById", id)
            .done(res => {
                if (res?.success) {
                    showAlert("#studentGlobalMsg", "success", "Deleted successfully.");
                    loadStudents();
                } else {
                    showAlert("#studentGlobalMsg", "warning", "Delete failed.");
                }
            })
            .fail(xhr => showAlert("#studentGlobalMsg", "danger", "Delete failed: " + parseError(xhr)));
    });

    // Events: modal buttons
    $("#btnSaveStudent").on("click", saveStudent);
    $("#btnUploadPhoto").on("click", uploadPhoto);
    $("#btnUploadDoc").on("click", uploadDocument);
    $(document).on("click", ".btn-del-doc", function () {
        const docId = parseInt($(this).closest("tr").data("doc-id"), 10);
        if (!docId) return;
        if (!confirm("Delete document ID " + docId + " ?")) return;
        deleteDocument(docId);
    });
    $("#btnSaveEnrollment").on("click", saveEnrollment);
    $("#btnReloadEnrollment").on("click", () => openEnrollModal(currentStudentId));

    // View modal toolbar actions
    $("#btnViewOpenEdit").on("click", function () {
        if (viewStudentId) openStudentForm(viewStudentId);
    });
    $("#btnViewOpenPhoto").on("click", function () {
        if (viewStudentId) openPhotoModal(viewStudentId);
    });
    $("#btnViewOpenDocs").on("click", function () {
        if (viewStudentId) openDocsModal(viewStudentId);
    });
    $("#btnViewOpenEnroll").on("click", function () {
        if (viewStudentId) openEnrollModal(viewStudentId);
    });
    $("#btnViewRefresh").on("click", function () {
        if (viewStudentId) openViewModal(viewStudentId);
    });

    // Init
    loadStudents();
});