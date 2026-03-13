using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using AutoLib;
using NetTools;

namespace SilverlightGraphicModule.DialogControl
{
    public partial class PageControlDigitalOut : UserControl
    {
        List<object> blockMember = new List<object>();

        public PageControlDigitalOut(List<object> array)
        {
            InitializeComponent();

            blockMember = array;

            BUTTON_DOUT_STRUCT block;
            TagPublicClass tp;

            for (int i = 0; i < blockMember.Count; i++)
            {
                block = (BUTTON_DOUT_STRUCT)blockMember[i];

                tp = TagLib.GetStructPublic(block.tag, ref block.tag_pos);

                this.list1.Items.Add(block.tag);
            }
        }

        private void buttonOFF_Click(object sender, RoutedEventArgs e)
        {
            SilverlightGraphicModule.ObjectButtonDigitalOut.ControlDoGroup(blockMember, 0, 0);
            dialogParent.Close();
        }

        private void buttonON_Click(object sender, RoutedEventArgs e)
        {
            SilverlightGraphicModule.ObjectButtonDigitalOut.ControlDoGroup(blockMember, 1, 0);
            dialogParent.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            dialogParent.Close();
        }

        SilverlightDialogControl.MyDialogCommon dialogParent;
        public void SetParent(SilverlightDialogControl.MyDialogCommon dialog)
        {
            dialogParent = dialog;
        }

        

    }
}
