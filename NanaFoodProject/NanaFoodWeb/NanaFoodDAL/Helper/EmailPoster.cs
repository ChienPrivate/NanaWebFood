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
            <p><strong>Email:</strong> TrinhVanTruonga6a@gmail.com</p>
            <p><strong>Số điện thoại:</strong> 84+ 123456789</p>
        </div>
    </div>

</body>

</html>";
        }
    }
}
