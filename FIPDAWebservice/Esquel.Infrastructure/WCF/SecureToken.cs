using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Esquel.Library.Infrastructure.WCF
{
    [DataContract]
    public class SecureToken
    {
        public SecureToken()
        {
            
        }

        public SecureToken(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string Token { get; set; }
         
    }
}
