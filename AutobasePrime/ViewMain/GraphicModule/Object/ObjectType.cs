using System;
using System.Drawing;

namespace GraphicModule
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
        ControlTabControl,
        ControlTreeView,
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
        DataGridView,

        MultiTrend2,
        WebView,
        VLCAx,
        SVG,
        AnalogGauge, //25-02-04 추가 hsjeong
        DonutChart, //25-02-04 추가 hsjeong
        CustomChart,     //25-02-24 Chart 컨트롤 기반 사용자 지정 차트
        DemandChart,
        BarcodeDisplay,  //26-03-09 바코드/QR 표시 오브젝트
        BarcodeScanner,  //26-03-09 바코드 스캐너 상태 오브젝트
        Table            //26-03-17 테이블 오브젝트
    }
	/// <summary>
	/// Summary description for ObjectType.
	/// </summary>
	[Serializable]
	public class ObjectType
	{
		public ObjectType()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public EnumObjectType enumObjectType = EnumObjectType.Unknown;
        protected bool bSupportObjectOnCE = true;

        [NonSerialized]
        public Bitmap previewOnStudio = null;   // 스튜디오에서 편집 시에만 사용하는 미리 보기 비트맵
        public bool bOnStudioSelected = false;  // 스튜디오에서 편집 시에만 사용하는 기능

        [NonSerialized]
        public ObjectPublicGroupLayer parentGroupLayer = null;

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

        public virtual string GetObjectMainTitle()
        {
            return "";
        }
	}
}
