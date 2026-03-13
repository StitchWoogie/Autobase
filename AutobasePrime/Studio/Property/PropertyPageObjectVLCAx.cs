using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio
{
    public partial class PropertyPageObjectVLCAx : Form
    {
        MultiSelectTextBox multiSelectMRL = new MultiSelectTextBox();
        MultiSelectTextBox multiSelectOptions = new MultiSelectTextBox();
        MultiSelectCheckBox multiSelectAutoPlay = new MultiSelectCheckBox();
        MultiSelectCheckBox multiSelectAutoLoop = new MultiSelectCheckBox();
        MultiSelectNumericUpDown multiSelectVolume = new MultiSelectNumericUpDown();


        public PropertyPageObjectVLCAx()
        {
            InitializeComponent();

            // Load가 호출되지 않으므로 여기서 등록
            this.multiSelectMRL.Add(this.textBox_mrl);
            this.multiSelectOptions.Add(this.textBox_options);
            this.multiSelectAutoPlay.Add(this.checkBox_autoplay);
            this.multiSelectAutoLoop.Add(this.checkBox_autoloop);
            this.multiSelectVolume.Add(this.numericUpDown_volume);
        }

        private void PropertyPageObjectVLCAx_Load(object sender, EventArgs e)
        {

        }

        public void Set(ObjectArgsVLCAx obj)
        {
            this.multiSelectMRL.Set(obj.mrl);
            this.multiSelectOptions.Set(obj.options);
            this.multiSelectAutoPlay.Set(obj.autoplay);
            this.multiSelectAutoLoop.Set(obj.autoloop);
            this.multiSelectVolume.Set(obj.volume);
        }

        public void Get(ObjectArgsVLCAx obj)
        {
            this.multiSelectMRL.Get(ref obj.mrl);
            this.multiSelectOptions.Get(ref obj.options);
            this.multiSelectAutoPlay.Get(ref obj.autoplay);
            this.multiSelectAutoLoop.Get(ref obj.autoloop);
            this.multiSelectVolume.Get(ref obj.volume);
        }
    }
}