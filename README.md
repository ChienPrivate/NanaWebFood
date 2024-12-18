
# NanaWebFood

NanaWebFood là một ứng dụng web quản lý nhà hàng, cho phép người dùng đặt món ăn trực tuyến và quản lý đơn hàng một cách hiệu quả.

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Tính năng](#tính-năng)
- [Cài đặt](#cài-đặt)
- [Sử dụng](#sử-dụng)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Giấy phép](#giấy-phép)

## Giới thiệu

NanaWebFood được phát triển nhằm cung cấp giải pháp quản lý bán thức ăn trực tuyến, giúp khách hàng dễ dàng đặt món và nhà hàng quản lý đơn hàng hiệu quả.

## Tính năng

- Đặt món ăn trực tuyến
- Quản lý thực đơn
- Quản lý đơn hàng
- Hỗ trợ thanh toán trực tuyến
- Gửi thông báo trạng thái đơn hàng

## Cài đặt

1. **Clone repository:**

   ```bash
   git clone https://github.com/ChienPrivate/NanaWebFood.git
   cd NanaWebFood
   ```

2. **Cấu hình cơ sở dữ liệu:**

   - Tạo cơ sở dữ liệu trong SQL Server.
   - Cập nhật chuỗi kết nối trong `appsettings.json`.

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }
   ```

4. **Áp dụng các di trú (migrations):**

   ```bash
   dotnet ef database update
   ```

5. **Chạy ứng dụng:**

   ```bash
   dotnet run
   ```

   Ứng dụng sẽ chạy tại `https://localhost:51326`.

## Sử dụng

- Truy cập `https://localhost:51326` để sử dụng ứng dụng.
- Đăng ký tài khoản hoặc đăng nhập để bắt đầu đặt món.

## Cấu trúc thư mục

```plaintext
NanaWebFood/
├── NanaFoodProject/
│   ├── NanaFoodWeb/        # Frontend của ứng dụng
│   ├── NanaFoodAPI/        # API backend
│   ├── NanaFood.Data/      # Lớp truy cập dữ liệu
│   ├── NanaFood.Services/  # Lớp dịch vụ
│   └── NanaFood.Models/    # Các mô hình dữ liệu
├── NanaFood - Test.xlsx    # Tệp kiểm thử
├── ProjectManagement.xlsx  # Tài liệu quản lý dự án
└── README.md               # Tệp hướng dẫn
```

## Giấy phép

Dự án này được cấp phép theo [MIT License](LICENSE).
