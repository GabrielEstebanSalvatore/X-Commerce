﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Dominio.Entidades;
using Dominio.UnidadDeTrabajo;
using IServicio.Articulo;
using IServicio.Articulo.DTOs;
using IServicio.BaseDto;
using IServicios.Articulo.DTOs;
using Servicios.Base;

namespace Servicios.Articulo
{
    public class ArticuloServicio : IArticuloServicio
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public ArticuloServicio(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            
        }

        public void Eliminar(long id)
        {
            _unidadDeTrabajo.ArticuloRepositorio.Eliminar(id);
            _unidadDeTrabajo.Commit();
        }

        public void Insertar(DtoBase dtoEntidad)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    var dto = (ArticuloCrudDto)dtoEntidad;

                    var entidad = new Dominio.Entidades.Articulo
                    {
                        MarcaId = dto.MarcaId,
                        RubroId = dto.RubroId,
                        UnidadMedidaId = dto.UnidadMedidaId,
                  
                        Codigo = dto.Codigo,
   
                        Abreviatura = dto.Abreviatura,
                        Descripcion = dto.Descripcion,
                        Detalle = dto.Detalle,
                        Ubicacion = dto.Ubicacion,

                        PermiteStockNegativo = dto.PermiteStockNegativo,
                        DescuentaStock = dto.DescuentaStock,
                        StockMinimo = dto.StockMinimo,
                        Foto = dto.Foto,
                        EstaEliminado = false
                    };

                    _unidadDeTrabajo.ArticuloRepositorio.Insertar(entidad);

               
                    var configSistema = _unidadDeTrabajo.ConfiguracionRepositorio.Obtener().FirstOrDefault();

                    if (configSistema == null)
                        throw new Exception("Ocurrió un error al Obtener la configuracion del sistema");

                    var depositos = _unidadDeTrabajo.DepositoRepositorio.Obtener();

                    foreach (var deposito in depositos)
                    {
                        _unidadDeTrabajo.StockRepositorio.Insertar(new Stock
                        {
                            DepositoId = deposito.Id,
                            ArticuloId = entidad.Id,
                            Cantidad = deposito.Id == configSistema.DepositoStockId
                                ? dto.StockActual
                                : 0,
                            EstaEliminado = false
                        });
                    }

                    _unidadDeTrabajo.Commit();

                    tran.Complete();
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void Modificar(DtoBase dtoEntidad)
        {
            var dto = (ArticuloCrudDto)dtoEntidad;

            var entidad = _unidadDeTrabajo.ArticuloRepositorio.Obtener(dto.Id);

            if (entidad == null) throw new Exception("Ocurrio un Error al Obtener el Artículo");


            entidad.MarcaId = dto.MarcaId;
            entidad.RubroId = dto.RubroId;
            entidad.UnidadMedidaId = dto.UnidadMedidaId;
    
            entidad.Codigo = dto.Codigo;

            entidad.Abreviatura = dto.Abreviatura;
            entidad.Descripcion = dto.Descripcion;
            entidad.Detalle = dto.Detalle;
            entidad.Ubicacion = dto.Ubicacion;
     
            entidad.PermiteStockNegativo = dto.PermiteStockNegativo;
            entidad.DescuentaStock = dto.DescuentaStock;
            entidad.StockMinimo = dto.StockMinimo;
            entidad.Foto = dto.Foto;
            
            _unidadDeTrabajo.ArticuloRepositorio.Modificar(entidad);
            _unidadDeTrabajo.Commit();
        }

        public DtoBase Obtener(long id)
        {
            var entidad = _unidadDeTrabajo.ArticuloRepositorio.Obtener(id, "Rubro, Marca, UnidadMedida, Iva, Stocks, Stocks.Deposito, Precios, Precios.ListaPrecio");

            return new ArticuloDto
            {
                Id = entidad.Id,
                MarcaId = entidad.MarcaId,
                Marca = entidad.Marca.Descripcion,
                RubroId = entidad.RubroId,
                Rubro = entidad.Rubro.Descripcion,
                UnidadMedidaId = entidad.UnidadMedidaId,
                UnidadMedida = entidad.UnidadMedida.Descripcion,
                IvaId = entidad.IvaId,
                Iva = entidad.Iva.Descripcion,
                Codigo = entidad.Codigo,
                CodigoBarra = entidad.CodigoBarra,
                Abreviatura = entidad.Abreviatura,
                Descripcion = entidad.Descripcion,
                Detalle = entidad.Detalle,
                Ubicacion = entidad.Ubicacion,
                ActivarLimiteVenta = entidad.ActivarLimiteVenta,
                LimiteVenta = entidad.LimiteVenta,
                ActivarHoraVenta = entidad.ActivarHoraVenta,
                HoraLimiteVentaDesde = entidad.HoraLimiteVentaDesde,
                HoraLimiteVentaHasta = entidad.HoraLimiteVentaHasta,
                PermiteStockNegativo = entidad.PermiteStockNegativo,
                DescuentaStock = entidad.DescuentaStock,
                StockMinimo = entidad.StockMinimo,
                Foto = entidad.Foto,
                Eliminado = entidad.EstaEliminado,
                Stocks = entidad.Stocks
                    .Select(x => new StockDepositoDto
                    {
                        Cantidad = x.Cantidad,
                        Deposito = x.Deposito.Descripcion
                    }).ToList(),

                Precios = entidad.Precios.GroupBy(y => y.ListaPrecio)
                    .Select(dto => entidad.Precios.FirstOrDefault(p => p.ListaPrecio == dto.Key))
                    .Select(list => new PreciosDto
                    {
                        Fecha = list.FechaActualizacion,
                        Precio = list.PrecioPublico,
                        ListaPrecio = list.ListaPrecio.Descripcion
                    }).ToList()
            };
        }



        public IEnumerable<DtoBase> Obtener(string cadenaBuscar, bool mostrarTodos = true)
        {
            Expression<Func<Dominio.Entidades.Articulo, bool>> filtro = x =>
             x.Descripcion.Contains(cadenaBuscar)
             || x.Marca.Descripcion.Contains(cadenaBuscar)
             || x.Rubro.Descripcion.Contains(cadenaBuscar)
             || x.UnidadMedida.Descripcion.Contains(cadenaBuscar)
             || x.Iva.Descripcion.Contains(cadenaBuscar);

            if (!mostrarTodos)
            {
                filtro = filtro.And(x => !x.EstaEliminado);
            }

            return _unidadDeTrabajo.ArticuloRepositorio.Obtener(filtro, "Rubro, Marca, UnidadMedida, Iva, Stocks, Stocks.Deposito, Precios, Precios.ListaPrecio ")

                .Select(entidad => new ArticuloDto
                {
                    Id = entidad.Id,
                    MarcaId = entidad.MarcaId,
                    Marca = entidad.Marca.Descripcion,
                    RubroId = entidad.RubroId,
                    Rubro = entidad.Rubro.Descripcion,
                    UnidadMedidaId = entidad.UnidadMedidaId,
                    UnidadMedida = entidad.UnidadMedida.Descripcion,
                    IvaId = entidad.IvaId,
                    Iva = entidad.Iva.Descripcion,
                    Codigo = entidad.Codigo,
                    CodigoBarra = entidad.CodigoBarra,
                    Abreviatura = entidad.Abreviatura,
                    Descripcion = entidad.Descripcion,
                    Detalle = entidad.Detalle,
                    Ubicacion = entidad.Ubicacion,
                    ActivarLimiteVenta = entidad.ActivarLimiteVenta,
                    LimiteVenta = entidad.LimiteVenta,
                    ActivarHoraVenta = entidad.ActivarHoraVenta,
                    HoraLimiteVentaDesde = entidad.HoraLimiteVentaDesde,
                    HoraLimiteVentaHasta = entidad.HoraLimiteVentaHasta,
                    PermiteStockNegativo = entidad.PermiteStockNegativo,
                    DescuentaStock = entidad.DescuentaStock,
                    StockMinimo = entidad.StockMinimo,
                    Foto = entidad.Foto,
                    Eliminado = entidad.EstaEliminado,
                    Stocks = entidad.Stocks
                    .Select(x => new StockDepositoDto
                    {
                        Cantidad = x.Cantidad,
                        Deposito = x.Deposito.Descripcion
                    }).ToList(),

                    Precios = entidad.Precios.GroupBy(y => y.ListaPrecio)
                        .Select(dto => entidad.Precios.FirstOrDefault(p => p.ListaPrecio == dto.Key
                         && p.FechaActualizacion == entidad.Precios.Where(u => u.ListaPrecio == dto.Key
                         && (u.FechaActualizacion <= DateTime.Now)).Max(f => f.FechaActualizacion)))
                        .Select(list => new PreciosDto
                        {
                            Fecha = list.FechaActualizacion,
                            Precio = list.PrecioPublico,
                            ListaPrecio = list.ListaPrecio.Descripcion
                        }).ToList()

              
                })
                .OrderBy(x => x.Descripcion)
                .ToList();

        }

        public int ObtenerSiguienteNroCodigo()
        {
            var articulos = _unidadDeTrabajo.ArticuloRepositorio.Obtener();

            return articulos.Any()
                ? articulos.Max(x => x.Codigo) + 1
                : 1;
        }

        public bool VerificarSiExiste(string datoVerificar, long? entidadId = null)
        {
            return entidadId.HasValue
                ? _unidadDeTrabajo.ArticuloRepositorio.Obtener(x => !x.EstaEliminado
                                                                        && x.Id != entidadId.Value
                                                                        && x.Descripcion.Equals(datoVerificar,
                                                                            StringComparison.CurrentCultureIgnoreCase))
                    .Any()
                : _unidadDeTrabajo.ArticuloRepositorio.Obtener(x => !x.EstaEliminado
                                                                        && x.Descripcion.Equals(datoVerificar,
                                                                            StringComparison.CurrentCultureIgnoreCase))
                    .Any();
        }

      

        public int ObtenerCantidadArticulos()
        {
            return _unidadDeTrabajo.ArticuloRepositorio.Obtener().Count();
        }
        public ArticuloVentaDto ObtenerPorCodigo(string codigo, long listaPrecioId, long depositoId)
        {
            var fechaActual = DateTime.Now;
            int.TryParse(codigo, out int _codigo);


            return _unidadDeTrabajo.ArticuloRepositorio.Obtener(x => x.CodigoBarra == codigo || x.Codigo == _codigo,
                    "Rubro, Marca, UnidadMedida, Iva, Stocks, Stocks.Deposito, Precios, Precios.ListaPrecio")
                .Select(x => new ArticuloVentaDto()
                {
                    Id = x.Id,
                    Iva = x.Iva.Porcentaje,
                    CodigoBarra = x.CodigoBarra,
                    Descripcion = x.Descripcion,
                    HoraDesde = x.HoraLimiteVentaDesde,
                    HoraHasta = x.HoraLimiteVentaHasta,
                    TieneRestriccionHorario = x.ActivarHoraVenta,
                    TieneRestriccionPorCantidad = x.ActivarLimiteVenta,
                    Limite = x.LimiteVenta,
                    PermiteStockNegativo = x.PermiteStockNegativo,
                    Stock = x.Stocks.Any()
                        ? x.Stocks.Where(d => d.DepositoId == depositoId).Sum(s => s.Cantidad)
                        : 0m,
                    Precio = x.Precios.Any()
                        ? x.Precios.FirstOrDefault(p => p.ListaPrecioId == listaPrecioId &&
                                                        p.FechaActualizacion <= fechaActual
                                                        && p.FechaActualizacion == x.Precios.Where(pre =>
                                                                pre.ListaPrecioId == listaPrecioId
                                                                && pre.FechaActualizacion <= fechaActual)
                                                            .Max(f => f.FechaActualizacion)).PrecioPublico
                        : 0m,
                    ListaPrecioId = listaPrecioId
                }).FirstOrDefault();
        }

       

        public IEnumerable<ArticuloVentaDto> ObtenerLookUp(string cadenaBuscar, long listaPrecioId)
        {
            var fechaActual = DateTime.Now;
            int.TryParse(cadenaBuscar, out int codigoArticulo);

            Expression<Func<Dominio.Entidades.Articulo, bool>> filtro = x => 
            !x.EstaEliminado && x.CodigoBarra == cadenaBuscar
            || x.Descripcion.Contains(cadenaBuscar)
            || x.Codigo == codigoArticulo;


            return _unidadDeTrabajo.ArticuloRepositorio.Obtener(filtro,
                    "Iva, Stocks, Stocks.Deposito, Precios, Precios.ListaPrecio")
                .Select(x => new ArticuloVentaDto()
                {
                    Id = x.Id,
                    Iva = x.Iva.Porcentaje,
                    CodigoBarra = x.CodigoBarra,
                    Descripcion = x.Descripcion,
                    HoraDesde = x.HoraLimiteVentaDesde,
                    HoraHasta = x.HoraLimiteVentaHasta,
                    TieneRestriccionHorario = x.ActivarHoraVenta,
                    TieneRestriccionPorCantidad = x.ActivarLimiteVenta,
                    Limite = x.LimiteVenta,
                    Stock = x.Stocks.Any()
                        ? x.Stocks.Sum(s => s.Cantidad)
                        : 0m,
                    Precio = x.Precios.Any()
                        ? x.Precios.FirstOrDefault(p => p.ListaPrecioId == listaPrecioId &&
                                                        p.FechaActualizacion <= fechaActual
                                                        && p.FechaActualizacion == x.Precios.Where(pre =>
                                                                pre.ListaPrecioId == listaPrecioId
                                                                && pre.FechaActualizacion <= fechaActual)
                                                            .Max(f => f.FechaActualizacion)).PrecioPublico
                        : 0m,
                    ListaPrecioId = listaPrecioId
                }).ToList();
        }

        public IEnumerable<ArticuloCompraDto> ObtenerLookUpCompra(string cadenaBuscar)
        {
            var fechaActual = DateTime.Now;
            int.TryParse(cadenaBuscar, out int codigoArticulo);

            Expression<Func<Dominio.Entidades.Articulo, bool>> filtro = x =>
            !x.EstaEliminado && x.CodigoBarra == cadenaBuscar
            || x.Descripcion.Contains(cadenaBuscar)
            || x.Codigo == codigoArticulo;


           var articulos = _unidadDeTrabajo.ArticuloRepositorio.Obtener(filtro,
                    "Iva, Stocks, Stocks.Deposito, Precios")
                .Select(x => new ArticuloCompraDto()
                {
                    Id = x.Id,
                    Iva = x.Iva.Porcentaje,
                    CodigoBarra = x.CodigoBarra,
                    Descripcion = x.Descripcion,
                    HoraDesde = x.HoraLimiteVentaDesde,
                    HoraHasta = x.HoraLimiteVentaHasta,
                    TieneRestriccionHorario = x.ActivarHoraVenta,
                    TieneRestriccionPorCantidad = x.ActivarLimiteVenta,
                    Limite = x.LimiteVenta,
                    Stock = x.Stocks.Any()
                        ? x.Stocks.Sum(s => s.Cantidad)
                        : 0m,
                    Precio = x.Precios.Any()
                        ? x.Precios.FirstOrDefault(p => p.FechaActualizacion <= fechaActual
                                                        && p.FechaActualizacion == x.Precios.Where(pre =>
                                                                pre.FechaActualizacion <= fechaActual)
                                                            .Max(f => f.FechaActualizacion)).PrecioCosto
                        : 0m,
                }).ToList();

            return articulos;
        }

        public ArticuloCompraDto ObtenerPorCodigoCompra(string codigo, long depositoId)
        {
            var fechaActual = DateTime.Now;
            int.TryParse(codigo, out int _codigo);

            return _unidadDeTrabajo.ArticuloRepositorio.Obtener(x => x.CodigoBarra == codigo || x.Codigo == _codigo,
                    "Rubro, Marca, UnidadMedida, Iva, Stocks, Stocks.Deposito, Precios")
                .Select(x => new ArticuloCompraDto()
                {
                    Id = x.Id,
                    Iva = x.Iva.Porcentaje,
                    CodigoBarra = x.CodigoBarra,
                    Descripcion = x.Descripcion,
                    HoraDesde = x.HoraLimiteVentaDesde,
                    HoraHasta = x.HoraLimiteVentaHasta,
                    TieneRestriccionHorario = x.ActivarHoraVenta,
                    TieneRestriccionPorCantidad = x.ActivarLimiteVenta,
                    Limite = x.LimiteVenta,
                    PermiteStockNegativo = x.PermiteStockNegativo,
                    Stock = x.Stocks.Any()
                        ? x.Stocks.Where(d => d.DepositoId == depositoId).Sum(s => s.Cantidad)
                        : 0m,
                    Precio = x.Precios.Any()
                        ? x.Precios.FirstOrDefault(p => p.FechaActualizacion <= fechaActual
                                                        && p.FechaActualizacion == x.Precios.Where(pre =>
                                                                pre.FechaActualizacion <= fechaActual)
                                                            .Max(f => f.FechaActualizacion)).PrecioCosto
                        : 0m

                }).FirstOrDefault();
        }
    }
}
