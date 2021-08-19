﻿using System.Collections.Generic;
using Models.Cargo;

namespace NextGenSoftware.OASIS.API.Providers.CargoOASIS.Models.Cargo
{
    public class GetUserTokensByContractResponse
    {
        public string TokenId { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
        public string TokenUrl { get; set; }
        public ResaleItem ResaleItem { get; set; }
        public string Owner { get; set; }
    }
}