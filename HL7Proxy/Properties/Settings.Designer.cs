﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HL7Proxy.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("localhost")]
        public string ListenIP {
            get {
                return ((string)(this["ListenIP"]));
            }
            set {
                this["ListenIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("24042")]
        public string ListenPort {
            get {
                return ((string)(this["ListenPort"]));
            }
            set {
                this["ListenPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("localhost")]
        public string GatewayIP {
            get {
                return ((string)(this["GatewayIP"]));
            }
            set {
                this["GatewayIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("24041")]
        public string GatewayPort {
            get {
                return ((string)(this["GatewayPort"]));
            }
            set {
                this["GatewayPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ListenUname {
            get {
                return ((string)(this["ListenUname"]));
            }
            set {
                this["ListenUname"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ListenPwd {
            get {
                return ((string)(this["ListenPwd"]));
            }
            set {
                this["ListenPwd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string GatewayUname {
            get {
                return ((string)(this["GatewayUname"]));
            }
            set {
                this["GatewayUname"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string GatewayPwd {
            get {
                return ((string)(this["GatewayPwd"]));
            }
            set {
                this["GatewayPwd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int LogLevel {
            get {
                return ((int)(this["LogLevel"]));
            }
            set {
                this["LogLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Off")]
        public global::motCommonLib.motErrorlLevel ErrorLevel {
            get {
                return ((global::motCommonLib.motErrorlLevel)(this["ErrorLevel"]));
            }
            set {
                this["ErrorLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoTruncate {
            get {
                return ((bool)(this["AutoTruncate"]));
            }
            set {
                this["AutoTruncate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Medicine-On-Time")]
        public string Organization {
            get {
                return ((string)(this["Organization"]));
            }
            set {
                this["Organization"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MOT-HL7-Gateway")]
        public string Processor {
            get {
                return ((string)(this["Processor"]));
            }
            set {
                this["Processor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int MaxLogLines {
            get {
                return ((int)(this["MaxLogLines"]));
            }
            set {
                this["MaxLogLines"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Sunday")]
        public string FirstDayOfWeek_RxSys {
            get {
                return ((string)(this["FirstDayOfWeek_RxSys"]));
            }
            set {
                this["FirstDayOfWeek_RxSys"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Sunday")]
        public string FirstDayOfWeek_MOT {
            get {
                return ((string)(this["FirstDayOfWeek_MOT"]));
            }
            set {
                this["FirstDayOfWeek_MOT"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string SSLCert {
            get {
                return ((string)(this["SSLCert"]));
            }
            set {
                this["SSLCert"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string SSLServerPort {
            get {
                return ((string)(this["SSLServerPort"]));
            }
            set {
                this["SSLServerPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseServerSSL {
            get {
                return ((bool)(this["UseServerSSL"]));
            }
            set {
                this["UseServerSSL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string SSLClientPort {
            get {
                return ((string)(this["SSLClientPort"]));
            }
            set {
                this["SSLClientPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseClientSSL {
            get {
                return ((bool)(this["UseClientSSL"]));
            }
            set {
                this["UseClientSSL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string RxSystemHL7ID {
            get {
                return ((string)(this["RxSystemHL7ID"]));
            }
            set {
                this["RxSystemHL7ID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Unknown")]
        public global::motInboundLib.SendingApplication RxSystemType {
            get {
                return ((global::motInboundLib.SendingApplication)(this["RxSystemType"]));
            }
            set {
                this["RxSystemType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SendEOF {
            get {
                return ((bool)(this["SendEOF"]));
            }
            set {
                this["SendEOF"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DebugMode {
            get {
                return ((bool)(this["DebugMode"]));
            }
            set {
                this["DebugMode"] = value;
            }
        }
    }
}
