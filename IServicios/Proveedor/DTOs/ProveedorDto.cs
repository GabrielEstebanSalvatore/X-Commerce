﻿using IServicio.BaseDto;

namespace IServicios.Proveedor.DTOs
{
    public class ProveedorDto : DtoBase
    {
        public string RazonSocial { get; set; }

        public string CUIT { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string Mail { get; set; }

        public long CondicionIvaId { get; set; }

        public string CondicionIva { get; set; }

        public long LocalidadId { get; set; }

        public string Localidad { get; set; }

        public long DepartamentoId { get; set; }

        public string Departamento { get; set; }

        public long ProvinciaId { get; set; }

        public string Provincia { get; set; }



    }
}
