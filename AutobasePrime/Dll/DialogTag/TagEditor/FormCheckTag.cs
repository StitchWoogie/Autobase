using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;

namespace DialogTag.TagEditor
{
    public partial class FormCheckTag : Form
    {
        public FormCheckTag()
        {
            InitializeComponent();
        }

        TagGrClass tagTemp;

        public void Set(TagGrClass taggr)
        {
            tagTemp = taggr;
        }

        void RecurseCheck()
        {

        }

        void Message(string format, params object[] args)
        {
            string buf = String.Format(format, args);

            this.listBox1.Items.Add(buf);
        }

        void Check()
        {
            TagListStruct[] list = TagLib.MakeTagList(tagTemp);
            TagDoClass dout1, dout2;
            TagDiClass di1, di2;
            TagPublicClass tp1, tp2;
            
            for (int i = 0; i < list.Length; i++)
            {
                tp1 = TagLib.GetStructPublic(list[i]);
                if (tp1.act == 0) continue;
                if (tp1.cTagLinkType != 0) continue;   // PLC_SCAN

                if (tp1.enumTagType == EnumTagType.DO)
                {
                    dout1 = (TagDoClass)tp1;

                    for (int j = i + 1; j < list.Length; j++)
                    {
                        tp2 = TagLib.GetStructPublic(list[j]);
                        if (tp2.act == 0) continue;
                        if (tp1.cTagLinkType != 0) continue;   // PLC_SCAN

                        if (tp2.enumTagType == EnumTagType.DO)
                        {
                            dout2 = (TagDoClass)tp2;

                            if (dout1.port == dout2.port &&
                                dout1.station == dout2.station &&
                                dout1.address == dout2.address &&
                                dout1.sExtraAddr == dout2.sExtraAddr &&
                                dout1.wExtraAddr == dout2.wExtraAddr)
                            {
                                Message("Tag1:{0} Tag2:{1} property same. Port:{2} Station:{3} Address:{4} Extra1:{5} Extra2:{6}", dout1.tag, dout2.tag, dout2.port, dout2.station, dout2.address, dout2.sExtraAddr, dout2.wExtraAddr);
                            }
                        }
                        else if (tp2.enumTagType == EnumTagType.DI)
                        {
                            di2 = (TagDiClass)tp2;

                            if (di2.bUseAsOutput != 1) continue;

                            if (dout1.port == di2.port &&
                                dout1.station == di2.station &&
                                dout1.address == di2.writeDo.address &&
                                dout1.sExtraAddr == di2.writeDo.sExtraAddr &&
                                dout1.wExtraAddr == di2.writeDo.wExtraAddr)
                            {
                                Message("Tag1:{0} Tag2:{1} property same. Port:{2} Station:{3} Address:{4} Extra1:{5} Extra2:{6}", dout1.tag, di2.tag, di2.port, di2.station, di2.writeDo.address, di2.writeDo.sExtraAddr, di2.writeDo.wExtraAddr);
                            }
                        }
                    }
                }
                else if (tp1.enumTagType == EnumTagType.DI)
                {
                    di1 = (TagDiClass)tp1;

                    if (di1.bUseAsOutput != 1) continue;
                    
                    for (int j = i + 1; j < list.Length; j++)
                    {
                        tp2 = TagLib.GetStructPublic(list[j]);
                        if (tp2.act == 0) continue;
                        if (tp1.cTagLinkType != 0) continue;   // PLC_SCAN

                        if (tp2.enumTagType == EnumTagType.DO)
                        {
                            dout2 = (TagDoClass)tp2;

                            if (di1.port == dout2.port &&
                                di1.station == dout2.station &&
                                di1.writeDo.address == dout2.address &&
                                di1.writeDo.sExtraAddr == dout2.sExtraAddr &&
                                di1.writeDo.wExtraAddr == dout2.wExtraAddr)
                            {
                                Message("Tag1:{0} Tag2:{1} property same. Port:{2} Station:{3} Address:{4} Extra1:{5} Extra2:{6}", di1.tag, dout2.tag, dout2.port, dout2.station, dout2.address, dout2.sExtraAddr, dout2.wExtraAddr);
                            }
                        }
                        else if (tp2.enumTagType == EnumTagType.DI)
                        {
                            di2 = (TagDiClass)tp2;

                            if (di2.bUseAsOutput != 1) continue;

                            if (di1.port == di2.port &&
                                di1.station == di2.station &&
                                di1.writeDo.address == di2.writeDo.address &&
                                di1.writeDo.sExtraAddr == di2.writeDo.sExtraAddr &&
                                di1.writeDo.wExtraAddr == di2.writeDo.wExtraAddr)
                            {
                                Message("Tag1:{0} Tag2:{1} property same. Port:{2} Station:{3} Address:{4} Extra1:{5} Extra2:{6}", di1.tag, di2.tag, di2.port, di2.station, di2.writeDo.address, di2.writeDo.sExtraAddr, di2.writeDo.wExtraAddr);
                            }
                        }
                    }
                }
            }
        }

        private void FormCheckTag_Load(object sender, EventArgs e)
        {
            Check();
        }
    }
}
