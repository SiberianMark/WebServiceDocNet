using System;
using System.Collections.Generic;
//using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
//using System.Threading.Tasks;

namespace Esquel.Library.Infrastructure.DBEntity
{
    /// <summary>
    /// 实体类型详细信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class EntityTypeDesc
    {
        /// <summary>
        /// 主键属性集合
        /// </summary>
        [DataMember]
        public PropertyInfo[] PrimaryKeys;

        /// <summary>
        /// 非主键属性集合
        /// </summary>
        [DataMember]
        public PropertyInfo[] DataMembers;
    }
}
