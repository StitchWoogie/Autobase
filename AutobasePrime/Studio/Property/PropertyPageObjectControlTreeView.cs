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
using Studio.Script;
using AutoLibLocal;

namespace Studio.Property
{
    public partial class PropertyPageObjectControlTreeView : Form
    {
        ScriptClass scriptEventDoubleClick;

        MultiSelectCheckBox multiSelectStyleBorder = new MultiSelectCheckBox();

        public PropertyPageObjectControlTreeView()
        {
            InitializeComponent();

            multiSelectStyleBorder.Add(this.checkBoxStyleBorder);
        }

        public void SetObjectArgs(ObjectArgsControlTreeView args)
        {
            scriptEventDoubleClick = args.scriptEventDoubleClick;

            int val;

            /*
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
            multiSelectStyleUppercase.Set(val);
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
            multiSelectStylePassword.Set(val);
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
            multiSelectStyleLowercase.Set(val);*/
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
            multiSelectStyleBorder.Set(val);

            //multiSelectFormat.Set(args.sFormat);

        }

        public ObjectArgsControlTreeView GetObjectArgs(ObjectArgsControlTreeView org)
        {
            ObjectArgsControlTreeView args = (ObjectArgsControlTreeView)Tools.CopyObject(org);

            args.scriptEventDoubleClick = scriptEventDoubleClick;
            
            EnumWindowStyleFlags flags = 0;
            int val;

            /*
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
            multiSelectStyleUppercase.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_UPPERCASE;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
            multiSelectStylePassword.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_PASSWORD;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
            multiSelectStyleLowercase.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_LOWERCASE;
            */
            val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
            multiSelectStyleBorder.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.WS_BORDER;

            args.dwWindowStyle = flags;
            //multiSelectFormat.Get(ref args.sFormat);

            return args;
        }

        private void buttonEventDoubleClick_Click(object sender, EventArgs e)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            if (NetTools.Tools.IsLangKorean())
                dialog.SetScript("DoubleClick 스크립트 편집", scriptEventDoubleClick);
            else
                dialog.SetScript("DoubleClick Script", scriptEventDoubleClick);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptEventDoubleClick = dialog.GetScript();
            }
        }

        void SetNotSupportedIfCE(Button control)
        {
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                control.ForeColor = Color.DarkGray;

                ToolTip tt = new ToolTip();
                if(Tools.IsLangKorean())
                    tt.SetToolTip(control, "CE에서 미지원");
                else
                    tt.SetToolTip(control, "Not supported at CE");

                tt.ShowAlways = true;
            }
        }

        void SetNotSupportedIfCE(CheckBox control)
        {
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                control.ForeColor = Color.DarkGray;

                ToolTip tt = new ToolTip();
                if (Tools.IsLangKorean())
                    tt.SetToolTip(control, "CE에서 미지원");
                else
                    tt.SetToolTip(control, "Not supported at CE");

                tt.ShowAlways = true;
            }
        }

        private void PropertyPageObjectControlTreeView_Load(object sender, EventArgs e)
        {
            SetNotSupportedIfCE(this.buttonEventDoubleClick);
            SetNotSupportedIfCE(this.checkBoxStyleBorder);
        }
    }
}
