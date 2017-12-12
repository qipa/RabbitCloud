﻿using App.Metrics;
using App.Metrics.Scheduling;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Cloud.Server.Monitor.Starter
{
    public class ReportRunnerService : BackgroundService
    {
        private readonly IMetricsRoot _metrics;

        public ReportRunnerService(IMetrics metrics)
        {
            _metrics = (IMetricsRoot)metrics;
        }

        #region Overrides of BackgroundService

        /// <inheritdoc />
        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduler = new AppMetricsTaskScheduler(
                TimeSpan.FromSeconds(5),
                async () =>
                {
                    await Task.WhenAll(_metrics.ReportRunner.RunAllAsync());
                });
            scheduler.Start();

            return Task.CompletedTask;
        }

        #endregion Overrides of BackgroundService
    }
}