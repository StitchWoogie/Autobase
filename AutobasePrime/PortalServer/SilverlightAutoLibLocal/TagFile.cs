using System;
using System.IO;
using NetTools;
using AutoLibLocal;
using System.Collections;
using System.Net;

namespace AutoLibLocal
{
    public static class WebSync
    {
        public static void connect(string filename, Action<System.Net.OpenReadCompletedEventArgs> callback)
        {
            //Exception ex = null;
            //String r = null;

            WebClient wc = new WebClient();

            wc.OpenReadCompleted += (sender, e) =>
            {
                callback(e);
            };

            wc.OpenReadAsync(new Uri(filename, UriKind.Absolute));
        }
    }

	/// <summary>
	/// 신버전 형태의 태그 파일을 읽어오고 저장하는 부분만 들어 있다.
	/// 구버전 읽어 오는 부분은 TagFileOld.cs에 있다.
	/// </summary>
	public class TagFile
	{
		public TagFile()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        
		/// <summary>
		/// 엑셀등에서 태그파일을 사용중이면 파일을 열수 없기 때문에 이부분이 추가되었다.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static StreamReader TextReaderWithCheck(string path, System.Text.Encoding encoding)
		{
			StreamReader reader;
			
			//retry:;
			try 
			{
				reader = new StreamReader(path, encoding);
			}
			catch 
			{
                /*
				DialogResult result;
				if(Tools.IsLangKorean()) 
				{
					result = MessageBox.Show("태그 파일을 열 수 없습니다.\n태그 파일을 다른 프로그램에서 사용중일 수 있습니다.\n태그파일 읽기를 재 시도할까요?", path, MessageBoxButtons.YesNo);
				}
				else 
				{
					result = MessageBox.Show("Can't Open the Tag file.\nFile is used another program.\nRetry reading?", path, MessageBoxButtons.RetryCancel);
				}

				if(result == DialogResult.Yes)	goto retry;*/

				return null;
			}

			return reader;
		}

		static void LoadToCommon(TagPublicClass tp, CommaTextReader comma)
		{
			comma.GetString(ref tp.tag);
			tp.tag = tp.tag.Trim();
			tp.name = tp.tag;
			comma.GetString(ref tp.description);
			comma.GetChar(ref tp.act);
			comma.GetBYTE(ref tp.cTagLinkType);
			comma.GetChar(ref tp.bLocalTag);

			comma.GetString(ref tp.sOpcServer);
			comma.GetString(ref tp.sOpcGroup);
			comma.GetString(ref tp.sOpcItem);
			comma.GetInt(ref tp.nOpcItemPos);

			comma.GetChar(ref tp.bUseAsOutput);	// reserved1 입/출력 겸용

            comma.GetString(ref tp.pbReserved02);	// reserved2 
            comma.GetString(ref tp.pbReserved03);	// reserved3
            comma.GetString(ref tp.pbReserved04);	// reserved4
            comma.GetString(ref tp.pbReserved05);	// reserved5
            comma.GetString(ref tp.pbReserved06);	// reserved6
            comma.GetString(ref tp.pbReserved07);	// reserved7
            comma.GetString(ref tp.pbReserved08);	// reserved8
            comma.GetString(ref tp.pbReserved09);	// reserved9
            comma.GetString(ref tp.pbReserved10);	// reserved10
		}

		public static void LoadToAI(TagAiClass ai, CommaTextReader comma)
		{
			LoadToCommon(ai, comma);		

			comma.GetInt(ref ai.port);
			comma.GetInt(ref ai.station);
			comma.GetWORD(ref ai.address);
			comma.GetString(ref ai.writeAo.sExtraAddr);	//extra1
			comma.GetWORD(ref ai.writeAo.wExtraAddr);	//extra2
			comma.GetString(ref ai.sDdeService);
			comma.GetString(ref ai.sDdeTopic);
			comma.GetString(ref ai.sDdeItem);
			comma.GetBYTE(ref ai.bDdeRequest);	

			comma.GetInt(ref ai.fn);	
			comma.GetString(ref ai.unit);	
			comma.GetFloat(ref ai.fBase);	
			comma.GetFloat(ref ai.fFull);	
			comma.GetFloat(ref ai.fPlcBase);	
			comma.GetFloat(ref ai.fPlcFull);
			comma.GetFloat(ref ai.hihi);	
			comma.GetFloat(ref ai.high);	
			comma.GetFloat(ref ai.low);	
			comma.GetFloat(ref ai.lolo);	
			comma.GetFloat(ref ai.view_full);	
			comma.GetFloat(ref ai.view_base);	
			comma.GetBYTE(ref ai.alarm);	
			comma.GetBYTE(ref ai.bFileSave);	
			comma.GetString(ref ai.sSubOutDigitalHiHi);	
			comma.GetString(ref ai.sSubOutDigitalLoLo);	
			comma.GetString(ref ai.sSubOutAnalog);	
			comma.GetString(ref ai.sSubOutAnalogSP);	
			
			comma.GetString(ref ai.sDisplayFormat);
			if(!TagUtil.StringToDisplayFormat(ai.sDisplayFormat, out ai.fDisplayFormat, out ai.cDisplayFormat))
			{
				ai.sDisplayFormat = "10.2";
			}

			comma.GetBYTE(ref ai.cAlarmType);	
			comma.GetString(ref ai.sGraphicFile);	
			comma.GetString(ref ai.sAlarmWaveFile);	
			comma.GetInt(ref ai.nCalculateFilter);	
			comma.GetBYTE(ref ai.bCutOverValue);	
			comma.GetInt(ref ai.nCalcDelay);	
			comma.GetWORD(ref ai.wAlarmPriority);	
			comma.GetBYTE(ref ai.cConfirmCount);	
			comma.GetBYTE(ref ai.bBcdValue);	
			comma.GetFloat(ref ai.fAlarmReturnGab);	
			comma.GetWORD(ref ai.wRateOfChangeLimit);	

			ushort flags = 0;
			comma.GetHexWORD(ref flags);			
			ai.wProtectFlags = (EnumProtectFlag)flags;

			comma.GetBYTE(ref ai.cAlarmProtectOnBigChangePercent);	
			comma.GetChar(ref ai.cAlarmProtectOnBigChangeSecond);	
			comma.GetWORD(ref ai.wScanTime);	

			comma.GetFloat(ref ai.fScrollUnit);
			if(ai.fScrollUnit == 0)	ai.fScrollUnit = 1;

            comma.GetInt(ref ai.nMemoryTypeSub);    // 9.3.0.3 부터 지원 - 임시버전 

            comma.GetHexDWORD(ref ai.writeAo.address);          // 출력겸용 옵션 Extra1, Extra2는 앞부분에
            comma.GetInt(ref ai.writeAo.nCalculateFilter);      
            comma.GetChar(ref ai.writeAo.cDdeDataFormat);

            comma.GetString(ref ai.sCalcScript);
            comma.GetString(ref ai.writeAo.sCalcScript);

            
		}

		public static void LoadToAO(TagAoClass ao, CommaTextReader comma)
		{
			LoadToCommon(ao, comma);		

			comma.GetInt(ref ao.port);
			comma.GetInt(ref ao.station);
			comma.GetHexDWORD(ref ao.address);
			comma.GetString(ref ao.sExtraAddr);	//extra1
			comma.GetWORD(ref ao.wExtraAddr);	//extra2
			comma.GetString(ref ao.sDdeService);
			comma.GetString(ref ao.sDdeTopic);
			comma.GetString(ref ao.sDdeItem);
            comma.GetBYTE(ref ao.bDdeRequest);	// 사용안함

			comma.GetInt(ref ao.fn);
			comma.GetString(ref ao.unit);
			comma.GetFloat(ref ao.fBase);
			comma.GetFloat(ref ao.fFull);
			comma.GetFloat(ref ao.plc_base);
			comma.GetFloat(ref ao.plc_full);
			comma.GetInt(ref ao.nCalculateFilter);
			comma.GetChar(ref ao.cDdeDataFormat);
			comma.GetChar(ref ao.bBcdValue);
			comma.GetChar(ref ao.bCutOverValue);

            comma.GetString(ref ao.sCalcScript);

            
		}

		public static void LoadToDI(TagDiClass di, CommaTextReader comma)
		{
			LoadToCommon(di, comma);
	
			comma.GetInt(ref di.port);
			comma.GetInt(ref di.station);
			
			string imsi = "";
			comma.GetString(ref imsi);
			CommaBlockString commaaddr = new CommaBlockString();
			commaaddr.SetBlockCode('.');
			commaaddr.Set(imsi);
			commaaddr.GetDWORD(ref di.address_word);

			if(imsi.Length > 0 && (imsi[imsi.Length-1] == ';')) // 10진수 표기
			{
				commaaddr.GetChar(ref di.address_bit);
			}
			else if(imsi.Length > 0 && (imsi[imsi.Length-1] == 'h' || imsi[imsi.Length-1] == 'H')) // 16진수표기
			{
				byte imsib = 0;
				commaaddr.GetHexBYTE(ref imsib);
				di.address_bit = (sbyte)imsib;
			}
			else 
			{
				commaaddr.GetChar(ref di.address_bit);
			}

            comma.GetString(ref di.writeDo.sExtraAddr);	//extra1
            comma.GetWORD(ref di.writeDo.wExtraAddr);	//extra2
			comma.GetString(ref di.sDdeService);
			comma.GetString(ref di.sDdeTopic);
			comma.GetString(ref di.sDdeItem);
			comma.GetBYTE(ref di.bDdeRequest);
	
			comma.GetInt(ref di.fn);
			comma.GetString(ref di.desON);
			comma.GetString(ref di.desOFF);
			comma.GetChar(ref di.alarm);
			comma.GetString(ref di.sSubOutDigital1);
			comma.GetString(ref di.sSubOutDigital2);
			comma.GetString(ref di.sSubOutDigitalOnTag);
			comma.GetString(ref di.sSubOutDigitalOffTag);
			comma.GetChar(ref di.bFileSave);
			comma.GetChar(ref di.cAlarmType);
			comma.GetString(ref di.sGraphicFile);
			comma.GetString(ref di.sAlarmWaveFile);
			comma.GetWORD(ref di.wAlarmPriority);
			comma.GetChar(ref di.cOutLinkMethod);
			comma.GetChar(ref di.cConfirmCount);
			
			ushort flags = 0;
			comma.GetHexWORD(ref flags);			
			di.wProtectFlags = (EnumProtectFlag)flags;
			comma.GetChar(ref di.bReverse);
			comma.GetWORD(ref di.wScanTime);

            comma.GetHexDWORD(ref di.writeDo.address);            // 출력겸용 옵션 Extra1, Extra2는 앞부분에
            comma.GetChar(ref di.writeDo.cRelayType);
            comma.GetWORD(ref di.writeDo.wRelaySecTarget);
            comma.GetInt(ref di.writeDo.nDelaySecON);
            comma.GetInt(ref di.writeDo.nDelaySecOFF);

            
		}

		public static void LoadToDO(TagDoClass dout, CommaTextReader comma)
		{
			LoadToCommon(dout, comma);

			comma.GetInt(ref dout.port);
			comma.GetInt(ref dout.station);
			comma.GetHexDWORD(ref dout.address);
			comma.GetString(ref dout.sExtraAddr);	//extra1
			comma.GetWORD(ref dout.wExtraAddr);	//extra2
			comma.GetString(ref dout.sDdeService);
			comma.GetString(ref dout.sDdeTopic);
			comma.GetString(ref dout.sDdeItem);
            comma.GetBYTE(ref dout.bDdeRequest);//사용안함

			comma.GetString(ref dout.desON);
			comma.GetString(ref dout.desOFF);
			comma.GetChar(ref dout.cRelayType);
			comma.GetWORD(ref dout.wRelaySecTarget);
			comma.GetChar(ref dout.bReverse);
			comma.GetInt(ref dout.nDelaySecON);
			comma.GetInt(ref dout.nDelaySecOFF);
		}

		public static void LoadToST(TagStClass st, CommaTextReader comma)
		{
			LoadToCommon(st, comma);		

			comma.GetInt(ref st.port);
			comma.GetInt(ref st.station);
			comma.GetWORD(ref st.address);
            comma.GetString(ref st.stReserved1);	//extra1
            comma.GetString(ref st.stReserved2);	//extra2
			comma.GetString(ref st.sDdeService);
			comma.GetString(ref st.sDdeTopic);
			comma.GetString(ref st.sDdeItem);
			comma.GetBYTE(ref st.bDdeRequest);	

			comma.GetBYTE(ref st.read_size);	
			comma.GetBYTE(ref st.read_method);	
			comma.GetChar(ref st.cMemoryType);
		}

		void LoadToGR(TagGrClass gr, CommaTextReader comma)
		{
			LoadToCommon(gr, comma);
		}

		void LoadToDoGroup(TagDoGroupClass gdo, CommaTextReader comma)
		{
			LoadToCommon(gdo, comma);

			while(true) 
			{
				DoGroupMember member = new DoGroupMember();
				comma.GetString(ref member.tag);
				member.tag = member.tag.Trim();
				if(member.tag.Length > 0) 
				{
					gdo.member.Add(member);
				}

				if(comma.IsEOS())	break;
			}
		}

        /*
        TagGrClass tagTemp;

        public bool LoadTag(string filename, TagGrClass tagtemp, System.Text.Encoding encoding, bool check_exists)
        {
            tagTemp = tagtemp;

            WebSync.connect(filename, (e) =>
                {
                    if (e.Error != null || e.Cancelled == true) return;

                    if (e.Result == null) return;

                    System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

                    TextReader reader = new StreamReader(resinfo.Stream);//. TextReaderWithCheck(filename, encoding);

                    if (reader == null) return;

                    string one_line;
                    CommaTextReader comma = new CommaTextReader();
                    string type = "";
                    TagAiClass ai;
                    TagAoClass ao;
                    TagDiClass di;
                    TagDoClass dout;
                    TagStClass st;
                    TagGrClass gr;
                    TagDoGroupClass gdo;

                    //bool check_exists = false;

                    while (true)
                    {
                        one_line = reader.ReadLine();
                        if (one_line == null) break;
                        if (one_line.Length == 0) continue;
                        if (one_line[0] == ';') continue;

                        comma.Set(one_line);
                        comma.GetString(ref type);
                        if (String.Compare(type, "AI", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            ai = new TagAiClass();
                            LoadToAI(ai, comma);
                            tagTemp.AddTag(ai, check_exists);
                        }
                        else if (String.Compare(type, "AO", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            ao = new TagAoClass();
                            LoadToAO(ao, comma);
                            tagTemp.AddTag(ao, check_exists);
                        }
                        else if (String.Compare(type, "DI", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            di = new TagDiClass();
                            LoadToDI(di, comma);
                            tagTemp.AddTag(di, check_exists);
                        }
                        else if (String.Compare(type, "DO", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            dout = new TagDoClass();
                            LoadToDO(dout, comma);
                            tagTemp.AddTag(dout, check_exists);
                        }
                        else if (String.Compare(type, "ST", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            st = new TagStClass();
                            LoadToST(st, comma);
                            tagTemp.AddTag(st, check_exists);
                        }
                        else if (String.Compare(type, "GR", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            gr = new TagGrClass();
                            LoadToGR(gr, comma);
                            tagTemp.AddTag(gr, check_exists);
                        }
                        else if (String.Compare(type, "GDO", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            gdo = new TagDoGroupClass();
                            LoadToDoGroup(gdo, comma);
                            tagTemp.AddTag(gdo, check_exists);
                        }
                        else { }
                    }

                    reader.Close();

                    TagLib.ReMakeTagList();
                    TagLib.ChangeTagNotFound();     // 그래픽 파일이나 태그 파일 중 어느것이 먼저 다운로드 끝날지 모르기 때문에 다 읽은 후 태그없음 번호를 바꿔준다.

                    if (eventHandlerOnTagFileReaded != null)
                        eventHandlerOnTagFileReaded(this, EventArgs.Empty);
                });

            int k = 10;

            return true;
        }

        public static event EventHandler eventHandlerOnTagFileReaded;*/

        
        TagGrClass tagTemp;

        public bool LoadTag(string filename, TagGrClass tagtemp, System.Text.Encoding encoding, bool check_exists)
		{
            tagTemp = tagtemp;

            System.Net.WebClient client = new System.Net.WebClient();
            client.OpenReadCompleted +=new System.Net.OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(filename, UriKind.Absolute));

            return true;
        }

        public static event EventHandler eventHandlerOnTagFileReaded;

        void client_OpenReadCompleted(object sender, System.Net.OpenReadCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null) return;

            System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

			TextReader reader = new StreamReader(resinfo.Stream);//. TextReaderWithCheck(filename, encoding);

			if(reader == null)	return;	

			string one_line;
			CommaTextReader comma = new CommaTextReader();
			string type="";
			TagAiClass ai;
			TagAoClass ao;
			TagDiClass di;
			TagDoClass dout;
			TagStClass st;
			TagGrClass gr;
			TagDoGroupClass gdo;

            bool check_exists = false;

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				if(one_line.Length == 0)	continue;
				if(one_line[0] == ';')	continue;

				comma.Set(one_line);
				comma.GetString(ref type);
				if(String.Compare(type, "AI", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					ai = new TagAiClass();
					LoadToAI(ai, comma);
					tagTemp.AddTag(ai,check_exists);
				}
                else if (String.Compare(type, "AO", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					ao = new TagAoClass();
					LoadToAO(ao, comma);
                    tagTemp.AddTag(ao, check_exists);
				}
                else if (String.Compare(type, "DI", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					di = new TagDiClass();
					LoadToDI(di, comma);
                    tagTemp.AddTag(di, check_exists);
				}
                else if (String.Compare(type, "DO", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					dout = new TagDoClass();
					LoadToDO(dout, comma);
                    tagTemp.AddTag(dout, check_exists);
				}
                else if (String.Compare(type, "ST", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					st = new TagStClass();
					LoadToST(st, comma);
                    tagTemp.AddTag(st, check_exists);
				}
                else if (String.Compare(type, "GR", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					gr = new TagGrClass();
					LoadToGR(gr, comma);
                    tagTemp.AddTag(gr, check_exists);
				}
                else if (String.Compare(type, "GDO", StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					gdo = new TagDoGroupClass();
					LoadToDoGroup(gdo, comma);
                    tagTemp.AddTag(gdo, check_exists);
				}
				else {}
			}

			reader.Close();

            TagLib.ReMakeTagList();
            TagLib.ChangeTagNotFound();     // 그래픽 파일이나 태그 파일 중 어느것이 먼저 다운로드 끝날지 모르기 때문에 다 읽은 후 태그없음 번호를 바꿔준다.

            if (eventHandlerOnTagFileReaded != null)
                eventHandlerOnTagFileReaded(this, EventArgs.Empty);
		}

        /*
        void SaveTagHeaderCommon(CommaTextWriter writer)
        {
            writer.Write(";TagType,");
            writer.Write("Tag,");
            writer.Write("Description,");
            writer.Write("Active,");
            writer.Write("LinkType,");
            writer.Write("LocalTag,");
            writer.Write("OpcServer,");
            writer.Write("OpcGroup,");
            writer.Write("OpcItem,");
            writer.Write("OpcItemPos,");

            writer.Write("UseAsBothInputOutput,");
            writer.Write("reserved2,");
            writer.Write("reserved3,");
            writer.Write("reserved4,");
            writer.Write("reserved5,");
            writer.Write("reserved6,");
            writer.Write("reserved7,");
            writer.Write("reserved8,");
            writer.Write("reserved9,");
            writer.Write("reserved10,");

            writer.Write("port,");
            writer.Write("station,");
            writer.Write("address,");
            writer.Write("extra1,");
            writer.Write("extra2,");	//extra2
            writer.Write("DdeService,");
            writer.Write("DdeTopic,");
            writer.Write("DdeItem,");
            writer.Write("DdeRequest,");
        }

        void SaveTagHeaderAI(CommaTextWriter writer)
        {
            writer.Write("fn,");
            writer.Write("unit,");
            writer.Write("fBase,");
            writer.Write("fFull,");
            writer.Write("fPlcBase,");
            writer.Write("fPlcFull,");
            writer.Write("hihi,");
            writer.Write("high,");
            writer.Write("low,");
            writer.Write("lolo,");
            writer.Write("view_full,");
            writer.Write("view_base,");
            writer.Write("alarm,");
            writer.Write("bFileSave,");
            writer.Write("sSubOutDigitalHiHi,");
            writer.Write("sSubOutDigitalLoLo,");
            writer.Write("sSubOutAnalog,");
            writer.Write("sSubOutAnalogSP,");
            writer.Write("sDisplayFormat,");
            writer.Write("cAlarmType,");
            writer.Write("sGraphicFile,");
            writer.Write("sAlarmWaveFile,");
            writer.Write("nCalculateFilter,");
            writer.Write("bCutOverValue,");
            writer.Write("nCalcDelay,");
            writer.Write("wAlarmPriority,");
            writer.Write("cConfirmCount,");
            writer.Write("bBcdValue,");
            writer.Write("fAlarmReturnGab,");
            writer.Write("wRateOfChangeLimit,");
            writer.Write("flags,");
            writer.Write("cAlarmProtectOnBigChangePercent,");
            writer.Write("cAlarmProtectOnBigChangeSecond,");
            writer.Write("wScanTime,");
            writer.Write("fScrollUnit,");

            writer.Write("nMemoryTypeSub,");

            writer.Write("Dio_Address,");
            writer.Write("Dio_nCalculateFilter,");
            writer.Write("Dio_cDdeDataFormat,");
            writer.Write("Read Calc Script,");
            writer.Write("Write Calc Script,");
        }

        void SaveTagHeaderAO(CommaTextWriter writer)
        {
            writer.Write("fn,");
            writer.Write("unit,");
            writer.Write("fBase,");
            writer.Write("fFull,");
            writer.Write("fPlcBase,");
            writer.Write("fPlcFull,");
            writer.Write("nCalculateFilter,");
            writer.Write("cDdeDataFormat,");
            writer.Write("bBcdValue,");
            writer.Write("bCutOverValue,");
            writer.Write("Write Calc Script,");
        }

        void SaveTagHeaderDI(CommaTextWriter writer)
        {
            writer.Write("fn,");
            writer.Write("desON,");
            writer.Write("desOFF,");
            writer.Write("alarm,");
            writer.Write("sSubOutDigital1,");
            writer.Write("sSubOutDigital2,");
            writer.Write("sSubOutDigitalOnTag,");
            writer.Write("sSubOutDigitalOffTag,");
            writer.Write("bFileSave,");
            writer.Write("cAlarmType,");
            writer.Write("sGraphicFile,");
            writer.Write("sAlarmWaveFile,");
            writer.Write("wAlarmPriority,");
            writer.Write("cOutLinkMethod,");
            writer.Write("cConfirmCount,");
            writer.Write("flags,");
            writer.Write("bReverse,");
            writer.Write("wScanTime,");

            writer.Write("dio_address,");
            writer.Write("dio_cRelayType,");
            writer.Write("dio_wRelaySecTarget,");
            writer.Write("dio_nDelaySecON,");
            writer.Write("dio_nDelaySecOFF,");
        }

        void SaveTagHeaderDO(CommaTextWriter writer)
        {
            writer.Write("desON,");
            writer.Write("desOFF,");
            writer.Write("cRelayType,");
            writer.Write("wRelaySecTarget,");
            writer.Write("bReverse,");
            writer.Write("nDelaySecON,");
            writer.Write("nDelaySecOFF,");
        }

        void SaveTagHeaderST(CommaTextWriter writer)
        {
            writer.Write("read_size,");
            writer.Write("read_method,");
            writer.Write("cMemoryType,");
        }

        EnumTagType enumTagTypeSaveHeader = EnumTagType.none;   // 태그 해더 저장 시 형식이 중복되면 하나만 기록한다.

        void SaveTagHeader(CommaTextWriter writer, EnumTagType enumTagType)
        {
            if (enumTagTypeSaveHeader == enumTagType) return;
            enumTagTypeSaveHeader = enumTagType;

            SaveTagHeaderCommon(writer);

            if (enumTagType == EnumTagType.AI)
            {
                SaveTagHeaderAI(writer);
            }
            else if (enumTagType == EnumTagType.AO)
            {
                SaveTagHeaderAO(writer);
            }
            else if (enumTagType == EnumTagType.DI)
            {
                SaveTagHeaderDI(writer);
            }
            else if (enumTagType == EnumTagType.DO)
            {
                SaveTagHeaderDO(writer);
            }
            else if (enumTagType == EnumTagType.ST)
            {
                SaveTagHeaderST(writer);
            }
            else
            {

            }

            writer.WriteLine();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="tagTemp"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public bool SaveTag(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool di_address_semicolon, bool save_header)
		{
			CommaTextWriter writer = new CommaTextWriter(filename, encoding);

			if(writer == null) 
			{
				if(Tools.IsLangKorean())
				{
					MessageBox.Show("파일을 쓸 수 없습니다.\n"+filename, "쓰기 오류");
				}
				else 
				{
					MessageBox.Show("Cannot write to file.\n"+filename, "File write error");
				}
				return false;
			}

			

            writer.WriteLine(";Version,{0},", Application.ProductVersion); // 9.3.3 부터 지원

            enumTagTypeSaveHeader = EnumTagType.none;

			RecurseWriteFile(writer, tagTemp.arrayTag, di_address_semicolon, save_header);

			writer.Close();

			return true;
		}

		void CommonWrite(CommaTextWriter writer, TagPublicClass tp)
		{
			writer.Write("{0},", tp.enumTagType.ToString());
			writer.Write("{0},", tp.tag);
			writer.Write("{0},", tp.description);
			writer.Write("{0},", tp.act);
			writer.Write("{0},", tp.cTagLinkType);
			writer.Write("{0},", tp.bLocalTag);
			writer.Write("{0},", tp.sOpcServer);
			writer.Write("{0},", tp.sOpcGroup);
			writer.Write("{0},", tp.sOpcItem);
			writer.Write("{0},", tp.nOpcItemPos);

			writer.Write("{0},", tp.bUseAsOutput);	// reserved1 입/출력 겸용
            writer.Write("{0},", tp.pbReserved02);	// reserved2
            writer.Write("{0},", tp.pbReserved03);	// reserved3
            writer.Write("{0},", tp.pbReserved04);	// reserved4
            writer.Write("{0},", tp.pbReserved05);	// reserved5
            writer.Write("{0},", tp.pbReserved06);	// reserved6
            writer.Write("{0},", tp.pbReserved07);	// reserved7
            writer.Write("{0},", tp.pbReserved08);	// reserved8
            writer.Write("{0},", tp.pbReserved09);	// reserved9
            writer.Write("{0},", tp.pbReserved10);	// reserved10
		}

		void WriteAI(CommaTextWriter writer, TagAiClass ai)
		{
			writer.Write("{0},", ai.port);
			writer.Write("{0},", ai.station);
			writer.Write("{0},", ai.address);
            writer.Write("{0},", ai.writeAo.sExtraAddr);	//extra1
            writer.Write("{0},", ai.writeAo.wExtraAddr);	//extra2
			writer.Write("{0},", ai.sDdeService);
			writer.Write("{0},", ai.sDdeTopic);
			writer.Write("{0},", ai.sDdeItem);
			writer.Write("{0},", ai.bDdeRequest);	

			writer.Write("{0},", ai.fn);	
			writer.Write("{0},", ai.unit);	
			writer.Write("{0},", ai.fBase);	
			writer.Write("{0},", ai.fFull);	
			writer.Write("{0},", ai.fPlcBase);	
			writer.Write("{0},", ai.fPlcFull);	
			writer.Write("{0},", ai.hihi);	
			writer.Write("{0},", ai.high);	
			writer.Write("{0},", ai.low);	
			writer.Write("{0},", ai.lolo);	
			writer.Write("{0},", ai.view_full);	
			writer.Write("{0},", ai.view_base);	
			writer.Write("{0},", ai.alarm);	
			writer.Write("{0},", ai.bFileSave);	
			writer.Write("{0},", ai.sSubOutDigitalHiHi);	
			writer.Write("{0},", ai.sSubOutDigitalLoLo);	
			writer.Write("{0},", ai.sSubOutAnalog);	
			writer.Write("{0},", ai.sSubOutAnalogSP);	
			
			writer.Write("{0},", ai.sDisplayFormat);	


			writer.Write("{0},", ai.cAlarmType);	
			writer.Write("{0},", ai.sGraphicFile);	
			writer.Write("{0},", ai.sAlarmWaveFile);	
			writer.Write("{0},", ai.nCalculateFilter);	
			writer.Write("{0},", ai.bCutOverValue);	
			writer.Write("{0},", ai.nCalcDelay);	
			writer.Write("{0},", ai.wAlarmPriority);	
			writer.Write("{0},", ai.cConfirmCount);	
			writer.Write("{0},", ai.bBcdValue);	
			writer.Write("{0},", ai.fAlarmReturnGab);	
			writer.Write("{0},", ai.wRateOfChangeLimit);
			writer.Write("{0:X},", ai.wProtectFlags);	
			writer.Write("{0},", ai.cAlarmProtectOnBigChangePercent);	
			writer.Write("{0},", ai.cAlarmProtectOnBigChangeSecond);	
			writer.Write("{0},", ai.wScanTime);	
			writer.Write("{0},", ai.fScrollUnit);
            writer.Write("{0},", ai.nMemoryTypeSub);

            writer.Write("{0:X},", ai.writeAo.address);
            writer.Write("{0},", ai.writeAo.nCalculateFilter);
            writer.Write("{0},", ai.writeAo.cDdeDataFormat);

            writer.Write("{0},", ai.sCalcScript);
            writer.Write("{0},", ai.writeAo.sCalcScript);

            

			writer.WriteLine();
		}

		void WriteAO(CommaTextWriter writer, TagAoClass ao)
		{
			writer.Write("{0},", ao.port);
			writer.Write("{0},", ao.station);
			writer.Write("{0:X},", ao.address);
			writer.Write("{0},", ao.sExtraAddr);	//extra1
			writer.Write("{0},", ao.wExtraAddr);	//extra2
			writer.Write("{0},", ao.sDdeService);
			writer.Write("{0},", ao.sDdeTopic);
			writer.Write("{0},", ao.sDdeItem);
			writer.Write("{0},", ao.bDdeRequest);	// 사용안함

			writer.Write("{0},", ao.fn);
			writer.Write("{0},", ao.unit);
			writer.Write("{0},", ao.fBase);
			writer.Write("{0},", ao.fFull);
			writer.Write("{0},", ao.plc_base);
			writer.Write("{0},", ao.plc_full);
			writer.Write("{0},", ao.nCalculateFilter);
			writer.Write("{0},", ao.cDdeDataFormat);
			writer.Write("{0},", ao.bBcdValue);
			writer.Write("{0},", ao.bCutOverValue);

            writer.Write("{0},", ao.sCalcScript);

            

			writer.WriteLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="di"></param>
		/// <param name="di_address_semicolon">CSV로 저장할 때만 엑셀에서 문제없이 사용할 수 있게 세미콜론을 넣는다</param>
		void WriteDI(CommaTextWriter writer, TagDiClass di, bool di_address_semicolon)
		{
			writer.Write("{0},", di.port);
			writer.Write("{0},", di.station);

			// 123.10과 같은 주소는 엑셀에서 123.1과 같은 식으로 저장되므로 123.10;와 같이 세미를 넣었다.
			if(di_address_semicolon)
				writer.Write("{0}.{1};,", di.address_word, di.address_bit);
			else 
				writer.Write("{0}.{1},", di.address_word, di.address_bit);

            writer.Write("{0},", di.writeDo.sExtraAddr);	//extra1
            writer.Write("{0},", di.writeDo.wExtraAddr);	//extra2
			writer.Write("{0},", di.sDdeService);
			writer.Write("{0},", di.sDdeTopic);
			writer.Write("{0},", di.sDdeItem);
			writer.Write("{0},", di.bDdeRequest);
	
			writer.Write("{0},", di.fn);
			writer.Write("{0},", di.desON);
			writer.Write("{0},", di.desOFF);
			writer.Write("{0},", di.alarm);
			writer.Write("{0},", di.sSubOutDigital1);
			writer.Write("{0},", di.sSubOutDigital2);
			writer.Write("{0},", di.sSubOutDigitalOnTag);
			writer.Write("{0},", di.sSubOutDigitalOffTag);
			writer.Write("{0},", di.bFileSave);
			writer.Write("{0},", di.cAlarmType);
			writer.Write("{0},", di.sGraphicFile);
			writer.Write("{0},", di.sAlarmWaveFile);
			writer.Write("{0},", di.wAlarmPriority);
			writer.Write("{0},", di.cOutLinkMethod);
			writer.Write("{0},", di.cConfirmCount);
			writer.Write("{0:X},", di.wProtectFlags);
			writer.Write("{0},", di.bReverse);
			writer.Write("{0},", di.wScanTime);

            writer.Write("{0:X},", di.writeDo.address);
            writer.Write("{0},", di.writeDo.cRelayType);
            writer.Write("{0},", di.writeDo.wRelaySecTarget);
            writer.Write("{0},", di.writeDo.nDelaySecON);
            writer.Write("{0},", di.writeDo.nDelaySecOFF);

            

			writer.WriteLine();
		}

		void WriteDO(CommaTextWriter writer, TagDoClass dout)
		{
			writer.Write("{0},", dout.port);
			writer.Write("{0},", dout.station);
			writer.Write("{0:X},", dout.address);
			writer.Write("{0},", dout.sExtraAddr);	//extra1
			writer.Write("{0},", dout.wExtraAddr);	//extra2
			writer.Write("{0},", dout.sDdeService);
			writer.Write("{0},", dout.sDdeTopic);
			writer.Write("{0},", dout.sDdeItem);
			writer.Write("{0},", dout.bDdeRequest); // 사용안함

			writer.Write("{0},", dout.desON);
			writer.Write("{0},", dout.desOFF);
			writer.Write("{0},", dout.cRelayType);
			writer.Write("{0},", dout.wRelaySecTarget);
			writer.Write("{0},", dout.bReverse);
			writer.Write("{0},", dout.nDelaySecON);
			writer.Write("{0},", dout.nDelaySecOFF);

            

			writer.WriteLine();
		}

		void WriteST(CommaTextWriter writer, TagStClass st)
		{
			writer.Write("{0},", st.port);
			writer.Write("{0},", st.station);
			writer.Write("{0},", st.address);
			writer.Write("{0},", st.stReserved1);	//extra1 자리
			writer.Write("{0},", st.stReserved2);	//extra2 자리
			writer.Write("{0},", st.sDdeService);
			writer.Write("{0},", st.sDdeTopic);
			writer.Write("{0},", st.sDdeItem);
			writer.Write("{0},", st.bDdeRequest);	

			writer.Write("{0},", st.read_size);	
			writer.Write("{0},", st.read_method);	
			writer.Write("{0},", st.cMemoryType);

            

			writer.WriteLine();
		}

		void WriteDoGroup(CommaTextWriter writer, TagDoGroupClass gdo)
		{
			DoGroupMember member;
			
			for(int i = 0; i < gdo.member.Count; i++) 
			{
				member = (DoGroupMember)gdo.member[i];
				writer.Write("{0},", member.tag);
			}

			writer.WriteLine();
		}

		void WriteGR(CommaTextWriter writer, TagGrClass gr)
		{
            

			writer.WriteLine();
		}

		void RecurseWriteFile(CommaTextWriter writer, ArrayList arrayTag, bool di_address_semicolon, bool save_header)
		{
			TagPublicClass tp;

			for(int i = 0; i < arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)arrayTag[i];

                

				if(tp.enumTagType == EnumTagType.GR) 
				{
					CommonWrite(writer, tp);
					WriteGR(writer, (TagGrClass)tp);
					RecurseWriteFile(writer, ((TagGrClass)tp).arrayTag, di_address_semicolon, save_header);
				}
				else 
				{
                    if (save_header) SaveTagHeader(writer, tp.enumTagType);

					CommonWrite(writer, tp);
					if(tp.enumTagType == EnumTagType.AI)		WriteAI(writer, (TagAiClass)tp);
					else if(tp.enumTagType == EnumTagType.AO)	WriteAO(writer, (TagAoClass)tp);
					else if(tp.enumTagType == EnumTagType.DI)	WriteDI(writer, (TagDiClass)tp, di_address_semicolon);
					else if(tp.enumTagType == EnumTagType.DO)	WriteDO(writer, (TagDoClass)tp);
					else if(tp.enumTagType == EnumTagType.ST)	WriteST(writer, (TagStClass)tp);
					else if(tp.enumTagType == EnumTagType.GDO)	WriteDoGroup(writer, (TagDoGroupClass)tp);
				}
			}			
		}*/

        /*
        /// <summary>
        /// 해당되는 태그만 읽어온다. 주로 웹페이지에서 태그의 특성을 얻을 때 사용한다.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tagTemp"></param>
        /// <returns></returns>
        public TagPublicClass LoadTagOne(string filename, string tag)
        {
            if (!File.Exists(filename)) return null;

            TextReader reader = TextReaderWithCheck(filename, System.Text.Encoding.UTF8);

            if (reader == null) return null;

            string one_line;
            CommaTextReader comma = new CommaTextReader();
            string type = "";
            TagAiClass ai;
            TagAoClass ao;
            TagDiClass di;
            TagDoClass dout;
            TagStClass st;
            TagGrClass gr;
            TagDoGroupClass gdo;
            string imsi = "";
            TagPublicClass tp;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                if (one_line.Length == 0) continue;
                if (one_line[0] == ';') continue;

                comma.Set(one_line);
                comma.GetString(ref type);
                comma.GetString(ref imsi);

                if (String.Compare(imsi, tag, true) != 0) continue;

                comma.Set(one_line);
                comma.GetString(ref type);

                tp = null;

                if (String.Compare(type, "AI", true) == 0)
                {
                    ai = new TagAiClass();
                    LoadToAI(ai, comma);
                    tp = ai;
                }
                else if (String.Compare(type, "AO", true) == 0)
                {
                    ao = new TagAoClass();
                    LoadToAO(ao, comma);
                    tp = ao;
                }
                else if (String.Compare(type, "DI", true) == 0)
                {
                    di = new TagDiClass();
                    LoadToDI(di, comma);
                    tp = di;
                }
                else if (String.Compare(type, "DO", true) == 0)
                {
                    dout = new TagDoClass();
                    LoadToDO(dout, comma);
                    tp = dout;
                }
                else if (String.Compare(type, "ST", true) == 0)
                {
                    st = new TagStClass();
                    LoadToST(st, comma);
                    tp = st;
                }
                else if (String.Compare(type, "GR", true) == 0)
                {
                    gr = new TagGrClass();
                    LoadToGR(gr, comma);
                    tp = gr;
                }
                else if (String.Compare(type, "GDO", true) == 0)
                {
                    gdo = new TagDoGroupClass();
                    LoadToDoGroup(gdo, comma);
                    tp = gdo;
                }
                else { }

                reader.Close();
                return tp;
            }

            reader.Close();

            return null;
        }*/
	}
}
