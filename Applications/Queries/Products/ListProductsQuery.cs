﻿using MediatR;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Products
{
    public class ListProductsQuery :IRequest<List<ProductDto>>
    {
    }
}
