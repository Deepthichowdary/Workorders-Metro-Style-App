﻿

#pragma checksum "C:\Users\SRUTHI NARESH\Documents\Fall 2014\project\Midterm\WorkOrdersApp\WorkOrdersApp\WorkOrdersApp\SplitPage1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "98BA16FF83A739963323CCFF9A4D66E6"
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
    partial class SplitPage1 : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 80 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.ItemListView_SelectionChanged;
                 #line default
                 #line hidden
                #line 81 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).DragItemsStarting += this.itemListView_DragItemsStarting;
                 #line default
                 #line hidden
                #line 82 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).DragOver += this.itemListView_DragOver;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 127 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.ScrollViewer)(target)).ViewChanged += this.itemDetail_ViewChanged;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 181 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Click_Logout;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 182 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Image_Maps_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 183 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Image_Camera_Tapped;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 184 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Image_Comments_Tapped;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 185 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.buttonWithFlyout1_Click;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 195 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.help_Tapped;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 187 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.FlyoutBase)(target)).Opened += this.flyout_Opened;
                 #line default
                 #line hidden
                break;
            case 10:
                #line 190 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.submit_Click;
                 #line default
                 #line hidden
                break;
            case 11:
                #line 175 "..\..\..\SplitPage1.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.submit_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


