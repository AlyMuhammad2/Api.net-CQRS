using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Report
{
    public class TransactionHistoryResult
    {
        public List<TransactionDto> Data { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string? NextPage { get; set; }
        public string? PreviousPage { get; set; }
    }

}
