﻿using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

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
            var orderDetails = await _context.OrderDetails.Where(x => x.OrderId == OrderId).ToListAsync();
            List<OrderDetailsDto> ListDetail = _mapper.Map<List<OrderDetailsDto>>(orderDetails);
            _response.Result = ListDetail;

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
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _response.IsSuccess = true;
            _response.Result = order.PaymentStatus;
            _response.Message = "Cập nhật hóa đơn thành công";


            return _response;
        }
    }
}
