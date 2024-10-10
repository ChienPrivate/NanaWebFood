// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// nút switch ES2
function onSwitchChange(args) {
    var badgeElement = $('#badgeStatus');
    if (args.checked) {  // Kiểm tra trạng thái bật/tắt của switch
        badgeElement.text('Hoạt động');
        badgeElement.removeClass('bg-secondary').addClass('bg-success');
    } else {
        badgeElement.text('Vô hiệu');
        badgeElement.removeClass('bg-success').addClass('bg-secondary');
    }
}

// ES2 kéo thả hình ảnh


