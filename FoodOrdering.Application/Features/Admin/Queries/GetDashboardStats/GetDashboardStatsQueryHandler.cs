using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Admin.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Admin.Queries.GetDashboardStats
{
    internal class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, ApiResponse<DashboardStatsDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetDashboardStatsQueryHandler(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<ApiResponse<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var totalOrders = await _uow.Orders.Query().CountAsync(cancellationToken);
            var pendingOrders = await _uow.Orders.Query().CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);
            var deliveredOrders = await _uow.Orders.Query().CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken);
            var cancelledOrders = await _uow.Orders.Query().CountAsync(o => o.Status == OrderStatus.Cancelled, cancellationToken);
            var totalRevenue = await _uow.Payments.Query()
                .Where(p => p.Status == PaymentStatus.Paid)
                .SumAsync(p => p.Amount, cancellationToken);
            var totalProducts = await _uow.Products.Query().CountAsync(cancellationToken);
            var totalCategories = await _uow.Categories.Query().CountAsync(cancellationToken);

            var customerUsers = await _userManager.GetUsersInRoleAsync("Customer");
            var totalCustomers = customerUsers.Count;

            var recentOrders = await _uow.Orders.Query()
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    UserName = o.User.FirstName + " " + o.User.LastName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<DashboardStatsDto>.Ok(new DashboardStatsDto
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalCustomers = totalCustomers,
                RecentOrders = recentOrders
            });
        }
    }


}
