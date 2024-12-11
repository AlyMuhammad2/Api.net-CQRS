using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Report
{
    public class GetTransactionHistoryQuery : IRequest<TransactionHistoryResult>
    {
        public int? ProductId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TransactionType { get; set; }
        public string? Category { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }

}
