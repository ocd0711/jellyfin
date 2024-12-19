using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Xml.Linq;
using System.IO;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Server.Implementations.Migrations
{
    /// <summary>
    /// The design time factory for <see cref="JellyfinDbContext"/>.
    /// This is only used for the creation of migrations and not during runtime.
    /// </summary>
    internal class DesignTimeJellyfinDbFactory : IDesignTimeDbContextFactory<JellyfinDbContext>
    {
        public JellyfinDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<JellyfinDbContext>();

            // optionsBuilder.UseSqlite("Data Source=jellyfin.db");

            // return new JellyfinDbContext(optionsBuilder.Options);

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
            optionsBuilder.UseMySql(
                // "Server=192.168.0.117;Database=jellyfin;User=root;Password=123456;",
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 2)),
                mysqlOptions =>
            {
                mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });

            return new JellyfinDbContext(optionsBuilder.Options);
        }
    }
}
