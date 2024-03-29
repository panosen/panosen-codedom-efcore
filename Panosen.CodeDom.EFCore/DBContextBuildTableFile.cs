﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// DBContextBuildTable
    /// </summary>
    public class DBContextBuildTableFile
    {
        /// <summary>
        /// 项目命名空间
        /// </summary>
        public string CSharpRootNamespace { get; set; }

        /// <summary>
        /// 上下文名称
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        /// 忽略外键
        /// </summary>
        public bool IgnoreForeignKey { get; set; }

        /// <summary>
        /// 需要构建的主体
        /// </summary>
        public Table Table { get; set; }

        /// <summary>
        /// 多租户服务名称 示例：tenantService
        /// </summary>
        public string TenantServiceName { get; set; }

        /// <summary>
        /// 租户服务内属性名称 示例：TenantId
        /// </summary>
        public string TenantServicePropertyName { get; set; }

        /// <summary>
        /// 实例内属性名称 示例：TenantId
        /// </summary>
        public string TenantTablePropertyName { get; set; }
    }
}
