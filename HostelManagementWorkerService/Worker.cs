using DataAccess.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HostelManagementWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IRentRepository rentRepository;
        private readonly IServiceProvider _serviceProvider;
        public Worker(ILogger<Worker> logger, IRentRepository _rentRepository, IServiceProvider serviceProvider)
        {
            _logger = logger;
            rentRepository = _rentRepository;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var rents = await rentRepository.GetRentList();
                var rentDeposits = rents.Where(r => r.IsDeposited == 0 && r.Status == 0);
                foreach (var item in rentDeposits)
                {
                    if (item.StartRentDate < DateTime.Now.AddHours(-24))
                    {
                        item.Status = 4;
                        await rentRepository.UpdateRent(item);
                        _logger.LogInformation("The rent {0} of {1} is cancel at {2}", item.RentId, item.RentedBy, DateTime.Now);
                    }
                    
                }
                var rentWaitingStart = rents.Where(r => r.IsDeposited == 2 && r.Status == 0);
                foreach (var item in rentWaitingStart)
                {
                    if (item.StartRentDate.Date == DateTime.Now.Date)
                    {
                        item.Status = 1;
                        await rentRepository.UpdateRent(item);
                        _logger.LogInformation("The rent {0} of {1} is start at {2}", item.RentId, item.RentedBy, DateTime.Now);
                    }
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}