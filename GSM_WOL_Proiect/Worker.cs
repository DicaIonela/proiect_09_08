using GSM_WOL_Proiect;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proiect_practicaDI
{
    public class Worker : BackgroundService
    {
        //private readonly ILogger<Worker> _logger;
        //public Worker(ILogger<Worker> logger)
        //{
        //    _logger = logger;
        //}
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Metode.StartCommandPromptMode(); /*Continua cu modul Command Prompt*/
                    await Task.Delay(4000, stoppingToken); // Delay de 1 secundă
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
