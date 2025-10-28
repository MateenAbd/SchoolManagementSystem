// wwwroot/js/script/attendance.js
(function () {
    "use strict";

    $(function () {
        if ($("#attendancePage").length === 0) return;

        const roles = ($("body").attr("data-roles") || "");
        const isAdmin = roles.includes("Admin");
        const isTeacher = roles.includes("Teacher");
        const isStudent = roles.includes("Student");
        const currentUserId = parseInt($("body").attr("data-user-id") || "0", 10) || null;

        function parseError(xhr) {
            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
            return xhr.status + " " + xhr.statusText;
        }
        function todayStr() {
            const t = new Date();
            return t.toISOString().substring(0, 10);
        }
        function toLocal(dt) {
            if (!dt) return "-";
            const d = new Date(dt);
            return isNaN(d) ? "-" : d.toLocaleString();
        }
        function setTodayIfEmpty(sel) {
            const $el = $(sel);
            if ($el.length && !$el.val()) $el.val(todayStr());
        }

        // Student attendance
        setTodayIfEmpty("#singleDate");
        setTodayIfEmpty("#bulkDate");
        setTodayIfEmpty("#classDate");
        // Leaves + Summary
        setTodayIfEmpty("#lvFrom"); setTodayIfEmpty("#lvTo");
        setTodayIfEmpty("#lvFilterFrom"); setTodayIfEmpty("#lvFilterTo");
        setTodayIfEmpty("#myLvFrom"); setTodayIfEmpty("#myLvTo");
        setTodayIfEmpty("#sumDate"); setTodayIfEmpty("#staffSumDate");
        // Staff
        setTodayIfEmpty("#stDate"); setTodayIfEmpty("#stRangeFrom"); setTodayIfEmpty("#stRangeTo");
        setTodayIfEmpty("#stDailyDate");
        // Biometric processing and Alerts/Logs
        setTodayIfEmpty("#procFrom"); setTodayIfEmpty("#procTo");
        setTodayIfEmpty("#alDate"); setTodayIfEmpty("#logFrom"); setTodayIfEmpty("#logTo");

        if (isAdmin || isTeacher) {
            // ----- Single mark -----
            $("#btnMarkSingle").on("click", function () {
                clearAlert("#singleMsg");
                const payload = {
                    studentId: parseInt($("#singleStudentId").val(), 10),
                    attendanceDate: $("#singleDate").val() || todayStr(),
                    className: $("#singleClass").val().trim(),
                    section: $("#singleSection").val().trim() || null,
                    status: $("#singleStatus").val(),
                    remarks: $("#singleRemarks").val().trim() || null,
                    markedByUserId: currentUserId,
                    subjectCode: $("#singleSubject").val().trim() || null,
                    periodNo: $("#singlePeriod").val() ? parseInt($("#singlePeriod").val(), 10) : null
                };
                if (!payload.studentId || !payload.className) {
                    showAlert("#singleMsg", "warning", "Student ID and Class are required.");
                    return;
                }
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/MarkStudentAttendance", payload)
                    .done(res => {
                        if (res?.success) {
                            showAlert("#singleMsg", "success", `Marked. Attendance ID = ${res.attendanceId}`);
                        } else {
                            showAlert("#singleMsg", "warning", "Mark failed.");
                        }
                    })
                    .fail(xhr => showAlert("#singleMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Mark Attendance"));
            });

            $("#btnClearSingle").on("click", function () {
                $("#singleStudentId, #singleClass, #singleSection, #singleSubject, #singlePeriod, #singleRemarks").val("");
                $("#singleStatus").val("Present");
                $("#singleDate").val(todayStr());
            });

            let roster = []; // { studentId, name, status }

            function renderRoster() {
                const $tb = $("#bulkTable tbody");
                if (!roster.length) {
                    $tb.html('<tr><td colspan="4" class="p-3 text-center text-muted">No roster loaded.</td></tr>');
                    $("#bulkCount").text("");
                    return;
                }
                const rows = roster.map(r => `
                    <tr data-student-id="${r.studentId}">
                        <td>${r.studentId}</td>
                        <td>${r.name || "-"}</td>
                        <td>
                            <div class="btn-group btn-group-sm" role="group">
                                <button class="btn btn-outline-success btn-st-status ${r.status === 'Present' ? 'active' : ''}" data-val="Present">Present</button>
                                <button class="btn btn-outline-danger btn-st-status ${r.status === 'Absent' ? 'active' : ''}" data-val="Absent">Absent</button>
                                <button class="btn btn-outline-warning btn-st-status ${r.status === 'Late' ? 'active' : ''}" data-val="Late">Late</button>
                                <button class="btn btn-outline-info btn-st-status ${r.status === 'Excused' ? 'active' : ''}" data-val="Excused">Excused</button>
                            </div>
                        </td>
                        <td><button class="btn btn-sm btn-outline-secondary btn-remove-row">Remove</button></td>
                    </tr>
                `).join("");
                $tb.html(rows);
                $("#bulkCount").text(`${roster.length} student(s) in roster`);
            }

            function setAllStatus(s) {
                roster.forEach(r => r.status = s);
                renderRoster();
            }

            function buildItemsFromRoster() {
                return roster.map(r => ({ studentId: r.studentId, status: r.status || "Present" }));
            }

            // Load roster from Student list (Admin) or fallback (Teacher) via existing attendance
            $("#btnLoadRoster").on("click", function () {
                clearAlert("#bulkMsg");
                roster = [];
                renderRoster();

                const cls = $("#bulkClass").val().trim();
                const sec = $("#bulkSection").val().trim();
                if (!cls) { showAlert("#bulkMsg", "warning", "Class is required."); return; }

                const $btn = $(this).prop("disabled", true).text("Loading...");
                apiGet("/Student/GetStudentList")
                    .done(list => {
                        const filtered = (list || []).filter(s => (s.className || "").toLowerCase() === cls.toLowerCase()
                            && (!sec || (s.section || "").toLowerCase() === sec.toLowerCase()));
                        if (!filtered.length) {
                            showAlert("#bulkMsg", "info", "No students found for selected Class/Section.");
                        }
                        roster = filtered.map(s => ({
                            studentId: s.studentId,
                            name: [s.firstName, s.lastName].filter(Boolean).join(" "),
                            status: "Present"
                        }));
                        renderRoster();
                    })
                    .fail(xhr => {
                        if (xhr.status === 403 || xhr.status === 401) {
                            showAlert("#bulkMsg", "info", "No permission to load students list. Loading from today's attendance if available...");
                            $("#btnLoadFromAttendance").trigger("click");
                        } else {
                            showAlert("#bulkMsg", "danger", parseError(xhr));
                        }
                    })
                    .always(() => $btn.prop("disabled", false).text("Load Roster"));
            });

            // Load roster from existing attendance for date/class/section
            $("#btnLoadFromAttendance").on("click", function () {
                clearAlert("#bulkMsg");
                roster = [];
                renderRoster();

                const date = $("#bulkDate").val() || todayStr();
                const cls = $("#bulkClass").val().trim();
                const sec = $("#bulkSection").val().trim() || null;
                const subjectCode = $("#bulkSubject").val().trim() || null;
                const periodNo = $("#bulkPeriod").val() ? parseInt($("#bulkPeriod").val(), 10) : null;
                if (!cls) { showAlert("#bulkMsg", "warning", "Class is required."); return; }

                const $btn = $(this).prop("disabled", true).text("Loading...");
                apiGet("/Attendance/GetClassAttendanceByDate", { attendanceDate: date, className: cls, section: sec, subjectCode, periodNo })
                    .done(list => {
                        const items = Array.isArray(list) ? list : [];
                        if (!items.length) {
                            showAlert("#bulkMsg", "info", "No existing attendance for this selection.");
                        }
                        roster = items.map(a => ({
                            studentId: a.studentId || a.StudentId || a.sid,
                            name: a.studentName || a.name || "",
                            status: a.status || "Present"
                        })).filter(r => r.studentId);
                        renderRoster();
                    })
                    .fail(xhr => showAlert("#bulkMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Load From Today’s Attendance"));
            });

            // Manual add
            $("#btnAddManualRow").on("click", function () {
                const id = prompt("Enter Student ID:");
                const sid = parseInt(id || "0", 10);
                if (!sid) return;
                if (roster.some(r => r.studentId === sid)) return;
                roster.push({ studentId: sid, name: "", status: "Present" });
                renderRoster();
            });

            // Helpers
            $("#btnClearRoster").on("click", function () { roster = []; renderRoster(); });
            $("#btnAllPresent").on("click", () => setAllStatus("Present"));
            $("#btnAllAbsent").on("click", () => setAllStatus("Absent"));

            // Row actions
            $(document).on("click", ".btn-st-status", function () {
                const s = $(this).data("val");
                const sid = parseInt($(this).closest("tr").data("student-id"), 10);
                const item = roster.find(r => r.studentId === sid);
                if (item) item.status = s;
                renderRoster();
            });
            $(document).on("click", ".btn-remove-row", function () {
                const sid = parseInt($(this).closest("tr").data("student-id"), 10);
                roster = roster.filter(r => r.studentId !== sid);
                renderRoster();
            });

            // Save bulk
            $("#btnSaveBulk").on("click", function () {
                clearAlert("#bulkMsg");
                if (!roster.length) { showAlert("#bulkMsg", "warning", "No students in roster."); return; }

                const payload = {
                    attendanceDate: $("#bulkDate").val() || todayStr(),
                    className: $("#bulkClass").val().trim(),
                    section: $("#bulkSection").val().trim() || null,
                    markedByUserId: currentUserId,
                    subjectCode: $("#bulkSubject").val().trim() || null,
                    periodNo: $("#bulkPeriod").val() ? parseInt($("#bulkPeriod").val(), 10) : null,
                    items: buildItemsFromRoster()
                };
                if (!payload.className) { showAlert("#bulkMsg", "warning", "Class is required."); return; }

                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/BulkMarkClassAttendance", payload)
                    .done(res => {
                        if (res?.success) {
                            showAlert("#bulkMsg", "success", `Saved. Count = ${res.count}`);
                        } else {
                            showAlert("#bulkMsg", "warning", "Bulk save failed.");
                        }
                    })
                    .fail(xhr => showAlert("#bulkMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Save Bulk Attendance"));
            });

            // ----- Manage Class Attendance -----
            function loadClassAttendance() {
                const date = $("#classDate").val() || todayStr();
                const cls = $("#classClass").val().trim();
                const sec = $("#classSection").val().trim() || null;
                const subjectCode = $("#classSubject").val().trim() || null;
                const periodNo = $("#classPeriod").val() ? parseInt($("#classPeriod").val(), 10) : null;

                if (!cls) { showAlert("#classMsg", "warning", "Class is required."); return; }

                $("#classTable tbody").html('<tr><td colspan="5" class="p-4 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetClassAttendanceByDate", { attendanceDate: date, className: cls, section: sec, subjectCode, periodNo })
                    .done(list => {
                        const items = Array.isArray(list) ? list : [];
                        if (!items.length) {
                            $("#classTable tbody").html('<tr><td colspan="5" class="p-4 text-center text-muted">No data.</td></tr>');
                            return;
                        }
                        const rows = items.map(a => {
                            const attendanceId = a.attendanceId || a.id || a.AttendanceId;
                            const studentId = a.studentId || a.StudentId;
                            const name = a.studentName || a.name || `Student ${studentId || ""}`;
                            const status = a.status || "Present";
                            const remarks = a.remarks || "";
                            return `
                                <tr data-att-id="${attendanceId}">
                                    <td>${studentId || "-"}</td>
                                    <td>${name}</td>
                                    <td>
                                        <select class="form-select form-select-sm att-status">
                                            <option ${status === 'Present' ? 'selected' : ''}>Present</option>
                                            <option ${status === 'Absent' ? 'selected' : ''}>Absent</option>
                                            <option ${status === 'Late' ? 'selected' : ''}>Late</option>
                                            <option ${status === 'Excused' ? 'selected' : ''}>Excused</option>
                                        </select>
                                    </td>
                                    <td><input type="text" class="form-control form-control-sm att-remarks" value="${remarks}"/></td>
                                    <td>
                                        <div class="btn-group btn-group-sm">
                                            <button class="btn btn-outline-primary btn-update-att">Update</button>
                                            <button class="btn btn-outline-danger btn-del-att">Delete</button>
                                        </div>
                                    </td>
                                </tr>
                            `;
                        }).join("");
                        $("#classTable tbody").html(rows);
                    })
                    .fail(xhr => showAlert("#classMsg", "danger", parseError(xhr)));
            }
            $("#btnLoadClassAtt").on("click", loadClassAttendance);

            $(document).on("click", ".btn-update-att", function () {
                clearAlert("#classMsg");
                const $tr = $(this).closest("tr");
                const attId = parseInt($tr.data("att-id"), 10);
                const status = $tr.find(".att-status").val();
                const remarks = $tr.find(".att-remarks").val() || null;
                const payload = { attendanceId: attId, status, remarks };
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/UpdateStudentAttendanceStatus", payload)
                    .done(res => {
                        if (res?.success) {
                            showAlert("#classMsg", "success", "Updated.");
                        } else {
                            showAlert("#classMsg", "warning", "Update failed.");
                        }
                    })
                    .fail(xhr => showAlert("#classMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Update"));
            });

            $(document).on("click", ".btn-del-att", function () {
                clearAlert("#classMsg");
                const $tr = $(this).closest("tr");
                const attId = parseInt($tr.data("att-id"), 10);
                if (!confirm("Delete this attendance record?")) return;
                apiPost("/Attendance/DeleteStudentAttendance", { attendanceId: attId })
                    .done(res => {
                        if (res?.success) {
                            showAlert("#classMsg", "success", "Deleted.");
                            $tr.remove();
                            if (!$("#classTable tbody tr").length) {
                                $("#classTable tbody").html('<tr><td colspan="5" class="p-4 text-center text-muted">No data.</td></tr>');
                            }
                        } else {
                            showAlert("#classMsg", "warning", "Delete failed.");
                        }
                    })
                    .fail(xhr => showAlert("#classMsg", "danger", parseError(xhr)));
            });
        }
        if (isAdmin || isTeacher || isStudent) {
            // ----- Apply Leave (any authenticated user) -----
            $("#btnApplyLeave").on("click", function () {
                clearAlert("#leaveApplyMsg");
                const payload = {
                    studentId: parseInt($("#lvStudentId").val(), 10),
                    leaveType: $("#lvType").val(),
                    fromDate: $("#lvFrom").val() || todayStr(),
                    toDate: $("#lvTo").val() || $("#lvFrom").val() || todayStr(),
                    reason: $("#lvReason").val().trim() || null,
                    appliedByUserId: currentUserId
                };
                if (!payload.studentId) { showAlert("#leaveApplyMsg", "warning", "Student ID is required."); return; }
                const $btn = $(this).prop("disabled", true).text("Submitting...");
                apiPost("/Attendance/ApplyStudentLeave", payload)
                    .done(res => {
                        if (res?.success) {
                            showAlert("#leaveApplyMsg", "success", `Leave applied. ID = ${res.leaveId}`);
                        } else {
                            showAlert("#leaveApplyMsg", "warning", "Apply failed.");
                        }
                    })
                    .fail(xhr => showAlert("#leaveApplyMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Apply"));
            });

            $("#btnClearLeaveForm").on("click", function () {
                $("#lvStudentId, #lvReason").val("");
                $("#lvType").val("Sick");
                const t = todayStr(); $("#lvFrom").val(t); $("#lvTo").val(t);
            });

            // ----- Pending/Recent leaves (Admin/Teacher) -----
            function renderLeaves(list) {
                const $tb = $("#lvTable tbody");
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html('<tr><td colspan="7" class="p-4 text-center text-muted">No data</td></tr>');
                    return;
                }
                const rows = list.map(x => {
                    const id = x.leaveId || x.LeaveId || x.id;
                    const sid = x.studentId || x.StudentId || "";
                    const sname = x.studentName || x.name || `Student ${sid}`;
                    const type = x.leaveType || x.type || "-";
                    const fd = x.fromDate || x.FromDate; const td = x.toDate || x.ToDate;
                    const dates = [fd, td].filter(Boolean).map(d => (new Date(d)).toLocaleDateString()).join(" - ");
                    const status = x.status || "Pending";
                    const reason = x.reason || "";
                    const disabled = (isAdmin || isTeacher) ? "" : "disabled";
                    return `
                        <tr data-lv-id="${id}">
                            <td>${id}</td>
                            <td>${sname} (${sid})</td>
                            <td>${type}</td>
                            <td>${dates || "-"}</td>
                            <td>${status}</td>
                            <td>${reason}</td>
                            <td>
                                <div class="btn-group btn-group-sm" role="group">
                                    <button class="btn btn-outline-success btn-lv-approve" ${disabled}>Approve</button>
                                    <button class="btn btn-outline-danger btn-lv-reject" ${disabled}>Reject</button>
                                    <button class="btn btn-outline-secondary btn-lv-cancel" ${disabled}>Cancel</button>
                                </div>
                            </td>
                        </tr>
                    `;
                }).join("");
                $tb.html(rows);
            }

            function loadPendingLeaves() {
                clearAlert("#leaveListMsg");
                const q = {
                    fromDate: $("#lvFilterFrom").val() || null,
                    toDate: $("#lvFilterTo").val() || null,
                    className: $("#lvFilterClass").val().trim() || null,
                    section: $("#lvFilterSection").val().trim() || null,
                    status: $("#lvFilterStatus").val() || null
                };
                $("#lvTable tbody").html('<tr><td colspan="7" class="p-4 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetPendingStudentLeaves", q)
                    .done(renderLeaves)
                    .fail(xhr => showAlert("#leaveListMsg", "danger", parseError(xhr)));
            }

            $("#btnLoadPendingLeaves").on("click", function () {
                if (!(isAdmin || isTeacher)) { showAlert("#leaveListMsg", "info", "Only Admin/Teacher can load pending leaves."); return; }
                loadPendingLeaves();
            });

            // Approve/Reject/Cancel modals
            const approveModal = new bootstrap.Modal(document.getElementById("lvApproveModal"), { backdrop: "static" });
            const rejectModal = new bootstrap.Modal(document.getElementById("lvRejectModal"), { backdrop: "static" });
            const cancelModal = new bootstrap.Modal(document.getElementById("lvCancelModal"), { backdrop: "static" });

            $(document).on("click", ".btn-lv-approve", function () {
                const id = parseInt($(this).closest("tr").data("lv-id"), 10);
                $("#lvApproveId").val(id); $("#lvApproveIdText").text(id);
                approveModal.show();
            });
            $(document).on("click", ".btn-lv-reject", function () {
                const id = parseInt($(this).closest("tr").data("lv-id"), 10);
                $("#lvRejectId").val(id); $("#lvRejectReason").val("");
                rejectModal.show();
            });
            $(document).on("click", ".btn-lv-cancel", function () {
                const id = parseInt($(this).closest("tr").data("lv-id"), 10);
                $("#lvCancelId").val(id); $("#lvCancelIdText").text(id);
                cancelModal.show();
            });

            $("#btnConfirmApproveLeave").on("click", function () {
                clearAlert("#leaveListMsg");
                const payload = { leaveId: parseInt($("#lvApproveId").val(), 10), approvedByUserId: currentUserId };
                const $btn = $(this).prop("disabled", true).text("Approving...");
                apiPost("/Attendance/ApproveStudentLeave", payload)
                    .done(res => {
                        if (res?.success) { showAlert("#leaveListMsg", "success", "Approved."); approveModal.hide(); loadPendingLeaves(); }
                        else { showAlert("#leaveListMsg", "warning", "Approve failed."); }
                    })
                    .fail(xhr => showAlert("#leaveListMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Approve"));
            });

            $("#btnConfirmRejectLeave").on("click", function () {
                clearAlert("#leaveListMsg");
                const payload = {
                    leaveId: parseInt($("#lvRejectId").val(), 10),
                    approvedByUserId: currentUserId,
                    rejectionReason: $("#lvRejectReason").val().trim() || "Not specified"
                };
                const $btn = $(this).prop("disabled", true).text("Rejecting...");
                apiPost("/Attendance/RejectStudentLeave", payload)
                    .done(res => {
                        if (res?.success) { showAlert("#leaveListMsg", "success", "Rejected."); rejectModal.hide(); loadPendingLeaves(); }
                        else { showAlert("#leaveListMsg", "warning", "Reject failed."); }
                    })
                    .fail(xhr => showAlert("#leaveListMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Reject"));
            });

            $("#btnConfirmCancelLeave").on("click", function () {
                clearAlert("#leaveListMsg");
                const payload = { leaveId: parseInt($("#lvCancelId").val(), 10) };
                const $btn = $(this).prop("disabled", true).text("Cancelling...");
                apiPost("/Attendance/CancelStudentLeave", payload)
                    .done(res => {
                        if (res?.success) { showAlert("#leaveListMsg", "success", "Cancelled."); cancelModal.hide(); loadPendingLeaves(); }
                        else { showAlert("#leaveListMsg", "warning", "Cancel failed."); }
                    })
                    .fail(xhr => showAlert("#leaveListMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Cancel Leave"));
            });

            // ----- Leave history -----
            function renderMyLeaves(list) {
                const $tb = $("#myLvTable tbody");
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html('<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>');
                    return;
                }
                const rows = list.map(x => {
                    const id = x.leaveId || x.LeaveId || x.id;
                    const fd = x.fromDate || x.FromDate; const td = x.toDate || x.ToDate;
                    const dates = [fd, td].filter(Boolean).map(d => (new Date(d)).toLocaleDateString()).join(" - ");
                    const type = x.leaveType || x.type || "-";
                    const status = x.status || "-";
                    const reason = x.reason || "";
                    return `<tr><td>${id}</td><td>${dates || "-"}</td><td>${type}</td><td>${status}</td><td>${reason}</td></tr>`;
                }).join("");
                $tb.html(rows);
            }

            $("#btnLoadMyLeaves").on("click", function () {
                clearAlert("#myLeavesMsg");
                const sid = parseInt($("#myLvStudentId").val(), 10);
                if (!sid) { showAlert("#myLeavesMsg", "warning", "Student ID is required."); return; }
                const q = {
                    studentId: sid,
                    fromDate: $("#myLvFrom").val() || todayStr(),
                    toDate: $("#myLvTo").val() || $("#myLvFrom").val() || todayStr()
                };
                $("#myLvTable tbody").html('<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetStudentLeavesRange", q)
                    .done(renderMyLeaves)
                    .fail(xhr => showAlert("#myLeavesMsg", "danger", parseError(xhr)));
            });

            // ----- Daily summaries -----
            function renderStdSummary(list) {
                const $tb = $("#stdSummaryTable tbody");
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html('<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>');
                    return;
                }
                const rows = list.map(x => {
                    const cls = x.className || x.ClassName || "-";
                    const present = x.presentCount ?? x.PresentCount ?? 0;
                    const absent = x.absentCount ?? x.AbsentCount ?? 0;
                    const total = x.total ?? x.Total ?? (present + absent);
                    const pct = total ? Math.round((present / total) * 100) : 0;
                    return `<tr><td>${cls}</td><td>${present}</td><td>${absent}</td><td>${total}</td><td>${pct}%</td></tr>`;
                }).join("");
                $tb.html(rows);
            }

            $("#btnLoadStdSummary").on("click", function () {
                clearAlert("#stdSumMsg");
                const q = {
                    attendanceDate: $("#sumDate").val() || todayStr(),
                    className: $("#sumClass").val().trim() || null,
                    section: $("#sumSection").val().trim() || null
                };
                $("#stdSummaryTable tbody").html('<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetDailyStudentAttendanceSummary", q)
                    .done(renderStdSummary)
                    .fail(xhr => showAlert("#stdSumMsg", "danger", parseError(xhr)));
            });

            $("#btnLoadStaffSummary").on("click", function () {
                clearAlert("#staffSumMsg");
                const q = { attendanceDate: $("#staffSumDate").val() || todayStr() };
                apiGet("/Attendance/GetDailyStaffAttendanceSummary", q)
                    .done(dto => {
                        $("#ssPresent").text(dto.presentCount ?? dto.PresentCount ?? 0);
                        $("#ssAbsent").text(dto.absentCount ?? dto.AbsentCount ?? 0);
                        $("#ssLate").text(dto.lateCount ?? dto.LateCount ?? 0);
                        $("#ssExcused").text(dto.excusedCount ?? dto.ExcusedCount ?? 0);
                        $("#ssTotal").text(dto.total ?? dto.Total ?? 0);
                    })
                    .fail(xhr => showAlert("#staffSumMsg", "danger", parseError(xhr)));
            });
        }

        if (isAdmin || isTeacher) {
            // ----- Staff: Mark (Admin only) -----
            $("#btnMarkStaff").on("click", function () {
                if (!isAdmin) { showAlert("#staffMarkMsg", "info", "Only Admin can mark staff attendance."); return; }
                clearAlert("#staffMarkMsg");
                const payload = {
                    userId: parseInt($("#stUserId").val(), 10),
                    attendanceDate: $("#stDate").val() || todayStr(),
                    status: $("#stStatus").val(),
                    inTime: $("#stInTime").val() || null,
                    outTime: $("#stOutTime").val() || null,
                    remarks: $("#stRemarks").val().trim() || null,
                    markedByUserId: currentUserId,
                    source: $("#stSource").val()
                };
                if (!payload.userId) { showAlert("#staffMarkMsg", "warning", "User ID is required."); return; }
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/MarkStaffAttendance", payload)
                    .done(res => res?.success ? showAlert("#staffMarkMsg", "success", "Saved. ID = " + res.attendanceId) : showAlert("#staffMarkMsg", "warning", "Save failed."))
                    .fail(xhr => showAlert("#staffMarkMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Mark"));
            });

            $("#btnClearStaffMark").on("click", function () {
                $("#stUserId,#stRemarks,#stInTime,#stOutTime").val("");
                $("#stStatus").val("Present"); $("#stSource").val("Manual"); $("#stDate").val(todayStr());
            });

            // ----- Staff: Range -----
            function renderStaffRange(list) {
                const $tb = $("#stRangeTable tbody");
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html('<tr><td colspan="8" class="p-4 text-center text-muted">No data</td></tr>');
                    return;
                }
                const rows = list.map(x => {
                    const attId = x.attendanceId || x.AttendanceId || x.id;
                    const uid = x.userId || x.UserId || "-";
                    const date = x.attendanceDate || x.AttendanceDate;
                    const status = x.status || "-";
                    const inTime = x.inTime || x.InTime;
                    const outTime = x.outTime || x.OutTime;
                    const remarks = x.remarks || "";
                    const actionBtns = isAdmin
                        ? `<div class="btn-group btn-group-sm">
                             <button class="btn btn-outline-primary btn-st-edit" data-id="${attId}">Edit</button>
                             <button class="btn btn-outline-danger btn-st-del" data-id="${attId}">Delete</button>
                           </div>`
                        : `<span class="text-muted">View only</span>`;
                    return `<tr data-att-id="${attId}">
                                <td>${attId}</td>
                                <td>${uid}</td>
                                <td>${date ? new Date(date).toLocaleDateString() : "-"}</td>
                                <td>${status}</td>
                                <td>${toLocal(inTime)}</td>
                                <td>${toLocal(outTime)}</td>
                                <td>${remarks}</td>
                                <td>${actionBtns}</td>
                            </tr>`;
                }).join("");
                $tb.html(rows);
            }

            $("#btnLoadStaffRange").on("click", function () {
                clearAlert("#staffRangeMsg");
                const uid = parseInt($("#stRangeUserId").val(), 10);
                const fromDate = $("#stRangeFrom").val() || todayStr();
                const toDate = $("#stRangeTo").val() || fromDate;
                if (!uid) { showAlert("#staffRangeMsg", "warning", "User ID required."); return; }
                $("#stRangeTable tbody").html('<tr><td colspan="8" class="p-4 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetStaffAttendanceRange", { userId: uid, fromDate, toDate })
                    .done(renderStaffRange)
                    .fail(xhr => showAlert("#staffRangeMsg", "danger", parseError(xhr)));
            });

            // Update modal and delete
            const stUpdModal = new bootstrap.Modal(document.getElementById("stUpdateModal"), { backdrop: "static" });

            $(document).on("click", ".btn-st-edit", function () {
                if (!isAdmin) { showAlert("#staffRangeMsg", "info", "Only Admin can update."); return; }
                const attId = parseInt($(this).data("id"), 10);
                $("#stUpdAttendanceId").val(attId);
                const $tr = $(this).closest("tr");
                $("#stUpdStatus").val($tr.find("td:nth-child(4)").text().trim());
                $("#stUpdRemarks").val($tr.find("td:nth-child(7)").text().trim());
                $("#stUpdInTime").val(""); $("#stUpdOutTime").val("");
                stUpdModal.show();
            });

            $("#btnDoUpdateStaff").on("click", function () {
                if (!isAdmin) return;
                clearAlert("#staffRangeMsg");
                const payload = {
                    attendanceId: parseInt($("#stUpdAttendanceId").val(), 10),
                    status: $("#stUpdStatus").val(),
                    remarks: $("#stUpdRemarks").val().trim() || null,
                    inTime: $("#stUpdInTime").val() || null,
                    outTime: $("#stUpdOutTime").val() || null
                };
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/UpdateStaffAttendanceStatus", payload)
                    .done(res => {
                        if (res?.success) { showAlert("#staffRangeMsg", "success", "Updated."); stUpdModal.hide(); $("#btnLoadStaffRange").trigger("click"); }
                        else { showAlert("#staffRangeMsg", "warning", "Update failed."); }
                    })
                    .fail(xhr => showAlert("#staffRangeMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Update"));
            });

            $(document).on("click", ".btn-st-del", function () {
                if (!isAdmin) { showAlert("#staffRangeMsg", "info", "Only Admin can delete."); return; }
                const attId = parseInt($(this).data("id"), 10);
                if (!confirm("Delete attendance ID " + attId + " ?")) return;
                apiPost("/Attendance/DeleteStaffAttendance", { attendanceId: attId })
                    .done(res => {
                        if (res?.success) { showAlert("#staffRangeMsg", "success", "Deleted."); $("#btnLoadStaffRange").trigger("click"); }
                        else { showAlert("#staffRangeMsg", "warning", "Delete failed."); }
                    })
                    .fail(xhr => showAlert("#staffRangeMsg", "danger", parseError(xhr)));
            });

            // ----- Staff: Daily list -----
            function renderStaffDaily(list) {
                const $tb = $("#stDailyTable tbody");
                if (!Array.isArray(list) || list.length === 0) {
                    $tb.html('<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>');
                    return;
                }
                const rows = list.map(x => {
                    const name = x.userName || x.staffName || `User ${x.userId || "-"}`;
                    const status = x.status || "-";
                    const inTime = x.inTime || x.InTime;
                    const outTime = x.outTime || x.OutTime;
                    const remarks = x.remarks || "";
                    return `<tr>
                                <td>${name}</td>
                                <td>${status}</td>
                                <td>${toLocal(inTime)}</td>
                                <td>${toLocal(outTime)}</td>
                                <td>${remarks}</td>
                            </tr>`;
                }).join("");
                $tb.html(rows);
            }

            $("#btnLoadStaffDaily").on("click", function () {
                clearAlert("#staffDailyMsg");
                const attendanceDate = $("#stDailyDate").val() || todayStr();
                const status = $("#stDailyStatus").val() || null;
                $("#stDailyTable tbody").html('<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetStaffDailyAttendance", { attendanceDate, status })
                    .done(renderStaffDaily)
                    .fail(xhr => showAlert("#staffDailyMsg", "danger", parseError(xhr)));
            });

            // ----- Biometric: Devices -----
            $("#btnRegisterDevice").on("click", function () {
                if (!isAdmin) { showAlert("#bioDeviceMsg", "info", "Only Admin can register devices."); return; }
                clearAlert("#bioDeviceMsg");
                const payload = {
                    name: $("#bioDevName").val().trim(),
                    serialNo: $("#bioDevSerial").val().trim(),
                    vendor: $("#bioDevVendor").val().trim() || null,
                    isActive: $("#bioDevActive").val() === "true"
                };
                if (!payload.name || !payload.serialNo) { showAlert("#bioDeviceMsg", "warning", "Name and Serial No are required."); return; }
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/RegisterBiometricDevice", payload)
                    .done(res => res?.success ? showAlert("#bioDeviceMsg", "success", "Registered. Device ID = " + res.deviceId) : showAlert("#bioDeviceMsg", "warning", "Register failed."))
                    .fail(xhr => showAlert("#bioDeviceMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Register"));
            });

            $("#btnClearDeviceForm").on("click", function () { $("#bioDevName,#bioDevSerial,#bioDevVendor").val(""); $("#bioDevActive").val("true"); });

            $("#btnLoadDevices").on("click", function () {
                clearAlert("#bioDevicesMsg");
                const isActive = $("#bioListActive").val();
                $("#bioDevicesTable tbody").html('<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetBiometricDevices", { isActive: isActive || null })
                    .done(list => {
                        const $tb = $("#bioDevicesTable tbody");
                        if (!Array.isArray(list) || list.length === 0) { $tb.html('<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>'); return; }
                        const rows = list.map(d => `<tr>
                                <td>${d.deviceId}</td><td>${d.name}</td><td>${d.serialNo}</td><td>${d.vendor || "-"}</td><td>${d.isActive ? "Yes" : "No"}</td>
                            </tr>`).join("");
                        $tb.html(rows);
                    })
                    .fail(xhr => showAlert("#bioDevicesMsg", "danger", parseError(xhr)));
            });

            // ----- Biometric: User Map -----
            $("#btnLoadMaps").on("click", function () {
                clearAlert("#bioMapMsg");
                const deviceId = $("#mapDeviceId").val() ? parseInt($("#mapDeviceId").val(), 10) : null;
                const personType = $("#mapPersonType").val() || null;
                $("#bioMapsTable tbody").html('<tr><td colspan="2" class="p-3 text-center text-muted">Loading...</td></tr>');
                apiGet("/Attendance/GetBiometricUserMaps", { deviceId, personType })
                    .done(list => {
                        const $tb = $("#bioMapsTable tbody");
                        if (!Array.isArray(list) || list.length === 0) { $tb.html('<tr><td colspan="2" class="p-3 text-center text-muted">No data</td></tr>'); return; }
                        const rows = list.map((m, idx) => `<tr>
                                <td>#${idx + 1}</td>
                                <td><pre class="mb-0" style="white-space:pre-wrap;">${JSON.stringify(m, null, 2)}</pre></td>
                            </tr>`).join("");
                        $tb.html(rows);
                    })
                    .fail(xhr => showAlert("#bioMapMsg", "danger", parseError(xhr)));
            });

            $("#btnUpsertMap").on("click", function () {
                if (!isAdmin) { showAlert("#bioMapMsg", "info", "Only Admin can upsert maps."); return; }
                clearAlert("#bioMapMsg");
                let payload = null;
                const raw = $("#mapRawJson").val().trim();
                if (raw) {
                    try { payload = JSON.parse(raw); } catch (e) { showAlert("#bioMapMsg", "danger", "Invalid JSON in Raw JSON."); return; }
                } else {
                    payload = {
                        deviceId: parseInt($("#mapUpsertDeviceId").val(), 10),
                        personType: $("#mapUpsertPersonType").val(),
                        externalUserId: $("#mapUpsertExternalUserId").val().trim(),
                        userId: parseInt($("#mapUpsertInternalUserId").val(), 10) || null
                    };
                }
                const $btn = $(this).prop("disabled", true).text("Saving...");
                apiPost("/Attendance/UpsertBiometricUserMap", payload)
                    .done(res => res?.success ? showAlert("#bioMapMsg", "success", "Map saved. ID = " + (res.mapId || res.id || "-")) : showAlert("#bioMapMsg", "warning", "Save failed."))
                    .fail(xhr => showAlert("#bioMapMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Upsert Map"));
            });

            // ----- Biometric: Import/Process Punches -----
            function renderPunchTableEmpty() {
                $("#punchTable tbody").html('<tr><td colspan="5" class="p-3 text-center text-muted">No punches to import</td></tr>');
            }
            function addPunchRow(row) {
                const r = row || { deviceId: "", externalUserId: "", punchTime: "", direction: "In" };
                const tr = `<tr>
                    <td><input type="number" class="form-control form-control-sm punch-device" value="${r.deviceId}"/></td>
                    <td><input type="text" class="form-control form-control-sm punch-ext" value="${r.externalUserId}"/></td>
                    <td><input type="datetime-local" class="form-control form-control-sm punch-time" value="${r.punchTime}"/></td>
                    <td>
                        <select class="form-select form-select-sm punch-dir">
                            <option ${r.direction === 'In' ? 'selected' : ''}>In</option>
                            <option ${r.direction === 'Out' ? 'selected' : ''}>Out</option>
                        </select>
                    </td>
                    <td><button class="btn btn-sm btn-outline-danger btn-del-punch">Remove</button></td>
                </tr>`;
                const $tb = $("#punchTable tbody");
                if ($tb.find("tr td").length === 1) $tb.empty();
                $tb.append(tr);
            }
            $("#btnAddPunchRow").on("click", function () { addPunchRow(); });
            $(document).on("click", ".btn-del-punch", function () { $(this).closest("tr").remove(); if ($("#punchTable tbody tr").length === 0) renderPunchTableEmpty(); });

            $("#btnImportPunches").on("click", function () {
                if (!isAdmin) { showAlert("#bioPunchMsg", "info", "Only Admin can import punches."); return; }
                clearAlert("#bioPunchMsg");
                const punches = [];
                $("#punchTable tbody tr").each(function () {
                    const deviceId = parseInt($(this).find(".punch-device").val(), 10);
                    const externalUserId = $(this).find(".punch-ext").val().trim();
                    const punchTime = $(this).find(".punch-time").val();
                    const direction = $(this).find(".punch-dir").val() || null;
                    if (deviceId && externalUserId && punchTime) {
                        punches.push({ deviceId, externalUserId, punchTime, direction });
                    }
                });
                if (!punches.length) { showAlert("#bioPunchMsg", "warning", "Add at least one valid punch row."); return; }
                const $btn = $(this).prop("disabled", true).text("Importing...");
                apiPost("/Attendance/ImportBiometricPunches", { punches })
                    .done(res => res?.success ? showAlert("#bioPunchMsg", "success", "Imported: " + res.imported) : showAlert("#bioPunchMsg", "warning", "Import failed."))
                    .fail(xhr => showAlert("#bioPunchMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Import"));
            });

            $("#btnProcessPunches").on("click", function () {
                if (!isAdmin) { showAlert("#bioPunchMsg", "info", "Only Admin can process punches."); return; }
                clearAlert("#bioPunchMsg");
                const fromDate = $("#procFrom").val() || todayStr();
                const toDate = $("#procTo").val() || fromDate;
                const $btn = $(this).prop("disabled", true).text("Processing...");
                apiPost("/Attendance/ProcessBiometricPunches", { fromDate, toDate })
                    .done(res => res?.success ? showAlert("#bioPunchMsg", "success", "Processed: " + res.affected) : showAlert("#bioPunchMsg", "warning", "Process failed."))
                    .fail(xhr => showAlert("#bioPunchMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Process"));
            });

            // ----- Alerts & Logs -----
            $("#btnSendAlerts").on("click", function () {
                if (!isAdmin) { showAlert("#alertsMsg", "info", "Only Admin can send alerts."); return; }
                clearAlert("#alertsMsg");
                const payload = {
                    attendanceDate: $("#alDate").val() || todayStr(),
                    className: $("#alClass").val().trim() || null,
                    section: $("#alSection").val().trim() || null,
                    sendEmail: $("#alEmail").is(":checked"),
                    sendSms: $("#alSms").is(":checked")
                };
                const $btn = $(this).prop("disabled", true).text("Sending...");
                apiPost("/Attendance/SendAbsenceAlerts", payload)
                    .done(res => res?.success ? showAlert("#alertsMsg", "success", "Alerts sent: " + res.sent) : showAlert("#alertsMsg", "warning", "Send failed."))
                    .fail(xhr => showAlert("#alertsMsg", "danger", parseError(xhr)))
                    .always(() => $btn.prop("disabled", false).text("Send Alerts"));
            });

            $("#btnLoadLogs").on("click", function () {
                clearAlert("#logsMsg");

                const q = {
                    fromDate: $("#logFrom").val() || null,
                    toDate: $("#logTo").val() || null,
                    type: $("#logType").val().trim() || null,
                    status: $("#logStatus").val().trim() || null,
                    className: $("#logClass").val().trim() || null,
                    section: $("#logSection").val().trim() || null,
                    studentId: $("#logStudentId").val() ? parseInt($("#logStudentId").val(), 10) : null
                };

                $("#logsTable tbody").html('<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>');

                apiGet("/Attendance/GetNotificationLogs", q)
                    .done(list => {
                        const $tb = $("#logsTable tbody");

                        if (!Array.isArray(list) || list.length === 0) {
                            $tb.html('<tr><td colspan="7" class="p-3 text-center text-muted">No logs</td></tr>');
                            return;
                        }

                        const rows = list.map(x => {
                            const dt = x.createdAtUtc || x.sentAt || x.dateTime || null;
                            const type = x.type || "-";
                            const status = x.status || "-";
                            const cls = x.className || "-";
                            const sec = x.section || "-";
                            const sid = x.studentId || "-";
                            const message = x.message || x.subject || "-";

                            return `<tr>
                                <td>${toLocal(dt)}</td>
                                <td>${type}</td>
                                <td>${status}</td>
                                <td>${cls}</td>
                                <td>${sec}</td>
                                <td>${sid}</td>
                                <td>${message}</td>
                            </tr>`;
                        }).join("");

                        $tb.html(rows);
                    })
                    .fail(xhr => showAlert("#logsMsg", "danger", parseError(xhr)));
            });

        }

    });

})();