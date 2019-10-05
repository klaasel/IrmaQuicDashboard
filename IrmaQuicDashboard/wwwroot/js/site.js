// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('#btnViewUploadSession').on('click', function (e) {
    var filter = {
        id: $('#dropdownSelectSession').val()
       
        };
     
    GetDashboard(filter)
});


function GetDashboard(filter) {
    $.ajax({
        url: '/Dashboard/LoadSession',
        type: 'POST',
        cache: false,
        async: true,
        dataType: "html",
        data : filter
    })
    .done(function (result) {
        $('#dashboard').html(result);
        }
    ).fail(function (xhr) {
        console.log('error : ' + xhr.status + ' - ' + xhr.statusText + ' - ' + xhr.responseText);
        });

}