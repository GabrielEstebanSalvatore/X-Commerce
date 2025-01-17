﻿using Dominio.Entidades;
using Dominio.Repositorio;
using Infraestructura.Repositorio;

namespace Infraestructura.UnidadDeTrabajo
{
    public partial class UnidadDeTrabajo
    {
        // ============================================================================================================ //

        private IRepositorio<Provincia> provinciaRepositorio;
        public IRepositorio<Provincia> ProvinciaRepositorio => provinciaRepositorio
                                                               ?? (provinciaRepositorio =
                                                                   new Repositorio<Provincia>(_context));

        // ============================================================================================================ //

        private IRepositorio<Departamento> departamentoRepositorio;

        public IRepositorio<Departamento> DepartamentoRepositorio => departamentoRepositorio
                                                                  ?? (departamentoRepositorio =
                                                                      new Repositorio<Departamento>(_context));

        // ============================================================================================================ //

        private IRepositorio<Localidad> localidadRepositorio;

        public IRepositorio<Localidad> LocalidadRepositorio => localidadRepositorio
                                                               ?? (localidadRepositorio =
                                                                   new Repositorio<Localidad>(_context));

        // ============================================================================================================ //

        private IRepositorio<CondicionIva> condicionIvaRepositorio;

        public IRepositorio<CondicionIva> CondicionIvaRepositorio => condicionIvaRepositorio
                                                                     ?? (condicionIvaRepositorio =
                                                                         new Repositorio<CondicionIva>(_context));

        // ============================================================================================================ //

        private IRepositorio<Marca> marcaRepositorio;

        public IRepositorio<Marca> MarcaRepositorio => marcaRepositorio
                                                       ?? (marcaRepositorio =
                                                           new Repositorio<Marca>(_context));

        // ============================================================================================================ //

        private IRepositorio<Rubro> rubroRepositorio;

        public IRepositorio<Rubro> RubroRepositorio => rubroRepositorio
                                                       ?? (rubroRepositorio =
                                                           new Repositorio<Rubro>(_context));

        // ============================================================================================================ //

        private IRepositorio<UnidadMedida> unidadMedidaRepositorio;

        public IRepositorio<UnidadMedida> UnidadMedidaRepositorio => unidadMedidaRepositorio
                                                                     ?? (unidadMedidaRepositorio =
                                                                         new Repositorio<UnidadMedida>(_context));

        // ============================================================================================================ //

        private IRepositorio<Iva> ivaRepositorio;

        public IRepositorio<Iva> IvaRepositorio => ivaRepositorio
                                                   ?? (ivaRepositorio =
                                                       new Repositorio<Iva>(_context));

        // ============================================================================================================ //

        private IRepositorio<Deposito> depositoRepositorio;

        public IRepositorio<Deposito> DepositoRepositorio => depositoRepositorio
                                                             ?? (depositoRepositorio =
                                                                 new Repositorio<Deposito>(_context));

        // ============================================================================================================ //

        private IRepositorio<PuestoTrabajo> puestoTrabajoRepositorio;

        public IRepositorio<PuestoTrabajo> PuestoTrabajoRepositorio => puestoTrabajoRepositorio
                                                             ?? (puestoTrabajoRepositorio =
                                                                 new Repositorio<PuestoTrabajo>(_context));

        // ============================================================================================================ //

        private IRepositorio<MotivoBaja> motivoBajaRepositorio;

        public IRepositorio<MotivoBaja> MotivoBajaRepositorio => motivoBajaRepositorio
                                                                   ?? (motivoBajaRepositorio =
                                                                       new Repositorio<MotivoBaja>(_context));

        // ============================================================================================================ //
        
        private IRepositorio<Banco> bancoRepositorio;

        public IRepositorio<Banco> BancoRepositorio => bancoRepositorio
                                                                   ?? (bancoRepositorio =
                                                                       new Repositorio<Banco>(_context));

        // ============================================================================================================ //
        private IRepositorio<Tarjeta> tarjetaRepositorio;

        public IRepositorio<Tarjeta> TarjetaRepositorio => tarjetaRepositorio
                                                                   ?? (tarjetaRepositorio =
                                                                       new Repositorio<Tarjeta>(_context));

        // ============================================================================================================ //
        private IRepositorio<ConceptoGasto> conceptoGastoRepositorio;

        public IRepositorio<ConceptoGasto> ConceptoGastoRepositorio => conceptoGastoRepositorio
                                                                   ?? (conceptoGastoRepositorio =
                                                                       new Repositorio<ConceptoGasto>(_context));

        // ============================================================================================================ //
        private IRepositorio<Gasto> gastoRepositorio;

        public IRepositorio<Gasto> GastoRepositorio => gastoRepositorio
                                                                   ?? (gastoRepositorio =
                                                                       new Repositorio<Gasto>(_context));

        // ============================================================================================================ //

       

    }
}
