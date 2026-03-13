using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NetTools;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
    public partial class ViewListAlarmDetailConfigSearch : Form
    {
        public string sFilter;
        public DateTime tFrom;
        public DateTime tTo;

        public ViewListAlarmDetailConfigSearch()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxFilterPort, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxFilterTag, 30)) return;
            
            int year, month, day;
            sFilter = "";

            year = ConvertTool.ToInt32(this.numericUpDownFrYear.Value);
            month = ConvertTool.ToInt32(this.numericUpDownFrMonth.Value);
            day = ConvertTool.ToInt32(this.numericUpDownFrDay.Value);

            if (!TimeUtil.IsDayExist(year, month, day))
            {
                MessageBox.Show("선택한 시작 날짜가 존재하지 않습니다.", "날짜 선택 오류");
                return;
            }

            tFrom = new DateTime(year, month, day);

            year = ConvertTool.ToInt32(this.numericUpDownToYear.Value);
            month = ConvertTool.ToInt32(this.numericUpDownToMonth.Value);
            day = ConvertTool.ToInt32(this.numericUpDownToDay.Value);

            if (!TimeUtil.IsDayExist(year, month, day))
            {
                MessageBox.Show("선택한 종료 날짜가 존재하지 않습니다.", "날짜 선택 오류");
                return;
            }

            // tTo = new DateTime(year, month, day);
            // tTo 를 해당 날짜의 마지막 시간으로 설정 251016 PSU
            tTo = new DateTime(year, month, day, 23, 59, 59, 999);

            if (checkBoxFilterTag.Checked)
            {
                sFilter += CommaTextWriter.MakeString("Tag=" + textBoxFilterTag.Text) + ",";
            }
            if (checkBoxFilterPort.Checked)
            {
                sFilter += CommaTextWriter.MakeString("Port=" + textBoxFilterPort.Text) + ",";
            }
            if (checkBoxFilterAlarmType.Checked)
            {
                string imsi = "";

                for (int i = 0; i < AutoLibLocal.AlarmClass.MAX_ALARM_MSG_TYPE; i++)
                {
                    if (this.listBoxFilterAlarmType.GetSelected(i))
                    {
                        imsi += i.ToString() + ",";
                    }
                }

                sFilter += CommaTextWriter.MakeString("Type="+imsi) + ",";
            }

            bSavePort = this.checkBoxFilterPort.Checked;
            bSaveTag = this.checkBoxFilterTag.Checked;
            bSaveType = this.checkBoxFilterAlarmType.Checked;

            sSavePort = this.textBoxFilterPort.Text;
            sSaveTag = this.textBoxFilterTag.Text;

            int count = AutoLibLocal.AlarmClass.MAX_ALARM_MSG_TYPE;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                count = 6;

            for (int i = 0; i < count; i++)
            {
                bSaveTypeSelect[i] = this.listBoxFilterAlarmType.GetSelected(i);
            }

            tSaveFrom = tFrom;
            tSaveTo = tTo;

            DialogResult = DialogResult.OK;

            Close();
        }

        static bool bSaveTag = false;
        static bool bSavePort = false;
        static bool bSaveType = false;
        static string sSaveTag = "*";
        static string sSavePort = "0,1,2,3,4,5,6,7,8,9";
        static bool[] bSaveTypeSelect = new bool[AutoLibLocal.AlarmClass.MAX_ALARM_MSG_TYPE] { true, true, true, true, true, true, true, true, true, true, true, true };
        static DateTime tSaveFrom = DateTimeServer.Now, tSaveTo = DateTimeServer.Now;

        private void ViewListAlarmDetailConfigSearch_Load(object sender, EventArgs e)
        {
            string[] type = AutoLibLocal.AlarmClass.GetAlarmTypeDescription();

            int count = AutoLibLocal.AlarmClass.MAX_ALARM_MSG_TYPE;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                count = 6;

            for (int i = 0; i < count; i++) 
            {
                this.listBoxFilterAlarmType.Items.Add(type[i]);
            }

            for (int i = 0; i < count; i++)
            {
                this.listBoxFilterAlarmType.SetSelected(i, bSaveTypeSelect[i]);
            }

            DateTime t = tSaveFrom;
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownFrYear, t.Year);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownFrMonth, t.Month);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownFrDay, t.Day);

            t = tSaveTo;
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownToYear, t.Year);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownToMonth, t.Month);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownToDay, t.Day);

            this.checkBoxFilterAlarmType.Checked = bSaveType;
            this.checkBoxFilterPort.Checked = bSavePort;
            this.checkBoxFilterTag.Checked = bSaveTag;

            this.textBoxFilterPort.Text = sSavePort;
            this.textBoxFilterTag.Text = sSaveTag;

            EnableAlarmType();
            EnablePort();
            EnableTag();
        }

        private void checkBoxFilterAlarmType_CheckedChanged(object sender, EventArgs e)
        {
            EnableAlarmType();
        }

        private void checkBoxFilterPort_CheckedChanged(object sender, EventArgs e)
        {
            EnablePort();
        }

        private void checkBoxFilterTag_CheckedChanged(object sender, EventArgs e)
        {
            EnableTag();
        }

        void EnableAlarmType()
        {
            this.listBoxFilterAlarmType.Enabled = checkBoxFilterAlarmType.Checked;
        }

        void EnablePort()
        {
            this.textBoxFilterPort.Enabled = checkBoxFilterPort.Checked;
        }

        void EnableTag()
        {
            this.textBoxFilterTag.Enabled = checkBoxFilterTag.Checked;
        }


    }
}