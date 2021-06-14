console.log('admin')
$(document).ready(function () {
    $("#files").kendoUpload({
        select: onSelect
    });
});
var viewModel = kendo.observable({
    fileLists:[],
});
kendo.bind($("#example"), viewModel);
function onSelect(e) {
    viewModel.fileLists.push(e.files[0].rawFile);
}
$("#upload").click(function () {
    var formData = new FormData();
    // Check file selected or not
    for (var i = 0; i < viewModel.fileLists.length; i++) {
        formData.append('UploadedImage', viewModel.fileLists[i]);
    }
    formData.append('orderRequest', true);
    $.ajax({
        type: "POST",
        url: "/UploadFile",
        contentType: false,
        processData: false,
        data: formData,
        dataType: "json",
        success: function (response) {
            if (response != 0) {
                $(".k-upload-files.k-reset").find("li").remove();
                alert('file uploaded');
            } else {
                $(".k-upload-files.k-reset").find("li").remove();
                alert('file not uploaded');
            }
        },
    });
});




function getFileInfo(e) {
    return $.map(e.files, function (file) {
        var info = file.name;

        // File size is not available in all browsers
        if (file.size > 0) {
            info += " (" + Math.ceil(file.size / 1024) + " KB)";
        }
        return info;
    }).join(", ");
}