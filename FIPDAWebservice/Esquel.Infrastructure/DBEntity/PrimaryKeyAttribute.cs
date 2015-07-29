using System;
using System.Collections.Generic;
//using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Esquel.Library.Infrastructure.DBEntity
{
    /// <summary>
    /// 主键扩展属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , Inherited = false)]
    [ComVisible(true)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute() { }
    }

    /// <summary>
    /// 非主键扩展属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , Inherited = false)]
    [ComVisible(true)]
    public sealed class FieldMemberAttribute : Attribute
    {
        public FieldMemberAttribute() { }
    }
}
