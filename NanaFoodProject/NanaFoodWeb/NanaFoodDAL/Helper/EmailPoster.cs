using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NanaFoodDAL.Model;
using NanaFoodDAL.Dto.UserDTO;

namespace NanaFoodDAL.Helper
{
    public class EmailPoster
    {
        public string SendMail(string to, string subject, string body)
        {
            string from = "chienprivate@gmail.com";
            string pass = "bchw xhsj ovtx qqze"; 
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(from);
            msg.To.Add(to);
            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.EnableSsl = true;
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(from, pass);
            try
            {
                client.Send(msg);
                return "gửi mail thành công";
            }
            catch (Exception ex)
            {
                return "Gửi mail thất bại \n" + ex.Message;
            }
        }

        public string EmailConfirmTemplate(string name,string content)
        {
            return $@"
<!DOCTYPE html>
<html>
  <head>
    <meta charset=""utf-8"" />
    <style>
      body {{
        font-family: Arial, sans-serif;
        margin: 0;
        padding: 0;
        background-color: #f4f4f4;
      }}
      .container {{
        background-color: #ffffff;
        width: 80%;
        max-width: 6000px;
        margin: 20px auto;
        padding: 20px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
      }}

      .header {{
        background-color: #f7d200;
        padding: 10px 20px;
        text-align: center;
        font-size: 24px;
        font-weight: bold;
      }}

      .content {{
        padding: 20px;
        text-align: center;
        line-height: 1.6;
      }}

      .footer {{
        background-color: #eeeeee;
        padding: 10px 20px;
        text-align: center;
        font-size: 12px;
      }}

      .confirmation-code {{
        text-align: left;
        font-size: 15px;
        color: #333;
        margin: 20px 0;
        padding: 10px;
        min-width: 200px;
        display: inline-block;
      }}
      .button {{
        display: inline-block;
        padding: 12px 25px;
        background-color: #f7d200;
        color: #141414;
        text-decoration: none;
        border-radius: 5px;
        font-weight: bold;
        transition: background-color 0.3s ease;
        display: flex;
        justify-content: center;
      }}

      .button:hover {{
        background-color: #d8ca09;
      }}
    </style>
  </head>

  <body>
    <div class=""container"">
      <div class=""header"">Chào mừng đến với NanaFood</div>
      <div class=""content"">
        <i>Xin chào </i><strong>{name}</strong>
        <i style=""margin: 0; display: block"">
          Cảm ơn bạn đã đăng ký tài khoản với chúng tôi. Để hoàn tất quá trình
          đăng ký, vui lòng nhấp vào liên kết xác nhận bên dưới
        </i>
        <div class=""confirmation-code"">
          <a href=""{content}"" class=""button"">Xác nhận email</a>
        </div>
      </div>
      <div class=""footer"">
        Đây là email tự động, vui lòng không trả lời. Nếu bạn cần hỗ trợ, xin
        vui lòng liên hệ sử dụng chức năng hỗ trợ trong ứng dụng.
      </div>
    </div>
  </body>
</html>

";
        }

        public string EmailForgorPasswordTemplate(string password)
        {
            return $@"<!DOCTYPE html>
<html lang=""en"">

<head>
    <!-- Link Google Fonts -->
    <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css2?family=Pacifico&display=swap"">

    <style>
        .container {{
            width: 100%;
            max-width: 600px;
            margin: auto;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(54, 134, 148, 0.1);
        }}

        .text-center {{
            text-align: center;
        }}

        .text-white {{
            color: white;
        }}

        .text-start {{
            text-align: start;
        }}

        .bg-white {{
            background-color: white;
        }}

        .fs-1 {{
            font-size: large;
        }}

        /* Class riêng cho gradient background */
        .bg-gradient {{
            background: linear-gradient(45deg, #f7b733, #fc4a1a);
            /* Gradient từ vàng sang cam */
        }}

        .content-container {{
            border-radius: 15px;
        }}

        /* Chữ ký cuối email */
        .signature {{
            display: flex;
            align-items: center;
            margin-top: 20px;
            font-size: 14px;
            line-height: 1.5;
            color: #333;
        }}

        .signature img {{
            width: 100px;
            /* Kích thước ảnh */
            margin-right: 20px;
            /* Khoảng cách giữa ảnh và thông tin */
            border-radius: 10px;
            /* Bo góc ảnh */
        }}

        .signature-info {{
            flex-grow: 1;
        }}

        /* Font chữ mềm mại cho tiêu đề Nana Food */
        .soft-font {{
            font-family: 'Pacifico', cursive;
        }}
    </style>
</head>

<body>
    <div class=""container bg-white"">
        <ul class=""bg-white"">
            <h2 class=""text-start"">Mật khẩu của bạn đã được làm mới!</h2>
            <p class=""text-start"">Hãy đổi mật khẩu trong
                lần đăng nhập tiếp theo.</p>
            <p class=""text-start"">Cảm ơn bạn đã sử dụng dịch vụ của cửa hàng chúng tôi.</p>
        </ul>
    </div>
    <div class=""container bg-gradient""> <!-- Áp dụng class bg-gradient -->
        <div class=""text-start"">
            <div>
                <h1 class=""text-white soft-font"">Nana Food</h1>
            </div>
            <hr>
        </div>
        <div class=""text-center bg-white fs-1 content-container"">
            <h2>{password}</h2>
        </div>
    </div>

    <!-- Chữ ký của cửa hàng -->
    <div class=""container bg-white signature"">
        <!-- Thêm hình ảnh vào bên trái -->
        <img src=""https://png.pngtree.com/png-clipart/20220131/original/pngtree-pizza-shop-selling-different-pizza-pages-there-is-an-eating-service-png-image_7256996.png""
            alt=""Pizza Shop Image"">
        <!-- Phần thông tin bên phải -->
        <div class=""signature-info"">
            <p><strong>Địa chỉ:</strong> QTSC 9 Building, Đ. Tô Ký, Tân Chánh Hiệp, Quận 12, Hồ Chí Minh, Việt Nam</p>
            <p><strong>Email:</strong> nanafoodStore@gmail.com</p>
            <p><strong>Số điện thoại:</strong> 84+ 123456789</p>
        </div>
    </div>

</body>

</html>";
        }

        public string OrderEmailTemplate(Order order, List<OrderDetails> orderDetailsList)
        {
            string orderItemsHtml = "";
            string receiveDateHtml = order.ReceiveDate == DateTime.MinValue
        ? "<p><strong>Ngày nhận hàng:</strong> Chưa nhận hàng</p>"
        : $"<p><strong>Ngày nhận hàng:</strong> {order.ReceiveDate.ToString("dd/MM/yyyy HH:mm:ss tt")}</p>";

            string headerColor = order.OrderStatus == "Đã huỷ" ? "#d9534f" : "#4CAF50";
            // Kiểm tra nếu có lý do hủy để hiển thị, nếu không thì bỏ qua dòng này
            string cancelReasonHtml = string.IsNullOrEmpty(order.CancelReason)
                ? ""
                : $"<p><strong>Lý do hủy:</strong> {order.CancelReason}</p>";



            // Duyệt qua từng OrderDetails để tạo hàng bảng cho từng sản phẩm
            foreach (var item in orderDetailsList)
            {
                orderItemsHtml += $@"
            <tr>
                <td>{item.ProductName}</td>
                <td><img src='{item.ImageUrl}' alt='{item.ProductName}' style='width: 50px; height: 50px;' /></td>
                <td>{item.Quantity}</td>
                <td>{item.Price.ToString("#,##")} VNĐ</td>
                <td>{item.Total.ToString("#,##")} VNĐ</td>
            </tr>";
            }

            // Template HTML cho email
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
            .container {{
        width: 100%;
        max-width: 600px;
        margin: auto;
        padding: 20px;
        font-family: Arial, sans-serif;
    }}
    
    /* Header có màu nền động và chữ màu trắng */
    .header {{
        background-color: {headerColor}; /* Màu nền thay đổi tùy thuộc vào trạng thái đơn hàng */
        color: white;
        padding: 10px;
        text-align: center;
    }}
    
    .order-info {{
        margin-top: 20px;
        font-size: 14px;
    }}
    
    .order-details table {{
        width: 100%;
        border-collapse: collapse;
        margin-top: 20px;
    }}
    
    /* Cập nhật màu nền và màu chữ trắng cho thẻ <th> */
    .order-details th {{
        background-color: #f7d200; /* Màu vàng cho tiêu đề bảng */
        color: white; /* Màu chữ trắng cho các thẻ th */
        padding: 8px;
        text-align: left;
    }}
    
    .order-details td {{
        border: 1px solid #ddd;
        padding: 8px;
        text-align: left;
    }}
    
    .footer {{
        margin-top: 20px;
        font-size: 12px;
        color: #555;
        text-align: center;
    }}
    /* Chữ ký cuối email */
        .signature {{
            display: flex;
            align-items: center;
            margin-top: 20px;
            font-size: 14px;
            line-height: 1.5;
            color: #333;
        }}

        .signature img {{
            width: 100px;
            /* Kích thước ảnh */
            margin-right: 20px;
            /* Khoảng cách giữa ảnh và thông tin */
            border-radius: 10px;
            /* Bo góc ảnh */
        }}

        .signature-info {{
            flex-grow: 1;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Chi tiết đơn hàng #{order.OrderId}</h2>
        </div>
        <div class='order-info'>
            <p><strong>Họ tên:</strong> {order.FullName}</p>
            <p><strong>Số điện thoại:</strong> {order.PhoneNumber}</p>
            <p><strong>Email:</strong> {order.Email}</p>
            <p><strong>Địa chỉ:</strong> {order.Address}</p>
            <p><strong>Ngày đặt hàng:</strong> {order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss tt")}</p>
            <p><strong>Thời gian giao hàng dự kiến:</strong> {order.ExpectedDeliveryDate}</p>
            {receiveDateHtml}
            <p><strong>Phương thức thanh toán:</strong> {order.PaymentType}</p>
            <p><strong>Trạng thái thanh toán:</strong> {order.PaymentStatus}</p>
            <p><strong>Phí vận chuyển:</strong> {order.ShipmentFee.ToString("#,##")} VNĐ</p>
            <p><strong>Tổng cộng:</strong> {order.Total.ToString("#,##")} VNĐ</p>
            <p><strong>Ghi chú:</strong> {order.Note ?? "Không có"}</p>
            {cancelReasonHtml}
        </div>
        <div class='order-details'>
            <h3>Chi tiết sản phẩm</h3>
            <table>
                <tr>
                    <th>Sản phẩm</th>
                    <th>Hình ảnh</th>
                    <th>Số lượng</th>
                    <th>Giá</th>
                    <th>Tổng</th>
                </tr>
                {orderItemsHtml}
            </table>
        </div>
        <div class='footer'>
            <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của Nana. Chúng tôi rất vui được phục vụ bạn!</p>
            <p>Đây là email tự động, vui lòng không trả lời. Nếu có bất kỳ thắc mắc nào, hãy liên hệ với chúng tôi qua thông tin hỗ trợ.</p>
        </div>
    </div>
    <!-- Chữ ký của cửa hàng -->
    <div class=""container bg-white signature"">
        <!-- Thêm hình ảnh vào bên trái -->
        <img src=""https://png.pngtree.com/png-clipart/20220131/original/pngtree-pizza-shop-selling-different-pizza-pages-there-is-an-eating-service-png-image_7256996.png""
            alt=""Pizza Shop Image"">
        <!-- Phần thông tin bên phải -->
        <div class=""signature-info"">
            <p><strong>Địa chỉ:</strong> QTSC 9 Building, Đ. Tô Ký, Tân Chánh Hiệp, Quận 12, Hồ Chí Minh, Việt Nam</p>
            <p><strong>Email:</strong> nanafoodStore@gmail.com</p>
            <p><strong>Số điện thoại:</strong> 84+ 123456789</p>
        </div>
    </div>
</body>
</html>";
        }


        public string BlockUserEmailTemplate(UserDto user, string reason)
        {
            string reasonHtml = string.IsNullOrEmpty(reason)
                ? "Vi phạm các điều khoản sử dụng của chúng tôi."
                : reason;

            // Template HTML cho email thông báo người dùng bị chặn
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        .container {{
            width: 100%;
            max-width: 600px;
            margin: auto;
            padding: 20px;
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
        }}

        .header {{
            background-color: #d9534f; /* Màu đỏ cảnh báo */
            color: white;
            padding: 15px;
            text-align: center;
        }}

        .content {{
            margin-top: 20px;
            font-size: 14px;
            color: #333;
        }}

        .footer {{
            margin-top: 30px;
            font-size: 12px;
            color: #777;
            text-align: center;
        }}

        .button {{
            display: inline-block;
            background-color: #d9534f;
            color: white;
            padding: 10px 20px;
            margin-top: 20px;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Thông báo khóa tài khoản</h2>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{user.FullName}</strong>,</p>
            <p>Tài khoản của bạn (<strong>{user.UserName}</strong>) đã bị <strong>chặn</strong> do vi phạm các điều khoản sử dụng của chúng tôi.</p>
            <p><strong>Lý do:</strong> {reasonHtml}</p>
            <p>Nếu bạn cho rằng đây là một nhầm lẫn hoặc cần thêm thông tin, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
            <a href='mailto:nanafoodStore@gmail.com' class='button'>Liên hệ hỗ trợ</a>
        </div>
        <div class='footer'>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của Nana. Chúng tôi luôn sẵn sàng hỗ trợ bạn!</p>
            <p>Email: nanafoodStore@gmail.com | Số điện thoại: 84+ 123456789</p>
            <p>Đây là email tự động, vui lòng không trả lời trực tiếp.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
