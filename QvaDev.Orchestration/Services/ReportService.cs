using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QvaDev.Data;

namespace QvaDev.Orchestration.Services
{
    public interface IReportService
    {
        Task OrderHistoryExport(DuplicatContext duplicatContext);
    }

    public class ReportService : IReportService
    {
        public Task OrderHistoryExport(DuplicatContext duplicatContext)
        {
            throw new NotImplementedException();
        }
    }
}
