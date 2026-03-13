using System;
using System.Data;
using AutoLibLocal;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for change_SelectionBufAnalysis.
	/// </summary>
	public class change_SelectionBufAnalysis
	{
		public int maxReadCount = BasicRptTool.nMaxSaveChangeData;

		public change_SelectionBufAnalysis()
		{
			//
			// TODO: Add constructor logic here
			//			
		}

		public int changeSelectionCount(string buf)
		{
			CommaBlockString comma = new CommaBlockString();
			int		count = 0;
			
			comma.Set(buf);
			while(true) 
			{
				if(comma.IsEOS()) break;
				comma.GetString(ref BasicRptTool.sSaveChangeData[count]);
				if(BasicRptTool.sSaveChangeData.Length <= 0) continue;		// 읽은 문자열이 없다
				//if(BasicRptTool.sSaveChangeData[count][0] != '$') return 0;			// 읽은 문자열이 $로 시작되지않을경우 return 0
				count++;
				if(count >= maxReadCount) return maxReadCount;			// 최대 50개 이하의 선택 개수만..
			}
			return count;
		}

		public int changeSelectionGetStart(int pos, bool bX)
		{
			if(pos >= maxReadCount) return 0;

			if(BasicRptTool.sSaveChangeData[pos].Length <= 2) return -1;
			if(String.Compare(BasicRptTool.sSaveChangeData[pos], 0, "1:65536", 0, 7) == 0) return 0;// 모든 행,셀 선택

			CommaBlockString comma = new CommaBlockString();
			string			buf = "";

			comma.SetBlockCode(':');
			comma.Set(BasicRptTool.sSaveChangeData[pos]);
			if(comma.IsEOS()) return -1;
			comma.GetString(ref buf);			
			return readXYPosition(buf, bX);
		}

		public int changeSelectionGetEnd(int pos, bool bX)
		{
			if(pos >= maxReadCount) return -1;

			if(BasicRptTool.sSaveChangeData[pos].Length <= 2) return -1;
			if(String.Compare(BasicRptTool.sSaveChangeData[pos], 0, "1:65536", 0, 7) == 0) // 모든 행,셀 선택
			{
				if(bX) return 128;//255;
				return 128;//65535;// 엑셀이 너무 늦어지므로
			}

			CommaBlockString comma = new CommaBlockString();
			string			buf = "";

			comma.SetBlockCode(':');
			comma.Set(BasicRptTool.sSaveChangeData[pos]);
			if(comma.IsEOS()) return -1;
			comma.GetString(ref buf);
			if(comma.IsEOS()) return -1;
			comma.GetString(ref buf);			
			return readXYPosition(buf, bX);		
		}

		int readXYPosition(string buf, bool bX)
		{			
			if(buf.Length <= 2 || buf[0] != '$') return -1;
			if(checkColumnCharacter(buf[1]) == false) return -1;	// 첫 글자는 문자라야 한다.

			int		x, y;
            int pos;

			x = buf[1] - 'A';					// 0번 부터
            for (pos = 2; pos < buf.Length; pos++)
            {
                if (!checkColumnCharacter(buf[pos])) 	// 문자가 A~Z 글자가 아니면 
                    break;
                
                x = (x + 1) * 26 + buf[pos] - 'A';
            }

			try 
			{
				y = ConvertTool.ToInt32(buf.Substring(pos));
				y --;
			}
			catch 
			{
				return -1;
			}
			if(x < 0 || y < 0) return -1;

            // Office 2007 부터는 x = 2^14=16,384, y = 2^20 = 1,048,576 로 크게 확장되었다.
            if (bX) return x;       
            return y;

			//if(bX) return x % 256;
			//return y % 65536;
		}


		bool checkColumnCharacter(char dat)
		{
			if(dat >= 'A' && dat <= 'Z') return true;
			return false;
		}

	}
}
