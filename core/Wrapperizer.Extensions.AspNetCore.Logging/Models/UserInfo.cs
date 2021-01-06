﻿using System.Collections.Generic;

 namespace Wrapperizer.Extensions.AspNetCore.Logging.Models
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, List<string>> UserClaims { get; set; }
    }
}
