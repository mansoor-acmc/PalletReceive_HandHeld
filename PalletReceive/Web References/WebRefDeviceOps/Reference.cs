﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.CompactFramework.Design.Data, Version 2.0.50727.1433.
// 
namespace PalletReceive.WebRefDeviceOps {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_IDeviceOps", Namespace="http://tempuri.org/")]
    public partial class DeviceOps : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public DeviceOps() {
            this.Url = "http://172.17.0.50/ServicesD365/DeviceOps.svc";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IDeviceOps/Ping", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Ping([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] DeviceMessage msg, out bool PingResult, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool PingResultSpecified) {
            object[] results = this.Invoke("Ping", new object[] {
                        msg});
            PingResult = ((bool)(results[0]));
            PingResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginPing(DeviceMessage msg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("Ping", new object[] {
                        msg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndPing(System.IAsyncResult asyncResult, out bool PingResult, out bool PingResultSpecified) {
            object[] results = this.EndInvoke(asyncResult);
            PingResult = ((bool)(results[0]));
            PingResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IDeviceOps/SaveMessage", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SaveMessage([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] DeviceMessage msg, out bool SaveMessageResult, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool SaveMessageResultSpecified) {
            object[] results = this.Invoke("SaveMessage", new object[] {
                        msg});
            SaveMessageResult = ((bool)(results[0]));
            SaveMessageResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSaveMessage(DeviceMessage msg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SaveMessage", new object[] {
                        msg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndSaveMessage(System.IAsyncResult asyncResult, out bool SaveMessageResult, out bool SaveMessageResultSpecified) {
            object[] results = this.EndInvoke(asyncResult);
            SaveMessageResult = ((bool)(results[0]));
            SaveMessageResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IDeviceOps/SaveMessages", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.datacontract.org/2004/07/SyncServices")]
        public DeviceMessage[] SaveMessages([System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.datacontract.org/2004/07/SyncServices")] DeviceMessage[] msgs) {
            object[] results = this.Invoke("SaveMessages", new object[] {
                        msgs});
            return ((DeviceMessage[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSaveMessages(DeviceMessage[] msgs, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SaveMessages", new object[] {
                        msgs}, callback, asyncState);
        }
        
        /// <remarks/>
        public DeviceMessage[] EndSaveMessages(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceMessage[])(results[0]));
        }
    }
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.datacontract.org/2004/07/SyncServices")]
    public partial class DeviceMessage {
        
        private System.DateTime dateOccurField;
        
        private bool dateOccurFieldSpecified;
        
        private string deviceIPField;
        
        private string deviceNameField;
        
        private long idField;
        
        private bool idFieldSpecified;
        
        private bool isSavedField;
        
        private bool isSavedFieldSpecified;
        
        private string messageField;
        
        private string methodNameField;
        
        private string parametersField;
        
        private string projectNameField;
        
        private string usernameField;
        
        /// <remarks/>
        public System.DateTime DateOccur {
            get {
                return this.dateOccurField;
            }
            set {
                this.dateOccurField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DateOccurSpecified {
            get {
                return this.dateOccurFieldSpecified;
            }
            set {
                this.dateOccurFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string DeviceIP {
            get {
                return this.deviceIPField;
            }
            set {
                this.deviceIPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string DeviceName {
            get {
                return this.deviceNameField;
            }
            set {
                this.deviceNameField = value;
            }
        }
        
        /// <remarks/>
        public long ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IDSpecified {
            get {
                return this.idFieldSpecified;
            }
            set {
                this.idFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public bool IsSaved {
            get {
                return this.isSavedField;
            }
            set {
                this.isSavedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsSavedSpecified {
            get {
                return this.isSavedFieldSpecified;
            }
            set {
                this.isSavedFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MethodName {
            get {
                return this.methodNameField;
            }
            set {
                this.methodNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Parameters {
            get {
                return this.parametersField;
            }
            set {
                this.parametersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string ProjectName {
            get {
                return this.projectNameField;
            }
            set {
                this.projectNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Username {
            get {
                return this.usernameField;
            }
            set {
                this.usernameField = value;
            }
        }
    }
}
