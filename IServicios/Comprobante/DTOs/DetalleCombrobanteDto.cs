﻿using IServicio.BaseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServicios.Comprobante.DTOs
{
    public class DetalleComprobanteDto : DtoBase
    {
        public long ArticuloId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Iva { get; set; }
        public decimal Precio { get; set; }
        public decimal SubTotal { get; set; }

        public long ListaPrecioId { get; set; }
    }
}
