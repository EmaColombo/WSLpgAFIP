using System;
using System.Globalization;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace ConsumoWSAFIP
{
    public class Autenticacion
    {
        public class LoginTicket
        {
            // Entero de 32 bits sin signo que identifica el requerimiento 
            public UInt32 UniqueId;
            // Momento en que fue generado el requerimiento 
            public DateTime GenerationTime;
            // Momento en el que exoira la solicitud 
            public DateTime ExpirationTime;
            // Identificacion del WSN para el cual se solicita el TA 
            public string Service;
            // Firma de seguridad recibida en la respuesta 
            public String Sign;
            // Token de seguridad recibido en la respuesta 
            public String Token;

            public XmlDocument XmlLoginTicketRequest = null;
            public XmlDocument XmlLoginTicketResponse = null;
            public string LoginTicketRquestString;
            public string RutaDelCertificadoFirmante;
            public String XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";
            //public String token, sign;
            private bool _verboseMode = true;

            private static UInt32 _globalUniqueID = 0;

            public TicketResponse ObtenerLoginTicketResponse(string argServicio, string argUrlWsaa, string argRutaCertX509Firmante, bool argVerbose, string SerialCert)
            {
                try
                {
                    this.RutaDelCertificadoFirmante = argRutaCertX509Firmante;
                    this._verboseMode = argVerbose;
                    CertificadosX509Lib.VerboseMode = argVerbose;


                    string cmsFirmadoBase64;
                    string loginTicketResponse;

                    XmlNode xmlNodoUniqueId;
                    XmlNode xmlNodoGenerationTime;
                    XmlNode xmlNodoExpirationTime;
                    XmlNode xmlNodoService;
                    XmlNode xmlNodoSource;
                    XmlNode xmlNodoDestination;
                    X509Certificate2 certFirmante;

                    // PASO 1: Genero el Login Ticket Request 
                    try
                    {
                        XmlLoginTicketRequest = new XmlDocument();
                        XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

                        xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
                        xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
                        xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
                        xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service");

                        xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
                        xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s");
                        xmlNodoUniqueId.InnerText = Convert.ToString(_globalUniqueID);
                        xmlNodoService.InnerText = argServicio;

                        this.Service = argServicio;

                        _globalUniqueID += 1;


                        if (this._verboseMode)
                        {
                            //Console.WriteLine(XmlLoginTicketRequest.OuterXml);
                        }
                    }

                    catch (Exception excepcionAlGenerarLoginTicketRequest)
                    {
                        throw new Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message);
                    }

                    // PASO 2: Firmo el Login Ticket Request 
                    try
                    {
                        if (this._verboseMode)
                        {
                            //Console.WriteLine("***Leyendo certificado: {0}", RutaDelCertificadoFirmante);
                        }

                        certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante, SerialCert);
                        if (this._verboseMode)
                        {
                            //Console.WriteLine("***Firmando: ");
                            //Console.WriteLine(XmlLoginTicketRequest.OuterXml);
                        }

                        // Convierto el login ticket request a bytes, para firmar 
                        Encoding EncodedMsg = Encoding.UTF8;
                        byte[] msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);

                        // Firmo el msg y paso a Base64 
                        byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
                        cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
                    }

                    catch (Exception excepcionAlFirmar)
                    {
                        throw new Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
                    }

                    // PASO 3: Invoco al WSAA para obtener el Login Ticket Response 
                    try
                    {

                        EjemploConsumoWSAFIP.Wsaa.LoginCMSClient servicioWsaa = new EjemploConsumoWSAFIP.Wsaa.LoginCMSClient(); //ACÁ SE CONSUME EL WSAA
                        servicioWsaa.ClientCredentials.ClientCertificate.Certificate = certFirmante;
                        loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64);

                        if (this._verboseMode)
                        {
                            //Console.WriteLine("***LoguinTicketResponse: ");
                            //Console.WriteLine(loginTicketResponse);
                        }
                    }

                    catch (Exception excepcionAlInvocarWsaa)
                    {
                        throw new Exception("***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message);
                    }


                    // PASO 4: Analizo el Login Ticket Response recibido del WSAA 
                    try
                    {
                        XmlLoginTicketResponse = new XmlDocument();
                        XmlLoginTicketResponse.LoadXml(loginTicketResponse);

                        this.UniqueId = UInt32.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
                        this.GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
                        this.ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
                        this.Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
                        this.Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;

                    }
                    catch (Exception excepcionAlAnalizarLoginTicketResponse)
                    {
                        throw new Exception("***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message);
                    }

                    //TicketResponse ES LA CLASE GENERADA PARA DEVOLVER LOS RESULTADOS QUE NECESITAMOS
                    TicketResponse ticketResponse = new TicketResponse();
                    ticketResponse.Sign = this.Sign;
                    ticketResponse.Token = this.Token;
                    ticketResponse.ExpirationDate = this.ExpirationTime;
                    return ticketResponse;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }

    class CertificadosX509Lib
    {

        public static bool VerboseMode = false;

        /// <summary> 
        /// Firma mensaje 
        /// </summary> 
        /// <param name="argBytesMsg">Bytes del mensaje</param> 
        /// <param name="argCertFirmante">Certificado usado para firmar</param> 
        /// <returns>Bytes del mensaje firmado</returns> 
        /// <remarks></remarks> 
        public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante)
        {
            try
            {
                // Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms) 
                ContentInfo infoContenido = new ContentInfo(argBytesMsg);
                SignedCms cmsFirmado = new SignedCms(infoContenido);

                // Creo objeto CmsSigner que tiene las caracteristicas del firmante 
                CmsSigner cmsFirmante = new CmsSigner(argCertFirmante);
                cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;
                if (VerboseMode)
                {
                    //Console.WriteLine("***Firmando bytes del mensaje...");
                }
                // Firmo el mensaje PKCS #7 
                cmsFirmado.ComputeSignature(cmsFirmante);
                if (VerboseMode)
                {
                    //Console.WriteLine("***OK mensaje firmado");
                }

                // Encodeo el mensaje PKCS #7. 
                return cmsFirmado.Encode();
            }
            catch (Exception excepcionAlFirmar)
            {
                throw new Exception("***Error al firmar: " + excepcionAlFirmar.Message);
            }
        }

        /// <summary> 
        /// Lee certificado de disco 
        /// </summary> 
        /// <param name="argArchivo">Ruta del certificado a leer.</param> 
        /// <returns>Un objeto certificado X509</returns> 
        /// <remarks></remarks> 
        public static X509Certificate2 ObtieneCertificadoDesdeArchivo(string argArchivo, string serial)
        {
            X509Certificate2 objCert = new X509Certificate2();

            try
            {
                //objCert.Import(Microsoft.VisualBasic.FileIO.FileSystem.ReadAllBytes(argArchivo));

                X509Store store = new X509Store();

                //SE ESPECIFICA CARPETA DONDE ESTA INSTALADO EL CERTIFICADO
                store = new X509Store(StoreName.Root, StoreLocation.LocalMachine  /*StoreLocation.LocalMachine*/ /*.CurrentUser*/);

                store.Open(OpenFlags.ReadOnly);

                //INDICAMOS METODO DE BUSQUEDA PARA EL CERTIFICADO. POR DEFECTO SE BUSCA VIA SERIAL
                X509Certificate2Collection collectionX509 = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);



                if (collectionX509.Count == 1)
                {

                    objCert = collectionX509[0];

                }

                else
                {

                    throw new Exception("No se puede encontrar el Certificado -> collectionX509.Count: " + collectionX509.Count.ToString(CultureInfo.InvariantCulture) + "\r\n\r\nServidor: " + "\r\nUser: ");

                }

                return objCert;

            }
            catch (Exception excepcionAlImportarCertificado)
            {
                throw new Exception("argArchivo=" + argArchivo + " excepcion=" + excepcionAlImportarCertificado.Message + " " + excepcionAlImportarCertificado.StackTrace);
            }
        }
    }

    public class TicketResponse
    {
        public string Token { get; set; }
        public string Sign { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
