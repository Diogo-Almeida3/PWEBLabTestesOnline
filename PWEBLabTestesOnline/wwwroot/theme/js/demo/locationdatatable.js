// Call the dataTables jQuery plugin
$(document).ready(function () {

    var table = $('#tableCalendar').DataTable({ "language": { "sSearch": "Location: ", searchPlaceholder: 'filter locations' } });

    $('.dataTables_filter input')
        .off()
        .on('keyup', function () {
            console.log('event')
            table.column(1).search(this.value).draw();
        });


});