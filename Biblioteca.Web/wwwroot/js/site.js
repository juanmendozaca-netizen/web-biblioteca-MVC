// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initialize DataTables for any table with class 'datatable'
$(function(){
    try {
        $('.datatable').each(function(){
            var $tbl = $(this);
            var disableFilter = $tbl.hasClass('dt-no-search');
            $tbl.DataTable({
                responsive: true,
                pageLength: 10,
                lengthChange: false,
                dom: disableFilter ? 'lrtip' : 'lfrtip', // remove built-in filter when dt-no-search present
                language: { search: "Buscar:", paginate: { previous: "Anterior", next: "Siguiente" }, info: "Mostrando _START_ a _END_ de _TOTAL_ registros" }
            });
        });
    } catch (e) {
        console && console.warn('DataTables init failed', e);
    }
});

// Enable Bootstrap tooltips globally
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
});
