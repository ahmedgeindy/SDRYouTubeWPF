﻿#pragma checksum "..\..\..\..\Windows\UserControls\InComment.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "517C2DCDB5CFB33CFA24E66D4ABAD42B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.UserControls;
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


namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.UserControls {
    
    
    /// <summary>
    /// InComment
    /// </summary>
    public partial class InComment : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 146 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid CommentBox;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel CommentDetails;
        
        #line default
        #line hidden
        
        
        #line 246 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ReplyButton;
        
        #line default
        #line hidden
        
        
        #line 250 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ReplyTextBlock;
        
        #line default
        #line hidden
        
        
        #line 259 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DeleteButton;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\..\..\Windows\UserControls\InComment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeleteTextBlock;
        
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
            System.Uri resourceLocater = new System.Uri("/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/windows/usercontrols/incomm" +
                    "ent.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\UserControls\InComment.xaml"
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
            this.CommentBox = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.CommentDetails = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.ReplyButton = ((System.Windows.Controls.Button)(target));
            
            #line 245 "..\..\..\..\Windows\UserControls\InComment.xaml"
            this.ReplyButton.Click += new System.Windows.RoutedEventHandler(this.ReplyButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ReplyTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.DeleteButton = ((System.Windows.Controls.Button)(target));
            
            #line 261 "..\..\..\..\Windows\UserControls\InComment.xaml"
            this.DeleteButton.Click += new System.Windows.RoutedEventHandler(this.DeleteButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.DeleteTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

