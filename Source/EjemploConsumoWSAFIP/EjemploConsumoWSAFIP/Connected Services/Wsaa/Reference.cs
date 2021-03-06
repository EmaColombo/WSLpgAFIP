﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EjemploConsumoWSAFIP.Wsaa {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LoginFault", Namespace="https://wsaa.afip.gov.ar/ws/services/LoginCms")]
    [System.SerializableAttribute()]
    public partial class LoginFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://wsaa.afip.gov.ar/ws/services/LoginCms", ConfigurationName="Wsaa.LoginCMS")]
    public interface LoginCMS {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el espacio de nombres de contenedor (http://wsaa.view.sua.dvadac.desein.afip.gov) del mensaje loginCmsRequest no coincide con el valor predeterminado (https://wsaa.afip.gov.ar/ws/services/LoginCms)
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(EjemploConsumoWSAFIP.Wsaa.LoginFault), Action="", Name="fault")]
        EjemploConsumoWSAFIP.Wsaa.loginCmsResponse loginCms(EjemploConsumoWSAFIP.Wsaa.loginCmsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<EjemploConsumoWSAFIP.Wsaa.loginCmsResponse> loginCmsAsync(EjemploConsumoWSAFIP.Wsaa.loginCmsRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="loginCms", WrapperNamespace="http://wsaa.view.sua.dvadac.desein.afip.gov", IsWrapped=true)]
    public partial class loginCmsRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://wsaa.view.sua.dvadac.desein.afip.gov", Order=0)]
        public string in0;
        
        public loginCmsRequest() {
        }
        
        public loginCmsRequest(string in0) {
            this.in0 = in0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="loginCmsResponse", WrapperNamespace="http://wsaa.view.sua.dvadac.desein.afip.gov", IsWrapped=true)]
    public partial class loginCmsResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://wsaa.view.sua.dvadac.desein.afip.gov", Order=0)]
        public string loginCmsReturn;
        
        public loginCmsResponse() {
        }
        
        public loginCmsResponse(string loginCmsReturn) {
            this.loginCmsReturn = loginCmsReturn;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LoginCMSChannel : EjemploConsumoWSAFIP.Wsaa.LoginCMS, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LoginCMSClient : System.ServiceModel.ClientBase<EjemploConsumoWSAFIP.Wsaa.LoginCMS>, EjemploConsumoWSAFIP.Wsaa.LoginCMS {
        
        public LoginCMSClient() {
        }
        
        public LoginCMSClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LoginCMSClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginCMSClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginCMSClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        EjemploConsumoWSAFIP.Wsaa.loginCmsResponse EjemploConsumoWSAFIP.Wsaa.LoginCMS.loginCms(EjemploConsumoWSAFIP.Wsaa.loginCmsRequest request) {
            return base.Channel.loginCms(request);
        }
        
        public string loginCms(string in0) {
            EjemploConsumoWSAFIP.Wsaa.loginCmsRequest inValue = new EjemploConsumoWSAFIP.Wsaa.loginCmsRequest();
            inValue.in0 = in0;
            EjemploConsumoWSAFIP.Wsaa.loginCmsResponse retVal = ((EjemploConsumoWSAFIP.Wsaa.LoginCMS)(this)).loginCms(inValue);
            return retVal.loginCmsReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<EjemploConsumoWSAFIP.Wsaa.loginCmsResponse> EjemploConsumoWSAFIP.Wsaa.LoginCMS.loginCmsAsync(EjemploConsumoWSAFIP.Wsaa.loginCmsRequest request) {
            return base.Channel.loginCmsAsync(request);
        }
        
        public System.Threading.Tasks.Task<EjemploConsumoWSAFIP.Wsaa.loginCmsResponse> loginCmsAsync(string in0) {
            EjemploConsumoWSAFIP.Wsaa.loginCmsRequest inValue = new EjemploConsumoWSAFIP.Wsaa.loginCmsRequest();
            inValue.in0 = in0;
            return ((EjemploConsumoWSAFIP.Wsaa.LoginCMS)(this)).loginCmsAsync(inValue);
        }
    }
}
