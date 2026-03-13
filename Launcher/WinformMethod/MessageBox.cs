using AutobaseApp.WinformMethod;
using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalMain;



using Avalonia.Controls.ApplicationLifetimes;


namespace AutobaseApp.WinformMethod
{
    public class MessageBox
    {

        //public static async Task<DialogResult> Show(Window owner, string text, string caption)
        //{
        //    return await chk_confirm_event(owner, text, caption);
        //}

        ////public static async Task<DialogResult> Show(string text)
        ////{
        ////    Window? currentwindow = ScadaX.App.MainAppWindow;
        ////    string buf = "";
        ////    return await chk_confirm_event(currentwindow, text, buf);
        ////}

        //public static async Task<DialogResult> Show(string text, Window window)
        //{
            
        //    string buf = "";
        //    return await chk_confirm_event(window, text, buf);
        //}

        ////public static async Task<DialogResult> Show(string text, string caption)
        ////{
        ////    Window? currentwindow = ScadaX.App.MainAppWindow;

        ////   return await chk_confirm_event(currentwindow, text, caption);
        ////}

        //public static async Task<DialogResult> Show(string text, string caption, Window window)
        //{
            
        //  return await chk_confirm_event(window, text, caption);
        //}

        //public static async Task<DialogResult> Show(string text, string caption, MessageBoxButtons button)
        //{
        //    Window? currentwindow = ScadaX.App.MainAppWindow;

        //    return await chk_confirm_event(currentwindow, text, caption, button);

            
        //}

        public static async Task<DialogResult> Show(string text, string caption, MessageBoxButtons button, Window _owner)
        {
            Window? currentwindow = _owner;

            return await chk_confirm_event(currentwindow, text, caption, button);


        }

        public static async Task<DialogResult> Show(Window window, string text, string caption, MessageBoxButtons button)
        {
            Window? currentwindow = window;

            return await chk_confirm_event(currentwindow, text, caption, button);


        }


        //public static async Task<DialogResult> chk_confirm_event(Window parentWindow, string _str, string caption)
        //{
        //    if (parentWindow == null) return DialogResult.None;
        //    if (caption.Length == 0) caption = "Message";

        //    //var result = await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        //    //{

        //    //    ContentTitle = caption,
        //    //    ContentMessage = _str.ToString(),
        //    //    FontFamily = new Avalonia.Media.FontFamily("Nanum Gothic"),
        //    //    ButtonDefinitions = ButtonEnum.YesNo, // Choose button configuration
        //    //    Icon = Icon.Database,
        //    //    WindowStartupLocation = WindowStartupLocation.CenterOwner,
        //    //    SystemDecorations = SystemDecorations.None,
        //    //    Topmost = true,
        //    //    CanResize = false,





        //    //}).ShowWindowDialogAsync(parentWindow);

        //    FormMessageBox msbox = new FormMessageBox(parentWindow, _str, caption, FormMessageBox.Icons.Warning, MessageBoxButtons.OK, ConfigViewMain.fontMain.fontAvalonia.FontFamily);
        //    //var result = await msbox.ShowDialog<DialogResult>(parentWindow);

        //    var result = await DialogHelper.ShowDialogMessageBoxModalAsync(msbox, parentWindow);


        //    if (result == DialogResult.Yes) return DialogResult.Yes;
        //    else if (result == DialogResult.OK) return DialogResult.OK;
        //    else if (result == DialogResult.No) return DialogResult.No;
        //    else if (result == DialogResult.Cancel) return DialogResult.Cancel;
        //    else return DialogResult.None;
        //}

        public static async Task<DialogResult> chk_confirm_event(Window parentWindow, string _str, string caption, MessageBoxButtons button)
        {
            if (parentWindow == null) return DialogResult.None;

            if (caption.Length == 0) caption = "Message";

            //var result = await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            //{

            //    ContentTitle = caption,
            //    ContentMessage = _str.ToString(),
            //    FontFamily = new Avalonia.Media.FontFamily("Nanum Gothic"),
            //    ButtonDefinitions = ButtonEnum.YesNo, // Choose button configuration
            //    Icon = Icon.Database,
            //    WindowStartupLocation = WindowStartupLocation.CenterOwner,
            //    SystemDecorations = SystemDecorations.None,
            //    Topmost = true,
            //    CanResize = false,





            //}).ShowWindowDialogAsync(parentWindow);

            FormMessageBox msbox = new FormMessageBox(parentWindow, _str, caption, FormMessageBox.Icons.Warning, button, null);

            var result = await msbox.ShowDialog<DialogResult>(parentWindow);
            //var result = await DialogHelper.ShowDialogMessageBoxModalAsync(msbox, parentWindow);

            if (result == DialogResult.Yes) return DialogResult.Yes;
            else if (result == DialogResult.No) return DialogResult.No;
            else if (result == DialogResult.OK) return DialogResult.OK;
            else if (result == DialogResult.Cancel) return DialogResult.Cancel;
            else return DialogResult.None;
        }





    }


}
