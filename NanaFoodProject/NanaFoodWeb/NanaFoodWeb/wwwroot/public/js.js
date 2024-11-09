// SIDEBAR DROPDOWN
const allDropdown = document.querySelectorAll('#sidebar .side-dropdown');
const sidebar = document.getElementById('sidebar');

allDropdown.forEach(item => {
	const a = item.parentElement.querySelector('a:first-child');
	a.addEventListener('click', function (e) {
		e.preventDefault();

		if (!this.classList.contains('active')) {
			allDropdown.forEach(i => {
				const aLink = i.parentElement.querySelector('a:first-child');

				aLink.classList.remove('active');
				i.classList.remove('show');
			})
		}

		this.classList.toggle('active');
		item.classList.toggle('show');
	})
})





// SIDEBAR COLLAPSE
const toggleSidebar = document.querySelector('nav .toggle-sidebar');
const allSideDivider = document.querySelectorAll('#sidebar .divider');

if (sidebar.classList.contains('hide')) {
	allSideDivider.forEach(item => {
		item.textContent = '-'
	})
	allDropdown.forEach(item => {
		const a = item.parentElement.querySelector('a:first-child');
		a.classList.remove('active');
		item.classList.remove('show');
	})
} else {
	allSideDivider.forEach(item => {
		item.textContent = item.dataset.text;
	})
}

toggleSidebar.addEventListener('click', function () {
	sidebar.classList.toggle('hide');

	if (sidebar.classList.contains('hide')) {
		allSideDivider.forEach(item => {
			item.textContent = '-'
		})

		allDropdown.forEach(item => {
			const a = item.parentElement.querySelector('a:first-child');
			a.classList.remove('active');
			item.classList.remove('show');
		})
	} else {
		allSideDivider.forEach(item => {
			item.textContent = item.dataset.text;
		})
	}
})









var options = {
	series: [{
		name: 'series1',
		data: [31, 40, 28, 51, 42, 109, 100]
	}, {
		name: 'series2',
		data: [11, 32, 45, 32, 34, 52, 41]
	}],
	chart: {
		height: 350,
		type: 'area'
	},
	dataLabels: {
		enabled: false
	},
	stroke: {
		curve: 'smooth'
	},
	xaxis: {
		type: 'category',
		categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"]
	},
	tooltip: {
		x: {
			format: 'dd/MM/yy HH:mm'
		},
	},
};

var chart = new ApexCharts(document.querySelector("#chart1"), options);
chart.render();

// Lắng nghe sự kiện click trên nút button
document.getElementById('deleteButton').addEventListener('click', function () {
	// Hiển thị SweetAlert để hỏi người dùng có muốn xóa không
	Swal.fire({
		title: 'Bạn có chắc chắn muốn xóa?',
		text: "Hành động này không thể hoàn tác!",
		icon: 'warning',
		showCancelButton: true,
		confirmButtonColor: '#3085d6',
		cancelButtonColor: '#d33',
		confirmButtonText: 'Có, xóa nó!',
		cancelButtonText: 'Hủy bỏ'
	}).then((result) => {
		if (result.isConfirmed) {
			// Hành động xóa được thực hiện ở đây
			Swal.fire(
				'Đã xóa!',
				'Dữ liệu của bạn đã bị xóa.',
				'success'
			)
		}
	})
});

