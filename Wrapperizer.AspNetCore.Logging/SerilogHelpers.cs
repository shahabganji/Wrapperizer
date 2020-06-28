using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.AspnetcoreHttpcontext;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using Wrapperizer.AspNetCore.Logging.Configuration;
using Wrapperizer.AspNetCore.Logging.Formatters;
using Wrapperizer.AspNetCore.Logging.Models;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class SerilogHelpers
    {
        /// <summary>
        /// Provides standardized, centralized Serilog wire-up for a suite of applications.
        /// </summary>
        /// <param name="loggerConfig">Provide this value from the UseSerilog method param</param>
        /// <param name="config">IConfiguration settings -- generally read this from appsettings.json</param>
        /// <param name="setupAction">Action to configure connections for logging</param>
        public static void WithWrapperizerConfiguration(this LoggerConfiguration loggerConfig,
            IConfiguration config, Action<WrapperizerLoggingConfiguration> setupAction)
        {
            var configuration = new WrapperizerLoggingConfiguration();
            setupAction?.Invoke(configuration);

            var assemblyName = Assembly.GetEntryAssembly()?.GetName();

            loggerConfig
                .ReadFrom.Configuration(config) // minimum levels defined per project in json files 
                // .Enrich.WithAspnetcoreHttpcontext( AddCustomHttpContextDetails)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Assembly", $"{assemblyName?.Name}")
                .Enrich.WithProperty("Version", $"{assemblyName?.Version}")
                .Enrich.WithProperty("ApplicationName", configuration.ApplicationName);

            if (configuration.SqlConnectionString != null)
            {
                loggerConfig.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Matching.WithProperty("ElapsedInMilliseconds"))
                    .WriteTo.MSSqlServer(configuration.SqlConnectionString, new SinkOptions
                    {
                        AutoCreateSqlTable = true, TableName = "PerfLog",
                        SchemaName = "log"
                    }, columnOptions: GetSqlColumnOptions()));
            }

            loggerConfig
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Matching.WithProperty("UsageName"))
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(configuration.ElasticSearchUri)
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                            IndexFormat = configuration.UsageIndexFormat,
                            InlineFields = true,
                            CustomFormatter = new CustomElasticsearchJsonFormatter(inlineFields: true,
                                renderMessageTemplate: false)
                        }
                    ))
                .WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(Matching.WithProperty("ElapsedMilliseconds"))
                    .Filter.ByExcluding(Matching.WithProperty("UsageName"))
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(configuration.ElasticSearchUri)
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                            IndexFormat = configuration.ErrorIndexFormat,
                            InlineFields = true,
                            CustomFormatter = new CustomElasticsearchJsonFormatter(inlineFields: true,
                                renderMessageTemplate: false)
                        }
                    ))
                ;
        }

        private static ColumnOptions GetSqlColumnOptions()
        {
            var options = new ColumnOptions();
            // options.Store.Remove(StandardColumn.Message);
            options.Store.Remove(StandardColumn.MessageTemplate);
            options.Store.Remove(StandardColumn.Level);
            options.Store.Remove(StandardColumn.Exception);

            options.Store.Remove(StandardColumn.Properties);
            options.Store.Add(StandardColumn.LogEvent);
            options.LogEvent.ExcludeStandardColumns = true;
            options.LogEvent.ExcludeAdditionalProperties = true;

            options.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn
                {
                    ColumnName = "PerfItem", AllowNull = true,
                    DataType = SqlDbType.NVarChar, DataLength = 100,
                    // NonClusteredIndex = true
                },
                new SqlColumn
                {
                    ColumnName = "ElapsedInMilliseconds", AllowNull = false,
                    DataType = SqlDbType.Int
                },
                new SqlColumn
                {
                    ColumnName = "ActionName", AllowNull = false
                },
                new SqlColumn
                {
                    ColumnName = "MachineName", AllowNull = false
                },
                new SqlColumn
                {
                    ColumnName = "ApplicationName", AllowNull = false, NonClusteredIndex = true
                }
            };

            return options;
        }

        public static UserInfo AddCustomHttpContextDetails(IHttpContextAccessor ctx)
        {
            var excluded = new List<string> {"nbf", "exp", "auth_time", "amr", "sub"};
            const string userIdClaimType = "sub";

            var context = ctx.HttpContext;
            var user = context?.User.Identity;
            if (user == null || !user.IsAuthenticated) return null;

            var userId = context.User.Claims.FirstOrDefault(a => a.Type == userIdClaimType)?.Value;
            var userInfo = new UserInfo
            {
                UserName = user.Name,
                UserId = userId,
                UserClaims = new Dictionary<string, List<string>>()
            };
            foreach (var distinctClaimType in context.User.Claims
                .Where(a => excluded.All(ex => ex != a.Type))
                .Select(a => a.Type)
                .Distinct())
            {
                userInfo.UserClaims[distinctClaimType] = context.User.Claims
                    .Where(a => a.Type == distinctClaimType)
                    .Select(c => c.Value)
                    .ToList();
            }

            return userInfo;
        }
    }
}
