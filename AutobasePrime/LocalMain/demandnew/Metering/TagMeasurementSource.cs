using System;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class TagMeasurementSource : IMeasurementSource
	{
		private readonly string _tagName;
		private int[] _tagPos;
		private bool _resolved;

		// DI 통신상태 태그 (0=통신이상, 1=정상)
		private readonly string _commStatusTagName;
		private int[] _commStatusTagPos;

		public TagMeasurementSource(string tagName, int[] tagPos,
			string commStatusTagName = null, int[] commStatusTagPos = null)
		{
			_tagName = tagName ?? "";
			_tagPos = tagPos;
			_resolved = false;
			_commStatusTagName = commStatusTagName ?? "";
			_commStatusTagPos = commStatusTagPos;
		}

		public string TagName
		{
			get { return _tagName; }
		}

		public bool IsAvailable
		{
			get
			{
				if (string.IsNullOrEmpty(_tagName)) return false;
				if (!_resolved) TryResolve();
				return _resolved;
			}
		}

		/// <summary>
		/// 통신 품질 판단 우선순위:
		/// 1) DI 통신상태 태그가 설정된 경우 → DI curr == 0이면 Bad (통신 이상)
		/// 2) Memory/Indirect/SYSTEM 태그 → 항상 Good (외부 통신 없음)
		/// 3) PLC/DDE/OPC → DeviceQuality로 실제 통신 상태 판단
		/// </summary>
		public MeasurementReading Read()
		{
			var reading = new MeasurementReading();
			reading.Timestamp = DateTime.Now;
			reading.Quality = MeasurementQuality.Bad;

			if (string.IsNullOrEmpty(_tagName))
				return reading;

			if (!_resolved) TryResolve();
			if (!_resolved) return reading;

			try
			{
				TagAiClass ai = TagLib.GetStructAI(_tagName, ref _tagPos);
				reading.Value = ai.curr;

				// 1순위: DI 통신상태 태그가 설정된 경우
				if (!string.IsNullOrEmpty(_commStatusTagName))
				{
					reading.Quality = CheckCommStatusTag();
				}
				// 2순위: Memory(2), Indirect(3), SYSTEM(4) → 외부 통신 없음 → 항상 Good
				else if (ai.cTagLinkType == 2 || ai.cTagLinkType == 3 || ai.cTagLinkType == 4)
				{
					reading.Quality = MeasurementQuality.Good;
				}
				// 3순위: PLC(0), DDE(1), OPC(5) → DeviceQuality fallback
				else
				{
					reading.Quality = MapDeviceQuality(ai.DeviceQuality);
				}
			}
			catch
			{
				reading.Quality = MeasurementQuality.Bad;
			}

			return reading;
		}

		/// <summary>
		/// DI 통신상태 태그 체크: curr == 1이면 Good, 0이면 Bad
		/// </summary>
		private MeasurementQuality CheckCommStatusTag()
		{
			try
			{
				if (_commStatusTagPos == null) _commStatusTagPos = new int[1];
				TagDiClass di = TagLib.GetStructDI(_commStatusTagName, ref _commStatusTagPos);

				// DI curr: 1 = 통신 정상, 0 = 통신 이상
				return (di.curr == 1) ? MeasurementQuality.Good : MeasurementQuality.Bad;
			}
			catch
			{
				// DI 태그 읽기 실패 → Bad
				return MeasurementQuality.Bad;
			}
		}

		/// <summary>
		/// OPC 표준 DeviceQuality → MeasurementQuality 매핑
		/// </summary>
		private static MeasurementQuality MapDeviceQuality(EnumDeviceQuality deviceQuality)
		{
			switch (deviceQuality)
			{
				case EnumDeviceQuality.Good:
					return MeasurementQuality.Good;
				case EnumDeviceQuality.Uncertain:
					return MeasurementQuality.Stale;
				default:
					return MeasurementQuality.Bad;
			}
		}

		private void TryResolve()
		{
			if (string.IsNullOrEmpty(_tagName)) return;

			try
			{
				if (_tagPos == null) _tagPos = new int[1];
				TagLib.GetTagPosAI(_tagName, ref _tagPos);
				_resolved = (_tagPos != null && _tagPos[0] >= 0);
			}
			catch
			{
				_resolved = false;
			}
		}
	}
}
