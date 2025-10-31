// wwwroot/js/script/academic.js
// PART 1/2
// Covers: Subjects, Courses, Syllabus, Classrooms, Available Rooms by Slot
// Requires common.js (BASE_URL, $.ajaxSetup, showAlert, clearAlert)

(function () {
    "use strict";

    $(function () {
        if ($("#academicPage").length === 0) return;

        const roles = ($("body").attr("data-roles") || "");
        const isAdmin = roles.includes("Admin");
        const isTeacher = roles.includes("Teacher");
        if (!(isAdmin || isTeacher)) return;

        // ---------------- Utilities ----------------
        function parseError(xhr) {
            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
            return xhr.status + " " + xhr.statusText;
        }
        function toLocalDate(d) {
            if (!d) return "-";
            const dt = new Date(d);
            return isNaN(dt) ? "-" : dt.toLocaleDateString();
        }
        function ensureTimeSpan(val) { // "HH:mm" -> "HH:mm:ss"
            if (!val) return null;
            return /^\d{2}:\d{2}:\d{2}$/.test(val) ? val : (val + ":00");
        }
        function boolStr(v) { return v ? "Yes" : "No"; }

        // Caches
        let subjectsCache = [];
        let roomsCache = [];
        let selectedCourseId = 0;

        // Fill selects from caches
        function fillSubjectSelect($sel, includeAny = true) {
            const opts = [];
            if (includeAny) opts.push(`<option value="">Any</option>`);
            subjectsCache.forEach(s => {
                opts.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`);
            });
            $sel.html(opts.join(""));
        }
        function fillSubjectSelectStrict($sel) {
            const opts = [`<option value="">Select</option>`];
            subjectsCache.forEach(s => {
                opts.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`);
            });
            $sel.html(opts.join(""));
        }
        function fillRoomSelect($sel, includeAny = true) {
            const opts = [];
            if (includeAny) opts.push(`<option value="">Any</option>`);
            roomsCache.forEach(r => {
                const name = r.name ? ` - ${r.name}` : "";
                opts.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
            });
            $sel.html(opts.join(""));
        }
        function fillRoomSelectStrict($sel) {
            const opts = [`<option value="">Select</option>`];
            roomsCache.forEach(r => {
                const name = r.name ? ` - ${r.name}` : "";
                opts.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
            });
            $sel.html(opts.join(""));
        }

        // Pre-load subjects/rooms for dropdowns across module
        function loadSubjectsForOptions(cb) {
            $.ajax({
                url: "/Academic/GetSubjectList",
                type: "GET",
                dataType: "json",
                data: { isActive: "" }
            })
                .done(list => {
                    subjectsCache = Array.isArray(list) ? list : [];
                    // Populate all subject selects on page
                    fillSubjectSelect($("#crFilterSubject"), true);
                    fillSubjectSelectStrict($("#crSubject"));
                    fillSubjectSelectStrict($("#ttSubject"));   // used in Part 2 (timetable)
                    fillSubjectSelect($("#lpSubject"), true);   // filter
                    fillSubjectSelectStrict($("#lpFSubject"));  // modal
                    fillSubjectSelectStrict($("#paperSubject"));// exam paper
                })
                .always(() => cb && cb());
        }
        function loadRoomsForOptions(cb) {
            $.ajax({
                url: "/Academic/GetClassroomList",
                type: "GET",
                dataType: "json",
                data: { isActive: "" }
            })
                .done(list => {
                    roomsCache = Array.isArray(list) ? list : [];
                    // Populate all room selects
                    fillRoomSelectStrict($("#ttRoom"));        // timetable
                    fillRoomSelectStrict($("#paperRoom"));     // exam paper
                })
                .always(() => cb && cb());
        }

        // =========================================================================
        // SUBJECTS
        // =========================================================================
        function renderSubjects(list) {
            const $tb = $("#subTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(s => `
        <tr data-id="${s.subjectId}">
          <td>${s.subjectId}</td>
          <td>${s.subjectCode}</td>
          <td>${s.subjectName}</td>
          <td>${s.shortName || ""}</td>
          <td>${s.description || ""}</td>
          <td>${boolStr(s.isActive)}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-sub-edit">Edit</button>
              <button class="btn btn-outline-danger btn-sub-del">Delete</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function loadSubjects() {
            clearAlert("#subMsg");
            const isActive = $("#subFilterActive").val();
            $("#subTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetSubjectList",
                type: "GET",
                dataType: "json",
                data: { isActive: isActive || "" }
            })
                .done(renderSubjects)
                .fail(xhr => showAlert("#subMsg", "danger", parseError(xhr)));
        }

        function openSubjectModal(subjectId) {
            clearAlert("#subFormMsg");
            $("#subId").val(subjectId || 0);
            $("#subModalLabel").text(subjectId ? "Edit Subject" : "New Subject");
            $("#subCode,#subName,#subShort,#subDesc").val("");
            $("#subActive").val("true");

            if (subjectId && subjectId > 0) {
                $.ajax({
                    url: "/Academic/GetSubjectById",
                    type: "GET",
                    dataType: "json",
                    data: { subjectId }
                })
                    .done(s => {
                        $("#subId").val(s.subjectId);
                        $("#subCode").val(s.subjectCode);
                        $("#subName").val(s.subjectName);
                        $("#subShort").val(s.shortName || "");
                        $("#subDesc").val(s.description || "");
                        $("#subActive").val(s.isActive ? "true" : "false");
                    })
                    .fail(xhr => showAlert("#subFormMsg", "danger", parseError(xhr)))
                    .always(() => subModal.show());
            } else {
                subModal.show();
            }
        }

        function saveSubject() {
            clearAlert("#subFormMsg");
            const dto = {
                subjectId: parseInt($("#subId").val(), 10) || 0,
                subjectCode: $("#subCode").val().trim(),
                subjectName: $("#subName").val().trim(),
                shortName: $("#subShort").val().trim() || null,
                description: $("#subDesc").val().trim() || null,
                isActive: $("#subActive").val() === "true"
            };
            if (!dto.subjectCode || !dto.subjectName) {
                showAlert("#subFormMsg", "warning", "Subject Code and Name are required.");
                return;
            }
            const $btn = $("#btnSaveSubject").prop("disabled", true).text("Saving...");
            const req = dto.subjectId > 0
                ? $.ajax({
                    url: "/Academic/UpdateSubject",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateSubject",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    subModal.hide();
                    showAlert("#subMsg", "success", "Saved.");
                    loadSubjects();
                    loadSubjectsForOptions(); // refresh dropdowns
                } else {
                    showAlert("#subFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#subFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteSubject(subjectId) {
            $.ajax({
                url: "/Academic/DeleteSubject",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(subjectId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#subMsg", "success", "Deleted.");
                        loadSubjects();
                        loadSubjectsForOptions();
                    } else {
                        showAlert("#subMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#subMsg", "danger", parseError(xhr)));
        }

        // =========================================================================
        // COURSES
        // =========================================================================
        function renderCourses(list) {
            const $tb = $("#crTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            // Build a map for subject names
            const subjMap = {};
            subjectsCache.forEach(s => subjMap[s.subjectId] = s.subjectName);

            const rows = list.map(c => `
        <tr data-id="${c.courseId}" data-subject-id="${c.subjectId}">
          <td>${c.courseId}</td>
          <td>${c.academicYear}</td>
          <td>${c.className}</td>
          <td>${subjMap[c.subjectId] || c.subjectId}</td>
          <td>${boolStr(c.isActive)}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-dark btn-cr-sy">Syllabus</button>
              <button class="btn btn-outline-primary btn-cr-edit">Edit</button>
              <button class="btn btn-outline-danger btn-cr-del">Delete</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function loadCourses() {
            clearAlert("#courseMsg");
            const q = {
                academicYear: $("#crFilterYear").val().trim() || null,
                className: $("#crFilterClass").val().trim() || null,
                subjectId: $("#crFilterSubject").val() ? parseInt($("#crFilterSubject").val(), 10) : null,
                isActive: $("#crFilterActive").val() || null
            };
            $("#crTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetCourseList",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderCourses)
                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
        }

        function openCourseModal(courseId) {
            clearAlert("#crFormMsg");
            $("#crId").val(courseId || 0);
            $("#crModalLabel").text(courseId ? "Edit Course" : "New Course");
            $("#crYear,#crClass,#crDesc").val("");
            $("#crSubject").val("");
            $("#crActive").val("true");

            if (courseId && courseId > 0) {
                $.ajax({
                    url: "/Academic/GetCourseById",
                    type: "GET",
                    dataType: "json",
                    data: { courseId }
                })
                    .done(c => {
                        $("#crId").val(c.courseId);
                        $("#crYear").val(c.academicYear || "");
                        $("#crClass").val(c.className || "");
                        $("#crSubject").val(c.subjectId || "");
                        $("#crActive").val(c.isActive ? "true" : "false");
                        $("#crDesc").val(c.description || "");
                    })
                    .fail(xhr => showAlert("#crFormMsg", "danger", parseError(xhr)))
                    .always(() => crModal.show());
            } else {
                crModal.show();
            }
        }

        function saveCourse() {
            clearAlert("#crFormMsg");
            const dto = {
                courseId: parseInt($("#crId").val(), 10) || 0,
                subjectId: $("#crSubject").val() ? parseInt($("#crSubject").val(), 10) : 0,
                className: $("#crClass").val().trim(),
                academicYear: $("#crYear").val().trim(),
                description: $("#crDesc").val().trim() || null,
                isActive: $("#crActive").val() === "true"
            };
            if (!dto.subjectId || !dto.className || !dto.academicYear) {
                showAlert("#crFormMsg", "warning", "Academic Year, Class and Subject are required.");
                return;
            }
            const $btn = $("#btnSaveCourse").prop("disabled", true).text("Saving...");
            const req = dto.courseId > 0
                ? $.ajax({
                    url: "/Academic/UpdateCourse",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateCourse",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    crModal.hide();
                    showAlert("#courseMsg", "success", "Saved.");
                    loadCourses();
                } else {
                    showAlert("#crFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#crFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteCourse(courseId) {
            $.ajax({
                url: "/Academic/DeleteCourse",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(courseId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#courseMsg", "success", "Deleted.");
                        loadCourses();
                    } else {
                        showAlert("#courseMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
        }

        // =========================================================================
        // SYLLABUS (for selected course)
        // =========================================================================
        function renderSyllabus(list) {
            const $tb = $("#syTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No syllabus items</td></tr>`);
                return;
            }
            const rows = list.sort((a, b) => (a.orderIndex || 0) - (b.orderIndex || 0)).map(it => `
        <tr data-id="${it.syllabusId}">
          <td>${it.orderIndex ?? ""}</td>
          <td>${it.unitNo ?? ""}</td>
          <td>${it.topic}</td>
          <td>${it.subTopic || ""}</td>
          <td>${it.estimatedHours ?? ""}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-sy-edit">Edit</button>
              <button class="btn btn-outline-danger btn-sy-del">Delete</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function loadSyllabus(courseId) {
            selectedCourseId = courseId;
            $("#btnNewSyllabus").prop("disabled", !courseId);
            $("#syCourseTitle").text(courseId ? `(Course ID ${courseId})` : "");
            if (!courseId) {
                $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Select a course to view syllabus</td></tr>`);
                return;
            }
            $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetSyllabusByCourse",
                type: "GET",
                dataType: "json",
                data: { courseId }
            })
                .done(renderSyllabus)
                .fail(xhr => $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
        }

        function openSyllabusModal(syllabusId) {
            if (!selectedCourseId) { showAlert("#courseMsg", "info", "Select a course first."); return; }
            clearAlert("#syFormMsg");
            $("#syId").val(syllabusId || 0);
            $("#syCourseId").val(selectedCourseId);
            $("#syModalLabel").text(syllabusId ? "Edit Syllabus Item" : "Add Syllabus Item");
            $("#syOrder,#syUnit,#syTopic,#sySubTopic,#syObjectives,#syRefs,#syHours").val("");

            if (syllabusId) {
                // Get list and find item
                $.ajax({
                    url: "/Academic/GetSyllabusByCourse",
                    type: "GET",
                    dataType: "json",
                    data: { courseId: selectedCourseId }
                })
                    .done(list => {
                        const it = (list || []).find(x => x.syllabusId === syllabusId);
                        if (!it) { showAlert("#syFormMsg", "danger", "Syllabus item not found."); return; }
                        $("#syId").val(it.syllabusId);
                        $("#syOrder").val(it.orderIndex ?? "");
                        $("#syUnit").val(it.unitNo ?? "");
                        $("#syTopic").val(it.topic || "");
                        $("#sySubTopic").val(it.subTopic || "");
                        $("#syObjectives").val(it.objectives || "");
                        $("#syRefs").val(it.referenceMaterials || "");
                        $("#syHours").val(it.estimatedHours ?? "");
                    })
                    .fail(xhr => showAlert("#syFormMsg", "danger", parseError(xhr)))
                    .always(() => syModal.show());
            } else {
                syModal.show();
            }
        }

        function saveSyllabus() {
            clearAlert("#syFormMsg");
            const dto = {
                syllabusId: parseInt($("#syId").val(), 10) || 0,
                courseId: parseInt($("#syCourseId").val(), 10),
                unitNo: $("#syUnit").val() ? parseInt($("#syUnit").val(), 10) : null,
                topic: $("#syTopic").val().trim(),
                subTopic: $("#sySubTopic").val().trim() || null,
                objectives: $("#syObjectives").val().trim() || null,
                referenceMaterials: $("#syRefs").val().trim() || null,
                estimatedHours: $("#syHours").val() ? parseFloat($("#syHours").val()) : null,
                orderIndex: $("#syOrder").val() ? parseInt($("#syOrder").val(), 10) : null
            };
            if (!dto.courseId || !dto.topic) {
                showAlert("#syFormMsg", "warning", "Topic is required.");
                return;
            }
            const $btn = $("#btnSaveSyllabus").prop("disabled", true).text("Saving...");
            const req = dto.syllabusId > 0
                ? $.ajax({
                    url: "/Academic/UpdateSyllabusItem",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/AddSyllabusItem",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    syModal.hide();
                    showAlert("#courseMsg", "success", "Syllabus saved.");
                    loadSyllabus(selectedCourseId);
                } else {
                    showAlert("#syFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#syFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteSyllabus(syllabusId) {
            $.ajax({
                url: "/Academic/DeleteSyllabusItem",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(syllabusId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#courseMsg", "success", "Deleted.");
                        loadSyllabus(selectedCourseId);
                    } else {
                        showAlert("#courseMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
        }

        // =========================================================================
        // CLASSROOMS
        // =========================================================================
        function renderRooms(list) {
            const $tb = $("#roomTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(r => `
        <tr data-id="${r.roomId}">
          <td>${r.roomId}</td>
          <td>${r.roomCode}</td>
          <td>${r.name || ""}</td>
          <td>${r.capacity ?? ""}</td>
          <td>${r.location || ""}</td>
          <td>${boolStr(r.isActive)}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-room-edit">Edit</button>
              <button class="btn btn-outline-danger btn-room-del">Delete</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function loadRooms() {
            clearAlert("#roomMsg");
            const isActive = $("#roomFilterActive").val() || "";
            $("#roomTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetClassroomList",
                type: "GET",
                dataType: "json",
                data: { isActive }
            })
                .done(list => {
                    renderRooms(list);
                    // refresh room cache for selects
                    roomsCache = Array.isArray(list) ? list : [];
                    fillRoomSelectStrict($("#ttRoom"));
                    fillRoomSelectStrict($("#paperRoom"));
                })
                .fail(xhr => showAlert("#roomMsg", "danger", parseError(xhr)));
        }

        function openRoomModal(roomId) {
            clearAlert("#roomFormMsg");
            $("#roomId").val(roomId || 0);
            $("#roomModalLabel").text(roomId ? "Edit Room" : "New Room");
            $("#roomCode,#roomName,#roomCapacity,#roomLocation").val("");
            $("#roomActive").val("true");

            if (roomId && roomId > 0) {
                $.ajax({
                    url: "/Academic/GetClassroomById",
                    type: "GET",
                    dataType: "json",
                    data: { roomId }
                })
                    .done(r => {
                        $("#roomId").val(r.roomId);
                        $("#roomCode").val(r.roomCode);
                        $("#roomName").val(r.name || "");
                        $("#roomCapacity").val(r.capacity ?? "");
                        $("#roomLocation").val(r.location || "");
                        $("#roomActive").val(r.isActive ? "true" : "false");
                    })
                    .fail(xhr => showAlert("#roomFormMsg", "danger", parseError(xhr)))
                    .always(() => roomModal.show());
            } else {
                roomModal.show();
            }
        }

        function saveRoom() {
            clearAlert("#roomFormMsg");
            const dto = {
                roomId: parseInt($("#roomId").val(), 10) || 0,
                roomCode: $("#roomCode").val().trim(),
                name: $("#roomName").val().trim() || null,
                capacity: $("#roomCapacity").val() ? parseInt($("#roomCapacity").val(), 10) : null,
                location: $("#roomLocation").val().trim() || null,
                isActive: $("#roomActive").val() === "true"
            };
            if (!dto.roomCode) {
                showAlert("#roomFormMsg", "warning", "Room code is required.");
                return;
            }
            const $btn = $("#btnSaveRoom").prop("disabled", true).text("Saving...");
            const req = dto.roomId > 0
                ? $.ajax({
                    url: "/Academic/UpdateClassroom",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateClassroom",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    roomModal.hide();
                    showAlert("#roomMsg", "success", "Saved.");
                    loadRooms();
                } else {
                    showAlert("#roomFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#roomFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteRoom(roomId) {
            $.ajax({
                url: "/Academic/DeleteClassroom",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(roomId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#roomMsg", "success", "Deleted.");
                        loadRooms();
                    } else {
                        showAlert("#roomMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#roomMsg", "danger", parseError(xhr)));
        }

        // Available rooms by slot
        function renderAvailableRooms(list) {
            const $tb = $("#availRoomTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(r => `
        <tr>
          <td>${r.roomId}</td>
          <td>${r.roomCode}</td>
          <td>${r.name || ""}</td>
          <td>${r.capacity ?? ""}</td>
          <td>${r.location || ""}</td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function findAvailableRooms() {
            clearAlert("#availRoomMsg");
            const q = {
                academicYear: $("#arYear").val().trim(),
                dayOfWeek: $("#arDay").val() ? parseInt($("#arDay").val(), 10) : null,
                periodNo: $("#arPeriod").val() ? parseInt($("#arPeriod").val(), 10) : null,
                startTime: ensureTimeSpan($("#arStart").val()),
                endTime: ensureTimeSpan($("#arEnd").val())
            };
            if (!q.academicYear || !q.dayOfWeek) {
                showAlert("#availRoomMsg", "warning", "Academic Year and Day are required.");
                return;
            }
            $("#availRoomTable tbody").html(`<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetAvailableRoomsBySlot",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderAvailableRooms)
                .fail(xhr => showAlert("#availRoomMsg", "danger", parseError(xhr)));
        }

        // ---------------- Modals ----------------
        const subModal = new bootstrap.Modal(document.getElementById("subModal"), { backdrop: "static" });
        const crModal = new bootstrap.Modal(document.getElementById("crModal"), { backdrop: "static" });
        const syModal = new bootstrap.Modal(document.getElementById("syModal"), { backdrop: "static" });
        const roomModal = new bootstrap.Modal(document.getElementById("roomModal"), { backdrop: "static" });

        // ---------------- Events ----------------
        // Subjects
        $("#btnLoadSubjects").on("click", loadSubjects);
        $("#btnNewSubject").on("click", () => openSubjectModal(0));
        $(document).on("click", ".btn-sub-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openSubjectModal(id);
        });
        $(document).on("click", ".btn-sub-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete subject ID " + id + " ?")) return;
            deleteSubject(id);
        });
        $("#btnSaveSubject").on("click", saveSubject);

        // Courses
        $("#btnLoadCourses").on("click", loadCourses);
        $("#btnNewCourse").on("click", () => openCourseModal(0));
        $(document).on("click", ".btn-cr-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openCourseModal(id);
        });
        $(document).on("click", ".btn-cr-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete course ID " + id + " ?")) return;
            deleteCourse(id);
        });
        $(document).on("click", ".btn-cr-sy", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            loadSyllabus(id);
        });
        $("#btnSaveCourse").on("click", saveCourse);

        // Syllabus
        $("#btnNewSyllabus").on("click", () => openSyllabusModal(0));
        $(document).on("click", ".btn-sy-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openSyllabusModal(id);
        });
        $(document).on("click", ".btn-sy-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete syllabus ID " + id + " ?")) return;
            deleteSyllabus(id);
        });
        $("#btnSaveSyllabus").on("click", saveSyllabus);

        // Rooms
        $("#btnLoadRooms").on("click", loadRooms);
        $("#btnNewRoom").on("click", () => openRoomModal(0));
        $(document).on("click", ".btn-room-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openRoomModal(id);
        });
        $(document).on("click", ".btn-room-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete room ID " + id + " ?")) return;
            deleteRoom(id);
        });
        $("#btnSaveRoom").on("click", saveRoom);

        // Available Rooms by Slot
        $("#btnFindRooms").on("click", findAvailableRooms);

        // ---------------- Initial Loads ----------------
        loadSubjectsForOptions(() => {
            loadCourses();
            loadSubjects(); // initial subjects
        });
        loadRoomsForOptions(() => {
            loadRooms();
        });
    });
})();



// wwwroot/js/script/academic.js
// PART 2/2
// Covers: Timetable, Lesson Plans, Academic Calendar, Exams (and Exam Papers)
// Requires common.js (BASE_URL, $.ajaxSetup, showAlert, clearAlert)

(function () {
    "use strict";

    $(function () {
        if ($("#academicPage").length === 0) return;

        const roles = ($("body").attr("data-roles") || "");
        const isAdmin = roles.includes("Admin");
        const isTeacher = roles.includes("Teacher");
        if (!(isAdmin || isTeacher)) return;

        // ---------------- Utilities ----------------
        function parseError(xhr) {
            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
            return xhr.status + " " + xhr.statusText;
        }
        function dayName(n) {
            const map = { 1: "Mon", 2: "Tue", 3: "Wed", 4: "Thu", 5: "Fri", 6: "Sat", 7: "Sun" };
            return map[n] || n;
        }
        function toLocalDate(d) {
            if (!d) return "-";
            const dt = new Date(d);
            return isNaN(dt) ? "-" : dt.toLocaleDateString();
        }
        function ensureTimeSpan(val) { // "HH:mm" -> "HH:mm:ss"
            if (!val) return null;
            return /^\d{2}:\d{2}:\d{2}$/.test(val) ? val : (val + ":00");
        }
        function fmtSpan(ts) {
            if (!ts) return "";
            // accept "HH:mm:ss" or "HH:mm"
            if (/^\d{2}:\d{2}$/.test(ts)) return ts;
            if (/^\d{2}:\d{2}:\d{2}$/.test(ts)) return ts.substring(0, 5);
            return ts;
        }

        // Caches (local for Part 2; safe if Part 1 not loaded yet)
        let subjectsCache2 = [];
        let roomsCache2 = [];

        function loadSubjectsForOptions2(cb) {
            $.ajax({
                url: "/Academic/GetSubjectList",
                type: "GET",
                dataType: "json",
                data: { isActive: "" }
            })
                .done(list => {
                    subjectsCache2 = Array.isArray(list) ? list : [];
                    // Fill selects this part uses
                    const optsSelect = (includeAny) => {
                        const arr = [];
                        if (includeAny) arr.push(`<option value="">Any</option>`);
                        subjectsCache2.forEach(s => arr.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`));
                        return arr.join("");
                    };
                    $("#ttSubject").html(`<option value="">Select</option>` + optsSelect(false));
                    $("#lpSubject").html(optsSelect(true));
                    $("#lpFSubject").html(`<option value="">Select</option>` + optsSelect(false));
                    $("#paperSubject").html(`<option value="">Select</option>` + optsSelect(false));
                })
                .always(() => cb && cb());
        }
        function loadRoomsForOptions2(cb) {
            $.ajax({
                url: "/Academic/GetClassroomList",
                type: "GET",
                dataType: "json",
                data: { isActive: "" }
            })
                .done(list => {
                    roomsCache2 = Array.isArray(list) ? list : [];
                    const optsSelectStrict = () => {
                        const arr = [`<option value="">Select</option>`];
                        roomsCache2.forEach(r => {
                            const name = r.name ? ` - ${r.name}` : "";
                            arr.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
                        });
                        return arr.join("");
                    };
                    $("#ttRoom").html(optsSelectStrict());
                    $("#paperRoom").html(optsSelectStrict());
                })
                .always(() => cb && cb());
        }

        function subjNameById(id) {
            const s = subjectsCache2.find(x => x.subjectId === id);
            return s ? s.subjectName : id;
        }
        function roomCodeById(id) {
            const r = roomsCache2.find(x => x.roomId === id);
            return r ? (r.roomCode + (r.name ? ` - ${r.name}` : "")) : id;
        }

        // =========================================================================
        // TIMETABLE
        // =========================================================================
        const ttConflictMap = {
            [-1]: "Class/Section conflict",
            [-2]: "Teacher conflict",
            [-3]: "Room conflict"
        };

        function newTimetableForm() {
            $("#ttId").val(0);
            $("#ttAY,#ttClass,#ttSection,#ttStart,#ttEnd").val("");
            $("#ttDay").val("1");
            $("#ttPeriod").val("");
            $("#ttSubject").val("");
            $("#ttCourseId").val("");
            $("#ttTeacher").val("");
            $("#ttRoom").val("");
            $("#ttActive").val("true");
            $("#ttValidateMsg").empty();
        }

        function buildTimetableDto() {
            return {
                timetableId: parseInt($("#ttId").val(), 10) || 0,
                academicYear: $("#ttAY").val().trim(),
                className: $("#ttClass").val().trim(),
                section: $("#ttSection").val().trim() || null,
                dayOfWeek: $("#ttDay").val() ? parseInt($("#ttDay").val(), 10) : 1,
                periodNo: $("#ttPeriod").val() ? parseInt($("#ttPeriod").val(), 10) : null,
                startTime: ensureTimeSpan($("#ttStart").val()),
                endTime: ensureTimeSpan($("#ttEnd").val()),
                subjectId: $("#ttSubject").val() ? parseInt($("#ttSubject").val(), 10) : 0,
                courseId: $("#ttCourseId").val() ? parseInt($("#ttCourseId").val(), 10) : null,
                teacherUserId: $("#ttTeacher").val() ? parseInt($("#ttTeacher").val(), 10) : null,
                roomId: $("#ttRoom").val() ? parseInt($("#ttRoom").val(), 10) : null,
                isActive: $("#ttActive").val() === "true"
            };
        }

        $("#btnNewTimetable").on("click", newTimetableForm);

        $("#btnValidateTimetable").on("click", function () {
            clearAlert("#ttMsg");
            $("#ttValidateMsg").empty();
            const dto = buildTimetableDto();
            if (!dto.academicYear || !dto.className || !dto.subjectId) {
                $("#ttValidateMsg").html(`<div class="alert alert-warning">Academic Year, Class and Subject are required.</div>`);
                return;
            }
            $.ajax({
                url: "/Academic/ValidateTimetableConflict",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    const code = res?.code ?? 0;
                    if (code === 0) {
                        $("#ttValidateMsg").html(`<div class="alert alert-success">No conflicts found.</div>`);
                    } else {
                        const msg = ttConflictMap[code] || `Conflict code ${code}`;
                        $("#ttValidateMsg").html(`<div class="alert alert-danger">${msg}</div>`);
                    }
                })
                .fail(xhr => $("#ttValidateMsg").html(`<div class="alert alert-danger">${parseError(xhr)}</div>`));
        });

        $("#btnSaveTimetable").on("click", function () {
            clearAlert("#ttMsg");
            $("#ttValidateMsg").empty();
            const dto = buildTimetableDto();
            if (!dto.academicYear || !dto.className || !dto.subjectId) {
                showAlert("#ttMsg", "warning", "Academic Year, Class and Subject are required.");
                return;
            }
            const $btn = $(this).prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Academic/UpsertTimetableEntry",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#ttMsg", "success", "Saved.");
                        $("#ttId").val(res.id || dto.timetableId);
                    } else {
                        showAlert("#ttMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => {
                    if (xhr.status === 409) {
                        const msg = xhr.responseJSON?.error || "Conflict.";
                        $("#ttValidateMsg").html(`<div class="alert alert-danger">${msg}</div>`);
                    } else {
                        showAlert("#ttMsg", "danger", parseError(xhr));
                    }
                })
                .always(() => $btn.prop("disabled", false).text("Save"));
        });

        // View Class Timetable
        function renderClassTimetable(list) {
            const $tb = $("#vtClassTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(x => {
                const time = (x.startTime || x.endTime) ? `${fmtSpan(x.startTime)} - ${fmtSpan(x.endTime)}` : "";
                return `<tr>
          <td>${dayName(x.dayOfWeek)}</td>
          <td>${x.periodNo ?? ""}</td>
          <td>${time}</td>
          <td>${subjNameById(x.subjectId)}</td>
          <td>${x.teacherUserId ?? ""}</td>
          <td>${roomCodeById(x.roomId)}</td>
        </tr>`;
            }).join("");
            $tb.html(rows);
        }

        $("#btnViewClassTimetable").on("click", function () {
            clearAlert("#ttMsg");
            const q = {
                academicYear: $("#vtAY").val().trim(),
                className: $("#vtClass").val().trim(),
                section: $("#vtSection").val().trim() || null
            };
            if (!q.academicYear || !q.className) {
                showAlert("#ttMsg", "warning", "AY and Class are required.");
                return;
            }
            $("#vtClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetClassTimetable",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderClassTimetable)
                .fail(xhr => $("#vtClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
        });

        // View Teacher Timetable
        function renderTeacherTimetable(list) {
            const $tb = $("#vtTeacherTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(x => {
                const time = (x.startTime || x.endTime) ? `${fmtSpan(x.startTime)} - ${fmtSpan(x.endTime)}` : "";
                return `<tr>
          <td>${dayName(x.dayOfWeek)}</td>
          <td>${x.periodNo ?? ""}</td>
          <td>${time}</td>
          <td>${x.className}${x.section ? ("-" + x.section) : ""}</td>
          <td>${subjNameById(x.subjectId)}</td>
          <td>${roomCodeById(x.roomId)}</td>
        </tr>`;
            }).join("");
            $tb.html(rows);
        }

        $("#btnViewTeacherTimetable").on("click", function () {
            clearAlert("#ttMsg");
            const q = {
                academicYear: $("#ttcAY").val().trim(),
                teacherUserId: $("#ttcTeacher").val() ? parseInt($("#ttcTeacher").val(), 10) : 0
            };
            if (!q.academicYear || !q.teacherUserId) {
                showAlert("#ttMsg", "warning", "AY and Teacher userId are required.");
                return;
            }
            $("#vtTeacherTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetTeacherTimetable",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderTeacherTimetable)
                .fail(xhr => $("#vtTeacherTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
        });

        // =========================================================================
        // LESSON PLANS
        // =========================================================================
        const lpModal = new bootstrap.Modal(document.getElementById("lpModal"), { backdrop: "static" });

        function renderPlans(list) {
            const $tb = $("#lpTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(p => `
        <tr data-id="${p.planId}">
          <td>${p.planId}</td>
          <td>${toLocalDate(p.planDate)}</td>
          <td>${p.className}${p.section ? ("-" + p.section) : ""}</td>
          <td>${subjNameById(p.subjectId)}</td>
          <td>${p.teacherUserId ?? ""}</td>
          <td>${p.topic}</td>
          <td><span class="badge ${p.status === 'Delivered' ? 'bg-success' : (p.status === 'Planned' ? 'bg-info text-dark' : 'bg-secondary')}">${p.status}</span></td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-lp-edit">Edit</button>
              <button class="btn btn-outline-success btn-lp-status" data-status="Delivered">Mark Delivered</button>
              <button class="btn btn-outline-secondary btn-lp-status" data-status="Planned">Mark Planned</button>
              <button class="btn btn-outline-danger btn-lp-del">Delete</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function loadPlans() {
            clearAlert("#lpMsg");
            const q = {
                academicYear: $("#lpAY").val().trim() || null,
                className: $("#lpClass").val().trim() || null,
                section: $("#lpSection").val().trim() || null,
                subjectId: $("#lpSubject").val() ? parseInt($("#lpSubject").val(), 10) : null,
                teacherUserId: $("#lpTeacher").val() ? parseInt($("#lpTeacher").val(), 10) : null,
                fromDate: $("#lpFrom").val() || null,
                toDate: $("#lpTo").val() || null,
                status: $("#lpStatus").val() || null
            };
            $("#lpTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetLessonPlanList",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderPlans)
                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
        }

        function openPlanModal(planId) {
            clearAlert("#lpFormMsg");
            $("#lpModalLabel").text(planId ? "Edit Lesson Plan" : "New Lesson Plan");
            $("#lpId").val(planId || 0);
            // reset modal
            $("#lpFAy,#lpFClass,#lpFSection,#lpTopic,#lpSubTopic,#lpObjectives,#lpActivities,#lpResources,#lpHomework,#lpAssess,#lpNotes").val("");
            $("#lpFSubject").val("");
            $("#lpFTeacher").val("");
            $("#lpDate").val("");
            $("#lpPeriod").val("");
            $("#lpStart").val("");
            $("#lpEnd").val("");
            $("#lpStatusVal").val("Draft");

            if (planId && planId > 0) {
                $.ajax({
                    url: "/Academic/GetLessonPlanById",
                    type: "GET",
                    dataType: "json",
                    data: { planId }
                })
                    .done(p => {
                        $("#lpId").val(p.planId);
                        $("#lpFAy").val(p.academicYear || "");
                        $("#lpFClass").val(p.className || "");
                        $("#lpFSection").val(p.section || "");
                        $("#lpFSubject").val(p.subjectId || "");
                        $("#lpFTeacher").val(p.teacherUserId || "");
                        if (p.planDate) $("#lpDate").val(new Date(p.planDate).toISOString().substring(0, 10));
                        $("#lpPeriod").val(p.periodNo ?? "");
                        $("#lpStart").val(p.startTime ? fmtSpan(p.startTime) : "");
                        $("#lpEnd").val(p.endTime ? fmtSpan(p.endTime) : "");
                        $("#lpStatusVal").val(p.status || "Draft");
                        $("#lpTopic").val(p.topic || "");
                        $("#lpSubTopic").val(p.subTopic || "");
                        $("#lpObjectives").val(p.objectives || "");
                        $("#lpActivities").val(p.activities || "");
                        $("#lpResources").val(p.resources || "");
                        $("#lpHomework").val(p.homework || "");
                        $("#lpAssess").val(p.assessmentMethods || "");
                        $("#lpNotes").val(p.notes || "");
                    })
                    .fail(xhr => showAlert("#lpFormMsg", "danger", parseError(xhr)))
                    .always(() => lpModal.show());
            } else {
                lpModal.show();
            }
        }

        function savePlan() {
            clearAlert("#lpFormMsg");
            const dto = {
                planId: parseInt($("#lpId").val(), 10) || 0,
                academicYear: $("#lpFAy").val().trim(),
                className: $("#lpFClass").val().trim(),
                section: $("#lpFSection").val().trim() || null,
                courseId: null, // optional link not captured in UI
                subjectId: $("#lpFSubject").val() ? parseInt($("#lpFSubject").val(), 10) : null,
                teacherUserId: $("#lpFTeacher").val() ? parseInt($("#lpFTeacher").val(), 10) : 0,
                planDate: $("#lpDate").val() || null,
                periodNo: $("#lpPeriod").val() ? parseInt($("#lpPeriod").val(), 10) : null,
                startTime: ensureTimeSpan($("#lpStart").val()),
                endTime: ensureTimeSpan($("#lpEnd").val()),
                topic: $("#lpTopic").val().trim(),
                subTopic: $("#lpSubTopic").val().trim() || null,
                objectives: $("#lpObjectives").val().trim() || null,
                activities: $("#lpActivities").val().trim() || null,
                resources: $("#lpResources").val().trim() || null,
                homework: $("#lpHomework").val().trim() || null,
                assessmentMethods: $("#lpAssess").val().trim() || null,
                status: $("#lpStatusVal").val() || "Draft",
                notes: $("#lpNotes").val().trim() || null
            };
            if (!dto.academicYear || !dto.className || !dto.teacherUserId || !dto.topic || !dto.planDate) {
                showAlert("#lpFormMsg", "warning", "Academic Year, Class, Teacher, Topic and Plan Date are required.");
                return;
            }
            const $btn = $("#btnSavePlan").prop("disabled", true).text("Saving...");
            const req = dto.planId > 0
                ? $.ajax({
                    url: "/Academic/UpdateLessonPlan",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateLessonPlan",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    lpModal.hide();
                    showAlert("#lpMsg", "success", "Saved.");
                    loadPlans();
                } else {
                    showAlert("#lpFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#lpFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function updatePlanStatus(planId, status) {
            $.ajax({
                url: "/Academic/UpdateLessonPlanStatus",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify({ planId, status })
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#lpMsg", "success", "Status updated.");
                        loadPlans();
                    } else {
                        showAlert("#lpMsg", "warning", "Status update failed.");
                    }
                })
                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
        }

        function deletePlan(planId) {
            $.ajax({
                url: "/Academic/DeleteLessonPlan",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(planId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#lpMsg", "success", "Deleted.");
                        loadPlans();
                    } else {
                        showAlert("#lpMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
        }

        $("#btnLoadPlans").on("click", loadPlans);
        $("#btnNewPlan").on("click", () => openPlanModal(0));
        $(document).on("click", ".btn-lp-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openPlanModal(id);
        });
        $(document).on("click", ".btn-lp-status", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            const st = $(this).data("status");
            if (id && st) updatePlanStatus(id, st);
        });
        $(document).on("click", ".btn-lp-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete plan ID " + id + " ?")) return;
            deletePlan(id);
        });
        $("#btnSavePlan").on("click", savePlan);

        // =========================================================================
        // ACADEMIC CALENDAR
        // =========================================================================
        const calModal = new bootstrap.Modal(document.getElementById("calModal"), { backdrop: "static" });

        function renderEvents(list) {
            const $tb = $("#calTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(e => {
                const dates = `${toLocalDate(e.startDate)} - ${toLocalDate(e.endDate)}`;
                return `<tr data-id="${e.eventId}">
          <td>${e.eventId}</td>
          <td>${e.academicYear || ""}</td>
          <td>${e.eventType || ""}</td>
          <td>${e.title || ""}</td>
          <td>${(e.className || "") + (e.section ? ("-" + e.section) : "")}</td>
          <td>${dates}</td>
          <td>${e.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-cal-edit">Edit</button>
              <button class="btn btn-outline-danger btn-cal-del">Delete</button>
            </div>
          </td>
        </tr>`;
            }).join("");
            $tb.html(rows);
        }

        function loadEvents() {
            clearAlert("#calMsg");
            const q = {
                academicYear: $("#calAY").val().trim() || null,
                fromDate: $("#calFrom").val() || null,
                toDate: $("#calTo").val() || null,
                className: $("#calClass").val().trim() || null,
                section: $("#calSection").val().trim() || null,
                eventType: $("#calType").val().trim() || null,
                isActive: $("#calActive").val() || null
            };
            $("#calTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetCalendarEvents",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderEvents)
                .fail(xhr => showAlert("#calMsg", "danger", parseError(xhr)));
        }

        function openEventModal(eventId) {
            clearAlert("#calFormMsg");
            $("#calModalLabel").text(eventId ? "Edit Event" : "New Event");
            $("#calId").val(eventId || 0);
            $("#calFAy,#calFType,#calFTitle,#calFClass,#calFSection,#calFDesc").val("");
            $("#calFStart,#calFEnd").val("");
            $("#calFActive").val("true");

            if (eventId && eventId > 0) {
                $.ajax({
                    url: "/Academic/GetCalendarEventById",
                    type: "GET",
                    dataType: "json",
                    data: { eventId }
                })
                    .done(e => {
                        $("#calId").val(e.eventId);
                        $("#calFAy").val(e.academicYear || "");
                        $("#calFType").val(e.eventType || "");
                        $("#calFTitle").val(e.title || "");
                        $("#calFClass").val(e.className || "");
                        $("#calFSection").val(e.section || "");
                        if (e.startDate) $("#calFStart").val(new Date(e.startDate).toISOString().substring(0, 10));
                        if (e.endDate) $("#calFEnd").val(new Date(e.endDate).toISOString().substring(0, 10));
                        $("#calFActive").val(e.isActive ? "true" : "false");
                        $("#calFDesc").val(e.description || "");
                    })
                    .fail(xhr => showAlert("#calFormMsg", "danger", parseError(xhr)))
                    .always(() => calModal.show());
            } else {
                calModal.show();
            }
        }

        function saveEvent() {
            clearAlert("#calFormMsg");
            const dto = {
                eventId: parseInt($("#calId").val(), 10) || 0,
                academicYear: $("#calFAy").val().trim(),
                eventType: $("#calFType").val().trim(),
                title: $("#calFTitle").val().trim(),
                className: $("#calFClass").val().trim() || null,
                section: $("#calFSection").val().trim() || null,
                startDate: $("#calFStart").val() || null,
                endDate: $("#calFEnd").val() || null,
                isActive: $("#calFActive").val() === "true",
                description: $("#calFDesc").val().trim() || null
            };
            if (!dto.academicYear || !dto.eventType || !dto.title) {
                showAlert("#calFormMsg", "warning", "AY, Type and Title are required.");
                return;
            }
            const $btn = $("#btnSaveEvent").prop("disabled", true).text("Saving...");
            const req = dto.eventId > 0
                ? $.ajax({
                    url: "/Academic/UpdateCalendarEvent",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateCalendarEvent",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    calModal.hide();
                    showAlert("#calMsg", "success", "Saved.");
                    loadEvents();
                } else {
                    showAlert("#calFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#calFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteEvent(eventId) {
            $.ajax({
                url: "/Academic/DeleteCalendarEvent",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(eventId)
            })
                .done(res => {
                    if (res?.success) { showAlert("#calMsg", "success", "Deleted."); loadEvents(); }
                    else showAlert("#calMsg", "warning", "Delete failed.");
                })
                .fail(xhr => showAlert("#calMsg", "danger", parseError(xhr)));
        }

        $("#btnLoadEvents").on("click", loadEvents);
        $("#btnNewEvent").on("click", () => openEventModal(0));
        $(document).on("click", ".btn-cal-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openEventModal(id);
        });
        $(document).on("click", ".btn-cal-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete event ID " + id + " ?")) return;
            deleteEvent(id);
        });
        $("#btnSaveEvent").on("click", saveEvent);

        // =========================================================================
        // EXAMS + EXAM PAPERS
        // =========================================================================
        const exModal = new bootstrap.Modal(document.getElementById("exModal"), { backdrop: "static" });
        const paperModal = new bootstrap.Modal(document.getElementById("paperModal"), { backdrop: "static" });
        let selectedExamId = 0;

        function renderExams(list) {
            const $tb = $("#exTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(e => {
                const dates = `${toLocalDate(e.startDate)} - ${toLocalDate(e.endDate)}`;
                return `
          <tr data-id="${e.examId}">
            <td>${e.examId}</td>
            <td>${e.academicYear || ""}</td>
            <td>${e.examName || ""}</td>
            <td>${e.examType || ""}</td>
            <td>${e.className}${e.section ? ("-" + e.section) : ""}</td>
            <td>${dates}</td>
            <td>${e.isPublished ? "Yes" : "No"}</td>
            <td>
              <div class="btn-group btn-group-sm">
                <button class="btn btn-outline-dark btn-ex-select">Select</button>
                <button class="btn btn-outline-primary btn-ex-edit">Edit</button>
                <button class="btn btn-outline-danger btn-ex-del">Delete</button>
              </div>
            </td>
          </tr>
        `;
            }).join("");
            $tb.html(rows);
        }

        function loadExams() {
            clearAlert("#examMsg");
            const q = {
                academicYear: $("#exFilterYear").val().trim() || null,
                className: $("#exFilterClass").val().trim() || null,
                section: $("#exFilterSection").val().trim() || null,
                examType: $("#exFilterType").val().trim() || null,
                isPublished: $("#exFilterPub").val() || null
            };
            $("#exTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetExamList",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderExams)
                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
        }

        function openExamModal(examId) {
            clearAlert("#exFormMsg");
            $("#exModalLabel").text(examId ? "Edit Exam" : "New Exam");
            $("#exId").val(examId || 0);
            $("#exYear,#exName,#exType,#exClass,#exSection,#exStart,#exEnd,#exDesc").val("");
            $("#exPublished").val("false");

            if (examId && examId > 0) {
                $.ajax({
                    url: "/Academic/GetExamById",
                    type: "GET",
                    dataType: "json",
                    data: { examId }
                })
                    .done(e => {
                        $("#exId").val(e.examId);
                        $("#exYear").val(e.academicYear || "");
                        $("#exName").val(e.examName || "");
                        $("#exType").val(e.examType || "");
                        $("#exClass").val(e.className || "");
                        $("#exSection").val(e.section || "");
                        // Adjust to local date
                        if (e.startDate) {
                            const startDate = new Date(e.startDate);
                            startDate.setMinutes(startDate.getMinutes() - startDate.getTimezoneOffset());
                            $("#exStart").val(startDate.toISOString().substring(0, 10));
                        }
                        if (e.endDate) {
                            const endDate = new Date(e.endDate);
                            endDate.setMinutes(endDate.getMinutes() - endDate.getTimezoneOffset());
                            $("#exEnd").val(endDate.toISOString().substring(0, 10));
                        }
                        $("#exPublished").val(e.isPublished ? "true" : "false");
                        $("#exDesc").val(e.description || "");
                    })
                    .fail(xhr => showAlert("#exFormMsg", "danger", parseError(xhr)))
                    .always(() => exModal.show());
            } else {
                exModal.show();
            }
        }

        function saveExam() {
            clearAlert("#exFormMsg");
            const dto = {
                examId: parseInt($("#exId").val(), 10) || 0,
                academicYear: $("#exYear").val().trim(),
                examName: $("#exName").val().trim(),
                examType: $("#exType").val().trim() || "Term",
                className: $("#exClass").val().trim(),
                section: $("#exSection").val().trim() || null,
                startDate: $("#exStart").val() || null,
                endDate: $("#exEnd").val() || null,
                isPublished: $("#exPublished").val() === "true",
                description: $("#exDesc").val().trim() || null
            };
            if (!dto.academicYear || !dto.examName || !dto.className) {
                showAlert("#exFormMsg", "warning", "AY, Exam Name, and Class are required.");
                return;
            }
            const $btn = $("#btnSaveExam").prop("disabled", true).text("Saving...");
            const req = dto.examId > 0
                ? $.ajax({
                    url: "/Academic/UpdateExam",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                })
                : $.ajax({
                    url: "/Academic/CreateExam",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=UTF-8",
                    data: JSON.stringify(dto)
                });

            req.done(res => {
                if (res?.success) {
                    exModal.hide();
                    showAlert("#examMsg", "success", "Saved.");
                    loadExams();
                } else {
                    showAlert("#exFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#exFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deleteExam(examId) {
            $.ajax({
                url: "/Academic/DeleteExam",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(examId)
            })
                .done(res => {
                    if (res?.success) { showAlert("#examMsg", "success", "Deleted."); loadExams(); }
                    else showAlert("#examMsg", "warning", "Delete failed.");
                })
                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
        }

        // ---- Exam Papers
        function renderPapers(list) {
            const $tb = $("#paperTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No papers</td></tr>`);
                return;
            }
            const rows = list.map(p => {
                const time = `${fmtSpan(p.startTime)} - ${fmtSpan(p.endTime)}`;
                const marks = `${p.maxMarks}${p.passingMarks ? "/" + p.passingMarks : ""}`;
                return `<tr data-id="${p.paperId}">
          <td>${p.paperId}</td>
          <td>${subjNameById(p.subjectId)}</td>
          <td>${toLocalDate(p.examDate)}</td>
          <td>${time}</td>
          <td>${roomCodeById(p.roomId)}</td>
          <td>${p.invigilatorUserId ?? ""}</td>
          <td>${marks}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-secondary btn-paper-validate">Validate</button>
              <button class="btn btn-outline-primary btn-paper-edit">Edit</button>
              <button class="btn btn-outline-danger btn-paper-del">Delete</button>
            </div>
          </td>
        </tr>`;
            }).join("");
            $tb.html(rows);
        }

        function loadPapersByExam(examId) {
            selectedExamId = examId;
            $("#btnNewPaper").prop("disabled", !examId);
            $("#exPapersTitle").text(examId ? `(Exam ID ${examId})` : "");
            if (!examId) {
                $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Select an exam to load papers</td></tr>`);
                return;
            }
            $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetExamTimetableByExam",
                type: "GET",
                dataType: "json",
                data: { examId }
            })
                .done(renderPapers)
                .fail(xhr => $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
        }

        function openPaperModal(paperId) {
            if (!selectedExamId) { showAlert("#examMsg", "info", "Select an exam first."); return; }
            clearAlert("#paperFormMsg");
            $("#paperModalLabel").text(paperId ? "Edit Exam Paper" : "Add Exam Paper");
            $("#paperId").val(paperId || 0);
            $("#paperExamId").val(selectedExamId);
            $("#paperSubject").val("");
            $("#paperDate,#paperStart,#paperEnd").val("");
            $("#paperDuration,#paperRoom,#paperInvigilator,#paperMax,#paperPass").val("");
            $("#paperNotes").val("");

            if (paperId && paperId > 0) {
                $.ajax({
                    url: "/Academic/GetExamTimetableByExam",
                    type: "GET",
                    dataType: "json",
                    data: { examId: selectedExamId }
                })
                    .done(list => {
                        const p = (list || []).find(x => x.paperId === paperId);
                        if (!p) { showAlert("#paperFormMsg", "danger", "Paper not found."); return; }
                        $("#paperId").val(p.paperId);
                        $("#paperSubject").val(p.subjectId || "");
                        if (p.examDate) {
                            const examDate = new Date(p.examDate);
                            examDate.setMinutes(examDate.getMinutes() - examDate.getTimezoneOffset());
                            $("#paperDate").val(examDate.toISOString().substring(0, 10));
                        }
                        $("#paperStart").val(p.startTime ? fmtSpan(p.startTime) : "");
                        $("#paperEnd").val(p.endTime ? fmtSpan(p.endTime) : "");
                        $("#paperDuration").val(p.durationMinutes ?? "");
                        $("#paperRoom").val(p.roomId || "");
                        $("#paperInvigilator").val(p.invigilatorUserId ?? "");
                        $("#paperMax").val(p.maxMarks ?? "");
                        $("#paperPass").val(p.passingMarks ?? "");
                        $("#paperNotes").val(p.notes || "");
                    })
                    .fail(xhr => showAlert("#paperFormMsg", "danger", parseError(xhr)))
                    .always(() => paperModal.show());
            } else {
                paperModal.show();
            }
        }

        function validatePaper() {
            clearAlert("#paperFormMsg");
            const dto = {
                paperId: parseInt($("#paperId").val(), 10) || 0,
                examId: parseInt($("#paperExamId").val(), 10),
                subjectId: $("#paperSubject").val() ? parseInt($("#paperSubject").val(), 10) : 0,
                examDate: $("#paperDate").val() || null,
                startTime: ensureTimeSpan($("#paperStart").val()),
                endTime: ensureTimeSpan($("#paperEnd").val()),
                durationMinutes: $("#paperDuration").val() ? parseInt($("#paperDuration").val(), 10) : 0,
                roomId: $("#paperRoom").val() ? parseInt($("#paperRoom").val(), 10) : null,
                invigilatorUserId: $("#paperInvigilator").val() ? parseInt($("#paperInvigilator").val(), 10) : null,
                maxMarks: $("#paperMax").val() ? parseInt($("#paperMax").val(), 10) : 0,
                passingMarks: $("#paperPass").val() ? parseInt($("#paperPass").val(), 10) : null,
                notes: $("#paperNotes").val().trim() || null,
                isActive: true
            };
            if (!dto.examId || !dto.subjectId || !dto.examDate) {
                showAlert("#paperFormMsg", "warning", "Exam, Subject and Exam Date are required.");
                return;
            }
            $.ajax({
                url: "/Academic/ValidateExamPaperConflict",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    const code = res?.code ?? 0;
                    if (code === 0) {
                        showAlert("#paperFormMsg", "success", "No conflicts found.");
                    } else {
                        const map = { [-1]: "Class-time conflict", [-2]: "Invigilator conflict", [-3]: "Room conflict" };
                        const msg = map[code] || `Conflict code ${code}`;
                        showAlert("#paperFormMsg", "danger", msg);
                    }
                })
                .fail(xhr => showAlert("#paperFormMsg", "danger", parseError(xhr)));
        }

        function savePaper() {
            clearAlert("#paperFormMsg");
            const dto = {
                paperId: parseInt($("#paperId").val(), 10) || 0,
                examId: parseInt($("#paperExamId").val(), 10),
                subjectId: $("#paperSubject").val() ? parseInt($("#paperSubject").val(), 10) : 0,
                examDate: $("#paperDate").val() || null,
                startTime: ensureTimeSpan($("#paperStart").val()),
                endTime: ensureTimeSpan($("#paperEnd").val()),
                durationMinutes: $("#paperDuration").val() ? parseInt($("#paperDuration").val(), 10) : 0,
                roomId: $("#paperRoom").val() ? parseInt($("#paperRoom").val(), 10) : null,
                invigilatorUserId: $("#paperInvigilator").val() ? parseInt($("#paperInvigilator").val(), 10) : null,
                maxMarks: $("#paperMax").val() ? parseInt($("#paperMax").val(), 10) : 0,
                passingMarks: $("#paperPass").val() ? parseInt($("#paperPass").val(), 10) : null,
                notes: $("#paperNotes").val().trim() || null,
                isActive: true
            };
            if (!dto.examId || !dto.subjectId || !dto.examDate) {
                showAlert("#paperFormMsg", "warning", "Exam, Subject and Exam Date are required.");
                return;
            }
            const $btn = $("#btnSavePaper").prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Academic/UpsertExamPaper",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        paperModal.hide();
                        showAlert("#examMsg", "success", "Paper saved.");
                        loadPapersByExam(dto.examId);
                    } else {
                        showAlert("#paperFormMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => {
                    if (xhr.status === 409) {
                        const msg = xhr.responseJSON?.error || "Conflict.";
                        showAlert("#paperFormMsg", "danger", msg);
                    } else {
                        showAlert("#paperFormMsg", "danger", parseError(xhr));
                    }
                })
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        function deletePaper(paperId) {
            $.ajax({
                url: "/Academic/DeleteExamPaper",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(paperId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#examMsg", "success", "Paper deleted.");
                        loadPapersByExam(selectedExamId);
                    } else {
                        showAlert("#examMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
        }

        // Exam Timetable by Class
        function renderExamTTByClass(list) {
            const $tb = $("#exTTClassTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(p => {
                const time = `${fmtSpan(p.startTime)} - ${fmtSpan(p.endTime)}`;
                return `<tr>
          <td>${p.examName || ""}</td>
          <td>${subjNameById(p.subjectId)}</td>
          <td>${toLocalDate(p.examDate)}</td>
          <td>${time}</td>
          <td>${roomCodeById(p.roomId)}</td>
          <td>${p.invigilatorUserId ?? ""}</td>
        </tr>`;
            }).join("");
            $tb.html(rows);
        }

        $("#btnLoadExams").on("click", loadExams);
        $("#btnNewExam").on("click", () => openExamModal(0));
        $(document).on("click", ".btn-ex-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openExamModal(id);
        });
        $(document).on("click", ".btn-ex-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete exam ID " + id + " ?")) return;
            deleteExam(id);
        });
        $(document).on("click", ".btn-ex-select", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) loadPapersByExam(id);
        });
        $("#btnSaveExam").on("click", saveExam);

        // Paper events
        $("#btnNewPaper").on("click", () => openPaperModal(0));
        $(document).on("click", ".btn-paper-edit", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (id) openPaperModal(id);
        });
        $(document).on("click", ".btn-paper-del", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            if (!confirm("Delete paper ID " + id + " ?")) return;
            deletePaper(id);
        });
        $(document).on("click", ".btn-paper-validate", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            openPaperModal(id);
            setTimeout(() => $("#btnValidatePaper").trigger("click"), 300);
        });
        $("#btnValidatePaper").on("click", validatePaper);
        $("#btnSavePaper").on("click", savePaper);

        $("#btnLoadExamTTByClass").on("click", function () {
            clearAlert("#examMsg");
            const q = {
                academicYear: $("#exTTYear").val().trim(),
                className: $("#exTTClass").val().trim(),
                section: $("#exTTSection").val().trim() || null
            };
            if (!q.academicYear || !q.className) {
                showAlert("#examMsg", "warning", "AY and Class are required.");
                return;
            }
            $("#exTTClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Academic/GetExamTimetableByClass",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderExamTTByClass)
                .fail(xhr => $("#exTTClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
        });

        // ---------------- Initial loads for Part 2 ----------------
        loadSubjectsForOptions2();
        loadRoomsForOptions2();
        // Optional: prefill some initial lists
        // loadEvents(); loadExams(); loadPlans();
    });
})();
















//// wwwroot/js/script/academic.js
//// PART 1/2
//// Covers: Subjects, Courses, Syllabus, Classrooms, Available Rooms by Slot
//// Requires common.js (apiGet, apiPost, showAlert, clearAlert)

//(function () {
//    "use strict";

//    $(function () {
//        if ($("#academicPage").length === 0) return;

//        const roles = ($("body").attr("data-roles") || "");
//        const isAdmin = roles.includes("Admin");
//        const isTeacher = roles.includes("Teacher");
//        if (!(isAdmin || isTeacher)) return;

//        // ---------------- Utilities ----------------
//        function parseError(xhr) {
//            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
//            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
//            return xhr.status + " " + xhr.statusText;
//        }
//        function toLocalDate(d) {
//            if (!d) return "-";
//            const dt = new Date(d);
//            return isNaN(dt) ? "-" : dt.toLocaleDateString();
//        }
//        function ensureTimeSpan(val) { // "HH:mm" -> "HH:mm:ss"
//            if (!val) return null;
//            return /^\d{2}:\d{2}:\d{2}$/.test(val) ? val : (val + ":00");
//        }
//        function boolStr(v) { return v ? "Yes" : "No"; }

//        // Caches
//        let subjectsCache = [];
//        let roomsCache = [];
//        let selectedCourseId = 0;

//        // Fill selects from caches
//        function fillSubjectSelect($sel, includeAny = true) {
//            const opts = [];
//            if (includeAny) opts.push(`<option value="">Any</option>`);
//            subjectsCache.forEach(s => {
//                opts.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`);
//            });
//            $sel.html(opts.join(""));
//        }
//        function fillSubjectSelectStrict($sel) {
//            const opts = [`<option value="">Select</option>`];
//            subjectsCache.forEach(s => {
//                opts.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`);
//            });
//            $sel.html(opts.join(""));
//        }
//        function fillRoomSelect($sel, includeAny = true) {
//            const opts = [];
//            if (includeAny) opts.push(`<option value="">Any</option>`);
//            roomsCache.forEach(r => {
//                const name = r.name ? ` - ${r.name}` : "";
//                opts.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
//            });
//            $sel.html(opts.join(""));
//        }
//        function fillRoomSelectStrict($sel) {
//            const opts = [`<option value="">Select</option>`];
//            roomsCache.forEach(r => {
//                const name = r.name ? ` - ${r.name}` : "";
//                opts.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
//            });
//            $sel.html(opts.join(""));
//        }

//        // Pre-load subjects/rooms for dropdowns across module
//        function loadSubjectsForOptions(cb) {
//            apiGet("/Academic/GetSubjectList", { isActive: "" })
//                .done(list => {
//                    subjectsCache = Array.isArray(list) ? list : [];
//                    // Populate all subject selects on page
//                    fillSubjectSelect($("#crFilterSubject"), true);
//                    fillSubjectSelectStrict($("#crSubject"));
//                    fillSubjectSelectStrict($("#ttSubject"));   // used in Part 2 (timetable)
//                    fillSubjectSelect($("#lpSubject"), true);   // filter
//                    fillSubjectSelectStrict($("#lpFSubject"));  // modal
//                    fillSubjectSelectStrict($("#paperSubject"));// exam paper
//                })
//                .always(() => cb && cb());
//        }
//        function loadRoomsForOptions(cb) {
//            apiGet("/Academic/GetClassroomList", { isActive: "" })
//                .done(list => {
//                    roomsCache = Array.isArray(list) ? list : [];
//                    // Populate all room selects
//                    fillRoomSelectStrict($("#ttRoom"));        // timetable
//                    fillRoomSelectStrict($("#paperRoom"));     // exam paper
//                })
//                .always(() => cb && cb());
//        }

//        // =========================================================================
//        // SUBJECTS
//        // =========================================================================
//        function renderSubjects(list) {
//            const $tb = $("#subTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(s => `
//        <tr data-id="${s.subjectId}">
//          <td>${s.subjectId}</td>
//          <td>${s.subjectCode}</td>
//          <td>${s.subjectName}</td>
//          <td>${s.shortName || ""}</td>
//          <td>${s.description || ""}</td>
//          <td>${boolStr(s.isActive)}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-primary btn-sub-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-sub-del">Delete</button>
//            </div>
//          </td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function loadSubjects() {
//            clearAlert("#subMsg");
//            const isActive = $("#subFilterActive").val();
//            $("#subTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetSubjectList", { isActive: isActive || "" })
//                .done(renderSubjects)
//                .fail(xhr => showAlert("#subMsg", "danger", parseError(xhr)));
//        }

//        function openSubjectModal(subjectId) {
//            clearAlert("#subFormMsg");
//            $("#subId").val(subjectId || 0);
//            $("#subModalLabel").text(subjectId ? "Edit Subject" : "New Subject");
//            $("#subCode,#subName,#subShort,#subDesc").val("");
//            $("#subActive").val("true");

//            if (subjectId && subjectId > 0) {
//                apiGet("/Academic/GetSubjectById", { subjectId })
//                    .done(s => {
//                        $("#subId").val(s.subjectId);
//                        $("#subCode").val(s.subjectCode);
//                        $("#subName").val(s.subjectName);
//                        $("#subShort").val(s.shortName || "");
//                        $("#subDesc").val(s.description || "");
//                        $("#subActive").val(s.isActive ? "true" : "false");
//                    })
//                    .fail(xhr => showAlert("#subFormMsg", "danger", parseError(xhr)))
//                    .always(() => subModal.show());
//            } else {
//                subModal.show();
//            }
//        }

//        function saveSubject() {
//            clearAlert("#subFormMsg");
//            const dto = {
//                subjectId: parseInt($("#subId").val(), 10) || 0,
//                subjectCode: $("#subCode").val().trim(),
//                subjectName: $("#subName").val().trim(),
//                shortName: $("#subShort").val().trim() || null,
//                description: $("#subDesc").val().trim() || null,
//                isActive: $("#subActive").val() === "true"
//            };
//            if (!dto.subjectCode || !dto.subjectName) {
//                showAlert("#subFormMsg", "warning", "Subject Code and Name are required.");
//                return;
//            }
//            const $btn = $("#btnSaveSubject").prop("disabled", true).text("Saving...");
//            const req = dto.subjectId > 0 ? apiPost("/Academic/UpdateSubject", dto)
//                : apiPost("/Academic/CreateSubject", dto);
//            req.done(res => {
//                if (res?.success) {
//                    subModal.hide();
//                    showAlert("#subMsg", "success", "Saved.");
//                    loadSubjects();
//                    loadSubjectsForOptions(); // refresh dropdowns
//                } else {
//                    showAlert("#subFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#subFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteSubject(subjectId) {
//            apiPost("/Academic/DeleteSubject", subjectId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#subMsg", "success", "Deleted.");
//                        loadSubjects();
//                        loadSubjectsForOptions();
//                    } else {
//                        showAlert("#subMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#subMsg", "danger", parseError(xhr)));
//        }

//        // =========================================================================
//        // COURSES
//        // =========================================================================
//        function renderCourses(list) {
//            const $tb = $("#crTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            // Build a map for subject names
//            const subjMap = {};
//            subjectsCache.forEach(s => subjMap[s.subjectId] = s.subjectName);

//            const rows = list.map(c => `
//        <tr data-id="${c.courseId}" data-subject-id="${c.subjectId}">
//          <td>${c.courseId}</td>
//          <td>${c.academicYear}</td>
//          <td>${c.className}</td>
//          <td>${subjMap[c.subjectId] || c.subjectId}</td>
//          <td>${boolStr(c.isActive)}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-dark btn-cr-sy">Syllabus</button>
//              <button class="btn btn-outline-primary btn-cr-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-cr-del">Delete</button>
//            </div>
//          </td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function loadCourses() {
//            clearAlert("#courseMsg");
//            const q = {
//                academicYear: $("#crFilterYear").val().trim() || null,
//                className: $("#crFilterClass").val().trim() || null,
//                subjectId: $("#crFilterSubject").val() ? parseInt($("#crFilterSubject").val(), 10) : null,
//                isActive: $("#crFilterActive").val() || null
//            };
//            $("#crTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetCourseList", q)
//                .done(renderCourses)
//                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
//        }

//        function openCourseModal(courseId) {
//            clearAlert("#crFormMsg");
//            $("#crId").val(courseId || 0);
//            $("#crModalLabel").text(courseId ? "Edit Course" : "New Course");
//            $("#crYear,#crClass,#crDesc").val("");
//            $("#crSubject").val("");
//            $("#crActive").val("true");

//            if (courseId && courseId > 0) {
//                apiGet("/Academic/GetCourseById", { courseId })
//                    .done(c => {
//                        $("#crId").val(c.courseId);
//                        $("#crYear").val(c.academicYear || "");
//                        $("#crClass").val(c.className || "");
//                        $("#crSubject").val(c.subjectId || "");
//                        $("#crActive").val(c.isActive ? "true" : "false");
//                        $("#crDesc").val(c.description || "");
//                    })
//                    .fail(xhr => showAlert("#crFormMsg", "danger", parseError(xhr)))
//                    .always(() => crModal.show());
//            } else {
//                crModal.show();
//            }
//        }

//        function saveCourse() {
//            clearAlert("#crFormMsg");
//            const dto = {
//                courseId: parseInt($("#crId").val(), 10) || 0,
//                subjectId: $("#crSubject").val() ? parseInt($("#crSubject").val(), 10) : 0,
//                className: $("#crClass").val().trim(),
//                academicYear: $("#crYear").val().trim(),
//                description: $("#crDesc").val().trim() || null,
//                isActive: $("#crActive").val() === "true"
//            };
//            if (!dto.subjectId || !dto.className || !dto.academicYear) {
//                showAlert("#crFormMsg", "warning", "Academic Year, Class and Subject are required.");
//                return;
//            }
//            const $btn = $("#btnSaveCourse").prop("disabled", true).text("Saving...");
//            const req = dto.courseId > 0 ? apiPost("/Academic/UpdateCourse", dto)
//                : apiPost("/Academic/CreateCourse", dto);
//            req.done(res => {
//                if (res?.success) {
//                    crModal.hide();
//                    showAlert("#courseMsg", "success", "Saved.");
//                    loadCourses();
//                } else {
//                    showAlert("#crFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#crFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteCourse(courseId) {
//            apiPost("/Academic/DeleteCourse", courseId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#courseMsg", "success", "Deleted.");
//                        loadCourses();
//                    } else {
//                        showAlert("#courseMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
//        }

//        // =========================================================================
//        // SYLLABUS (for selected course)
//        // =========================================================================
//        function renderSyllabus(list) {
//            const $tb = $("#syTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No syllabus items</td></tr>`);
//                return;
//            }
//            const rows = list.sort((a, b) => (a.orderIndex || 0) - (b.orderIndex || 0)).map(it => `
//        <tr data-id="${it.syllabusId}">
//          <td>${it.orderIndex ?? ""}</td>
//          <td>${it.unitNo ?? ""}</td>
//          <td>${it.topic}</td>
//          <td>${it.subTopic || ""}</td>
//          <td>${it.estimatedHours ?? ""}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-primary btn-sy-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-sy-del">Delete</button>
//            </div>
//          </td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function loadSyllabus(courseId) {
//            selectedCourseId = courseId;
//            $("#btnNewSyllabus").prop("disabled", !courseId);
//            $("#syCourseTitle").text(courseId ? `(Course ID ${courseId})` : "");
//            if (!courseId) {
//                $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Select a course to view syllabus</td></tr>`);
//                return;
//            }
//            $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetSyllabusByCourse", { courseId })
//                .done(renderSyllabus)
//                .fail(xhr => $("#syTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//        }

//        function openSyllabusModal(syllabusId) {
//            if (!selectedCourseId) { showAlert("#courseMsg", "info", "Select a course first."); return; }
//            clearAlert("#syFormMsg");
//            $("#syId").val(syllabusId || 0);
//            $("#syCourseId").val(selectedCourseId);
//            $("#syModalLabel").text(syllabusId ? "Edit Syllabus Item" : "Add Syllabus Item");
//            $("#syOrder,#syUnit,#syTopic,#sySubTopic,#syObjectives,#syRefs,#syHours").val("");

//            if (syllabusId) {
//                // Get list and find item
//                apiGet("/Academic/GetSyllabusByCourse", { courseId: selectedCourseId })
//                    .done(list => {
//                        const it = (list || []).find(x => x.syllabusId === syllabusId);
//                        if (!it) { showAlert("#syFormMsg", "danger", "Syllabus item not found."); return; }
//                        $("#syId").val(it.syllabusId);
//                        $("#syOrder").val(it.orderIndex ?? "");
//                        $("#syUnit").val(it.unitNo ?? "");
//                        $("#syTopic").val(it.topic || "");
//                        $("#sySubTopic").val(it.subTopic || "");
//                        $("#syObjectives").val(it.objectives || "");
//                        $("#syRefs").val(it.referenceMaterials || "");
//                        $("#syHours").val(it.estimatedHours ?? "");
//                    })
//                    .fail(xhr => showAlert("#syFormMsg", "danger", parseError(xhr)))
//                    .always(() => syModal.show());
//            } else {
//                syModal.show();
//            }
//        }

//        function saveSyllabus() {
//            clearAlert("#syFormMsg");
//            const dto = {
//                syllabusId: parseInt($("#syId").val(), 10) || 0,
//                courseId: parseInt($("#syCourseId").val(), 10),
//                unitNo: $("#syUnit").val() ? parseInt($("#syUnit").val(), 10) : null,
//                topic: $("#syTopic").val().trim(),
//                subTopic: $("#sySubTopic").val().trim() || null,
//                objectives: $("#syObjectives").val().trim() || null,
//                referenceMaterials: $("#syRefs").val().trim() || null,
//                estimatedHours: $("#syHours").val() ? parseFloat($("#syHours").val()) : null,
//                orderIndex: $("#syOrder").val() ? parseInt($("#syOrder").val(), 10) : null
//            };
//            if (!dto.courseId || !dto.topic) {
//                showAlert("#syFormMsg", "warning", "Topic is required.");
//                return;
//            }
//            const $btn = $("#btnSaveSyllabus").prop("disabled", true).text("Saving...");
//            const req = dto.syllabusId > 0 ? apiPost("/Academic/UpdateSyllabusItem", dto)
//                : apiPost("/Academic/AddSyllabusItem", dto);
//            req.done(res => {
//                if (res?.success) {
//                    syModal.hide();
//                    showAlert("#courseMsg", "success", "Syllabus saved.");
//                    loadSyllabus(selectedCourseId);
//                } else {
//                    showAlert("#syFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#syFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteSyllabus(syllabusId) {
//            apiPost("/Academic/DeleteSyllabusItem", syllabusId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#courseMsg", "success", "Deleted.");
//                        loadSyllabus(selectedCourseId);
//                    } else {
//                        showAlert("#courseMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#courseMsg", "danger", parseError(xhr)));
//        }

//        // =========================================================================
//        // CLASSROOMS
//        // =========================================================================
//        function renderRooms(list) {
//            const $tb = $("#roomTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(r => `
//        <tr data-id="${r.roomId}">
//          <td>${r.roomId}</td>
//          <td>${r.roomCode}</td>
//          <td>${r.name || ""}</td>
//          <td>${r.capacity ?? ""}</td>
//          <td>${r.location || ""}</td>
//          <td>${boolStr(r.isActive)}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-primary btn-room-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-room-del">Delete</button>
//            </div>
//          </td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function loadRooms() {
//            clearAlert("#roomMsg");
//            const isActive = $("#roomFilterActive").val() || "";
//            $("#roomTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetClassroomList", { isActive })
//                .done(list => {
//                    renderRooms(list);
//                    // refresh room cache for selects
//                    roomsCache = Array.isArray(list) ? list : [];
//                    fillRoomSelectStrict($("#ttRoom"));
//                    fillRoomSelectStrict($("#paperRoom"));
//                })
//                .fail(xhr => showAlert("#roomMsg", "danger", parseError(xhr)));
//        }

//        function openRoomModal(roomId) {
//            clearAlert("#roomFormMsg");
//            $("#roomId").val(roomId || 0);
//            $("#roomModalLabel").text(roomId ? "Edit Room" : "New Room");
//            $("#roomCode,#roomName,#roomCapacity,#roomLocation").val("");
//            $("#roomActive").val("true");

//            if (roomId && roomId > 0) {
//                apiGet("/Academic/GetClassroomById", { roomId })
//                    .done(r => {
//                        $("#roomId").val(r.roomId);
//                        $("#roomCode").val(r.roomCode);
//                        $("#roomName").val(r.name || "");
//                        $("#roomCapacity").val(r.capacity ?? "");
//                        $("#roomLocation").val(r.location || "");
//                        $("#roomActive").val(r.isActive ? "true" : "false");
//                    })
//                    .fail(xhr => showAlert("#roomFormMsg", "danger", parseError(xhr)))
//                    .always(() => roomModal.show());
//            } else {
//                roomModal.show();
//            }
//        }

//        function saveRoom() {
//            clearAlert("#roomFormMsg");
//            const dto = {
//                roomId: parseInt($("#roomId").val(), 10) || 0,
//                roomCode: $("#roomCode").val().trim(),
//                name: $("#roomName").val().trim() || null,
//                capacity: $("#roomCapacity").val() ? parseInt($("#roomCapacity").val(), 10) : null,
//                location: $("#roomLocation").val().trim() || null,
//                isActive: $("#roomActive").val() === "true"
//            };
//            if (!dto.roomCode) {
//                showAlert("#roomFormMsg", "warning", "Room code is required.");
//                return;
//            }
//            const $btn = $("#btnSaveRoom").prop("disabled", true).text("Saving...");
//            const req = dto.roomId > 0 ? apiPost("/Academic/UpdateClassroom", dto)
//                : apiPost("/Academic/CreateClassroom", dto);
//            req.done(res => {
//                if (res?.success) {
//                    roomModal.hide();
//                    showAlert("#roomMsg", "success", "Saved.");
//                    loadRooms();
//                } else {
//                    showAlert("#roomFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#roomFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteRoom(roomId) {
//            apiPost("/Academic/DeleteClassroom", roomId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#roomMsg", "success", "Deleted.");
//                        loadRooms();
//                    } else {
//                        showAlert("#roomMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#roomMsg", "danger", parseError(xhr)));
//        }

//        // Available rooms by slot
//        function renderAvailableRooms(list) {
//            const $tb = $("#availRoomTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="5" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(r => `
//        <tr>
//          <td>${r.roomId}</td>
//          <td>${r.roomCode}</td>
//          <td>${r.name || ""}</td>
//          <td>${r.capacity ?? ""}</td>
//          <td>${r.location || ""}</td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function findAvailableRooms() {
//            clearAlert("#availRoomMsg");
//            const q = {
//                academicYear: $("#arYear").val().trim(),
//                dayOfWeek: $("#arDay").val() ? parseInt($("#arDay").val(), 10) : null,
//                periodNo: $("#arPeriod").val() ? parseInt($("#arPeriod").val(), 10) : null,
//                startTime: ensureTimeSpan($("#arStart").val()),
//                endTime: ensureTimeSpan($("#arEnd").val())
//            };
//            if (!q.academicYear || !q.dayOfWeek) {
//                showAlert("#availRoomMsg", "warning", "Academic Year and Day are required.");
//                return;
//            }
//            $("#availRoomTable tbody").html(`<tr><td colspan="5" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetAvailableRoomsBySlot", q)
//                .done(renderAvailableRooms)
//                .fail(xhr => showAlert("#availRoomMsg", "danger", parseError(xhr)));
//        }

//        // ---------------- Modals ----------------
//        const subModal = new bootstrap.Modal(document.getElementById("subModal"), { backdrop: "static" });
//        const crModal = new bootstrap.Modal(document.getElementById("crModal"), { backdrop: "static" });
//        const syModal = new bootstrap.Modal(document.getElementById("syModal"), { backdrop: "static" });
//        const roomModal = new bootstrap.Modal(document.getElementById("roomModal"), { backdrop: "static" });

//        // ---------------- Events ----------------
//        // Subjects
//        $("#btnLoadSubjects").on("click", loadSubjects);
//        $("#btnNewSubject").on("click", () => openSubjectModal(0));
//        $(document).on("click", ".btn-sub-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openSubjectModal(id);
//        });
//        $(document).on("click", ".btn-sub-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete subject ID " + id + " ?")) return;
//            deleteSubject(id);
//        });
//        $("#btnSaveSubject").on("click", saveSubject);

//        // Courses
//        $("#btnLoadCourses").on("click", loadCourses);
//        $("#btnNewCourse").on("click", () => openCourseModal(0));
//        $(document).on("click", ".btn-cr-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openCourseModal(id);
//        });
//        $(document).on("click", ".btn-cr-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete course ID " + id + " ?")) return;
//            deleteCourse(id);
//        });
//        $(document).on("click", ".btn-cr-sy", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            loadSyllabus(id);
//        });
//        $("#btnSaveCourse").on("click", saveCourse);

//        // Syllabus
//        $("#btnNewSyllabus").on("click", () => openSyllabusModal(0));
//        $(document).on("click", ".btn-sy-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openSyllabusModal(id);
//        });
//        $(document).on("click", ".btn-sy-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete syllabus ID " + id + " ?")) return;
//            deleteSyllabus(id);
//        });
//        $("#btnSaveSyllabus").on("click", saveSyllabus);

//        // Rooms
//        $("#btnLoadRooms").on("click", loadRooms);
//        $("#btnNewRoom").on("click", () => openRoomModal(0));
//        $(document).on("click", ".btn-room-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openRoomModal(id);
//        });
//        $(document).on("click", ".btn-room-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete room ID " + id + " ?")) return;
//            deleteRoom(id);
//        });
//        $("#btnSaveRoom").on("click", saveRoom);

//        // Available Rooms by Slot
//        $("#btnFindRooms").on("click", findAvailableRooms);

//        // ---------------- Initial Loads ----------------
//        loadSubjectsForOptions(() => {
//            loadCourses();
//            loadSubjects(); // initial subjects
//        });
//        loadRoomsForOptions(() => {
//            loadRooms();
//        });
//    });
//})();



//// wwwroot/js/script/academic.js
//// PART 2/2
//// Covers: Timetable, Lesson Plans, Academic Calendar, Exams (and Exam Papers)
//// Requires common.js (apiGet, apiPost, showAlert, clearAlert)

//(function () {
//    "use strict";

//    $(function () {
//        if ($("#academicPage").length === 0) return;

//        const roles = ($("body").attr("data-roles") || "");
//        const isAdmin = roles.includes("Admin");
//        const isTeacher = roles.includes("Teacher");
//        if (!(isAdmin || isTeacher)) return;

//        // ---------------- Utilities ----------------
//        function parseError(xhr) {
//            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
//            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
//            return xhr.status + " " + xhr.statusText;
//        }
//        function dayName(n) {
//            const map = { 1: "Mon", 2: "Tue", 3: "Wed", 4: "Thu", 5: "Fri", 6: "Sat", 7: "Sun" };
//            return map[n] || n;
//        }
//        function toLocalDate(d) {
//            if (!d) return "-";
//            const dt = new Date(d);
//            return isNaN(dt) ? "-" : dt.toLocaleDateString();
//        }
//        function ensureTimeSpan(val) { // "HH:mm" -> "HH:mm:ss"
//            if (!val) return null;
//            return /^\d{2}:\d{2}:\d{2}$/.test(val) ? val : (val + ":00");
//        }
//        function fmtSpan(ts) {
//            if (!ts) return "";
//            // accept "HH:mm:ss" or "HH:mm"
//            if (/^\d{2}:\d{2}$/.test(ts)) return ts;
//            if (/^\d{2}:\d{2}:\d{2}$/.test(ts)) return ts.substring(0, 5);
//            return ts;
//        }

//        // Caches (local for Part 2; safe if Part 1 not loaded yet)
//        let subjectsCache2 = [];
//        let roomsCache2 = [];

//        function loadSubjectsForOptions2(cb) {
//            apiGet("/Academic/GetSubjectList", { isActive: "" })
//                .done(list => {
//                    subjectsCache2 = Array.isArray(list) ? list : [];
//                    // Fill selects this part uses
//                    const optsSelect = (includeAny) => {
//                        const arr = [];
//                        if (includeAny) arr.push(`<option value="">Any</option>`);
//                        subjectsCache2.forEach(s => arr.push(`<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`));
//                        return arr.join("");
//                    };
//                    $("#ttSubject").html(`<option value="">Select</option>` + optsSelect(false));
//                    $("#lpSubject").html(optsSelect(true));
//                    $("#lpFSubject").html(`<option value="">Select</option>` + optsSelect(false));
//                    $("#paperSubject").html(`<option value="">Select</option>` + optsSelect(false));
//                })
//                .always(() => cb && cb());
//        }
//        function loadRoomsForOptions2(cb) {
//            apiGet("/Academic/GetClassroomList", { isActive: "" })
//                .done(list => {
//                    roomsCache2 = Array.isArray(list) ? list : [];
//                    const optsSelectStrict = () => {
//                        const arr = [`<option value="">Select</option>`];
//                        roomsCache2.forEach(r => {
//                            const name = r.name ? ` - ${r.name}` : "";
//                            arr.push(`<option value="${r.roomId}">${r.roomCode}${name}</option>`);
//                        });
//                        return arr.join("");
//                    };
//                    $("#ttRoom").html(optsSelectStrict());
//                    $("#paperRoom").html(optsSelectStrict());
//                })
//                .always(() => cb && cb());
//        }

//        function subjNameById(id) {
//            const s = subjectsCache2.find(x => x.subjectId === id);
//            return s ? s.subjectName : id;
//        }
//        function roomCodeById(id) {
//            const r = roomsCache2.find(x => x.roomId === id);
//            return r ? (r.roomCode + (r.name ? ` - ${r.name}` : "")) : id;
//        }

//        // =========================================================================
//        // TIMETABLE
//        // =========================================================================
//        const ttConflictMap = {
//            [-1]: "Class/Section conflict",
//            [-2]: "Teacher conflict",
//            [-3]: "Room conflict"
//        };

//        function newTimetableForm() {
//            $("#ttId").val(0);
//            $("#ttAY,#ttClass,#ttSection,#ttStart,#ttEnd").val("");
//            $("#ttDay").val("1");
//            $("#ttPeriod").val("");
//            $("#ttSubject").val("");
//            $("#ttCourseId").val("");
//            $("#ttTeacher").val("");
//            $("#ttRoom").val("");
//            $("#ttActive").val("true");
//            $("#ttValidateMsg").empty();
//        }

//        function buildTimetableDto() {
//            return {
//                timetableId: parseInt($("#ttId").val(), 10) || 0,
//                academicYear: $("#ttAY").val().trim(),
//                className: $("#ttClass").val().trim(),
//                section: $("#ttSection").val().trim() || null,
//                dayOfWeek: $("#ttDay").val() ? parseInt($("#ttDay").val(), 10) : 1,
//                periodNo: $("#ttPeriod").val() ? parseInt($("#ttPeriod").val(), 10) : null,
//                startTime: ensureTimeSpan($("#ttStart").val()),
//                endTime: ensureTimeSpan($("#ttEnd").val()),
//                subjectId: $("#ttSubject").val() ? parseInt($("#ttSubject").val(), 10) : 0,
//                courseId: $("#ttCourseId").val() ? parseInt($("#ttCourseId").val(), 10) : null,
//                teacherUserId: $("#ttTeacher").val() ? parseInt($("#ttTeacher").val(), 10) : null,
//                roomId: $("#ttRoom").val() ? parseInt($("#ttRoom").val(), 10) : null,
//                isActive: $("#ttActive").val() === "true"
//            };
//        }

//        $("#btnNewTimetable").on("click", newTimetableForm);

//        $("#btnValidateTimetable").on("click", function () {
//            clearAlert("#ttMsg");
//            $("#ttValidateMsg").empty();
//            const dto = buildTimetableDto();
//            if (!dto.academicYear || !dto.className || !dto.subjectId) {
//                $("#ttValidateMsg").html(`<div class="alert alert-warning">Academic Year, Class and Subject are required.</div>`);
//                return;
//            }
//            apiPost("/Academic/ValidateTimetableConflict", dto)
//                .done(res => {
//                    const code = res?.code ?? 0;
//                    if (code === 0) {
//                        $("#ttValidateMsg").html(`<div class="alert alert-success">No conflicts found.</div>`);
//                    } else {
//                        const msg = ttConflictMap[code] || `Conflict code ${code}`;
//                        $("#ttValidateMsg").html(`<div class="alert alert-danger">${msg}</div>`);
//                    }
//                })
//                .fail(xhr => $("#ttValidateMsg").html(`<div class="alert alert-danger">${parseError(xhr)}</div>`));
//        });

//        $("#btnSaveTimetable").on("click", function () {
//            clearAlert("#ttMsg");
//            $("#ttValidateMsg").empty();
//            const dto = buildTimetableDto();
//            if (!dto.academicYear || !dto.className || !dto.subjectId) {
//                showAlert("#ttMsg", "warning", "Academic Year, Class and Subject are required.");
//                return;
//            }
//            const $btn = $(this).prop("disabled", true).text("Saving...");
//            apiPost("/Academic/UpsertTimetableEntry", dto)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#ttMsg", "success", "Saved.");
//                        $("#ttId").val(res.id || dto.timetableId);
//                    } else {
//                        showAlert("#ttMsg", "warning", "Save failed.");
//                    }
//                })
//                .fail(xhr => {
//                    if (xhr.status === 409) {
//                        const msg = xhr.responseJSON?.error || "Conflict.";
//                        $("#ttValidateMsg").html(`<div class="alert alert-danger">${msg}</div>`);
//                    } else {
//                        showAlert("#ttMsg", "danger", parseError(xhr));
//                    }
//                })
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        });

//        // View Class Timetable
//        function renderClassTimetable(list) {
//            const $tb = $("#vtClassTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(x => {
//                const time = (x.startTime || x.endTime) ? `${fmtSpan(x.startTime)} - ${fmtSpan(x.endTime)}` : "";
//                return `<tr>
//          <td>${dayName(x.dayOfWeek)}</td>
//          <td>${x.periodNo ?? ""}</td>
//          <td>${time}</td>
//          <td>${subjNameById(x.subjectId)}</td>
//          <td>${x.teacherUserId ?? ""}</td>
//          <td>${roomCodeById(x.roomId)}</td>
//        </tr>`;
//            }).join("");
//            $tb.html(rows);
//        }

//        $("#btnViewClassTimetable").on("click", function () {
//            clearAlert("#ttMsg");
//            const q = {
//                academicYear: $("#vtAY").val().trim(),
//                className: $("#vtClass").val().trim(),
//                section: $("#vtSection").val().trim() || null
//            };
//            if (!q.academicYear || !q.className) {
//                showAlert("#ttMsg", "warning", "AY and Class are required.");
//                return;
//            }
//            $("#vtClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetClassTimetable", q)
//                .done(renderClassTimetable)
//                .fail(xhr => $("#vtClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//        });

//        // View Teacher Timetable
//        function renderTeacherTimetable(list) {
//            const $tb = $("#vtTeacherTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(x => {
//                const time = (x.startTime || x.endTime) ? `${fmtSpan(x.startTime)} - ${fmtSpan(x.endTime)}` : "";
//                return `<tr>
//          <td>${dayName(x.dayOfWeek)}</td>
//          <td>${x.periodNo ?? ""}</td>
//          <td>${time}</td>
//          <td>${x.className}${x.section ? ("-" + x.section) : ""}</td>
//          <td>${subjNameById(x.subjectId)}</td>
//          <td>${roomCodeById(x.roomId)}</td>
//        </tr>`;
//            }).join("");
//            $tb.html(rows);
//        }

//        $("#btnViewTeacherTimetable").on("click", function () {
//            clearAlert("#ttMsg");
//            const q = {
//                academicYear: $("#ttcAY").val().trim(),
//                teacherUserId: $("#ttcTeacher").val() ? parseInt($("#ttcTeacher").val(), 10) : 0
//            };
//            if (!q.academicYear || !q.teacherUserId) {
//                showAlert("#ttMsg", "warning", "AY and Teacher userId are required.");
//                return;
//            }
//            $("#vtTeacherTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetTeacherTimetable", q)
//                .done(renderTeacherTimetable)
//                .fail(xhr => $("#vtTeacherTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//        });

//        // =========================================================================
//        // LESSON PLANS
//        // =========================================================================
//        const lpModal = new bootstrap.Modal(document.getElementById("lpModal"), { backdrop: "static" });

//        function renderPlans(list) {
//            const $tb = $("#lpTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(p => `
//        <tr data-id="${p.planId}">
//          <td>${p.planId}</td>
//          <td>${toLocalDate(p.planDate)}</td>
//          <td>${p.className}${p.section ? ("-" + p.section) : ""}</td>
//          <td>${subjNameById(p.subjectId)}</td>
//          <td>${p.teacherUserId ?? ""}</td>
//          <td>${p.topic}</td>
//          <td><span class="badge ${p.status === 'Delivered' ? 'bg-success' : (p.status === 'Planned' ? 'bg-info text-dark' : 'bg-secondary')}">${p.status}</span></td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-primary btn-lp-edit">Edit</button>
//              <button class="btn btn-outline-success btn-lp-status" data-status="Delivered">Mark Delivered</button>
//              <button class="btn btn-outline-secondary btn-lp-status" data-status="Planned">Mark Planned</button>
//              <button class="btn btn-outline-danger btn-lp-del">Delete</button>
//            </div>
//          </td>
//        </tr>
//      `).join("");
//            $tb.html(rows);
//        }

//        function loadPlans() {
//            clearAlert("#lpMsg");
//            const q = {
//                academicYear: $("#lpAY").val().trim() || null,
//                className: $("#lpClass").val().trim() || null,
//                section: $("#lpSection").val().trim() || null,
//                subjectId: $("#lpSubject").val() ? parseInt($("#lpSubject").val(), 10) : null,
//                teacherUserId: $("#lpTeacher").val() ? parseInt($("#lpTeacher").val(), 10) : null,
//                fromDate: $("#lpFrom").val() || null,
//                toDate: $("#lpTo").val() || null,
//                status: $("#lpStatus").val() || null
//            };
//            $("#lpTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetLessonPlanList", q)
//                .done(renderPlans)
//                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
//        }

//        function openPlanModal(planId) {
//            clearAlert("#lpFormMsg");
//            $("#lpModalLabel").text(planId ? "Edit Lesson Plan" : "New Lesson Plan");
//            $("#lpId").val(planId || 0);
//            // reset modal
//            $("#lpFAy,#lpFClass,#lpFSection,#lpTopic,#lpSubTopic,#lpObjectives,#lpActivities,#lpResources,#lpHomework,#lpAssess,#lpNotes").val("");
//            $("#lpFSubject").val("");
//            $("#lpFTeacher").val("");
//            $("#lpDate").val("");
//            $("#lpPeriod").val("");
//            $("#lpStart").val("");
//            $("#lpEnd").val("");
//            $("#lpStatusVal").val("Draft");

//            if (planId && planId > 0) {
//                apiGet("/Academic/GetLessonPlanById", { planId })
//                    .done(p => {
//                        $("#lpId").val(p.planId);
//                        $("#lpFAy").val(p.academicYear || "");
//                        $("#lpFClass").val(p.className || "");
//                        $("#lpFSection").val(p.section || "");
//                        $("#lpFSubject").val(p.subjectId || "");
//                        $("#lpFTeacher").val(p.teacherUserId || "");
//                        if (p.planDate) $("#lpDate").val(new Date(p.planDate).toISOString().substring(0, 10));
//                        $("#lpPeriod").val(p.periodNo ?? "");
//                        $("#lpStart").val(p.startTime ? fmtSpan(p.startTime) : "");
//                        $("#lpEnd").val(p.endTime ? fmtSpan(p.endTime) : "");
//                        $("#lpStatusVal").val(p.status || "Draft");
//                        $("#lpTopic").val(p.topic || "");
//                        $("#lpSubTopic").val(p.subTopic || "");
//                        $("#lpObjectives").val(p.objectives || "");
//                        $("#lpActivities").val(p.activities || "");
//                        $("#lpResources").val(p.resources || "");
//                        $("#lpHomework").val(p.homework || "");
//                        $("#lpAssess").val(p.assessmentMethods || "");
//                        $("#lpNotes").val(p.notes || "");
//                    })
//                    .fail(xhr => showAlert("#lpFormMsg", "danger", parseError(xhr)))
//                    .always(() => lpModal.show());
//            } else {
//                lpModal.show();
//            }
//        }

//        function savePlan() {
//            clearAlert("#lpFormMsg");
//            const dto = {
//                planId: parseInt($("#lpId").val(), 10) || 0,
//                academicYear: $("#lpFAy").val().trim(),
//                className: $("#lpFClass").val().trim(),
//                section: $("#lpFSection").val().trim() || null,
//                courseId: null, // optional link not captured in UI
//                subjectId: $("#lpFSubject").val() ? parseInt($("#lpFSubject").val(), 10) : null,
//                teacherUserId: $("#lpFTeacher").val() ? parseInt($("#lpFTeacher").val(), 10) : 0,
//                planDate: $("#lpDate").val() || null,
//                periodNo: $("#lpPeriod").val() ? parseInt($("#lpPeriod").val(), 10) : null,
//                startTime: ensureTimeSpan($("#lpStart").val()),
//                endTime: ensureTimeSpan($("#lpEnd").val()),
//                topic: $("#lpTopic").val().trim(),
//                subTopic: $("#lpSubTopic").val().trim() || null,
//                objectives: $("#lpObjectives").val().trim() || null,
//                activities: $("#lpActivities").val().trim() || null,
//                resources: $("#lpResources").val().trim() || null,
//                homework: $("#lpHomework").val().trim() || null,
//                assessmentMethods: $("#lpAssess").val().trim() || null,
//                status: $("#lpStatusVal").val() || "Draft",
//                notes: $("#lpNotes").val().trim() || null
//            };
//            if (!dto.academicYear || !dto.className || !dto.teacherUserId || !dto.topic || !dto.planDate) {
//                showAlert("#lpFormMsg", "warning", "Academic Year, Class, Teacher, Topic and Plan Date are required.");
//                return;
//            }
//            const $btn = $("#btnSavePlan").prop("disabled", true).text("Saving...");
//            const req = dto.planId > 0 ? apiPost("/Academic/UpdateLessonPlan", dto)
//                : apiPost("/Academic/CreateLessonPlan", dto);
//            req.done(res => {
//                if (res?.success) {
//                    lpModal.hide();
//                    showAlert("#lpMsg", "success", "Saved.");
//                    loadPlans();
//                } else {
//                    showAlert("#lpFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#lpFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function updatePlanStatus(planId, status) {
//            apiPost("/Academic/UpdateLessonPlanStatus", { planId, status })
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#lpMsg", "success", "Status updated.");
//                        loadPlans();
//                    } else {
//                        showAlert("#lpMsg", "warning", "Status update failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
//        }

//        function deletePlan(planId) {
//            apiPost("/Academic/DeleteLessonPlan", planId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#lpMsg", "success", "Deleted.");
//                        loadPlans();
//                    } else {
//                        showAlert("#lpMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#lpMsg", "danger", parseError(xhr)));
//        }

//        $("#btnLoadPlans").on("click", loadPlans);
//        $("#btnNewPlan").on("click", () => openPlanModal(0));
//        $(document).on("click", ".btn-lp-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openPlanModal(id);
//        });
//        $(document).on("click", ".btn-lp-status", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            const st = $(this).data("status");
//            if (id && st) updatePlanStatus(id, st);
//        });
//        $(document).on("click", ".btn-lp-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete plan ID " + id + " ?")) return;
//            deletePlan(id);
//        });
//        $("#btnSavePlan").on("click", savePlan);

//        // =========================================================================
//        // ACADEMIC CALENDAR
//        // =========================================================================
//        const calModal = new bootstrap.Modal(document.getElementById("calModal"), { backdrop: "static" });

//        function renderEvents(list) {
//            const $tb = $("#calTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(e => {
//                const dates = `${toLocalDate(e.startDate)} - ${toLocalDate(e.endDate)}`;
//                return `<tr data-id="${e.eventId}">
//          <td>${e.eventId}</td>
//          <td>${e.academicYear || ""}</td>
//          <td>${e.eventType || ""}</td>
//          <td>${e.title || ""}</td>
//          <td>${(e.className || "") + (e.section ? ("-" + e.section) : "")}</td>
//          <td>${dates}</td>
//          <td>${e.isActive ? "Yes" : "No"}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-primary btn-cal-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-cal-del">Delete</button>
//            </div>
//          </td>
//        </tr>`;
//            }).join("");
//            $tb.html(rows);
//        }

//        function loadEvents() {
//            clearAlert("#calMsg");
//            const q = {
//                academicYear: $("#calAY").val().trim() || null,
//                fromDate: $("#calFrom").val() || null,
//                toDate: $("#calTo").val() || null,
//                className: $("#calClass").val().trim() || null,
//                section: $("#calSection").val().trim() || null,
//                eventType: $("#calType").val().trim() || null,
//                isActive: $("#calActive").val() || null
//            };
//            $("#calTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetCalendarEvents", q)
//                .done(renderEvents)
//                .fail(xhr => showAlert("#calMsg", "danger", parseError(xhr)));
//        }

//        function openEventModal(eventId) {
//            clearAlert("#calFormMsg");
//            $("#calModalLabel").text(eventId ? "Edit Event" : "New Event");
//            $("#calId").val(eventId || 0);
//            $("#calFAy,#calFType,#calFTitle,#calFClass,#calFSection,#calFDesc").val("");
//            $("#calFStart,#calFEnd").val("");
//            $("#calFActive").val("true");

//            if (eventId && eventId > 0) {
//                apiGet("/Academic/GetCalendarEventById", { eventId })
//                    .done(e => {
//                        $("#calId").val(e.eventId);
//                        $("#calFAy").val(e.academicYear || "");
//                        $("#calFType").val(e.eventType || "");
//                        $("#calFTitle").val(e.title || "");
//                        $("#calFClass").val(e.className || "");
//                        $("#calFSection").val(e.section || "");
//                        if (e.startDate) $("#calFStart").val(new Date(e.startDate).toISOString().substring(0, 10));
//                        if (e.endDate) $("#calFEnd").val(new Date(e.endDate).toISOString().substring(0, 10));
//                        $("#calFActive").val(e.isActive ? "true" : "false");
//                        $("#calFDesc").val(e.description || "");
//                    })
//                    .fail(xhr => showAlert("#calFormMsg", "danger", parseError(xhr)))
//                    .always(() => calModal.show());
//            } else {
//                calModal.show();
//            }
//        }

//        function saveEvent() {
//            clearAlert("#calFormMsg");
//            const dto = {
//                eventId: parseInt($("#calId").val(), 10) || 0,
//                academicYear: $("#calFAy").val().trim(),
//                eventType: $("#calFType").val().trim(),
//                title: $("#calFTitle").val().trim(),
//                className: $("#calFClass").val().trim() || null,
//                section: $("#calFSection").val().trim() || null,
//                startDate: $("#calFStart").val() || null,
//                endDate: $("#calFEnd").val() || null,
//                isActive: $("#calFActive").val() === "true",
//                description: $("#calFDesc").val().trim() || null
//            };
//            if (!dto.academicYear || !dto.eventType || !dto.title) {
//                showAlert("#calFormMsg", "warning", "AY, Type and Title are required.");
//                return;
//            }
//            const $btn = $("#btnSaveEvent").prop("disabled", true).text("Saving...");
//            const req = dto.eventId > 0 ? apiPost("/Academic/UpdateCalendarEvent", dto)
//                : apiPost("/Academic/CreateCalendarEvent", dto);
//            req.done(res => {
//                if (res?.success) {
//                    calModal.hide();
//                    showAlert("#calMsg", "success", "Saved.");
//                    loadEvents();
//                } else {
//                    showAlert("#calFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#calFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteEvent(eventId) {
//            apiPost("/Academic/DeleteCalendarEvent", eventId)
//                .done(res => {
//                    if (res?.success) { showAlert("#calMsg", "success", "Deleted."); loadEvents(); }
//                    else showAlert("#calMsg", "warning", "Delete failed.");
//                })
//                .fail(xhr => showAlert("#calMsg", "danger", parseError(xhr)));
//        }

//        $("#btnLoadEvents").on("click", loadEvents);
//        $("#btnNewEvent").on("click", () => openEventModal(0));
//        $(document).on("click", ".btn-cal-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openEventModal(id);
//        });
//        $(document).on("click", ".btn-cal-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete event ID " + id + " ?")) return;
//            deleteEvent(id);
//        });
//        $("#btnSaveEvent").on("click", saveEvent);

//        // =========================================================================
//        // EXAMS + EXAM PAPERS
//        // =========================================================================
//        const exModal = new bootstrap.Modal(document.getElementById("exModal"), { backdrop: "static" });
//        const paperModal = new bootstrap.Modal(document.getElementById("paperModal"), { backdrop: "static" });
//        let selectedExamId = 0;

//        function renderExams(list) {
//            const $tb = $("#exTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(e => {
//                const dates = `${toLocalDate(e.startDate)} - ${toLocalDate(e.endDate)}`;
//                return `
//          <tr data-id="${e.examId}">
//            <td>${e.examId}</td>
//            <td>${e.academicYear || ""}</td>
//            <td>${e.examName || ""}</td>
//            <td>${e.examType || ""}</td>
//            <td>${e.className}${e.section ? ("-" + e.section) : ""}</td>
//            <td>${dates}</td>
//            <td>${e.isPublished ? "Yes" : "No"}</td>
//            <td>
//              <div class="btn-group btn-group-sm">
//                <button class="btn btn-outline-dark btn-ex-select">Select</button>
//                <button class="btn btn-outline-primary btn-ex-edit">Edit</button>
//                <button class="btn btn-outline-danger btn-ex-del">Delete</button>
//              </div>
//            </td>
//          </tr>
//        `;
//            }).join("");
//            $tb.html(rows);
//        }

//        function loadExams() {
//            clearAlert("#examMsg");
//            const q = {
//                academicYear: $("#exFilterYear").val().trim() || null,
//                className: $("#exFilterClass").val().trim() || null,
//                section: $("#exFilterSection").val().trim() || null,
//                examType: $("#exFilterType").val().trim() || null,
//                isPublished: $("#exFilterPub").val() || null
//            };
//            $("#exTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetExamList", q)
//                .done(renderExams)
//                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
//        }

//        function openExamModal(examId) {
//            clearAlert("#exFormMsg");
//            $("#exModalLabel").text(examId ? "Edit Exam" : "New Exam");
//            $("#exId").val(examId || 0);
//            $("#exYear,#exName,#exType,#exClass,#exSection,#exStart,#exEnd,#exDesc").val("");
//            $("#exPublished").val("false");

//            if (examId && examId > 0) {
//                apiGet("/Academic/GetExamById", { examId })
//                    .done(e => {
//                        $("#exId").val(e.examId);
//                        $("#exYear").val(e.academicYear || "");
//                        $("#exName").val(e.examName || "");
//                        $("#exType").val(e.examType || "");
//                        $("#exClass").val(e.className || "");
//                        $("#exSection").val(e.section || "");
//                        //if (e.startDate) $("#exStart").val(new Date(e.startDate).toISOString().substring(0, 10));
//                        //if (e.endDate) $("#exEnd").val(new Date(e.endDate).toISOString().substring(0, 10));
//                        if (e.startDate) {
//                            const startDate = new Date(e.startDate);
//                            startDate.setMinutes(startDate.getMinutes() - startDate.getTimezoneOffset()); // Adjust to local time
//                            $("#exStart").val(startDate.toISOString().substring(0, 10));
//                        }

//                        if (e.endDate) {
//                            const endDate = new Date(e.endDate);
//                            endDate.setMinutes(endDate.getMinutes() - endDate.getTimezoneOffset()); // Adjust to local time
//                            $("#exEnd").val(endDate.toISOString().substring(0, 10));
//                        }
//                        $("#exPublished").val(e.isPublished ? "true" : "false");
//                        $("#exDesc").val(e.description || "");
//                    })
//                    .fail(xhr => showAlert("#exFormMsg", "danger", parseError(xhr)))
//                    .always(() => exModal.show());
//            } else {
//                exModal.show();
//            }
//        }

//        function saveExam() {
//            clearAlert("#exFormMsg");
//            const dto = {
//                examId: parseInt($("#exId").val(), 10) || 0,
//                academicYear: $("#exYear").val().trim(),
//                examName: $("#exName").val().trim(),
//                examType: $("#exType").val().trim() || "Term",
//                className: $("#exClass").val().trim(),
//                section: $("#exSection").val().trim() || null,
//                startDate: $("#exStart").val() || null,
//                endDate: $("#exEnd").val() || null,
//                isPublished: $("#exPublished").val() === "true",
//                description: $("#exDesc").val().trim() || null
//            };
//            if (!dto.academicYear || !dto.examName || !dto.className) {
//                showAlert("#exFormMsg", "warning", "AY, Exam Name, and Class are required.");
//                return;
//            }
//            const $btn = $("#btnSaveExam").prop("disabled", true).text("Saving...");
//            const req = dto.examId > 0 ? apiPost("/Academic/UpdateExam", dto)
//                : apiPost("/Academic/CreateExam", dto);
//            req.done(res => {
//                if (res?.success) {
//                    exModal.hide();
//                    showAlert("#examMsg", "success", "Saved.");
//                    loadExams();
//                } else {
//                    showAlert("#exFormMsg", "warning", "Save failed.");
//                }
//            }).fail(xhr => showAlert("#exFormMsg", "danger", parseError(xhr)))
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deleteExam(examId) {
//            apiPost("/Academic/DeleteExam", examId)
//                .done(res => {
//                    if (res?.success) { showAlert("#examMsg", "success", "Deleted."); loadExams(); }
//                    else showAlert("#examMsg", "warning", "Delete failed.");
//                })
//                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
//        }

//        // ---- Exam Papers
//        function renderPapers(list) {
//            const $tb = $("#paperTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No papers</td></tr>`);
//                return;
//            }
//            const rows = list.map(p => {
//                const time = `${fmtSpan(p.startTime)} - ${fmtSpan(p.endTime)}`;
//                const marks = `${p.maxMarks}${p.passingMarks ? "/" + p.passingMarks : ""}`;
//                return `<tr data-id="${p.paperId}">
//          <td>${p.paperId}</td>
//          <td>${subjNameById(p.subjectId)}</td>
//          <td>${toLocalDate(p.examDate)}</td>
//          <td>${time}</td>
//          <td>${roomCodeById(p.roomId)}</td>
//          <td>${p.invigilatorUserId ?? ""}</td>
//          <td>${marks}</td>
//          <td>
//            <div class="btn-group btn-group-sm">
//              <button class="btn btn-outline-secondary btn-paper-validate">Validate</button>
//              <button class="btn btn-outline-primary btn-paper-edit">Edit</button>
//              <button class="btn btn-outline-danger btn-paper-del">Delete</button>
//            </div>
//          </td>
//        </tr>`;
//            }).join("");
//            $tb.html(rows);
//        }

//        function loadPapersByExam(examId) {
//            selectedExamId = examId;
//            $("#btnNewPaper").prop("disabled", !examId);
//            $("#exPapersTitle").text(examId ? `(Exam ID ${examId})` : "");
//            if (!examId) {
//                $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Select an exam to load papers</td></tr>`);
//                return;
//            }
//            $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetExamTimetableByExam", { examId })
//                .done(renderPapers)
//                .fail(xhr => $("#paperTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//        }

//        function openPaperModal(paperId) {
//            if (!selectedExamId) { showAlert("#examMsg", "info", "Select an exam first."); return; }
//            clearAlert("#paperFormMsg");
//            $("#paperModalLabel").text(paperId ? "Edit Exam Paper" : "Add Exam Paper");
//            $("#paperId").val(paperId || 0);
//            $("#paperExamId").val(selectedExamId);
//            $("#paperSubject").val("");
//            $("#paperDate,#paperStart,#paperEnd").val("");
//            $("#paperDuration,#paperRoom,#paperInvigilator,#paperMax,#paperPass").val("");
//            $("#paperNotes").val("");

//            if (paperId && paperId > 0) {
//                apiGet("/Academic/GetExamTimetableByExam", { examId: selectedExamId })
//                    .done(list => {
//                        const p = (list || []).find(x => x.paperId === paperId);
//                        if (!p) { showAlert("#paperFormMsg", "danger", "Paper not found."); return; }
//                        $("#paperId").val(p.paperId);
//                        $("#paperSubject").val(p.subjectId || "");
//                        //if (p.examDate) $("#paperDate").val(new Date(p.examDate).toISOString().substring(0, 10));
//                        if (p.examDate) {
//                            const examDate = new Date(p.examDate);
//                            examDate.setMinutes(examDate.getMinutes() - examDate.getTimezoneOffset());
//                            $("#paperDate").val(examDate.toISOString().substring(0, 10));
//                        }

//                        $("#paperStart").val(p.startTime ? fmtSpan(p.startTime) : "");
//                        $("#paperEnd").val(p.endTime ? fmtSpan(p.endTime) : "");
//                        $("#paperDuration").val(p.durationMinutes ?? "");
//                        $("#paperRoom").val(p.roomId || "");
//                        $("#paperInvigilator").val(p.invigilatorUserId ?? "");
//                        $("#paperMax").val(p.maxMarks ?? "");
//                        $("#paperPass").val(p.passingMarks ?? "");
//                        $("#paperNotes").val(p.notes || "");
//                    })
//                    .fail(xhr => showAlert("#paperFormMsg", "danger", parseError(xhr)))
//                    .always(() => paperModal.show());
//            } else {
//                paperModal.show();
//            }
//        }

//        function validatePaper() {
//            clearAlert("#paperFormMsg");
//            const dto = {
//                paperId: parseInt($("#paperId").val(), 10) || 0,
//                examId: parseInt($("#paperExamId").val(), 10),
//                subjectId: $("#paperSubject").val() ? parseInt($("#paperSubject").val(), 10) : 0,
//                examDate: $("#paperDate").val() || null,
//                startTime: ensureTimeSpan($("#paperStart").val()),
//                endTime: ensureTimeSpan($("#paperEnd").val()),
//                durationMinutes: $("#paperDuration").val() ? parseInt($("#paperDuration").val(), 10) : 0,
//                roomId: $("#paperRoom").val() ? parseInt($("#paperRoom").val(), 10) : null,
//                invigilatorUserId: $("#paperInvigilator").val() ? parseInt($("#paperInvigilator").val(), 10) : null,
//                maxMarks: $("#paperMax").val() ? parseInt($("#paperMax").val(), 10) : 0,
//                passingMarks: $("#paperPass").val() ? parseInt($("#paperPass").val(), 10) : null,
//                notes: $("#paperNotes").val().trim() || null,
//                isActive: true
//            };
//            if (!dto.examId || !dto.subjectId || !dto.examDate) {
//                showAlert("#paperFormMsg", "warning", "Exam, Subject and Exam Date are required.");
//                return;
//            }
//            apiPost("/Academic/ValidateExamPaperConflict", dto)
//                .done(res => {
//                    const code = res?.code ?? 0;
//                    if (code === 0) {
//                        showAlert("#paperFormMsg", "success", "No conflicts found.");
//                    } else {
//                        const map = { [-1]: "Class-time conflict", [-2]: "Invigilator conflict", [-3]: "Room conflict" };
//                        const msg = map[code] || `Conflict code ${code}`;
//                        showAlert("#paperFormMsg", "danger", msg);
//                    }
//                })
//                .fail(xhr => showAlert("#paperFormMsg", "danger", parseError(xhr)));
//        }

//        function savePaper() {
//            clearAlert("#paperFormMsg");
//            const dto = {
//                paperId: parseInt($("#paperId").val(), 10) || 0,
//                examId: parseInt($("#paperExamId").val(), 10),
//                subjectId: $("#paperSubject").val() ? parseInt($("#paperSubject").val(), 10) : 0,
//                examDate: $("#paperDate").val() || null,
//                startTime: ensureTimeSpan($("#paperStart").val()),
//                endTime: ensureTimeSpan($("#paperEnd").val()),
//                durationMinutes: $("#paperDuration").val() ? parseInt($("#paperDuration").val(), 10) : 0,
//                roomId: $("#paperRoom").val() ? parseInt($("#paperRoom").val(), 10) : null,
//                invigilatorUserId: $("#paperInvigilator").val() ? parseInt($("#paperInvigilator").val(), 10) : null,
//                maxMarks: $("#paperMax").val() ? parseInt($("#paperMax").val(), 10) : 0,
//                passingMarks: $("#paperPass").val() ? parseInt($("#paperPass").val(), 10) : null,
//                notes: $("#paperNotes").val().trim() || null,
//                isActive: true
//            };
//            if (!dto.examId || !dto.subjectId || !dto.examDate) {
//                showAlert("#paperFormMsg", "warning", "Exam, Subject and Exam Date are required.");
//                return;
//            }
//            const $btn = $("#btnSavePaper").prop("disabled", true).text("Saving...");
//            apiPost("/Academic/UpsertExamPaper", dto)
//                .done(res => {
//                    if (res?.success) {
//                        paperModal.hide();
//                        showAlert("#examMsg", "success", "Paper saved.");
//                        loadPapersByExam(dto.examId);
//                    } else {
//                        showAlert("#paperFormMsg", "warning", "Save failed.");
//                    }
//                })
//                .fail(xhr => {
//                    if (xhr.status === 409) {
//                        const msg = xhr.responseJSON?.error || "Conflict.";
//                        showAlert("#paperFormMsg", "danger", msg);
//                    } else {
//                        showAlert("#paperFormMsg", "danger", parseError(xhr));
//                    }
//                })
//                .always(() => $btn.prop("disabled", false).text("Save"));
//        }

//        function deletePaper(paperId) {
//            apiPost("/Academic/DeleteExamPaper", paperId)
//                .done(res => {
//                    if (res?.success) {
//                        showAlert("#examMsg", "success", "Paper deleted.");
//                        loadPapersByExam(selectedExamId);
//                    } else {
//                        showAlert("#examMsg", "warning", "Delete failed.");
//                    }
//                })
//                .fail(xhr => showAlert("#examMsg", "danger", parseError(xhr)));
//        }

//        // Exam Timetable by Class
//        function renderExamTTByClass(list) {
//            const $tb = $("#exTTClassTable tbody");
//            if (!Array.isArray(list) || list.length === 0) {
//                $tb.html(`<tr><td colspan="6" class="p-3 text-center text-muted">No data</td></tr>`);
//                return;
//            }
//            const rows = list.map(p => {
//                const time = `${fmtSpan(p.startTime)} - ${fmtSpan(p.endTime)}`;
//                return `<tr>
//          <td>${p.examName || ""}</td>
//          <td>${subjNameById(p.subjectId)}</td>
//          <td>${toLocalDate(p.examDate)}</td>
//          <td>${time}</td>
//          <td>${roomCodeById(p.roomId)}</td>
//          <td>${p.invigilatorUserId ?? ""}</td>
//        </tr>`;
//            }).join("");
//            $tb.html(rows);
//        }

//        $("#btnLoadExams").on("click", loadExams);
//        $("#btnNewExam").on("click", () => openExamModal(0));
//        $(document).on("click", ".btn-ex-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openExamModal(id);
//        });
//        $(document).on("click", ".btn-ex-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete exam ID " + id + " ?")) return;
//            deleteExam(id);
//        });
//        $(document).on("click", ".btn-ex-select", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) loadPapersByExam(id);
//        });
//        $("#btnSaveExam").on("click", saveExam);

//        // Paper events
//        $("#btnNewPaper").on("click", () => openPaperModal(0));
//        $(document).on("click", ".btn-paper-edit", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (id) openPaperModal(id);
//        });
//        $(document).on("click", ".btn-paper-del", function () {
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            if (!id) return;
//            if (!confirm("Delete paper ID " + id + " ?")) return;
//            deletePaper(id);
//        });
//        $(document).on("click", ".btn-paper-validate", function () {
//            // Open edit modal then validate current values, or validate current table values?
//            // We'll open modal for clarity.
//            const id = parseInt($(this).closest("tr").data("id"), 10);
//            openPaperModal(id);
//            setTimeout(() => $("#btnValidatePaper").trigger("click"), 300);
//        });
//        $("#btnValidatePaper").on("click", validatePaper);
//        $("#btnSavePaper").on("click", savePaper);

//        $("#btnLoadExamTTByClass").on("click", function () {
//            clearAlert("#examMsg");
//            const q = {
//                academicYear: $("#exTTYear").val().trim(),
//                className: $("#exTTClass").val().trim(),
//                section: $("#exTTSection").val().trim() || null
//            };
//            if (!q.academicYear || !q.className) {
//                showAlert("#examMsg", "warning", "AY and Class are required.");
//                return;
//            }
//            $("#exTTClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-muted">Loading...</td></tr>`);
//            apiGet("/Academic/GetExamTimetableByClass", q)
//                .done(renderExamTTByClass)
//                .fail(xhr => $("#exTTClassTable tbody").html(`<tr><td colspan="6" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));
//        });

//        // ---------------- Initial loads for Part 2 ----------------
//        loadSubjectsForOptions2();
//        loadRoomsForOptions2();
//        // Optional: prefill some initial lists
//        // loadEvents(); loadExams(); loadPlans();
//    });
//})();