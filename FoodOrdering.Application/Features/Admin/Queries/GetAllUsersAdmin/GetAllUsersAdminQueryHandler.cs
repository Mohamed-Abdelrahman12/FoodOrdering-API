using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using FoodOrdering.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Admin.Queries.GetAllUsersAdmin
{
    internal class GetAllUsersAdminQueryHandler : IRequestHandler<GetAllUsersAdminQuery, ApiResponse<PagedResult<UserDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersAdminQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<UserDto>>> Handle(GetAllUsersAdminQuery request, CancellationToken cancellationToken)
        {
            var users = _userManager.Users
                .OrderBy(u => u.FirstName)
                .AsQueryable();

            var totalCount = await users.CountAsync(cancellationToken);
            var pagedUsers = await users.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(cancellationToken);

            var dtos = new List<UserDto>();
            foreach (var user in pagedUsers)
            {
                var dto = _mapper.Map<UserDto>(user);
                dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                dtos.Add(dto);
            }

            return ApiResponse<PagedResult<UserDto>>.Ok(new PagedResult<UserDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
    }
}