﻿namespace sistemcommerce.Models.ViewModels
{
    public class OrdenVM
    {
        public Orden Orden { get; set; }
        public IEnumerable<OrdenDetalle> OrdenDetalle { get; set; }
    }
}
