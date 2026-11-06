using FoodOrdering.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : IRequest<ApiResponse<bool>>;

}
