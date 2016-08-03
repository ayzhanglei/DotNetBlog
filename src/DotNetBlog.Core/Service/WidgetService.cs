﻿using DotNetBlog.Core.Data;
using DotNetBlog.Core.Entity;
using DotNetBlog.Core.Enums;
using DotNetBlog.Core.Model.Widget;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetBlog.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using DotNetBlog.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetBlog.Core.Service
{
    public class WidgetService
    {
        private static readonly string CacheKey = "Cache_Widget";

        private static readonly Dictionary<WidgetType, string> DefaultNames = new Dictionary<WidgetType, string>
        {
            { WidgetType.Administration, "管理" },
            { WidgetType.Category, "分类" },
            { WidgetType.RecentComment, "最新评论" },
            { WidgetType.MonthStatistics, "归档" },
            { WidgetType.Page, "页面" },
            { WidgetType.Search, "搜索" },
            { WidgetType.Tag, "标签" },
            { WidgetType.RecentTopic, "最新文章" }
        };

        private static readonly Dictionary<WidgetType, Type> DefaultWidgetConfigTypes = new Dictionary<WidgetType, Type>
        {
            { WidgetType.Administration, typeof(AdministratorWidgetConfigModel) },
            { WidgetType.Category, typeof(CategoryWidgetConfigModel) },
            { WidgetType.RecentComment, typeof(RecentCommentWidgetConfigModel) },
            { WidgetType.MonthStatistics, typeof(MonthStatisticeWidgetConfigModel) },
            { WidgetType.Page, typeof(PageWidgetConfigModel) },
            { WidgetType.Search, typeof(SearchWidgetConfigModel) },
            { WidgetType.Tag, typeof(TagWidgetConfigModel) },
            { WidgetType.RecentTopic, typeof(RecentTopicWidgetConfigModel) }
        };

        private BlogContext BlogContext { get; set; }

        private IMemoryCache Cache { get; set; }

        public WidgetService(BlogContext blogContext, IMemoryCache cache)
        {
            this.BlogContext = blogContext;
            this.Cache = cache;
        }

        public List<AvailableWidgetModel> QueryAvailable()
        {
            List<AvailableWidgetModel> result = new List<AvailableWidgetModel>();

            var arr = Enum.GetValues(typeof(WidgetType));
            foreach(byte item in arr)
            {
                WidgetType type = (WidgetType)item;
                result.Add(new AvailableWidgetModel
                {
                    Type = type,
                    Name = DefaultNames[type]                    
                });
            }

            return result;
        }

        private async Task<List<Widget>> All()
        {
            var result = await this.Cache.RetriveCacheAsync(CacheKey, async () =>
            {
                var list = await this.BlogContext.Widgets.ToListAsync();
                return list;
            });

            return result;
        }

        public async Task<List<WidgetModel>> Query()
        {
            var entityList = await this.All();

            var result = entityList.OrderBy(t => t.ID).Select(t => new WidgetModel
            {
                Type = t.Type,
                Config = JsonConvert.DeserializeObject(t.Config, DefaultWidgetConfigTypes[t.Type]) as WidgetConfigModelBase
            });

            return result.ToList();
        }

        public async Task<OperationResult> Save(List<WidgetModel> widgetList)
        {
            using (var tran = await this.BlogContext.Database.BeginTransactionAsync())
            {
                var entityList = await this.BlogContext.Widgets.ToListAsync();
                this.BlogContext.RemoveRange(entityList);
                await this.BlogContext.SaveChangesAsync();

                entityList = widgetList.Select(t => new Widget
                {
                    Type = t.Type,
                    ID = widgetList.IndexOf(t) + 1,
                    Config = JsonConvert.SerializeObject(t.Config)
                }).ToList();
                this.BlogContext.AddRange(entityList);
                await this.BlogContext.SaveChangesAsync();

                tran.Commit();
                return new OperationResult();
            }
        }

        public WidgetConfigModelBase Transform(WidgetType type, JObject config)
        {
            Type targetType = DefaultWidgetConfigTypes[type];
            return config.ToObject(targetType) as WidgetConfigModelBase;
        }
    }
}