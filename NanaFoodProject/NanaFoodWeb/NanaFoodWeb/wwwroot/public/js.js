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


var options = {
	series: [
		{
			name: 'Q1 Budget',
			group: 'budget',
			data: [44000, 55000, 41000, 67000, 22000, 43000]
		},
		{
			name: 'Q1 Actual',
			group: 'actual',
			data: [48000, 50000, 40000, 65000, 25000, 40000]
		},
		{
			name: 'Q2 Budget',
			group: 'budget',
			data: [13000, 36000, 20000, 8000, 13000, 27000]
		},
		{
			name: 'Q2 Actual',
			group: 'actual',
			data: [20000, 40000, 25000, 10000, 12000, 28000]
		}
	],
	chart: {
		type: 'bar',
		height: 350,
		stacked: true,
	},
	stroke: {
		width: 1,
		colors: ['#fff']
	},
	dataLabels: {
		formatter: (val) => {
			return val / 1000 + 'K'
		}
	},
	plotOptions: {
		bar: {
			horizontal: false
		}
	},
	xaxis: {
		categories: [
			'Online advertising',
			'Sales Training',
			'Print advertising',
			'Catalogs',
			'Meetings',
			'Public relations'
		]
	},
	fill: {
		opacity: 1
	},
	colors: ['#80c7fd', '#008FFB', '#80f1cb', '#00E396'],
	yaxis: {
		labels: {
			formatter: (val) => {
				return val / 1000 + 'K'
			}
		}
	},
	legend: {
		position: 'top',
		horizontalAlign: 'left'
	}
};

var chart = new ApexCharts(document.querySelector("#chart"), options);
chart.render();







//nút xóa

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


//check
document.addEventListener("DOMContentLoaded", function () {
	const selectAllCheckbox = document.getElementById("selectAll");
	const rowCheckboxes = document.querySelectorAll(".rowCheckbox");

	selectAllCheckbox.addEventListener("change", function () {
		rowCheckboxes.forEach(checkbox => {
			checkbox.checked = selectAllCheckbox.checked;
		});
	});
});
function uploadFile() {
	const fileInput = document.createElement('input');
	fileInput.type = 'file';
	fileInput.accept = '.csv, .xls, .xlsx, .pdf'; // specify accepted file types

	fileInput.onchange = () => {
		const file = fileInput.files[0];
		if (file) {
			// Process file upload
			console.log('File uploaded:', file.name);
			// Add your upload logic here (e.g., AJAX request to server)
		}
	};

	fileInput.click(); // Open file dialog
}

function printData() {
	window.print(); // Open print dialog
}

function copyData() {
	const dataToCopy = "Your data to copy"; // Replace with your actual data
	navigator.clipboard.writeText(dataToCopy)
		.then(() => {
			alert("Data copied to clipboard!");
		})
		.catch(err => {
			console.error('Error copying data: ', err);
		});
}

function exportToExcel() {
	// Logic to convert data to Excel format and download
	console.log("Exporting to Excel...");
	// Use libraries like SheetJS (xlsx) to handle Excel export
}

function exportToPDF() {
	// Logic to convert data to PDF format and download
	console.log("Exporting to PDF...");
	// Use libraries like jsPDF to handle PDF export
}

function deleteAll() {
	const confirmation = confirm("Bạn có chắc chắn muốn xóa tất cả không?");
	if (confirmation) {
		// Logic to delete all data
		console.log("All data deleted.");
		// Add your deletion logic here
	}
}

