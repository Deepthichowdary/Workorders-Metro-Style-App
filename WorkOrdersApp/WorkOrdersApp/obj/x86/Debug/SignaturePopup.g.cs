﻿

#pragma checksum "C:\Users\talla_000\Desktop\WorkOrdersApp (2)\WorkOrdersApp\WorkOrdersApp\WorkOrdersApp\SignaturePopup.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5A01CFA8398DFE56979B0E876E589FBD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkOrdersApp
{
    partial class SignaturePopup : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 29 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerWheelChanged += this.panelcanvas_PointerWheelChanged;
                 #line default
                 #line hidden
                #line 29 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerPressed += this.InkCanvas_PointerPressed;
                 #line default
                 #line hidden
                #line 29 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerMoved += this.InkCanvas_PointerMoved;
                 #line default
                 #line hidden
                #line 29 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerReleased += this.InkCanvas_PointerReleased;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 32 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BtnSave_OnClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 33 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BtnErase_OnClick;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 22 "..\..\..\SignaturePopup.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BtnClose_OnClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


