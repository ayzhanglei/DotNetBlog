﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetBlog.Core.Enums;
using Newtonsoft.Json;

namespace DotNetBlog.Core.Model.Widget
{
    public abstract class WidgetConfigModelBase
    {
        public string Title { get; set; }

        [JsonIgnore]
        public virtual bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Title);
            }
        }
    }

    public class AdministrationWidgetConfigModel : WidgetConfigModelBase
    {
        public AdministrationWidgetConfigModel()
        {
            this.Title = "管理";
        }
    }

    public class CategoryWidgetConfigModel : WidgetConfigModelBase
    {
        public CategoryWidgetConfigModel()
        {
            this.Title = "分类";
            this.ShowRSS = true;
            this.ShowNumbersOfTopics = true;
        }

        public bool ShowRSS { get; set; }

        public bool ShowNumbersOfTopics { get; set; }
    }

    public class RecentCommentWidgetConfigModel : WidgetConfigModelBase
    {
        public RecentCommentWidgetConfigModel()
        {
            this.Title = "最新评论";
            this.Number = 10;
        }

        public int Number { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.Number > 0;
            }
        }
    }
    
    public class MonthStatisticeWidgetConfigModel : WidgetConfigModelBase
    {
        public MonthStatisticeWidgetConfigModel()
        {
            this.Title = "归档";
        }
    }

    public class PageWidgetConfigModel : WidgetConfigModelBase
    {
        public PageWidgetConfigModel()
        {
            this.Title = "页面";
        }
    }

    public class RecentTopicWidgetConfigModel : WidgetConfigModelBase
    {
        public RecentTopicWidgetConfigModel()
        {
            this.Title = "最新文章";
            this.Number = 10;
        }

        public int Number { get; set; }

        public int? Category { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.Number > 0;
            }
        }
    }

    public class SearchWidgetConfigModel : WidgetConfigModelBase
    {
        public SearchWidgetConfigModel()
        {
            this.Title = "搜索";
        }
    }

    public class TagWidgetConfigModel : WidgetConfigModelBase
    {
        public TagWidgetConfigModel()
        {
            this.Title = "标签";
            this.Number = 100;
            this.MinTopicNumber = 1;
        }

        public int? Number { get; set; }

        public int MinTopicNumber { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && (!this.Number.HasValue || this.Number > 0) && this.MinTopicNumber > 0;
            }
        }
    }

    public class LinkWidgetConfigModel : WidgetConfigModelBase
    {
        public LinkWidgetConfigModel()
        {
            this.Title = "友情链接";
            this.LinkList = new List<LinkModel>();
        }

        public List<LinkModel> LinkList { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.LinkList != null;
            }
        }

        public class LinkModel
        {
            public string Title { get; set; }

            public string Url { get; set; }

            public bool OpenInNewWindow { get; set; }
        }
    }
}
