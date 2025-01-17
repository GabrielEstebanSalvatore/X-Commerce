﻿using Dominio.Entidades;
using Dominio.UnidadDeTrabajo;
using IServicios.Precios;
using Servicios.Base;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace Servicios.Precios
{
    public class PrecioServicio : IPrecioServicio
    {
        IUnidadDeTrabajo _unidadDeTrabajo;
    

        public PrecioServicio(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public void ActualizarPrecio(decimal valor, bool esPorcentaje, long? marcaId = null, long? rubroId = null,
            long? listaPrecioId = null, int? codigoDesde = null, int? codigoHasta = null)

        { 
            using (var tran = new TransactionScope())
            {
                try
                {
                    Expression<Func<Dominio.Entidades.Articulo, bool>> filtro = x => true;

                    if (marcaId.HasValue)
                    {
                        filtro = filtro.And(x => x.MarcaId == marcaId.Value);
                    }

                    if (rubroId.HasValue)
                    {
                        filtro = filtro.And(x => x.RubroId == rubroId.Value);
                    }

                    if (codigoDesde.HasValue && codigoHasta.HasValue)
                    {
                        filtro = filtro.And(x => x.Codigo >= codigoDesde && x.Codigo <= codigoHasta);
                    }

                    var listaDeArticulosParaActualizar = _unidadDeTrabajo
                        .ArticuloRepositorio.Obtener(filtro, "Precios");
                    var listasPrecios = _unidadDeTrabajo.ListaPrecioRepositorio.Obtener();
                    var fechaActual = DateTime.Now;

                    foreach (var articulo in listaDeArticulosParaActualizar)
                    {
                        if (listaPrecioId.HasValue) 
                        {
                            var ultimoPrecioArticulo = articulo.Precios
                                .FirstOrDefault(x => x.ListaPrecioId == listaPrecioId.Value
                                                     && x.FechaActualizacion <= fechaActual
                                                     && x.FechaActualizacion == articulo.Precios.Where(p =>
                                                             p.ListaPrecioId == listaPrecioId.Value
                                                             && x.FechaActualizacion <= fechaActual)
                                                         .Max(f => f.FechaActualizacion));

                            var precioCosto = esPorcentaje
                                ? ultimoPrecioArticulo.PrecioCosto + (ultimoPrecioArticulo.PrecioCosto * (valor / 100m))
                                : ultimoPrecioArticulo.PrecioCosto + valor;
                            var listaSeleccionada = listasPrecios
                                .FirstOrDefault(x => x.Id == listaPrecioId.Value);
                            var precioPublico = precioCosto + (precioCosto * (listaSeleccionada.PorcentajeGanancia / 100m));
                            var nuevoPrecio = new Dominio.Entidades.Precio
                            {
                                ArticuloId = articulo.Id,
                                ListaPrecioId = listaPrecioId.Value, 
                                FechaActualizacion = fechaActual,
                                PrecioCosto = precioCosto,
                                PrecioPublico = precioPublico,
                                EstaEliminado = false,
                            };

                            _unidadDeTrabajo.PrecioRepositorio.Insertar(nuevoPrecio);
                        }
                        else
                        {
                            foreach (var lista in listasPrecios)
                            {
                                var ultimoPrecioArticulo = articulo.Precios
                                    .FirstOrDefault(x => x.ListaPrecioId == lista.Id
                                                         && x.FechaActualizacion <= fechaActual
                                                         && x.FechaActualizacion == articulo.Precios.Where(p =>
                                                                 p.ListaPrecioId == lista.Id
                                                                 && x.FechaActualizacion <= fechaActual)
                                                             .Max(f => f.FechaActualizacion));

                                var precioCosto = esPorcentaje
                                    ? ultimoPrecioArticulo.PrecioCosto + (ultimoPrecioArticulo.PrecioCosto * (valor / 100m))
                                    : ultimoPrecioArticulo.PrecioCosto + valor;
                                var precioPublico = precioCosto + (precioCosto * (lista.PorcentajeGanancia / 100m));
                                var nuevoPrecio = new Dominio.Entidades.Precio
                                {
                                    ArticuloId = articulo.Id,
                                    ListaPrecioId = lista.Id, 
                                    FechaActualizacion = fechaActual,
                                    PrecioCosto = precioCosto,
                                    PrecioPublico = precioPublico,
                                    EstaEliminado = false,
                                };

                                _unidadDeTrabajo.PrecioRepositorio.Insertar(nuevoPrecio);
                            }
                        }
                    }

                    _unidadDeTrabajo.Commit();

                    tran.Complete();
                }
                catch (Exception e)
                {
                    tran.Dispose();
                    throw new Exception(e.Message);
                }
            }
        }

        public void ActualizarPrecio(decimal valor, long articuloId)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    Expression<Func<Dominio.Entidades.Articulo, bool>> filtro = x => true;

                    var ArticuloParaActualizar = _unidadDeTrabajo
                        .ArticuloRepositorio.Obtener(articuloId, "Precios");
                    var listasPrecios = _unidadDeTrabajo.ListaPrecioRepositorio.Obtener();
                    var fechaActual = DateTime.Now;

                    foreach (var lista in listasPrecios)
                    {
                        var ultimoPrecioArticulo = ArticuloParaActualizar.Precios
                            .FirstOrDefault(x => x.ListaPrecioId == lista.Id
                                                    && x.FechaActualizacion <= fechaActual
                                                    && x.FechaActualizacion == ArticuloParaActualizar.Precios.Where(p =>
                                                            p.ListaPrecioId == lista.Id
                                                            && x.FechaActualizacion <= fechaActual)
                                                        .Max(f => f.FechaActualizacion));

                        var precioCosto = ultimoPrecioArticulo.PrecioCosto + valor;
                        var precioPublico = precioCosto + (precioCosto * (lista.PorcentajeGanancia / 100m));
                        var nuevoPrecio = new Dominio.Entidades.Precio
                        {
                            ArticuloId = ArticuloParaActualizar.Id,
                            ListaPrecioId = lista.Id, 
                            FechaActualizacion = fechaActual,
                            PrecioCosto = precioCosto,
                            PrecioPublico = precioPublico,
                            EstaEliminado = false,
                        };

                        _unidadDeTrabajo.PrecioRepositorio.Insertar(nuevoPrecio);
                    }

                    _unidadDeTrabajo.Commit();

                    tran.Complete();
                }
                catch (Exception e)
                {
                    tran.Dispose();
                    throw new Exception(e.Message);
                }
            }
        }

        private decimal calcularAumento(bool esPorcentaje, bool esMonto, decimal valor, decimal precioPublico)
        {
            decimal resultado = 0;

            if (esPorcentaje)
            {
                resultado = precioPublico * (valor / 100);
            }

            if (esMonto)
            {
                resultado = precioPublico + valor;
            }

            return resultado;
        }
    }
}
