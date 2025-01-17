﻿using Dominio.UnidadDeTrabajo;
using IServicios.Comprobante;
using IServicios.Comprobante.DTOs;
using Servicios.Base;
using System;
using System.Collections.Generic;

namespace Servicios.Comprobante
{
    public class ComprobanteServicio : IComprobanteServicio
    {
        protected readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private Dictionary<Type, string> _diccionario;
        public ComprobanteServicio(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _diccionario = new Dictionary<Type, string>();
            InicializadorDiccionario();
        }
        private void InicializadorDiccionario()
        {
            _diccionario.Add(typeof(FacturaDto), "Servicios.Comprobante.Factura");
            _diccionario.Add(typeof(FacturaCompraDto), "Servicios.Comprobante.FacturaCompra");
            _diccionario.Add(typeof(CtaCteComprobanteDto), "Servicios.Comprobante.CuentaCorriente");
            _diccionario.Add(typeof(CtaCteComprobanteProveedorDto), "Servicios.Comprobante.CuentaCorriente");
            _diccionario.Add(typeof(PresupuestoDto), "Servicios.Comprobante.Presupuesto"); 
        }
        public void AgregarOpcionDiccionario(Type type, string value)
        {
            _diccionario.Add(type, value);
        }
        public virtual long Insertar(ComprobanteDto dto)
        {
            var comprobante = GenericInstance<Comprobante>.InstanciarEntidad(dto,_diccionario);
            return comprobante.Insertar(dto);
        }
        public long InsertarProveedor(ComprobanteDto dto)
        {
            var comprobante = GenericInstance<Comprobante>.InstanciarEntidad(dto, _diccionario);
            return comprobante.InsertarProveedor(dto);
        }

        public long Obtener(string cadenaBuscar, bool mostrarTodos = true)
        {
            return 0;
        }

    }
}
