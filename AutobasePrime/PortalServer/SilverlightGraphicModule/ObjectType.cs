using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightGraphicModule
{
    public enum EnumObjectType
    {
        Unknown,
        AnalogMeter,
        AnalogRectangle,
        AnalogRotate,
        AnalogStatus,
        AnalogString,
        Animation,
        Bitmap,
        ButtonDigitalOut,
        ButtonModule3D,
        ButtonModuleHide,
        ButtonProgramm,
        Circle,
        Clock,
        ControlCheckBox,
        ControlComboBox,
        ControlEditBox,
        ControlListBox,
        ControlRadioButton,
        ControlDatePicker,
        Curve,
        Date,
        DigitalAnimation,
        DigitalCircle,
        DigitalRectangle,
        DigitalString,
        Group,
        Line,
        Module,
        MultiGraph,
        MultiTrend,
        Poly,
        Rectangle,
        Root,
        RoundRectangle,
        SingleText,
        StringString,
        Text,
        DatabaseTrend,
        Database,
        WindowAlarm,
        DemandWindow,
        MilliDataWindow,
        MilliDataTrend,
        RealTimeTestGraph,
        XYGraph,
        ChangeValueDisplay,	// 값 변환 표시 오브젝트
        WebBrowser,
        Layer,
        TagAnimation,
    }
    /// <summary>
    /// Summary description for ObjectType.
    /// </summary>
    //[Serializable]
    public class ObjectType
    {
        public ObjectType()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public EnumObjectType enumObjectType = EnumObjectType.Unknown;
        public bool bOnStudioLocked = false;    // 스튜디오에서 편집 시에만 사용하는 기능
        public bool bOnStudioVisible = true;    // 스튜디오에서 편집 시에만 사용하는 기능
        //public Bitmap previewOnStudio = null;   // 스튜디오에서 편집 시에만 사용하는 미리 보기 비트맵
        public bool bOnStudioSelected = false;  // 스튜디오에서 편집 시에만 사용하는 기능
        public ObjectPublicGroupLayer parentGroupLayer = null;

        /*
        // 레이어에서 사용하는 조그만 미리보기 비트맵을 만든다.
        public virtual void MakeLayerPreview(int width, int height)
        {
            previewOnStudio = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(previewOnStudio);
            g.FillRectangle(Brushes.DarkGray, 0, 0, width, height);
        }

        public EnumNotType GetNotType()
        {
            switch (enumObjectType)
            {
                case EnumObjectType.Line:
                    return EnumNotType.NOT_TYPE_LINE;
                case EnumObjectType.Rectangle:
                case EnumObjectType.RoundRectangle:
                case EnumObjectType.ButtonModule3D:
                case EnumObjectType.ButtonModuleHide:
                case EnumObjectType.ButtonProgramm:
                case EnumObjectType.ButtonDigitalOut:
                    return EnumNotType.NOT_TYPE_POINT8;
                case EnumObjectType.Poly:
                    return EnumNotType.NOT_TYPE_POLY;
                case EnumObjectType.Curve:
                    return EnumNotType.NOT_TYPE_RECT;
                case EnumObjectType.Group:
                    return EnumNotType.NOT_TYPE_GROUP;
                default:
                    return EnumNotType.NOT_TYPE_RECT;
            }
        }
         */
    }
}
