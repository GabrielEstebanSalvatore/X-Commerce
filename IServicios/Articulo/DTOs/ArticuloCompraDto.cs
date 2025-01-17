﻿using System;

namespace IServicios.Articulo.DTOs
{
    public class ArticuloCompraDto
    {
        public long Id { get; set; }

        //public int Codigo { get; set; }
        public string CodigoBarra { get; set; }
        public string Descripcion { get; set; }
        public decimal Iva { get; set; }
        public decimal Precio { get; set; }
        public string PrecioStr => Precio.ToString("C");
        public decimal Stock { get; set; }
        //public long ListaPrecioId { get; set; }
        // ==================================================== //
        public DateTime HoraDesde { get; set; }
        public DateTime HoraHasta { get; set; }
        public bool TieneRestriccionHorario { get; set; }
        // ==================================================== //
        public bool TieneRestriccionPorCantidad { get; set; }
        public decimal Limite { get; set; }
        // ==================================================== //

        public bool PermiteStockNegativo { get; set; }
    }
}
