using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Esquel.Library.Infrastructure.WCF
{
    public class SecureTokenContext
    {
        public static SecureToken Current
        {
            get
            {
                if (GenericContext<SecureToken>.Current == null)
                    return null;

                return GenericContext<SecureToken>.Current.Value;
            }

            set
            {
                GenericContext<SecureToken>.Current = new GenericContext<SecureToken>(value);
            }
        }
    }
}
