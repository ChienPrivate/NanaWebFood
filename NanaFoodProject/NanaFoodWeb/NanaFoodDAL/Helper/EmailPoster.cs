using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Xác nhận email</title>
    <style>
      body {{
        font-family: ""Arial"", sans-serif;
        margin: 0;
        padding: 0;
        background-color: #f4f4f4;
      }}

      .card {{
        max-width: 600px;
        margin: 0 auto;
        padding: 20px;
        background-color: #fff;
        border-radius: 8px;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        text-align: center;
      }}

      .header {{
        font-size: 24px;
        color: #333;
        margin-bottom: 20px;
      }}

      .body {{
        margin-bottom: 20px;
      }}

      p {{
        line-height: 1.6;
        color: #555;
      }}

      .button {{
        display: inline-block;
        padding: 12px 25px;
        background-color: #007bff;
        color: #fff;
        text-decoration: none;
        border-radius: 5px;
        font-weight: bold;
        transition: background-color 0.3s ease;
      }}

      .button:hover {{
        background-color: #0056b3;
      }}

      .footer {{
        font-size: 0.8em;
        color: #888;
      }}
    </style>
  </head>
  <body>
    <div class=""card"">
      <div class=""header"">Chào mừng đến với <strong>NanaFood</strong></div>
      <div class=""body"">
        <p>Xin chào <strong>{name}</strong>,</p>
        <p>
          Cảm ơn bạn đã đăng ký tài khoản với chúng tôi. Để hoàn tất quá trình
          đăng ký, vui lòng nhấp vào liên kết xác nhận bên dưới
        </p>
        <a href=""{content}"" class=""button"">Xác nhận email</a>
      </div>
      <div class=""footer"">
        Nếu bạn không yêu cầu xác nhận email này, vui lòng bỏ qua email này.
      </div>
    </div>
  </body>
</html>
";
        }

        public string EmailForgorPasswordTemplate(string content)
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
            <h2>{content}</h2>
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
            <p><strong>Email:</strong> TrinhVanTruonga6a@gmail.com</p>
            <p><strong>Số điện thoại:</strong> 84+ 123456789</p>
        </div>
    </div>

</body>

</html>";
        }
    }
}
