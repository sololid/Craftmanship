var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            {"data":"name", "width": "15%"},
            {"data":"description", "width": "15%"},
            {"data":"price", "width": "15%"},
            {"data":"category.name", "width": "15%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Product/Update?id=${data}"
                            class="btn btn-black mx-2"> <i class="bi bi-pencil-square"></i> &nbsp; Uppdatera</a>
                            <a onClick=Delete('/Admin/Product/Delete/${data}')
                            class="btn btn-danger mx-2"> <i class="bi bi-x-circle"></i> &nbsp; Radera</a>
                        </div>
                            `
                },
                "width": "15%"
            }
            
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Är det säkert att du vill radera produkten?',
        text: "Detta går inte att ångra!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Ja, radera!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}