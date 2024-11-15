$(document).ready(function () {
    const defaultProvinceId = 202;

    // Hàm reset các dropdown
    function resetDropdowns() {
        $('#DistrictList').empty().append('<option value=""> Hãy chọn quận/huyện </option>');
        $('#WardList').empty().append('<option value=""> Hãy chọn phường/xã </option>');
        $('#ServiceList').empty().append('<option value=""> Hãy chọn dịch vụ </option>');
    }

    // Hàm gọi AJAX lấy quận/huyện theo provinceId
    function fetchDistricts(provinceId) {
        return $.ajax({
            url: '/Order/GetDistricts',
            type: 'GET',
            data: { provinceId: provinceId }
        });
    }

    // Hàm gọi AJAX lấy phường/xã theo districtId
    function fetchWards(districtId) {
        return $.ajax({
            url: '/Order/GetWards',
            type: 'GET',
            data: { districtId: districtId }
        });
    }

    // Hàm gọi AJAX lấy dịch vụ theo từDistrict và toDistrict
    function fetchAvailableService(fromDistrict, toDistrict) {
        return $.ajax({
            url: '/Order/GetAvailableService',
            type: 'GET',
            data: { fromDistrict: fromDistrict, toDistrict: toDistrict }
        });
    }

    // Hàm tính phí ship
    function calculateShippingFee(serviceId, toDistrictId, wardCode) {
        return $.ajax({
            url: '/Order/CalculateShippingFee',
            type: 'POST',
            data: { serviceId, toDistrictId, wardCode }
        });
    }

    // Hàm tính thời gian giao hàng dự kiến
    function calculateShippingTime(serviceId, toDistrict, toWardCode) {
        return $.ajax({
            url: '/Order/CalculateShippingTime',
            type: 'POST',
            data: { serviceId, toDistrict, toWardCode }
        });
    }

    // Gọi loadDistricts khi trang tải với tỉnh mặc định
    fetchDistricts(defaultProvinceId).then(data => {
        $('#DistrictList').empty().append('<option value=""> Hãy chọn quận/huyện </option>');
        $.each(data, function (i, district) {
            $('#DistrictList').append(`<option value="${district.value}">${district.text}</option>`);
        });
    }).catch(() => alert("Không thể lấy danh sách quận/huyện"));

    // Sự kiện khi thay đổi tỉnh/thành phố
    $('#ProvinceList').change(function () {
        const provinceId = $(this).val();
        if (provinceId) {
            resetDropdowns();
            fetchDistricts(provinceId).then(data => {
                $('#DistrictList').empty().append('<option value=""> Hãy chọn quận/huyện </option>');
                $.each(data, function (i, district) {
                    $('#DistrictList').append(`<option value="${district.value}">${district.text}</option>`);
                });
            }).catch(() => alert("Không thể lấy danh sách quận/huyện"));
        } else {
            resetDropdowns();
        }
    });

    // Sự kiện khi thay đổi quận/huyện
    $('#DistrictList').change(function () {
        const districtId = $(this).val();
        $('#shippingFee').text('0 VNĐ');
        $('#shipmentFeeInput').val(0);
        if (districtId) {
            resetDropdowns();
            fetchWards(districtId).then(data => {
                $('#WardList').empty().append('<option value=""> Hãy chọn phường/xã </option>');
                $.each(data, function (i, ward) {
                    $('#WardList').append(`<option value="${ward.value}">${ward.text}</option>`);
                });
            }).catch(() => alert("Không thể lấy danh sách phường/xã"));

            fetchAvailableService(1454, districtId).then(data => {
                $('#ServiceList').empty().append('<option value=""> Hãy chọn dịch vụ </option>');
                $.each(data, function (i, service) {
                    $('#ServiceList').append(`<option value="${service.value}">${service.text}</option>`);
                });
            }).catch(() => alert("Không thể lấy danh sách dịch vụ"));
        }
    });

    // Sự kiện khi thay đổi phường/xã hoặc dịch vụ để tính phí ship và thời gian giao hàng
    $('#WardList, #ServiceList').change(function () {
        const wardCode = $('#WardList').val();
        const toDistrictId = $('#DistrictList').val();
        const serviceId = $('#ServiceList').val();
        if (wardCode && toDistrictId && serviceId) {
            calculateShippingFee(serviceId, toDistrictId, wardCode).then(response => {
                const shippingFee = Math.round(response.data / 1000) * 1000;
                $('#shippingFee').text(`${shippingFee.toLocaleString()} VNĐ`);
            }).catch(() => alert("Không thể tính phí ship"));

            calculateShippingTime(serviceId, toDistrictId, wardCode).then(response => {
                $('#estimatedDeliveryTime').text(response.formattedLeadTime);
                $('#estimatedDeliveryTimeInput').val(response.formattedLeadTime);
            }).catch(() => alert("Không thể lấy thời gian giao hàng dự kiến."));
        }
    });

    // Hàm hiển thị thông báo
    function showNotification(message) {
        $('#notification .modal-body').text(message);
        $('#notification').modal('show');
    }
});
