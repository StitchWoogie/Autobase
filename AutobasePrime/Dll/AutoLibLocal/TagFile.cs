using System;
using System.IO;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using System.Collections;
using System.Security.Cryptography;
using System.Linq;

namespace AutoLibLocal
{
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
			
			retry:;
			try 
			{
				reader = new StreamReader(path, encoding);
			}
			catch 
			{
				DialogResult result;
				if(Tools.IsLangKorean()) 
				{
					result = MessageBox.Show("태그 파일을 열 수 없습니다.\n태그 파일을 다른 프로그램에서 사용중일 수 있습니다.\n태그파일 읽기를 재 시도할까요?", path, MessageBoxButtons.YesNo);
				}
				else 
				{
                    result = MessageBox.Show("Can't Open the Tag file.\nFile is used another program.\nRetry reading?", path, MessageBoxButtons.YesNo);
				}

				if(result == DialogResult.Yes)	goto retry;

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
            comma.GetString(ref tp.sScriptTagEvent);	// reserved2 2009.5.6 사용

            comma.GetHexDWORD(ref tp.flagsOpcServer);	// reserved3 2016.4-12 사용

            comma.GetBYTE(ref tp.flagOpcUAClient);   // reserved4 OPC UA 24-09-02 추가 hsjeong
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
            comma.GetInt(ref ai.address);
			comma.GetString(ref ai.writeAo.sExtraAddr);	//extra1
			comma.GetWORD(ref ai.writeAo.wExtraAddr);	//extra2
			comma.GetString(ref ai.sDdeService);
			comma.GetString(ref ai.sDdeTopic);
			comma.GetString(ref ai.sDdeItem);
			comma.GetBYTE(ref ai.bDdeRequest);	

			comma.GetInt(ref ai.fn);	
			comma.GetString(ref ai.unit);
            comma.GetDouble(ref ai.fBase);
            comma.GetDouble(ref ai.fFull);
            comma.GetDouble(ref ai.fPlcBase);
            comma.GetDouble(ref ai.fPlcFull);
            comma.GetDouble(ref ai.hihi);
            comma.GetDouble(ref ai.high);
            comma.GetDouble(ref ai.low);
            comma.GetDouble(ref ai.lolo);
            comma.GetDouble(ref ai.view_full);
            comma.GetDouble(ref ai.view_base);	
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
			comma.GetInt(ref ai.nAlarmProtectOnBigChangeSecond);	
			comma.GetWORD(ref ai.wScanTime);	

			comma.GetFloat(ref ai.fScrollUnit);
			if(ai.fScrollUnit == 0)	ai.fScrollUnit = 1;

            comma.GetInt(ref ai.nMemoryTypeSub);    // 9.3.0.3 부터 지원 - 임시버전 

            comma.GetHexDWORD(ref ai.writeAo.address);          // 출력겸용 옵션 Extra1, Extra2는 앞부분에
            comma.GetInt(ref ai.writeAo.nCalculateFilter);      
            comma.GetChar(ref ai.writeAo.cDdeDataFormat);

            comma.GetString(ref ai.sCalcScript);
            comma.GetString(ref ai.writeAo.sCalcScript);

            comma.GetBYTE(ref ai.bAccumulatedValue); //20250225 PSU

            //comma.GetStringTotalRemain(ref ai.pbReservedLast); 
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
			comma.GetDouble(ref ao.fBase);
            comma.GetDouble(ref ao.fFull);
            comma.GetDouble(ref ao.plc_base);
            comma.GetDouble(ref ao.plc_full);
			comma.GetInt(ref ao.nCalculateFilter);
			comma.GetChar(ref ao.cDdeDataFormat);
			comma.GetChar(ref ao.bBcdValue);
			comma.GetChar(ref ao.bCutOverValue);

            comma.GetString(ref ao.sCalcScript);

            //comma.GetStringTotalRemain(ref ao.pbReservedLast); 
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

            // 주소가 0이하인 경우가 있다. 201-10-17
            if (di.address_bit < 0) di.address_bit = 0;

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

            //comma.GetStringTotalRemain(ref di.pbReservedLast); 
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

            //comma.GetStringTotalRemain(ref dout.pbReservedLast); 
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

            //comma.GetStringTotalRemain(ref st.pbReservedLast); 
		}

		void LoadToGR(TagGrClass gr, CommaTextReader comma)
		{
			LoadToCommon(gr, comma);

            //comma.GetStringTotalRemain(ref gr.pbReservedLast); 
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

        void LoadTagPublic(TextReader reader, TagGrClass tagTemp, bool check_exists)
        {
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

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                if (one_line.Length == 0) continue;
                if (one_line[0] == ';') continue;

                comma.Set(one_line);
                comma.GetString(ref type);
                if (String.Compare(type, "AI", true) == 0)
                {
                    ai = new TagAiClass();
                    LoadToAI(ai, comma);
                    tagTemp.AddTag(ai, check_exists);
                }
                else if (String.Compare(type, "AO", true) == 0)
                {
                    ao = new TagAoClass();
                    LoadToAO(ao, comma);
                    tagTemp.AddTag(ao, check_exists);
                }
                else if (String.Compare(type, "DI", true) == 0)
                {
                    di = new TagDiClass();
                    LoadToDI(di, comma);
                    tagTemp.AddTag(di, check_exists);
                }
                else if (String.Compare(type, "DO", true) == 0)
                {
                    dout = new TagDoClass();
                    LoadToDO(dout, comma);
                    tagTemp.AddTag(dout, check_exists);
                }
                else if (String.Compare(type, "ST", true) == 0)
                {
                    st = new TagStClass();
                    LoadToST(st, comma);
                    tagTemp.AddTag(st, check_exists);
                }
                else if (String.Compare(type, "GR", true) == 0)
                {
                    gr = new TagGrClass();
                    LoadToGR(gr, comma);
                    tagTemp.AddTag(gr, check_exists);
                }
                else if (String.Compare(type, "GDO", true) == 0)
                {
                    gdo = new TagDoGroupClass();
                    LoadToDoGroup(gdo, comma);
                    tagTemp.AddTag(gdo, check_exists);
                }
                else { }
            }
        }

        public bool LoadTag(Stream stream, TagGrClass tagTemp, bool check_exists)
        {
            TextReader reader = new StreamReader(stream);

            if (reader == null) return false;

            LoadTagPublic(reader, tagTemp, check_exists);

            return true;
        }

        /*
        /// <summary>
        /// 엑셀등에서 태그파일을 사용중이면 파일을 열수 없기 때문에 이부분이 추가되었다.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static StreamReader BinaryReaderWithCheck(string path)
        {
            StreamReader reader;

        retry: ;
            try
            {
                reader = new StreamReader(path, encoding);
            }
            catch
            {
                DialogResult result;
                if (Tools.IsLangKorean())
                {
                    result = MessageBox.Show("태그 파일을 열 수 없습니다.\n태그 파일을 다른 프로그램에서 사용중일 수 있습니다.\n태그파일 읽기를 재 시도할까요?", path, MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show("Can't Open the Tag file.\nFile is used another program.\nRetry reading?", path, MessageBoxButtons.RetryCancel);
                }

                if (result == DialogResult.Yes) goto retry;

                return null;
            }

            return reader;
        }*/

        // 파일을 읽어온다.
        byte[] LoadFileAndMirror(string filename)
        {
            if (!File.Exists(filename)) return null;    // 원본 파일이 존재하지 않으면 미러본도 읽을 필요가 없다.

            byte[] data_org;

            retry:

            try
            {
                data_org = File.ReadAllBytes(filename);
            }
            catch
            {
                DialogResult result;
                if (Tools.IsLangKorean())
                {
                    result = MessageBox.Show("파일을 열 수 없습니다.\n태그 파일을 다른 프로그램에서 사용중일 수 있습니다.\n파일 읽기를 재 시도할까요?", filename, MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show("Can't Open the Tag file.\nMaybe file is used another program.\nRetry reading?", filename, MessageBoxButtons.YesNo);
                }

                if (result == DialogResult.Yes) goto retry;

                data_org = null;
            }

            // 이 부분이 있으면 LoadFileAndMirror를 사용하는 파일은 같은 환경을 사용할 수 있다.
            if (!ConfigViewMain.bUseAutomaticFileRecovery)
                return data_org;

            SHA1 sha = new SHA1CryptoServiceProvider();
            string directory = Path.GetDirectoryName(filename);
            string fileext = Path.GetFileName(filename);
            string file_ext = fileext.Replace('.', '_'); // local.tagx 파일을 local_tagx로 바꾸어준다.

            if (data_org == null)
            {
                // exception이 발생했으므로 crc 검사없이 mirror를 체크한다.
                goto check_mirror_file;
            }

            byte[] crc;// = sha.ComputeHash(data);
            byte[] file_crc;    // 파일에서 읽은 crc

            crc = sha.ComputeHash(data_org);
            
            string crc_filename;

            crc_filename = String.Format("{0}\\_crc\\{1}.crc", directory, file_ext);

            // CRC 파일이 없으면 이전의 프로젝트일 수 있으므로 맞는 것으로 가정해야 한다.
            if (!File.Exists(crc_filename)) {
                return data_org;                
            }

            try
            {
                file_crc = File.ReadAllBytes(crc_filename);
            }
            catch
            {
                file_crc = new byte[1];
            }

            if (crc.SequenceEqual(file_crc)) return data_org;   // crc 가 일치한다.

            // 아래는 원본 파일이 있으나 CRC가 맞지 않는 경우이다.
            check_mirror_file:

            string mirror_folder = String.Format("{0}\\_mir", directory);

            string mirror_filename = String.Format("{0}\\{1}", mirror_folder, fileext);

            if (!File.Exists(mirror_filename))
            {
                MessageBox.Show("File CRC mismatched.\nand Mirror file not found.\nOriginal File will be load.", filename);
                return data_org; // 미러링 파일이 없다.
            }

            crc_filename = String.Format("{0}\\_crc\\{1}.crc", mirror_folder, file_ext);

            if (!File.Exists(crc_filename))
            {
                MessageBox.Show("File CRC mismatched.\nMirror crc file not found.\nOriginal File will be load.", filename);
                return data_org; // 미러링 파일은 있지만 CRC 파일이 없는 경우는 정확도를 조사할 수 없다.
            }

            byte[] data_mirr;

            try
            {
                data_mirr = File.ReadAllBytes(mirror_filename); // Mirror
            }
            catch
            {
                MessageBox.Show("Mirror file cannot opened.\nOriginal File will be load.", filename);
                return data_org;
            }

            crc = sha.ComputeHash(data_mirr);

            try
            {
                file_crc = File.ReadAllBytes(crc_filename);
            }
            catch
            {
                MessageBox.Show("Mirror CRC file cannot opened.\nOriginal File will be load.", filename);
                return data_org;
            }

            if (crc.SequenceEqual(file_crc))
            {
                MessageBox.Show("File CRC mismatched.\nbut Mirror file CRC O.K.\nMirror file will be load.", filename);
                return data_mirr;   // crc 가 일치한다.
            }

            MessageBox.Show("File CRC mismatched.\nand Mirror file CRC mismatched.\nOriginal File will be load.", filename);
            return data_org;        // 원본과 미러링 모두 crc가 맞지 않으면 원본을 로딩한다. 이것은 autobase 이전 버전에서 프로젝트를 편집하면 충분히 생길 수 있는 문제이다.
        }

        // 이 함수만 있어도 되지만 만약의 버그를 위해서 함수를 따로 만들었다.
        bool LoadTagNew(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool check_exists)
        {
            if (!File.Exists(filename)) return false;

            byte[] data = LoadFileAndMirror(filename);

            if (data == null)
                return false;

            MemoryStream ms = new MemoryStream(data);

            TextReader reader = new StreamReader(ms, encoding);

            if (reader == null) return false;

            LoadTagPublic(reader, tagTemp, check_exists);

            reader.Close();
            ms.Close();

            return true;
        }

        // 이 함수 대신 LoadTagNew 를 사용해도 되지만 만약의 버그를 위해서 함수를 따로 만들었다.
        bool LoadTagOld(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool check_exists)
        {
            if (!File.Exists(filename)) return false;

            TextReader reader = TextReaderWithCheck(filename, encoding);

            if (reader == null) return false;

            LoadTagPublic(reader, tagTemp, check_exists);

            reader.Close();

            return true;
        }

        public bool LoadTag(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool check_exists)
        {
            // LoadTagNew에서 bUseAutomaticFileRecovery를 검사해서 LoadTagNew 를 LoadTag로 대체해도 되지만 만약을 위해서 따로 만들어서 사용한다.
            if (ConfigViewMain.bUseAutomaticFileRecovery)
            {
                return LoadTagNew(filename, tagTemp, encoding, check_exists);
            }
            else
            {
                return LoadTagOld(filename, tagTemp, encoding, check_exists);
            }
        }

        /*
        public bool LoadTag(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool check_exists)
		{
			if(!File.Exists(filename))	return false;

			TextReader reader = TextReaderWithCheck(filename, encoding);

			if(reader == null)	return false;

            LoadTagPublic(reader, tagTemp, check_exists);

            reader.Close();     

            return true;
		}*/

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
            writer.Write("TagEventScript,");
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
            writer.Write("bAccumulatedValue,"); //20250225 PSU
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

        public bool SaveTag(Stream stream, TagGrClass tagTemp, bool di_address_semicolon, bool save_header)
        {
            // 태그파일은 읽기/쓰기 CommaTextWriter, CommaTextReader 로 모두 잘되어 있다.
            CommaTextWriter writer = new CommaTextWriter(stream);

            //writer.Write((char)0xFEFF);

            writer.WriteLine(";Version,{0},", Application.ProductVersion); // 9.3.3 부터 지원

            enumTagTypeSaveHeader = EnumTagType.none;

            RecurseWriteFile(writer, tagTemp.arrayTag, di_address_semicolon, save_header);

            writer.Flush();     // 2017-1-10 추가함.  경일에서 태그의 중간이 잘린다고 하여 추가함. 종성도 이전에 비슷한 현상이 있는듯 하여 추가함. 
            writer.Close();

            return true;
        }

        // 파일을 저장하고 crc 파일을 만들고 Mirror 폴더에 같은 파일과 CRC 파일을 만든다.
        
        void SaveFileAndMirror(string filename, byte[] data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] crc = sha.ComputeHash(data);

            File.WriteAllBytes(filename, data);

            string directory = Path.GetDirectoryName(filename);
            string fileext = Path.GetFileName(filename);
            string file_ext = fileext.Replace('.', '_'); // local.tagx 파일을 local_tagx로 바꾸어준다.

            string crc_folder;
            string crc_filename;

            // _crc 폴더를 만들어서 하는것이 좋을 듯 하다. 파일이 폴더에 많은 경우 부담이 될 수 있다.
            crc_folder = String.Format("{0}\\_crc", directory);
            crc_filename = String.Format("{0}\\{1}.crc", crc_folder, file_ext);

            // 자동 복구를 사용할 때만 CRC를 만든다. 이전의 사용자가 이상한 파일이 생겼다고 이야기 할 듯.
            if (ConfigViewMain.bUseAutomaticFileRecovery)
            {
                if (!Directory.Exists(crc_folder))
                {
                    Directory.CreateDirectory(crc_folder);
                }

                File.WriteAllBytes(crc_filename, crc);
            }
            else
            {
                // CRC 파일도 삭제해 준다.
                if (File.Exists(crc_filename))
                    File.Delete(crc_filename);
            }

            // _mir 폴더를 만들어서 하는것이 좋을 듯 하다. 파일이 폴더에 많은 경우 부담이 될 수 있다.
            string mirror_folder = String.Format("{0}\\_mir", directory);
            string mirror_filename = String.Format("{0}\\{1}", mirror_folder, fileext);

            crc_folder = String.Format("{0}\\_crc", mirror_folder);
            crc_filename = String.Format("{0}\\{1}.crc", crc_folder, file_ext);

            // 옵션을 사용할때만 미러 파일을 만들고 그렇지 않으면 파일을 삭제해 준다. 파일을 삭제하지 않으면 너무 이전 파일을 나중에 참조하는 경우가 있을 듯...
            if (ConfigViewMain.bUseAutomaticFileRecovery)
            {
                if (!Directory.Exists(mirror_folder))
                {
                    Directory.CreateDirectory(mirror_folder);
                }

                File.WriteAllBytes(mirror_filename, data); // Mirror

                if (!Directory.Exists(crc_folder))
                {
                    Directory.CreateDirectory(crc_folder);
                }
                
                File.WriteAllBytes(crc_filename, crc);
            }
            else
            {
                if(File.Exists(mirror_filename))
                    File.Delete(mirror_filename);

                if (File.Exists(crc_filename))
                    File.Delete(crc_filename);
            }
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
            // 태그파일은 읽기/쓰기 CommaTextWriter, CommaTextReader 로 모두 잘되어 있다.
            CommaTextWriter writer;
            string err_msg = "";

            MemoryStream ms = new MemoryStream();

            writer = new CommaTextWriter(ms, encoding);

            //writer.Write((char)0xFEFF);

            writer.WriteLine(";Version,{0},", Application.ProductVersion); // 9.3.3 부터 지원

            enumTagTypeSaveHeader = EnumTagType.none;

            RecurseWriteFile(writer, tagTemp.arrayTag, di_address_semicolon, save_header);

            writer.Flush();     // 2017-1-10 추가함.  경일에서 태그의 중간이 잘린다고 하여 추가함. 종성도 이전에 비슷한 현상이 있는듯 하여 추가함. 
            writer.Close();

            byte[]  data = ms.ToArray();
            ms.Close();

            try
            {
                SaveFileAndMirror(filename, data);
            }
            catch (Exception exception)
            {
                err_msg = exception.Message;

                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("파일을 쓸 수 없습니다.\n" + err_msg + "\n" + filename, "쓰기 오류");
                }
                else
                {
                    MessageBox.Show("Cannot write to file.\n" + err_msg + "\n" + filename, "File write error");
                }
                return false;
            }

            return true;
        }

        /* 이 함수는 미러링을 사용하지 않는 옵션에서도 사용하면 안된다. 항상 CRC와 미러링을 만들어 놓지 않으면 자동 복구 옵션을 사용할 때 문제가 생길 수 있다.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tagTemp"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public bool SaveTag(string filename, TagGrClass tagTemp, System.Text.Encoding encoding, bool di_address_semicolon, bool save_header)
        {
            CommaTextWriter writer;
            string err_msg = "";

            try
            {
                writer = new CommaTextWriter(filename, encoding);
            }
            catch (Exception exception)
            {
                writer = null;
                err_msg = exception.Message;
            }

            if(writer == null)
            {
                if(Tools.IsLangKorean())
                {
                    MessageBox.Show("파일을 쓸 수 없습니다.\n"+err_msg+"\n"+filename, "쓰기 오류");
                }
                else 
                {
                    MessageBox.Show("Cannot write to file.\n" + err_msg + "\n" + filename, "File write error");
                }
                return false;
            }

            //writer.Write((char)0xFEFF);

            writer.WriteLine(";Version,{0},", Application.ProductVersion); // 9.3.3 부터 지원

            enumTagTypeSaveHeader = EnumTagType.none;

            RecurseWriteFile(writer, tagTemp.arrayTag, di_address_semicolon, save_header);

            writer.Flush();     // 2017-1-10 추가함.  경일에서 태그의 중간이 잘린다고 하여 추가함. 종성도 이전에 비슷한 현상이 있는듯 하여 추가함. 
            writer.Close();

            return true;
        }*/

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
            writer.Write("{0},", tp.sScriptTagEvent);	// reserved2  2009.5.6 사용

            writer.Write("{0:X},", tp.flagsOpcServer);	// reserved3     2016.4-12 사용
            writer.Write("{0},", tp.flagOpcUAClient);	// reserved4 OPC UA 24-09-02 추가 hsjeong
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
			writer.Write("{0},", ai.nAlarmProtectOnBigChangeSecond);	
			writer.Write("{0},", ai.wScanTime);	
			writer.Write("{0},", ai.fScrollUnit);
            writer.Write("{0},", ai.nMemoryTypeSub);

            writer.Write("{0:X},", ai.writeAo.address);
            writer.Write("{0},", ai.writeAo.nCalculateFilter);
            writer.Write("{0},", ai.writeAo.cDdeDataFormat);

            writer.Write("{0},", ai.sCalcScript);
            writer.Write("{0},", ai.writeAo.sCalcScript);
            writer.Write("{0},", ai.bAccumulatedValue); //20250225 PSU 누적값

            //writer.Write("{0}", ai.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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

            //writer.Write("{0}", ao.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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

            //writer.Write("{0}", di.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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

            //writer.Write("{0}", dout.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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

            //writer.Write("{0}", st.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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
            //writer.Write("{0}", gr.pbReservedLast); // 다른 버전에서 저장되었을지도 모르기 때문에 원본으로 저장해준다. 콤마를 붙이지 말것(자꾸늘어남)

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
		}

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
        }

        
    }


    
}
