using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructue.Notifications
{
    public class NotifyArchivedTransactions : IRequest
    {
        public DateTime ArchiveDate { get; }

        public NotifyArchivedTransactions(DateTime archiveDate)
        {
            ArchiveDate = archiveDate;
        }
    }
}
