using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Report
{
    public class GetLowStockReportQuery : IRequest<LowStockReportResult>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
