﻿#pragma checksum "..\..\..\FixerBox.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CCDAFE90F4CC03BD4207E8425FB1A526AD301661"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Paneless;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace Paneless {
    
    
    /// <summary>
    /// FixerBox
    /// </summary>
    public partial class FixerBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 55 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image FixerImg;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FixerTitle;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border FixedButton;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border FixButton;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel FixerTags;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\FixerBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FixerDesc;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Paneless;component/fixerbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\FixerBox.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.FixerImg = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.FixerTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.FixedButton = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            
            #line 63 "..\..\..\FixerBox.xaml"
            ((System.Windows.Shapes.Rectangle)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 64 "..\..\..\FixerBox.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 65 "..\..\..\FixerBox.xaml"
            ((System.Windows.Controls.Label)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.FixButton = ((System.Windows.Controls.Border)(target));
            return;
            case 8:
            
            #line 70 "..\..\..\FixerBox.xaml"
            ((System.Windows.Shapes.Rectangle)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 71 "..\..\..\FixerBox.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 72 "..\..\..\FixerBox.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FixBtnClick);
            
            #line default
            #line hidden
            return;
            case 11:
            this.FixerTags = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 12:
            this.FixerDesc = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

