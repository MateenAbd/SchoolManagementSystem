$(document).ready(function () {

    $('#addEmployeeModal').modal({
        backdrop: 'static',
        keyboard: false
    });

    // Handle close button manually
    $('#btnCloseModal').click(function () {
        $('#addEmployeeModal').modal('hide');
        window.$("#formAddEmployee").parsley().destroy();
    });

    // Initialize the grid


    initializeGrid();
    window.$("#formAddEmployee").parsley();
    
    fetchEmployeeList();
    $('#formAddEmployee').parsley();
    
    $('#formAddEmployee').submit(async function (event) {
        event.preventDefault();
        
        if ($('#formAddEmployee').parsley().isValid()) {
            await addEmployee();
            fetchEmployeeList(); 
            $('#addEmployeeModal').modal('hide'); 
        }
    });


    // Event handler for button click to get employee by ID
    $('#btn1').click(function () {
        getEmployeeById();
    });
});

// Column definitions for the grid
// Column definitions with filters
const columnDefs = [
    {
        headerName: '#',
        field: 'employeeId',
        filter: false,
        suppressSizeToFit: false,
        resizable: false,
        menuTabs: [],
    },
    {
        headerName: 'Name',
        field: 'name',
        filter: "agTextColumnFilter",
    },
    {
        headerName: 'Description',
        field: 'description',
        filter: "agTextColumnFilter",
    },
    {
        headerName: 'Phone Number',
        field: 'phoneNumber',
        filter: "agTextColumnFilter",
    },
    {
        headerName: 'Email',
        field: 'email',
        filter: "agTextColumnFilter",
    },
    {
        headerName: 'Address',
        field: 'address',
        filter: "agTextColumnFilter",
    }
];

// Grid options configuration
const gridOptions = {
    columnDefs: columnDefs,
    rowData: [],
    animateRows: true,
    pagination: true,

    // sets 10 rows per page (default is 100)
    paginationPageSize: 10,
    defaultColDef: {
        resizable: true,
        filter: true,
        sortable: true,
        floatingFilter: true,
        allowContextMenuWithControlKey: true,
        menuTabs: ['filterMenuTab', 'columnsMenuTab', 'generalMenuTab']
    },
    autoGroupColumnDef: {
        minWidth: 200,
    },
    sideBar: {
        toolPanels: ["columns"],
    },
    pivotPanelShow: "always",
    columnDefs: columnDefs,
    getContextMenuItems: function (params) {
        return [
            {
                name: 'Add',
                action: function () {
                    $('#formAddEmployee')[0].reset();
                    $('#addEmployeeModal').modal('show');
                },
                icon: '<img src="/images/icon_new_rate.png" alt="">',
            },
            {
                name: 'Delete',
                action: function () {
                    const row = params.node.data;

                    if (row.employeeId !== null) {
                        Swal.fire({
                            title: 'Are you sure?',
                            text: "Do you want to delete this employee?",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, delete it!',
                            cancelButtonText: 'No, cancel!',
                        }).then((result) => {
                            if (result.isConfirmed) {
                                deleteEmployee(row); // Ensure deleteEmployee function is defined
                                Swal.fire(
                                    'Deleted!',
                                    'The employee has been deleted.',
                                    'success'
                                );
                            } else {
                                Swal.fire(
                                    'Cancelled',
                                    'The employee was not deleted.',
                                    'error'
                                );
                            }
                        });
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Something went wrong!",
                            footer: '<a href="#">Why do I have this issue?</a>'
                        });
                    }
                },
                icon: '<img src="/images/icondelete.png" alt="">',
            }
        ];
    },
    onGridReady: function (params) {
        params.api.sizeColumnsToFit();
        fetchEmployeeList(); // Fetch data when grid is ready
    }
};

// Initialize the grid
function initializeGrid() {
    const gridDiv = document.querySelector('#EmployeesGrid');
    new agGrid.Grid(gridDiv, gridOptions);
}

// Function to fetch employee list and set it to the grid
function fetchEmployeeList() {
    $.ajax({
        url: '/Employee/GetEmployeeList',
        method: 'GET',
        success: function (data) {
            console.log('Data fetched from server:', data);
            if (Array.isArray(data)) {
                debugger;
                gridOptions.api.setRowData(data);
            } else {
                console.error('Data format is not correct:', data);
            }
        },
        error: function (error) {
            console.error('Error fetching employee data:', error);
        }
    });
}



// Function to add an employee
async function addEmployee() {
    const employee = {
        Name: $('#employeeName').val(),
        Description: $('#employeeDescription').val(),
        PhoneNumber: $('#employeePhoneNumber').val(),
        Email: $('#employeeEmail').val(),
        Address: $('#employeeAddress').val()
    };
    window.$("#formAddEmployee").parsley();
    try {
        await $.ajax({
            url: '/Employee/AddEmployee',
            method: 'POST',
            data: employee,
            success: function (response) {
                debugger;
                console.log('Employee added:', response); 
                Swal.fire({
                    title: "Sweet!",
                    text: "Employee Added Successfully.",
                    imageUrl: "https://unsplash.it/400/200",
                    imageWidth: 400,
                    imageHeight: 200,
                    imageAlt: "Custom image"
                });
            },
            error: function (error) {
                console.error('Error adding employee:', error);
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Something went wrong!",
                    footer: '<a href="#">Why do I have this issue?</a>'
                });
            }
        });
    } catch (error) {
        console.error('Error in addEmployee function:', error);
    }
}

// Function to delete selected rows
function deleteEmployee(row) {
    const id = row.employeeId;
    
    $.ajax({
        url: '/Employee/DeleteEmployeeById',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ ids: [id] }), 
        success: function () {
            gridOptions.api.applyTransaction({ remove: [row] });

            // Show success message
            Swal.fire({
                title: "Success!",
                text: "Employee deleted successfully.",
                icon: "success",
                confirmButtonText: "OK"
            });
        },
        error: function (error) {
            // Log and show error message
            console.error('Error deleting employee:', error);
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Something went wrong while deleting the employee.",
                footer: '<a href="#">Why do I have this issue?</a>'
            });
        }
    });
}



function GetEmployeeById() {

    const jsonData = { empIdentityCode: $('#txt1').val() };
    try {
        $.ajax({
            method: 'GET',
            url: '/Employee/GetEmployeeById',
            data: jsonData,
            success: function (response) {

                var id = response.employeeId;
                var name = response.name;

                swal(id + " ----------- " + name);

            }
        });
    }
    catch (e) {
        console.log(e);
    }

}

//function GetEmployeeList() {
//    try {
//        $.ajax({
//            method: 'GET',
//            url: '/Employee/GetEmployeeList',
//            data: null,
//            success: function (response) {
//                var result = '';
//                window.$(response).each(function (i, item) {
//                    result += ` <tr class="even">
//                                        <td class="table-user">
//                                            ${item.employeeId}
//                                        </td>
//                                        <td>
//                                            ${item.name}
//                                        </td>
//                                        <td>
//                                            ${item.phoneNumber}
//                                        </td>
//                                        <td>
//                                           ${item.description}
//                                        </td>
//                                         <td>
//                                           ${item.email}
//                                        </td>
//                                           <td>
//                                           ${item.address}
//                                        </td>
//                                        <td>
//                                            <a href="javascript:void(0);" class="action-icon"> <i class="mdi mdi-square-edit-outline"></i></a>
//                                            <a href="javascript:void(0);" onclick="DeleteEmployee(${item.employeeId})" class="action-icon"> <i class="mdi mdi-delete"></i></a>
//                                        </td>
//                                    </tr>`;
//                });
//                $('#employeeTableBody').empty().append(result);

//            }
//        });
//    }
//    catch (e) {
//        console.log(e);
//    }
   
//}

//function DeleteEmployee(employeeId) {
//    swal({
//        title: "Are you sure?",
//        text: "You want to delete this file?",
//        icon: "warning",
//        buttons: ["Cancel", "Yes"]
//    }).then(async function (isConfirm) {
//        if (isConfirm) {
//            try {
//                $.ajax({
//                    method: 'POST',
//                    url: '/Employee/DeleteEmployeeById',
//                    data: { employeeId: employeeId },
//                    success: function (response) {
//                        swal({
//                            title: "Deleted Successfully!",
//                            text: "id : " + response,
//                            icon: "success"
//                        }); 
//                        GetEmployeeList();
//                    }
//                });
//            }
//            catch (e) {
//                console.log(e);
//            }
//        }
//    });



//}


