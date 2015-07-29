using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Esquel.Library.Infrastructure.DBEntity
{
    /// <summary>
    /// 实体基类
    /// </summary>
    [Serializable]
    public abstract class ObjectBase
    {
        /// <summary>
        /// 对应的数据库表名
        /// </summary>
        public abstract string TableName_V { get; }

        /// <summary>
        /// 主键集合
        /// </summary>
        public abstract string[] PrimaryKeyCollection { get; }
    }
}
