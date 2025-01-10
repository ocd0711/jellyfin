using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emby.Server.Implementations.ScheduledTasks.Tasks
{
    /// <summary>
    /// Optimizes Jellyfin's database by issuing a VACUUM command.
    /// </summary>
    public class OptimizeDatabaseTask : IScheduledTask, IConfigurableScheduledTask
    {
        private readonly ILogger<OptimizeDatabaseTask> _logger;
        private readonly ILocalizationManager _localization;
        private readonly IDbContextFactory<JellyfinDbContext> _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizeDatabaseTask" /> class.
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/> interface.</param>
        /// <param name="localization">Instance of the <see cref="ILocalizationManager"/> interface.</param>
        /// <param name="provider">Instance of the <see cref="IDbContextFactory{JellyfinDbContext}"/> interface.</param>
        public OptimizeDatabaseTask(
            ILogger<OptimizeDatabaseTask> logger,
            ILocalizationManager localization,
            IDbContextFactory<JellyfinDbContext> provider)
        {
            _logger = logger;
            _localization = localization;
            _provider = provider;
        }

        /// <inheritdoc />
        public string Name => _localization.GetLocalizedString("TaskOptimizeDatabase");

        /// <inheritdoc />
        public string Description => _localization.GetLocalizedString("TaskOptimizeDatabaseDescription");

        /// <inheritdoc />
        public string Category => _localization.GetLocalizedString("TasksMaintenanceCategory");

        /// <inheritdoc />
        public string Key => "OptimizeDatabaseTask";

        /// <inheritdoc />
        public bool IsHidden => false;

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public bool IsLogged => true;

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return
            [
                // Every so often
                new TaskTriggerInfo { Type = TaskTriggerInfoType.IntervalTrigger, IntervalTicks = TimeSpan.FromHours(24).Ticks }
            ];
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting database optimization...");

            try
            {
                var context = await _provider.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                await using (context.ConfigureAwait(false))
                {
                    if (context.Database.IsSqlite())
                    {
                        // SQLite optimization
                        await context.Database.ExecuteSqlRawAsync("PRAGMA optimize", cancellationToken).ConfigureAwait(false);
                        await context.Database.ExecuteSqlRawAsync("VACUUM", cancellationToken).ConfigureAwait(false);
                        _logger.LogInformation("SQLite database optimized successfully!");
                    }
                    else if (context.Database.IsMySql())
                    {
                        // MySQL optimization: Get table names
                        var tables = new List<string>();
                        await using (var command = context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "SHOW TABLES";
                            await context.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        
                            await using (var result = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                            {
                                while (await result.ReadAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    tables.Add(result.GetString(0));
                                }
                            }
                        }


                        foreach (var table in tables)
                        {
                            await context.Database.ExecuteSqlInterpolatedAsync($"OPTIMIZE TABLE {table}", cancellationToken).ConfigureAwait(false);
                            // await context.Database.ExecuteSqlRawAsync("OPTIMIZE TABLE " + table, cancellationToken).ConfigureAwait(false);
                        }
                        _logger.LogInformation("MySQL database optimized successfully!");
                    }
                    else
                    {
                        _logger.LogInformation("This database type doesn't support optimization commands.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while optimizing the database");
            }

        }
    }
}
