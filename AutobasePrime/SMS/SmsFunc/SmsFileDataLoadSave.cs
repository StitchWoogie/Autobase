using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using SMS.SmsComm;

namespace SMS.SmsFunc
{
	/// <summary>
	/// Summary description for SmsFileDataLoadSave.
	/// </summary>
	public class SmsFileDataLoadSave
	{
		public SmsFileDataLoadSave()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public void LoadAllUserData()
		{
			smsUserConfig user;
			for(int i = 0; i < 256; i++) 
			{
				user = new smsUserConfig();
				setUserDefaultValue(user, i);
				LoadOneUserData(user, i);
				SmsBasic.arrUserSetting.Add(user);
			}

							// 스크립트 사용자 등록
			SmsBasic.smsScriptUser.bActive = true;
			SmsBasic.smsScriptUser.nUserNo = 1000;
			if(Tools.IsLangKorean()) SmsBasic.smsScriptUser.userName = "스크립트";
			else SmsBasic.smsScriptUser.userName = "Script";
			SmsBasic.smsScriptUser.phoneNo = "000-0000-0000";
		}

		static void setUserDefaultValue(smsUserConfig user, int i)
		{
			user.nUserNo = i+1;
			user.userName = String.Format("User {0,3:d03}", i+1);
			user.phoneNo = "000-0000-0000";
		}

		static void LoadOneUserData(smsUserConfig user, int i)
		{
			string			path = TotalConfig.sDirWorkProject + "\\sms";
			
			if(Directory.Exists(path) == false) return;
			string filename = path + string.Format("\\user{0,3:d03}.sms", i);
			if(File.Exists(filename)) LoadUserData(user, filename);
			else LoadOldUserData(user, path+string.Format("\\user{0,3:d03}.ini", i), i);
		}

		static void LoadUserData(smsUserConfig user, string filename)
		{
			if(File.Exists(filename) == false) return;
			FileStream fs = File.OpenRead(filename);			
			if(fs == null) return;

			TextReader				reader = new StreamReader(fs);
			CommaBlockString		comma = new CommaBlockString();
			string					one_line;
			int						val = 0;
			
			comma.SetBlockCode(',');
			try 
			{				
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref val);
					user.bActive = ( val == 1 ) ? true : false;
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetString(ref user.userName);					
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetString(ref user.phoneNo);					
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref val);
					user.bAlarmPriority = ( val == 1 ) ? true : false;
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref val);
					user.bTagType = ( val == 1 ) ? true : false;
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref val);
					user.bTagOrder = ( val == 1 ) ? true : false;
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					for(int i = 0; i < 1000; i++) 
					{
						if(comma.IsEOS()) break;
						comma.GetInt(ref val);
						user.bPriority[i] = ( val == 1 ) ? true : false;
					}
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref user.nSendCondition);					
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetString(ref user.diTag);					
				}
				one_line = reader.ReadLine();
				if(one_line != null) 
				{
					comma.Set(one_line);
					comma.GetInt(ref user.nSmsType);					
				}

                //20241111 PSU 텔레그램
                one_line = reader.ReadLine();
                if (one_line != null)
                {
                    comma.Set(one_line);
                    comma.GetString(ref user.ChatID);
                }
			}
			catch {}
			reader.Close();
		}

		static void LoadOldUserData(smsUserConfig user, string filename, int pos)
		{
			if(File.Exists(filename) == false) return;

			try 
			{
				int	val = 0;

				val = Profile.GetPrivateProfileIntA("Config", "Active", 0, filename);
				user.bActive = ( val == 1) ? true : false;
				Profile.GetPrivateProfileStringA("Config", "Title", string.Format("User {0,3:03d}", pos+1), ref user.userName, filename);
				Profile.GetPrivateProfileStringA("Config", "PhoneNum", "000-0000-0000", ref user.phoneNo, filename);			
				val = Profile.GetPrivateProfileIntA("Config", "CallAlarm", 1, filename);
				user.bAlarmPriority = ( val == 1) ? true : false;
				val = Profile.GetPrivateProfileIntA("Config", "CallTag", 1, filename);
				user.bTagType = ( val == 1) ? true : false;
				val = Profile.GetPrivateProfileIntA("Config", "CallTagPos", 1, filename);	
				user.bTagOrder = ( val == 1) ? true : false;

				string	imsi = "";
				Profile.GetPrivateProfileStringA("Config", "Priority", "0", ref imsi,  filename);
	
				CommaBlockString comma = new CommaBlockString();
				comma.Set(imsi);				
				for(int i = 0; i < 1000; i++) 
				{
					if(comma.IsEOS()) break;
					comma.GetInt(ref val);
					user.bPriority[i] = (val == 1) ? true : false;
				}
				user.nSendCondition = Profile.GetPrivateProfileIntA("Config", "CallCondition", 0, filename);
				Profile.GetPrivateProfileStringA("Config", "DiTag", "", ref user.diTag, filename);
			}
			catch {}			
		}

		static bool checkAndMakeSmsDirectory()
		{
			string	path = TotalConfig.sDirWorkProject + "\\sms";
			if(Directory.Exists(path)) return true;
			Directory.CreateDirectory(path);
			if(Directory.Exists(path)) return true;
			return false;
		}

		static public void SaveOneUserData(int nUser)
		{
			if(nUser < 0 || nUser > 256) return;
			if(checkAndMakeSmsDirectory() == false) return;

			smsUserConfig user = (smsUserConfig)SmsBasic.arrUserSetting[nUser];
			string filename = TotalConfig.sDirWorkProject + "\\sms" + string.Format("\\user{0,3:d03}.sms", nUser);
			
			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter				writer = new StreamWriter(fs);
			writer.WriteLine("{0},", (user.bActive) ? 1 : 0);
			writer.WriteLine("{0},", user.userName);
			writer.WriteLine("{0},", user.phoneNo);
			writer.WriteLine("{0},", (user.bAlarmPriority) ? 1 : 0);
			writer.WriteLine("{0},", (user.bTagType) ? 1 : 0);
			writer.WriteLine("{0},", (user.bTagOrder) ? 1 : 0);
			for(int i = 0; i < 1000; i += 5) 
			{
				writer.Write("{0},{1},{2},{3},{4},", (user.bPriority[i]) ? 1 : 0, (user.bPriority[i+1]) ? 1 : 0, (user.bPriority[i+2]) ? 1 : 0, (user.bPriority[i+3]) ? 1 : 0, (user.bPriority[i+4]) ? 1 : 0);
				i += 5;
				writer.Write("{0},{1},{2},{3},{4},", (user.bPriority[i]) ? 1 : 0, (user.bPriority[i+1]) ? 1 : 0, (user.bPriority[i+2]) ? 1 : 0, (user.bPriority[i+3]) ? 1 : 0, (user.bPriority[i+4]) ? 1 : 0);
			}
			writer.WriteLine("");
			writer.WriteLine("{0},", user.nSendCondition);
			writer.WriteLine("{0},", user.diTag);			
			writer.WriteLine("{0},", user.nSmsType);
            //20241111 PSU 텔레그램
            writer.WriteLine("{0},", user.ChatID);
			writer.Close();
		}

		static void setDataToUserConfig(string one_line)
		{
			if(one_line == null) return;
			CommaBlockString		comma = new CommaBlockString();
			comma.Set(one_line);
			comma.SetBlockCode(',');
			byte		imsi = 0;
			comma.GetBYTE(ref imsi);
			SmsBasic.smsConfig.eConnectType = (SendSMSData.eConectionType)imsi;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nPort);
			if(SmsBasic.smsConfig.nPort <= 0 || SmsBasic.smsConfig.nPort > 256) SmsBasic.smsConfig.nPort = 1;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nBaud);			
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nParity);
			if(SmsBasic.smsConfig.nParity < 0 || SmsBasic.smsConfig.nParity > 1) SmsBasic.smsConfig.nParity = 0;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nData);
			if(SmsBasic.smsConfig.nData < 7 || SmsBasic.smsConfig.nData > 8) SmsBasic.smsConfig.nData = 8;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nStop);
			if(SmsBasic.smsConfig.nStop < 1 || SmsBasic.smsConfig.nStop > 2) SmsBasic.smsConfig.nStop = 1;
			if(comma.IsEOS()) return;

			int val = 0;
			comma.GetInt(ref val);
			SmsBasic.smsConfig.bUseServer = ( val == 1) ? true : false;
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref SmsBasic.smsConfig.cIP1);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref SmsBasic.smsConfig.cIP2);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref SmsBasic.smsConfig.cIP3);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref SmsBasic.smsConfig.cIP4);
			if(comma.IsEOS()) return;

			comma.GetWORD(ref SmsBasic.smsConfig.wPortNo);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref SmsBasic.smsConfig.cAckTimeOut);
			if(comma.IsEOS()) return;

			comma.GetInt(ref val);
			SmsBasic.smsConfig.bStopSmsCall = ( val == 1) ? true : false;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nRetryCount);
			if(SmsBasic.smsConfig.nRetryCount < 0 || SmsBasic.smsConfig.nRetryCount > 10) SmsBasic.smsConfig.nRetryCount = 2;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nRetryErrorCount);
			if(SmsBasic.smsConfig.nRetryErrorCount < 0 || SmsBasic.smsConfig.nRetryErrorCount > 10) SmsBasic.smsConfig.nRetryErrorCount = 2;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nDelaySiteCall);
			if(SmsBasic.smsConfig.nDelaySiteCall < 0 || SmsBasic.smsConfig.nDelaySiteCall > 120) SmsBasic.smsConfig.nDelaySiteCall = 3;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nDelayDataCall);
			if(SmsBasic.smsConfig.nDelayDataCall < 0 || SmsBasic.smsConfig.nDelayDataCall > 120) SmsBasic.smsConfig.nDelayDataCall = 3;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nTimeOutBasic);
			if(SmsBasic.smsConfig.nTimeOutBasic < 0 || SmsBasic.smsConfig.nTimeOutBasic > 120) SmsBasic.smsConfig.nTimeOutBasic = 5;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nTimeOutMessage);
			if(SmsBasic.smsConfig.nTimeOutMessage < 0 || SmsBasic.smsConfig.nTimeOutMessage > 120) SmsBasic.smsConfig.nTimeOutMessage = 30;
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nMaxSendChar);
			if(SmsBasic.smsConfig.nMaxSendChar < 10 || SmsBasic.smsConfig.nMaxSendChar > 1000) SmsBasic.smsConfig.nTimeOutMessage = 80;
			if(comma.IsEOS()) return;

			comma.GetInt(ref val);
			SmsBasic.smsConfig.eMessageType = ( val < 1 || val > 3) ? SendSMSData.eMsgType.NORMAL : (SendSMSData.eMsgType)val;
			//if(SmsBasic.smsConfig.eMessageType < 0 || SmsBasic.smsConfig.nMessageType > 2) SmsBasic.smsConfig.nMessageType = 0;
			if(comma.IsEOS()) return;

			comma.GetInt(ref val);
			SmsBasic.smsConfig.eCdma = (SendSMSData.eCdmaType)val;
			if(comma.IsEOS()) return;

			comma.GetString(ref SmsBasic.smsConfig.sSendNumber);
			if(comma.IsEOS()) return;

			string		familyName = "";
			comma.GetString(ref familyName);
			if(comma.IsEOS()) return;

			float		emSize = 0.0F;
			comma.GetFloat(ref emSize);			
			if(comma.IsEOS()) return;

			comma.GetInt(ref val);			
			try 
			{
				emSize = ( emSize <= 0.0F || emSize > 100.0F) ? 10.0F : emSize;
				familyName = ( familyName.Length <= 0 ) ? "굴림" : familyName;
				val = (val < 0 || val > 15) ? 0 : val;
				SmsBasic.smsConfig.fontList = new Font(familyName, emSize, (System.Drawing.FontStyle)val);
			}
			catch {}
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nRts);
			if(comma.IsEOS()) return;

			comma.GetInt(ref SmsBasic.smsConfig.nDtr);
			if(comma.IsEOS()) return;

            comma.GetBYTE(ref SmsBasic.smsConfig.cMobiconID);// add 2010-11-18
            if (comma.IsEOS()) return;

            comma.GetInt(ref SmsBasic.smsConfig.nMaxCountOfDay);// add 2011-11-23
            if (comma.IsEOS()) return;
		}


		static public void LoadSmsConfigData()
		{
			string filename = TotalConfig.sDirWorkProject + "\\Sms" + "\\SmsConfig.ini";
			if(File.Exists(filename) == false) return;
			FileStream fs = File.OpenRead(filename);			
			if(fs == null) return;

			TextReader				reader = new StreamReader(fs);			
			setDataToUserConfig(reader.ReadLine());
			reader.Close();
		}

		static public void SaveSmsConfigData()
		{
			if(SmsBasic.smsConfig.bChange == false) return;
			if(checkAndMakeSmsDirectory() == false) return;
			string filename = TotalConfig.sDirWorkProject + "\\Sms" + "\\SmsConfig.ini";
			
			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter				writer = new StreamWriter(fs);
			writer.Write("{0},{1},{2},{3},{4},{5},", (byte)SmsBasic.smsConfig.eConnectType, SmsBasic.smsConfig.nPort, SmsBasic.smsConfig.nBaud, SmsBasic.smsConfig.nParity, SmsBasic.smsConfig.nData, SmsBasic.smsConfig.nStop);
			writer.Write("{0},{1},{2},{3},{4},", (SmsBasic.smsConfig.bUseServer) ? 1 : 0, SmsBasic.smsConfig.cIP1, SmsBasic.smsConfig.cIP2, SmsBasic.smsConfig.cIP3, SmsBasic.smsConfig.cIP4);
			writer.Write("{0},{1},", SmsBasic.smsConfig.wPortNo, SmsBasic.smsConfig.cAckTimeOut);
			writer.Write("{0},{1},{2},", (SmsBasic.smsConfig.bStopSmsCall) ? 1 : 0, SmsBasic.smsConfig.nRetryCount, SmsBasic.smsConfig.nRetryErrorCount);
			writer.Write("{0},{1},{2},{3},", SmsBasic.smsConfig.nDelaySiteCall, SmsBasic.smsConfig.nDelayDataCall, SmsBasic.smsConfig.nTimeOutBasic, SmsBasic.smsConfig.nTimeOutMessage);
			writer.Write("{0},{1},{2},", SmsBasic.smsConfig.nMaxSendChar, (int)SmsBasic.smsConfig.eMessageType, (int)SmsBasic.smsConfig.eCdma);
			writer.Write("{0},{1},{2},{3},", SmsBasic.smsConfig.sSendNumber, SmsBasic.smsConfig.fontList.Name, SmsBasic.smsConfig.fontList.Size, (int)SmsBasic.smsConfig.fontList.Style);
			writer.Write("{0},{1},", SmsBasic.smsConfig.nRts, SmsBasic.smsConfig.nDtr);
            writer.Write("{0},", SmsBasic.smsConfig.cMobiconID);// add 2010-11-18
            writer.Write("{0},", SmsBasic.smsConfig.nMaxCountOfDay);// add 2011-11-23
			writer.WriteLine("");
			writer.Close();
		}

		static public void LoadSmsMessageFormat()
		{
			string filename = TotalConfig.sDirWorkProject + "\\sms" + "\\msgFormat.ini";
			if(File.Exists(filename) == false) return;
			FileStream fs = File.OpenRead(filename);			
			if(fs == null) return;

			TextReader				reader = new StreamReader(fs);

			try 
			{
				string one_line = reader.ReadLine();
				if(one_line != null) SmsBasic.msgFormat.bUserDefine = ( ConvertTool.ToInt32(one_line) == 1 ) ? true : false;
				one_line = reader.ReadLine();
				if(one_line != null) SmsBasic.msgFormat.format = one_line;
			}
			catch {}
			reader.Close();
		}

		static public void SaveSmsMessageFormat()
		{
			if(SmsBasic.msgFormat.bChange == false) return;
			if(checkAndMakeSmsDirectory() == false) return;
			string filename = TotalConfig.sDirWorkProject + "\\sms" + "\\msgFormat.ini";
			
			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter				writer = new StreamWriter(fs);
			writer.WriteLine("{0}", (SmsBasic.msgFormat.bUserDefine) ? 1 : 0);
			writer.WriteLine("{0}", SmsBasic.msgFormat.format);
			writer.Close();
		}

		static public void LoadSendSmsFileName()
		{
			SmsBasic.arrSendList.Clear();

			string			path = TotalConfig.GetProjectDataDirectory() + "\\sms";
			sendFileList	list;
			DirectoryInfo	info;

			if(Directory.Exists(path)) 
			{
				info = new DirectoryInfo(path);			
				foreach(FileInfo fi in info.GetFiles("*.sms")) 
				{
					list = new sendFileList();
					list.filename = fi.FullName;
					list.bOldData = true;
					if(fi.Name.Length < 12) list.text = fi.Name;	// 8 + .sms
					else 
					{
						if(Tools.IsLangKorean()) 
						{
							list.text = fi.Name.Substring(0, 4) + "년" + fi.Name.Substring(4, 2) + "월" + fi.Name.Substring(6, 2) + "일";
						}
						else 
						{
							list.text = fi.Name.Substring(0, 4) + "/" + fi.Name.Substring(4, 2) + "/" + fi.Name.Substring(6, 2);
						}
					}
					SmsBasic.arrSendList.Add(list);
				}
			}

			path = TotalConfig.GetProjectDataDirectory() + "\\smsx";
			if(Directory.Exists(path)) 
			{
				info = new DirectoryInfo(path);
				foreach(FileInfo fi in info.GetFiles("*.smsx")) 
				{
					list = new sendFileList();
					list.filename = fi.FullName;
					list.bOldData = false;							// 새로(unicode) 저장된 형식
					if(fi.Name.Length < 13) list.text = fi.Name;	// 8 + .smx
					else 
					{
						if(Tools.IsLangKorean()) 
						{
							list.text = fi.Name.Substring(0, 4) + "년" + fi.Name.Substring(4, 2) + "월" + fi.Name.Substring(6, 2) + "일";
						}
						else 
						{
							list.text = fi.Name.Substring(0, 4) + "/" + fi.Name.Substring(4, 2) + "/" + fi.Name.Substring(6, 2);
						}
					}
					SmsBasic.arrSendList.Add(list);
				}
			}
		}

		static public void LoadSendSmsData(bool bOld, string filename)
		{
			SmsBasic.arrSendData.Clear();
			if(!File.Exists(filename))	return;
			//if(bOld) 
			//{
			//	LoadOldSendSmsData(filename);
			//	return;
			//}
			
			FileStream fs = File.OpenRead(filename);			
			if(fs == null) return;

			TextReader				reader;			
			CommaBlockString		comma = new CommaBlockString();
			string					one_line;
			int						pos = 1;
			sendDataList			data;

			if(bOld) reader = new StreamReader(fs, System.Text.Encoding.Default);
			else	 reader = new StreamReader(fs);
			
			comma.SetBlockCode('>');
			while(true)
			{
				one_line = reader.ReadLine();
				if(one_line == null) break;
				data = new sendDataList();
				comma.Set(one_line);
				data.pos = pos.ToString();				

				comma.GetString(ref data.hour);
				if(comma.IsEOS()) continue;

				comma.GetString(ref data.userName);
				if(comma.IsEOS()) continue;

				comma.GetString(ref data.phoneNo);
				if(comma.IsEOS()) continue;

				comma.GetString(ref data.data);
				data.data = data.data.Trim();
				SmsBasic.arrSendData.Add(data);
				pos++;
				if(pos > 30000) break;
			}
			reader.Close();
		}

		

	}
}
