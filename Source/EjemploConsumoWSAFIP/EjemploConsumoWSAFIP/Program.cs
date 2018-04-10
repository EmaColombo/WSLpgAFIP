using ConsumoWSAFIP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploConsumoWSAFIP
{
    class Program
    {
        const string DEFAULT_SERVICIO = "wslpg"; //Siglas de servicio a consumir
        const bool DEFAULT_VERBOSE = true;

        static void Main(string[] args)
        {

#region auth
            var wsaaClient = new Wsaa.LoginCMSClient();

            string DEFAULT_URLWSAAWSDL = string.Format(wsaaClient.Endpoint.Address.Uri.ToString() + "{0}", "?wsdl"); //Toma la URL del Web Service WSAA para pasar como parámetro
            
            //TICKET DE ACCESO A WEB SERVICES
            var auth = new WSLpg.LpgAuthType();

            //ESTOS SON VALORES DE EJEMPLO HARDCODEADOS PARA TESTEAR QUE FUNCIONE ESTA APLICACION. ACÁ SE DEBE BUSCAR DESDE BD SI EXISTE ALGUN TICKET VALIDO PARA USAR 
            string tokenObtenido = "";
            string signObtenido = "";
            DateTime fechaExpiracion = Convert.ToDateTime("06/04/2018 22:44:00");

            //CUIT de quien se está conectando al Web Service (es el cuit que se puso cuando se creó el certificado digital)
            auth.cuit = 0;



            if (fechaExpiracion > DateTime.Now) //EXISTE UN TICKET ACTIVO Y VALIDO
            {
                auth.token = tokenObtenido;
                auth.sign = signObtenido;
            }
            else //NO EXISTE TICKET ACTIVO Y VALIDO. SE DEBE VOLVER A CONSUMIR EL WEB SERVICE WSAA PARA OBTENER NUEVO ACCESO
            {
                Autenticacion.LoginTicket ticket = new Autenticacion.LoginTicket(); //Clase desde donde se consume el WSAA.
                string serialCertificado = "2182408960951dd6"; //ESTE VALOR DEBERÍA SER GUARDADO Y OBTENIDO DESDE BD. ES EL SERIAL DEL CERTIFICADO DIGITAL
                TicketResponse resultado = ticket.ObtenerLoginTicketResponse(DEFAULT_SERVICIO, DEFAULT_URLWSAAWSDL, "", DEFAULT_VERBOSE, serialCertificado);
                //NUEVO TICKET DE ACCESO VALIDO POR 12 HORAS. GUARDAR EN BD Y UTILIZARLO HASTA QUE EXPIRE
                auth.token = resultado.Token; 
                auth.sign = resultado.Sign;
            }

            #endregion

            //VALOR DE COE OBTENIDO DESDE ARCHIVO. DEBE SER UN BUCLE SI ES MAS DE UN COE.
            long coe = 0;

            //GENERAR ARCHIVO PDF?. SI NO SE USA SE PUEDE PONER WSLpg.LpgSiNoType.N.

            try
            {
                var webServiceClient = new WSLpg.LpgPortTypeClient(); //INSTANCIA DE CLIENTE PARA CONSUMIR METODOS

                var estadoServicios = webServiceClient.dummy(); //DUMMY() ES EL METODO PARA CONSULTAR ESTADO DE SERVIDORES DE AFIP. ES EL UNICO QUE NO RECIBE PARAMETROS

                if(estadoServicios.appserver == "OK" && estadoServicios.authserver == "OK" && estadoServicios.dbserver == "OK")
                {
                    var program = new Program();
                    //bool lpg = program.LiquidacionPrimaria(auth, webServiceClient);
                    //bool lsg = program.LiquidacionSecundaria(auth, webServiceClient);
                    //bool cg = program.Certificado(auth, webServiceClient);
                    //bool ajusteunificado = program.AjusteUnificado(auth, webServiceClient);
                    //bool ajusteliquidacionsecundaria = program.AjusteLiquidacionSecundaria(auth, webServiceClient);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool LiquidacionPrimaria(WSLpg.LpgAuthType auth, WSLpg.LpgPortTypeClient webServiceClient)
        {
            var liquidacion = new WSLpg.LpgLiquidacionBaseType();
            liquidacion.actuaCorredor = WSLpg.LpgSiNoType.N;
            decimal iva = Convert.ToDecimal("10,5");
            liquidacion.actuaCorredor = WSLpg.LpgSiNoType.N;
            liquidacion.actuaCorredorSpecified = true;
            liquidacion.alicIvaOperacion = iva;
            liquidacion.alicIvaOperacionSpecified = true;
            liquidacion.campaniaPPal = 1516;
            liquidacion.codGrano = 23;
            liquidacion.codLocalidadProcedencia = 15292;
            liquidacion.codProvProcedencia = 20;
            liquidacion.codProvProcedenciaSinCertificadoSpecified = true;
            liquidacion.codProvProcedenciaSinCertificado = 20;
            liquidacion.codLocalidadProcedenciaSinCertificado = 15292;
            liquidacion.codLocalidadProcedenciaSinCertificadoSpecified = true;
            liquidacion.codPuerto = 8;
            liquidacion.codTipoOperacion = "1";
            liquidacion.contProteico = 0;
            liquidacion.contProteicoSpecified = true;
            liquidacion.comisionCorredor = 0;
            liquidacion.comisionCorredorSpecified = false;
            liquidacion.cuitComprador = 0;
            liquidacion.cuitCorredor = 20111111112;
            liquidacion.cuitCorredorSpecified = false;
            liquidacion.cuitVendedor = 20111111112;
            liquidacion.esCanje = WSLpg.LpgEsCanjeType.T;
            liquidacion.esCanjeSpecified = true;
            liquidacion.factorEnt = 100;
            liquidacion.factorEntSpecified = true;
            liquidacion.fechaPrecioOperacion = Convert.ToDateTime("19/12/2017"); //CAMBIAR FECHA
            liquidacion.liquidaCorredor = WSLpg.LpgSiNoType.N;
            liquidacion.esLiquidacionPropiaSpecified = true;
            liquidacion.esLiquidacionPropia = WSLpg.LpgSiNoType.N;
            liquidacion.nroActComprador = 29;
            liquidacion.nroIngBrutoComprador = 0;
            liquidacion.nroIngBrutoCorredor = 1006;
            liquidacion.nroIngBrutoVendedor = 0;
            liquidacion.nroOrden = webServiceClient.liquidacionUltimoNroOrdenConsultar(auth, 1).nroOrden + 1;

            liquidacion.pesoNetoSinCertificado = 20000;
            liquidacion.pesoNetoSinCertificadoSpecified = true;
            liquidacion.precioFleteTn = 0;
            liquidacion.precioRefTn = Convert.ToDecimal("2770");
            liquidacion.ptoEmision = 1;

            //var retencionIVA = new WSLpg.LpgRetencionType();
            //retencionIVA.codigoConcepto = "RI";
            //retencionIVA.alicuota = Convert.ToDecimal("10,5");
            //retencionIVA.baseCalculo = 0;
            //retencionIVA.baseCalculoSpecified = true;

            //var retencionGcia = new WSLpg.LpgRetencionType();
            //retencionGcia.codigoConcepto = "RG";
            //retencionGcia.baseCalculo = 0;
            //retencionGcia.baseCalculoSpecified = true;
            //retencionGcia.alicuota = Convert.ToDecimal("35");

            //var retencionesList = new List<WSLpg.LpgRetencionType>();
            //retencionesList.Add(retencionIVA);
            //retencionesList.Add(retencionGcia);

            List<WSLpg.LpgDeduccionType> ListaDeducciones = new List<WSLpg.LpgDeduccionType>();
            var deduccion = new WSLpg.LpgDeduccionType();
            deduccion.alicuotaIva = Convert.ToDecimal("0");
            deduccion.baseCalculo = Convert.ToDecimal("2770");
            deduccion.baseCalculoSpecified = true;
            //var deduccionesTipos = webServiceClient.tipoDeduccionConsultar(auth);
            deduccion.codigoConcepto = "OD";
            deduccion.comisionGastosAdm = 0;
            deduccion.comisionGastosAdmSpecified = true;
            deduccion.detalleAclaratorio = "A cobrar en la final";
            deduccion.diasAlmacenajeSpecified = false;
            deduccion.precioPKGdiarioSpecified = false;
            ListaDeducciones.Add(deduccion);

            var resultadoinvocacion = webServiceClient.liquidacionAutorizar(auth, liquidacion, ListaDeducciones.ToArray(), null, null);

            long COElpg = 330100046270;
            var liquidacionResponse = webServiceClient.liquidacionXCoeConsultar(auth, COElpg, WSLpg.LpgSiNoType.S); //Liquidacion primaria
            File.WriteAllBytes(String.Format("D:\\CABL\\Proyecto SAP\\GHU\\WS AFIP Granos\\LPG {0}.pdf", COElpg.ToString()), liquidacionResponse.pdf);
            return true;
        }

        private bool LiquidacionSecundaria(WSLpg.LpgAuthType auth, WSLpg.LpgPortTypeClient webServiceClient)
        {
            var liquidacionSec = new WSLpg.LsgLiqBaseType();
            liquidacionSec.actuaCorredor = WSLpg.LpgSiNoType.N;
            liquidacionSec.actuaCorredorSpecified = true;
            liquidacionSec.alicIvaOperacion = Convert.ToDecimal("10,5"); ;
            liquidacionSec.alicIvaOperacionSpecified = true;
            liquidacionSec.campaniaPPal = 1718;
            liquidacionSec.cantidadTn = Convert.ToDecimal("2,755");
            liquidacionSec.nroActVendedor = 29;
            liquidacionSec.codGrano = 15;
            liquidacionSec.codProvincia = 20;
            liquidacionSec.codLocalidad = 15292;
            liquidacionSec.codPuerto = 8;
            liquidacionSec.cuitComprador = 20111111112;
            liquidacionSec.cuitCorredor = 20111111112; //Pongo el cuit pero en la linea de abajo indico que no se usa
            liquidacionSec.cuitCorredorSpecified = false;
            liquidacionSec.cuitVendedor = 0;
            liquidacionSec.fechaPrecioOperacion = Convert.ToDateTime("31/01/2018");
            liquidacionSec.liquidaCorredor = WSLpg.LpgSiNoType.N;
            liquidacionSec.nroIngBrutoComprador = 0;
            //liquidacionSec.nroIngBrutoCorredor = 1006;
            liquidacionSec.nroIngBrutoVendedor = 0;
            liquidacionSec.nroOrden = 5;
            liquidacionSec.precioOperacion = 2600;
            liquidacionSec.precioRefTn = 2600;
            liquidacionSec.ptoEmision = 2;
            //List<WSLpg.LsgDeduccionType> ListaDeducciones = new List<WSLpg.LsgDeduccionType>();

            //var deduccion = new WSLpg.LsgDeduccionType();
            //deduccion.alicuotaIVA = Convert.ToDecimal("0");
            //deduccion.baseCalculo = Convert.ToDecimal("6200"); //2,5%
            ////var deduccionesTipos = webServiceClient.tipoDeduccionConsultar(auth);
            //deduccion.detalleAclaratoria = "A cobrar en la final";
            //ListaDeducciones.Add(deduccion);

            //liquidacionSec.deduccion = ListaDeducciones.ToArray();
            //var resultadoinvocacionlsg = webServiceClient.lsgAutorizar(auth, liquidacionSec, null);

            //long COElsg = 331000008509;
            //var liquidacionSecundariaResponse = webServiceClient.lsgConsultarXCoe(auth, COElsg, WSLpg.LpgSiNoType.S); //Liquidacion secundaria
            //File.WriteAllBytes(String.Format("D:\\CABL\\Proyecto SAP\\GHU\\WS AFIP Granos\\Liquidacion Secundaria Parcial {0}.pdf", COElsg.ToString()), liquidacionSecundariaResponse.pdf);
            return true;
        }

        private bool Certificado(WSLpg.LpgAuthType auth, WSLpg.LpgPortTypeClient webServiceClient)
        {
            var cabecera = new WSLpg.CgCabeceraAutorizarType();
            cabecera.campania = 1516;
            cabecera.codGrano = 23;
            cabecera.cuitDepositante = 20111111112;
            cabecera.cuitDepositanteSpecified = true;
            cabecera.nroIngBrutoDepositanteSpecified = true;
            cabecera.nroIngBrutoDepositante = 0;
            cabecera.nroOrden = 7;
            cabecera.ptoEmision = 5;
            cabecera.titularGrano = WSLpg.CgTipoTitularGranoType.T;
            cabecera.tipoCertificado = WSLpg.CgTipoCertificadoType.P;

            var primaria = new WSLpg.CgAutorizarPrimariaType();

            var cartaporte = new WSLpg.CgCTGType();
            cartaporte.nroCartaDePorte = 530321573;
            cartaporte.nroCTG = 9500;
            cartaporte.importeSecado = 0;
            cartaporte.importeZarandeo = 0;
            cartaporte.pesoNetoConfirmadoDefinitivo = 10000;
            cartaporte.pesoNetoMermaSecado = 0;
            cartaporte.pesoNetoMermaZarandeo = 0;
            cartaporte.porcentajeSecadoHumedad = 0;
            cartaporte.tarifaSecado = 0;
            cartaporte.tarifaZarandeo = 0;
            List<WSLpg.CgCTGType> cartaPorteList = new List<WSLpg.CgCTGType>();
            cartaPorteList.Add(cartaporte);

            primaria.ctg = cartaPorteList.ToArray();
            //var calidad = new WSLpg.CgCalidadType();
            //calidad.valorFactor = Convert.ToDecimal("111.95");
            //calidad.valorFactorSpecified = true;
            //calidad.valorContProteico = 0;
            //calidad.valorGrado = 0;
            //calidad.valorGradoSpecified = true;
            //calidad.nroBoletin = 1;
            //calidad.analisisMuestra = 1;
            //var analisisMuestra = new WSLpg.CgDetalleMuestraAnalisisType();
            //analisisMuestra.
            //calidad.detalleMuestraAnalisis = "xx";
            //primaria.calidad = calidad;
            primaria.descripcionTipoGrano = "Soja";
            primaria.montoAcarreo = 0;
            primaria.montoAlmacenaje = 0;
            primaria.montoGastosGenerales = 0;
            primaria.montoPorCadaPuntoExceso = 0;
            primaria.montoOtros = 0;
            primaria.montoSecado = 0;
            primaria.montoZarandeo = 0;
            primaria.nroActDepositario = 29;
            primaria.pesoNetoMermaVolatil = 0;
            primaria.porcentajeSecadoA = 0;
            primaria.porcentajeSecadoDe = 0;

            //var cgResult = webServiceClient.cgAutorizar(auth, cabecera, primaria, null, null, null);
            //long COECertif = 332000005371;
            //var cgResponse = webServiceClient.cgConsultarXCoe(auth, COECertif, WSLpg.LpgSiNoType.S); //Certificado
            //File.WriteAllBytes(String.Format("D:\\CABL\\Proyecto SAP\\GHU\\WS AFIP Granos\\Certificado {0}.pdf", COECertif.ToString()), cgResponse.pdf);
            return true;
        }

        private bool AjusteUnificado(WSLpg.LpgAuthType auth, WSLpg.LpgPortTypeClient webServiceClient)
        {
            var ajusteunif = new WSLpg.LpgAjusteUnifBaseType();

            var certificadoDep = new WSLpg.LpgCertType();
            certificadoDep.campania = 1516;
            certificadoDep.codLocalidadProcedencia = 15292;
            certificadoDep.codProvProcedencia = 20;
            certificadoDep.fechaCierre = Convert.ToDateTime("06/02/2018"); //CAMBIAR FECHA
            certificadoDep.pesoNeto = 10000;
            certificadoDep.pesoNetoTotalCertificado = 10000;
            certificadoDep.pesoNetoTotalCertificadoSpecified = true;
            certificadoDep.nroCertificadoDeposito = 332000005370;
            certificadoDep.tipoCertificadoDeposito = "332"; //Certificado Electrónico de Depósito

            var certificadoDep2 = new WSLpg.LpgCertType();
            certificadoDep2.campania = 1516;
            certificadoDep2.codLocalidadProcedencia = 15292;
            certificadoDep2.codProvProcedencia = 20;
            certificadoDep2.fechaCierre = Convert.ToDateTime("06/02/2018"); //CAMBIAR FECHA
            certificadoDep2.pesoNeto = 10000;
            certificadoDep2.pesoNetoTotalCertificado = 10000;
            certificadoDep2.pesoNetoTotalCertificadoSpecified = true;
            certificadoDep2.nroCertificadoDeposito = 332000005371;
            certificadoDep2.tipoCertificadoDeposito = "332"; //Certificado Electrónico de Depósito

            List<WSLpg.LpgCertType> listCertifDep = new List<WSLpg.LpgCertType>();
            listCertifDep.Add(certificadoDep);
            listCertifDep.Add(certificadoDep2);
            ajusteunif.certificados = listCertifDep.ToArray();
            ajusteunif.ptoEmision = 6;
            ajusteunif.nroOrden = 5;
            ajusteunif.coeAjustado = 330100046270;
            ajusteunif.codProv = 20;
            ajusteunif.codLocalidad = 15292;


            var ajustedebito = new WSLpg.LpgAjusteDebitoType();
            ajustedebito.codGrado = "G1";
            ajustedebito.diferenciaPrecioFleteTn = 0;
            ajustedebito.diferenciaPesoNeto = 0;
            ajustedebito.diferenciaPrecioOperacion = 0;
            ajustedebito.conceptoImporteIva0 = "5% Restante";
            ajustedebito.importeAjustarIva0 = Convert.ToDecimal("2770");
            ajustedebito.importeAjustarIva0Specified = true;

            //List<WSLpg.LpgDeduccionType> ListaDedicciones = new List<WSLpg.LpgDeduccionType>();
            //var deducciones = new WSLpg.LpgDeduccionType();
            //deducciones.alicuotaIva = 0;
            //deducciones.baseCalculo = Convert.ToDecimal("231");
            //deducciones.baseCalculoSpecified = true;
            //deducciones.codigoConcepto = "OD";
            //deducciones.detalleAclaratorio = "HONORARIOS CAMARA";
            //deducciones.comisionGastosAdm = 0;
            //deducciones.comisionGastosAdmSpecified = true;
            //deducciones.diasAlmacenajeSpecified = false;
            //deducciones.precioPKGdiarioSpecified = false;

            //var deducciones2 = new WSLpg.LpgDeduccionType();
            //deducciones2.alicuotaIva = Convert.ToDecimal("10,5");
            //deducciones2.baseCalculo = Convert.ToDecimal("2500");
            //deducciones2.baseCalculoSpecified = true;
            //deducciones2.codigoConcepto = "OD";
            //deducciones2.detalleAclaratorio = "G3 59111";
            //deducciones2.comisionGastosAdm = 0;
            //deducciones2.comisionGastosAdmSpecified = true;
            //deducciones2.diasAlmacenajeSpecified = false;
            //deducciones2.precioPKGdiarioSpecified = false;


            //ListaDedicciones.Add(deducciones);
            //ListaDedicciones.Add(deducciones2);
            //ajustedebito.deducciones = ListaDedicciones.ToArray();


            var ajustecredito = new WSLpg.LpgAjusteCreditoType();
            ajustecredito.codGrado = "G1";
            ajustecredito.diferenciaPrecioFleteTn = 0;
            ajustecredito.diferenciaPesoNeto = 0;
            ajustecredito.diferenciaPrecioOperacion = 0;
            ajustecredito.conceptoImporteIva0 = "HONORARIOS CAMARA";
            ajustecredito.importeAjustarIva0 = Convert.ToDecimal("231");
            ajustecredito.importeAjustarIva0Specified = true;
            ajustecredito.conceptoImporteIva105 = "G3 59111";
            ajustecredito.importeAjustarIva105 = 4500;
            ajustecredito.importeAjustarIva105Specified = true;
            ajustecredito.importeAjustarIva21Specified = false;
            //List<WSLpg.LpgCertPesoAjusteType> certifList = new List<WSLpg.LpgCertPesoAjusteType>();
            //var certif = new WSLpg.LpgCertPesoAjusteType();
            //certif.coe = 332000005358;
            //certif.pesoAjustado = 0;
            //certifList.Add(certif);
            //ajustedebito.certificados = certifList.ToArray();

            //var resultadoajuste = webServiceClient.liquidacionAjustarUnificado(auth, ajusteunif, ajustecredito, ajustedebito);
            //long COEAjusteUnif = 330100046271;
            //var ajusteResponse = webServiceClient.ajusteXCoeConsultar(auth, COEAjusteUnif, WSLpg.LpgSiNoType.S); //Liquidacion con ajuste unificado
            //File.WriteAllBytes(String.Format("D:\\CABL\\Proyecto SAP\\GHU\\WS AFIP Granos\\Ajuste Unificado {0}.pdf", COEAjusteUnif.ToString()), ajusteResponse.pdf);

            return true;
        }

        private bool AjusteLiquidacionSecundaria(WSLpg.LpgAuthType auth, WSLpg.LpgPortTypeClient webServiceClient)
        {
            //var ajustedebitoSecundaria = new WSLpg.LsgAjusteBaseReqType();
            //ajustedebitoSecundaria.conceptoIva0 = "2.50% RESTANTE";
            //ajustedebitoSecundaria.importeAjustar0 = Convert.ToDecimal("692,50");
            //ajustedebitoSecundaria.importeAjustar0Specified = true;
            //ajustedebitoSecundaria.importeAjustar10Specified = false;
            //ajustedebitoSecundaria.importeAjustar21Specified = false;

            var ajustecreditoSecundaria = new WSLpg.LsgAjusteBaseReqType();
            ajustecreditoSecundaria.conceptoIva0 = "OTRAS DEDUCCIONES";
            ajustecreditoSecundaria.importeAjustar0 = Convert.ToDecimal("21,78");
            ajustecreditoSecundaria.importeAjustar0Specified = true;
            ajustecreditoSecundaria.conceptoIva10 = "CALIDAD";
            ajustecreditoSecundaria.importeAjustar10 = Convert.ToDecimal("114,61");
            ajustecreditoSecundaria.importeAjustar10Specified = true;
            ajustecreditoSecundaria.importeAjustar21Specified = false;

            long COElsg = 331000008509;
            var ajusteliquidacionsecundaria = webServiceClient.lsgAjustarXCoe(auth, COElsg, 6, 3, 15292, 20, ajustecreditoSecundaria, null, null);
            var liquidacionSecundariaResponse = webServiceClient.lsgConsultarXCoe(auth, 331000008511, WSLpg.LpgSiNoType.S); //Liquidacion secundaria
            File.WriteAllBytes(String.Format("D:\\CABL\\Proyecto SAP\\GHU\\WS AFIP Granos\\Ajuste Liquidacion Secundaria Parcial {0}.pdf", 331000008511.ToString()), liquidacionSecundariaResponse.pdf);

            return true;
        }
    }
}
