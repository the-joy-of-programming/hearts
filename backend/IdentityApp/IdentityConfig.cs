// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;

namespace IdentityApp
{
    public class IdentityProviderConfig
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class IdentityConfig
    {
        public List<IdentityProviderConfig> Providers { get; set; }
        
    }

    public class IdentityAppConfig
    {
        public IdentityConfig Identity { get; set; }
    }
}