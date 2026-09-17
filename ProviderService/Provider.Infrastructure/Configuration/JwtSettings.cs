using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.Infrastructure.Configuration
{
    public class JwtSettings
    {
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpiresMinutes { get; set; }
    }
}
