using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio.Property
{
    public partial class PropertyPageObjectControlDatePicker : Form
    {
        MultiSelectTextBox multiSelectFormat = new MultiSelectTextBox();

        public PropertyPageObjectControlDatePicker()
        {
            InitializeComponent();

            multiSelectFormat.Add(this.textBoxFormat);
        }

        public void SetObjectArgs(ObjectArgsControlDatePicker args)
        {
            /*
            int val;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
            multiSelectStyleUppercase.Set(val);
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
            multiSelectStylePassword.Set(val);
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
            multiSelectStyleLowercase.Set(val);
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
            multiSelectStyleBorder.Set(val);*/

            multiSelectFormat.Set(args.sFormat);

        }

        public ObjectArgsControlDatePicker GetObjectArgs(ObjectArgsControlDatePicker org)
        {
            ObjectArgsControlDatePicker args = (ObjectArgsControlDatePicker)Tools.CopyObject(org);

            /*
            EnumWindowStyleFlags flags = 0;
            int val;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
            multiSelectStyleUppercase.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_UPPERCASE;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
            multiSelectStylePassword.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_PASSWORD;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
            multiSelectStyleLowercase.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_LOWERCASE;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
            multiSelectStyleBorder.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.WS_BORDER;

            args.dwWindowStyle = flags;*/
            multiSelectFormat.Get(ref args.sFormat);

            return args;
        }
    }
}
