﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace CSF.Zpt.Cli.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class OutputMessages {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal OutputMessages() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("CSF.Zpt.Cli.Resources.OutputMessages", typeof(OutputMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string UsageStatement {
            get {
                return ResourceManager.GetString("UsageStatement", resourceCulture);
            }
        }
        
        internal static string VersionFormat {
            get {
                return ResourceManager.GetString("VersionFormat", resourceCulture);
            }
        }
        
        internal static string CannotParseOptionsError {
            get {
                return ResourceManager.GetString("CannotParseOptionsError", resourceCulture);
            }
        }
        
        internal static string HtmlAndXmlModesMutuallyExclusiveError {
            get {
                return ResourceManager.GetString("HtmlAndXmlModesMutuallyExclusiveError", resourceCulture);
            }
        }
        
        internal static string InvalidInputPathFormat {
            get {
                return ResourceManager.GetString("InvalidInputPathFormat", resourceCulture);
            }
        }
        
        internal static string CouldNotCreateContextVisitorFormat {
            get {
                return ResourceManager.GetString("CouldNotCreateContextVisitorFormat", resourceCulture);
            }
        }
        
        internal static string CouldNotCreateRenderingContextFactory {
            get {
                return ResourceManager.GetString("CouldNotCreateRenderingContextFactory", resourceCulture);
            }
        }
        
        internal static string InvalidOutputPath {
            get {
                return ResourceManager.GetString("InvalidOutputPath", resourceCulture);
            }
        }
        
        internal static string InvalidKeywordOptionFormat {
            get {
                return ResourceManager.GetString("InvalidKeywordOptionFormat", resourceCulture);
            }
        }
        
        internal static string InvalidEncoding {
            get {
                return ResourceManager.GetString("InvalidEncoding", resourceCulture);
            }
        }
        
        internal static string NoInputs {
            get {
                return ResourceManager.GetString("NoInputs", resourceCulture);
            }
        }
        
        internal static string CannotInputFromStdInAndPaths {
            get {
                return ResourceManager.GetString("CannotInputFromStdInAndPaths", resourceCulture);
            }
        }
        
        internal static string UnexpectedErrorFormat {
            get {
                return ResourceManager.GetString("UnexpectedErrorFormat", resourceCulture);
            }
        }
    }
}
