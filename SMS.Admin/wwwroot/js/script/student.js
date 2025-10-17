// wwwroot/js/script/student.js
(function () {
    // Columns (use camelCase to match default JSON)
    const columnDefs = [
        { headerName: "ID", field: "studentId", width: 90, filter: false },
        { headerName: "Admission No", field: "admissionNo" },
        { headerName: "First Name", field: "firstName" },
        { headerName: "Last Name", field: "lastName" },
        { headerName: "Class", field: "className" },
        { headerName: "Section", field: "section" },
        { headerName: "Gender", field: "gender" },
        { headerName: "Email", field: "email" },
        { headerName: "Phone", field: "phone" },
        { headerName: "Address", field: "address" },
        { headerName: "Guardian", field: "guardianName" },
        { headerName: "Health Info", field: "healthInfo" },
        {
            headerName: "Photo", field: "photoUrl",
            cellRenderer: params => params.value ? `<a href="${params.value}" target="_blank">View</a>` : ""
        },
        {
            headerName: "Actions",
            colId: "actions",
            width: 160,
            sortable: false,
            filter: false,
            cellRenderer: () => `
        <button class="btn btn-sm btn-primary mr-1" data-action="edit">Edit</button>
        <button class="btn btn-sm btn-danger" data-action="delete">Delete</button>
      `
        }
    ];

    const gridOptions = {
        columnDefs,
        rowData: [],
        animateRows: true,
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: { sortable: true, filter: true, resizable: true, floatingFilter: true },
        onGridReady: params => {
            params.api.sizeColumnsToFit();
            loadStudents();
        },
        onCellClicked: params => {
            if (params.column.getColId() !== "actions") return;
            const action = params.event.target && params.event.target.getAttribute("data-action");
            if (action === "edit") openEditModal(params.data);
            if (action === "delete") confirmDelete(params.data);
        }
    };

    $(function () {
        // Init grid
        const gridDiv = document.querySelector("#StudentsGrid");
        new agGrid.Grid(gridDiv, gridOptions);

        // Form + modal
        $("#studentForm").parsley();

        $("#btnOpenAdd").on("click", openAddModal);
        $("#btnRefresh").on("click", loadStudents);

        $("#btnFindById").on("click", function () {
            const id = parseInt($("#txtFindId").val(), 10);
            if (!id) {
                Swal.fire("Info", "Enter a valid Student ID.", "info");
                return;
            }
            getStudentById(id)
                .then(dto => {
                    if (!dto) { Swal.fire("Not found", "No student with that ID.", "warning"); return; }
                    openEditModal(dto);
                })
                .catch(err => {
                    console.error(err);
                    Swal.fire("Error", "Failed to fetch student.", "error");
                });
        });

        $("#studentForm").on("submit", function (e) {
            e.preventDefault();
            const $form = $(this);
            if (!$form.parsley().isValid()) return;

            const model = collectFormData();
            const isEdit = !!model.studentId && model.studentId > 0;

            const op = isEdit ? updateStudent(model) : addStudent(model);
            op.then(() => {
                Swal.fire(isEdit ? "Updated" : "Added", "Success.", "success");
                $("#studentModal").modal("hide");
                resetForm();
                loadStudents();
            }).catch(err => {
                console.error(err);
                Swal.fire("Error", (err && err.error) || "Operation failed.", "error");
            });
        });
    });

    // CRUD calls
    function loadStudents() {
        return $.ajax({ url: "/Student/GetStudentList", method: "GET" })
            .done(list => gridOptions.api.setRowData(Array.isArray(list) ? list : []))
            .fail(err => {
                console.error("GetStudentList failed", err);
                gridOptions.api.setRowData([]);
                Swal.fire("Error", "Failed to load students.", "error");
            });
    }

    function getStudentById(studentId) {
        return $.ajax({ url: "/Student/GetStudentById", method: "GET", data: { studentId } });
    }

    function addStudent(dto) {
        return $.ajax({
            url: "/Student/AddStudent",
            method: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dto)
        }).then(res => {
            if (!res || res.success !== true) return $.Deferred().reject(res || { error: "Add failed" }).promise();
            return res;
        });
    }

    function updateStudent(dto) {
        return $.ajax({
            url: "/Student/UpdateStudent",
            method: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dto)
        }).then(res => {
            if (!res || res.success !== true) return $.Deferred().reject(res || { error: "Update failed" }).promise();
            return res;
        });
    }

    function deleteStudentById(studentId) {
        // Controller expects a raw JSON number in body: [FromBody] int studentId
        return $.ajax({
            url: "/Student/DeleteStudentById",
            method: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(studentId)
        }).then(res => {
            if (!res || res.success !== true) return $.Deferred().reject(res || { error: "Delete failed" }).promise();
            return res;
        });
    }

    // UI helpers
    function openAddModal() {
        resetForm();
        $("#studentModalLabel").text("Add Student");
        $("#studentModal").modal({ backdrop: "static", keyboard: false });
    }

    function openEditModal(data) {
        resetForm();
        $("#studentModalLabel").text("Edit Student");
        $("#studentId").val(data.studentId || "");
        $("#admissionNo").val(data.admissionNo || "");
        $("#firstName").val(data.firstName || "");
        $("#lastName").val(data.lastName || "");
        $("#className").val(data.className || "");
        $("#section").val(data.section || "");
        $("#gender").val(data.gender || "");
        $("#email").val(data.email || "");
        $("#phone").val(data.phone || "");
        $("#address").val(data.address || "");
        $("#guardianName").val(data.guardianName || "");
        $("#healthInfo").val(data.healthInfo || "");
        $("#photoUrl").val(data.photoUrl || "");
        $("#studentModal").modal({ backdrop: "static", keyboard: false });
    }

    function confirmDelete(row) {
        Swal.fire({
            title: "Are you sure?",
            text: "This will permanently delete the student.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete",
            cancelButtonText: "Cancel"
        }).then(result => {
            if (!result.isConfirmed) return;
            deleteStudentById(row.studentId)
                .then(() => {
                    gridOptions.api.applyTransaction({ remove: [row] });
                    Swal.fire("Deleted", "Student deleted successfully.", "success");
                })
                .catch(err => {
                    console.error(err);
                    Swal.fire("Error", (err && err.error) || "Delete failed.", "error");
                });
        });
    }

    function collectFormData() {
        return {
            studentId: parseInt($("#studentId").val() || "0", 10) || 0,
            admissionNo: $("#admissionNo").val(),
            firstName: $("#firstName").val(),
            lastName: $("#lastName").val(),
            className: $("#className").val(),
            section: $("#section").val(),
            gender: $("#gender").val(),
            email: $("#email").val(),
            phone: $("#phone").val(),
            address: $("#address").val(),
            guardianName: $("#guardianName").val(),
            healthInfo: $("#healthInfo").val(),
            photoUrl: $("#photoUrl").val()
        };
    }

    function resetForm() {
        const $form = $("#studentForm");
        $form[0].reset();
        $form.parsley().reset();
        $("#studentId").val("");
    }
})();