using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System.Runtime.CompilerServices;

namespace NanaFoodDAL.IRepository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        public OrderRepository(ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        public async Task<Order> AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public void AddOrderDetails(IEnumerable<OrderDetails> listOrderdetails)
        {
            _context.OrderDetails.AddRange(listOrderdetails);
            _context.SaveChanges();
        }

        public async Task<ResponseDto> CalculateProfitAsync()
        {
            var profit = await _context.OrderDetails.SumAsync(x => x.Total);
            _response.IsSuccess = true;
            _response.Result = profit;
            _response.Message = "trả về kết quả thành công";

            return _response;
        }

        public async Task<ResponseDto> CancelOrderAsync(int OrderId, string message)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(Order => Order.OrderId == OrderId);
            try
            {
                order.CancelReason = message;
                order.PaymentStatus = "Đã huỷ";
                order.OrderStatus = "Đã huỷ";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();


                _response.IsSuccess = true;
                _response.Message = "Hủy đơn thành công";
                _response.Result = order;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = order;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public async Task<ResponseDto> GetAllOrderAync()
        {
            var orders = _mapper.Map<List<OrderDto>>(await _context.Orders.ToListAsync());

            _response.IsSuccess = true;
            _response.Result = orders;
            _response.Message = "Lấy danh sách thành công";

            return _response;
        }

        public async Task<ResponseDto> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Hoá đơn này không tồn tại";
                    return _response;
                }
                _response.Result = _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetOrderDetailsAsync(int OrderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(x => x.OrderId == OrderId).ToListAsync();
                List<OrderDetailsDto> ListDetail = _mapper.Map<List<OrderDetailsDto>>(orderDetails);

                _response.IsSuccess = true;
                _response.Result = ListDetail;
                _response.Message = "Lấy danh sách sản phẩm nằm trong đơn hàng thành công";
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                _response.Result = new List<OrderDetailsDto>();
            }

            return _response;
        }

        public async Task<ResponseDto> GetUserOrderIdAsync(string UserId)
        {
            try
            {
                var orders = await _context.Orders.Where(o => o.UserId == UserId).ToListAsync(); // Convert to List
                if (orders.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Hoá đơn này không tồn tại";
                    _response.Result = orders;
                    return _response;
                }
                _response.Result = _mapper.Map<List<OrderDto>>(orders); // Map the list
                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách hóa đơn thành công";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> UpdateOrderStatus(int OrderId, string message)
        {
            var order = _context.Orders.SingleOrDefault(Order => Order.OrderId == OrderId);
            order.OrderStatus = message;
            if (message == "Đã giao")
            {
                order.PaymentStatus = "Đã thanh toán";
            }
            else if(message == "Đang chuẩn bị")
            {
                order.OrderStatus = "Đang giao";
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _response.IsSuccess = true;
            _response.Result = order.PaymentStatus;
            _response.Message = "Cập nhật hóa đơn thành công";


            return _response;
        }

        public async Task<ResponseDto> GetRebuyOrder(int orderId)
        {
            var rebuyOrders = await (from od in _context.OrderDetails
                                     join p in _context.Products on od.ProductId equals p.ProductId into productGroup
                                     from p in productGroup.DefaultIfEmpty() // Left join
                                     where od.OrderId == orderId
                                     select new RebuyOrderDto
                                     {
                                         ProductId = od.ProductId,
                                         OrderId = od.OrderId,
                                         ProductName = p != null ? p.ProductName : od.ProductName, // Dùng tên sản phẩm từ OrderDetails nếu Product bị xóa
                                         ProductImage = p != null ? p.ImageUrl : od.ImageUrl,     // Dùng hình ảnh từ OrderDetails nếu Product bị xóa
                                         CurrentPrice = p != null ? p.Price : 0,                  // Giá hiện tại là 0 nếu Product bị xóa
                                         OldPrice = od.Price,
                                         Quantity = od.Quantity,
                                         Total = od.Total,
                                         IsActive = p != null && p.IsActive // Gán false nếu Product không tồn tại
                                     }).ToListAsync();

            _response.Result = rebuyOrders;
            _response.IsSuccess = true;
            _response.Message = rebuyOrders.Any() ? "Lấy danh sách đơn hàng thành công" : "không tìm thấy sản phẩm nào";

            return _response;
        }

        public async Task<ResponseDto> RebuyOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Đơn hàng không tồn tại";
                return _response;
            }

            var userId = order.UserId;

            // Lấy danh sách sản phẩm từ phương thức GetRebuyOrder
            var orderItemList = await GetRebuyOrder(orderId);
            var itemList = ((List<RebuyOrderDto>)orderItemList.Result)
                           .Where(item => item.IsActive) // Lọc các sản phẩm có IsActive = true
                           .ToList();

            // Kiểm tra nếu itemList rỗng
            if (!itemList.Any())
            {
                _response.IsSuccess = false;
                _response.Message = "Không có sản phẩm nào có thể mua lại.";
                return _response;
            }

            // Duyệt qua từng sản phẩm và kiểm tra trong giỏ hàng
            foreach (var item in itemList)
            {
                var existingCartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.UserId == userId && cd.ProductId == item.ProductId);

                if (existingCartDetail != null)
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng Quantity
                    existingCartDetail.Quantity += item.Quantity;

                    // Kiểm tra nếu Quantity vượt quá 10, gán giá trị tối đa là 10
                    if (existingCartDetail.Quantity > 10)
                    {
                        existingCartDetail.Quantity = 10;
                    }

                    existingCartDetail.Total = existingCartDetail.Quantity * item.CurrentPrice;
                }
                else
                {
                    // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới vào CartDetails
                    var newCartDetail = new CartDetails
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Total = item.Quantity * item.CurrentPrice
                    };
                    _context.CartDetails.Add(newCartDetail);
                }
            }

            // Lưu các thay đổi vào database
            await _context.SaveChangesAsync();

            _response.Result = itemList;
            _response.IsSuccess = true;
            _response.Message = "Mua thành công";

            return _response;
        }

        public async Task<ResponseDto> UpdateProductQuantity(int orderId, int state)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy đơn hàng";
                    _response.Result = order;
                    return _response;
                }

                var orderDetails = await _context.OrderDetails.Where(o => o.OrderId == orderId).ToListAsync();

                int quantityModifier = state == 1 ? -1 : state == -1 ? 1 : 0;

                if (quantityModifier != 0)
                {
                    foreach (var item in orderDetails)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                        if (product != null)
                        {
                            if (state == 1 && product.Quantity < item.Quantity)
                            {
                                _context.Orders.Remove(order);
                                await _context.SaveChangesAsync();

                                _response.IsSuccess = false;
                                _response.Message = "Sản phẩm " + product.ProductName + " đã hết hàng";
                                return _response;
                            }

                            product.Quantity += item.Quantity * quantityModifier;

                            if (product.Quantity < 0)
                            {
                                product.Quantity = 0;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Cập nhật số lượng sản phẩm thành công";
                    _response.Result = orderDetails;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Trạng thái không hợp lệ";
                    _response.Result = orderDetails;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public async Task<ResponseDto> ApplyCoupon(int orderId, string couponCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(or => or.OrderId == orderId);

            if (order != null)
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(cou => cou.CouponCode == couponCode);
                
                if (coupon != null)
                {

                    order.CouponCode = coupon.CouponCode;
                    order.Discount = coupon.Discount;
                    order.MinAmount = coupon.MinAmount;

                    _context.Orders.Update(order);

                    coupon.MaxUsage -= 1;
                    coupon.TimesUsed += 1;

                    _context.Coupons.Update(coupon);

                    UserCoupon userCoupon = new UserCoupon() 
                    {
                        UserId = order.UserId,
                        CouponCode = coupon.CouponCode,
                        AppliedAt = DateTime.Now,
                    };

                    await _context.UserCoupons.AddAsync(userCoupon);

                    await _context.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = $"Áp dụng mã giảm giá thành công cho đơn hàng {orderId}";
                    _response.Result = order;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy mã giảm giá";
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Không tìm thấy đơn hàng";
            }
            return _response;
        }

    }
}
