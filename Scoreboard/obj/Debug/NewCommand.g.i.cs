﻿#pragma checksum "..\..\NewCommand.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B6B1BA0C7332CD2B6FE17504E620957703686ED1B9DD1859DFF72C9374D0463D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Scoreboard;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Scoreboard {
    
    
    /// <summary>
    /// NewCommand
    /// </summary>
    public partial class NewCommand : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 113 "..\..\NewCommand.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbCommandName;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\NewCommand.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbCommandValue;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\NewCommand.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSaveCommand;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Scoreboard;component/newcommand.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\NewCommand.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbCommandName = ((System.Windows.Controls.TextBox)(target));
            
            #line 113 "..\..\NewCommand.xaml"
            this.tbCommandName.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbCommandName_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbCommandValue = ((System.Windows.Controls.TextBox)(target));
            
            #line 116 "..\..\NewCommand.xaml"
            this.tbCommandValue.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbCommandValue_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 116 "..\..\NewCommand.xaml"
            this.tbCommandValue.KeyDown += new System.Windows.Input.KeyEventHandler(this.tbCommandValue_KeyDown);
            
            #line default
            #line hidden
            
            #line 116 "..\..\NewCommand.xaml"
            this.tbCommandValue.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbCommandValue_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnSaveCommand = ((System.Windows.Controls.Button)(target));
            
            #line 117 "..\..\NewCommand.xaml"
            this.btnSaveCommand.Click += new System.Windows.RoutedEventHandler(this.SaveCommand);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

