using System;
using System.IO;
using MediaBrowser.Common.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Xml.Linq;

namespace Jellyfin.Server.Implementations.Extensions;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IDbContextFactory{TContext}"/> interface to the service collection with second level caching enabled.
    /// </summary>
    /// <param name="serviceCollection">An instance of the <see cref="IServiceCollection"/> interface.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddJellyfinDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddPooledDbContextFactory<JellyfinDbContext>((serviceProvider, opt) =>
        {
            // var applicationPaths = serviceProvider.GetRequiredService<IApplicationPaths>();
            // opt.UseSqlite($"Filename={Path.Combine(applicationPaths.DataPath, "jellyfin.db")}");

            var configFilePath = Path.Combine(AppContext.BaseDirectory, "mysql.xml");
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException($"MySQL configuration file not found: {configFilePath}");
            }
            // 使用 XML 解析获取 ConnectionString
            // <Configuration>
            //   <ConnectionString>Server=192.168.0.117;Database=jellyfin;User=root;Password=123456;</ConnectionString>
            // </Configuration>
            var xml = XDocument.Load(configFilePath);
            if (xml.Root == null)
            {
                throw new InvalidOperationException("The MySQL configuration file structure is invalid, missing the root node!");
            }
            var connectionString = xml.Root.Element("ConnectionString")?.Value;
    
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("No valid connection string found in the MySQL configuration file!");
            }
            opt.UseMySql(
                // "Server=192.168.0.117;Database=jellyfin;User=root;Password=123456;",
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 2)),
                mysqlOptions =>
            {
                mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
        });

        return serviceCollection;
    }
}
