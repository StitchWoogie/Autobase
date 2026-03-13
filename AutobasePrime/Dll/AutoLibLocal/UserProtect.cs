using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Collections;
using NetTools;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AutoLibLocal
{
	public enum EnumUserRights
	{
		RIGHT_PROGRAMM_END = 0,			// programm end right
		RIGHT_TAG_CHANGE = 1,			// tag의 속성을 바꿀수 있는 권한 
		RIGHT_CONFIG_ALARM = 2,
		RIGHT_CONFIG_ETC = 3,
		RIGHT_ALARM_CONFIRM = 4,
		RIGHT_CONFIG_DATA = 5,
		RIGHT_SCRIPT = 6,
		RIGHT_PLCSCAN_EDIT = 7,
		RIGHT_HAND_INPUT = 8,
		RIGHT_DELETE_ALARM_FILE = 9,
		RIGHT_DELETE_LOG_FILE = 10,
        RIGHT_ALARM_EVENT_DELETE = 11,
        RIGHT_SCHEDULE_SETUP = 12,
        RIGHT_RUN_STUDIO = 13,          // 스튜디오 실행권한 10.2.8.3 부터 지원

        //OPCUA Client 열람/수정 권한 추가 필요.
        //OPCUA Server 실행(중지) / 설정 권한 추가 필요.

        //RESTAPI Monitor 열람/수정 권한 추가 필요.

        RIGHT_RECIPE_VIEW = 14,             // 레시피 열람 권한
        RIGHT_RECIPE_EXECUTE = 15,          // 레시피 실행 권한 (Download/Upload)
        RIGHT_RECIPE_MODIFY = 16,           // 레시피 편집 권한 (생성/수정/삭제)
        RIGHT_RECIPE_APPROVE = 17,          // 레시피 승인 권한 (Draft→Approved, Approved→Obsolete)
        RIGHT_RECIPE_BATCH_CONTROL = 18,    // 배치 제어 권한 (Hold/Restart/Abort)

        RIGHT_IS_ADMIN = 999,
		RIGHT_TAG_MEMBER_ALARM_LEVEL = 1000,
		RIGHT_TAG_MEMBER_DATASAVE = 1001,
		RIGHT_TAG_MEMBER_VIEW_RANGE = 1002,
		RIGHT_TAG_MEMBER_ALARM_ACTIVE = 1003,
	}

    public class UserGroupInfoPublic
    {
        public string sUsername = "public";
        public string sDescription;

        public ArrayList arrayTag = new ArrayList();

        public bool bRightProgrammEnd;			// programm end right
        public bool bRightTagChange;			// Tag 수정 권한
        public bool bRightConfigAlarm;			// 환경설정 수정 권한.
        public bool bRightConfigEtc;			// 기타환경 수정 권한.
        public bool bRightTagMemberAlarmLevel;
        public bool bRightTagMemberDataSave;
        public bool bRightAlarmEventConfirm;
        public bool bRightAlarmEventDelete;     // 9.3.8 부터 추가
        public bool bRightHandInput;
        public bool bRightDeleteAlarmFile;		// 8.5.2, 9.0.2 부터 추가
        public bool bRightDeleteLogFile;		// 8.5.2, 9.0.2 부터 추가

        public bool bRightTagMemberViewRange;	// View-Full View-Base
        public bool bRightConfigData;			// 
        public bool bRightScript;				// 
        public bool bRightPlcScanEdit;			// 
        public bool bRightTagMemberAlarmActive;	// 태그의 경보 여부
        public bool bRightScheduleSetup;	    // 스케쥴 설정 9.3.9 부터 추가
        public bool bRightRunStudio;            // 스튜디오 실행권한 10.2.8 부터 지원

        //OPCUA Client 열람/수정 권한 추가 필요.
        //OPCUA Server 실행(중지) / 설정 권한 추가 필요.

        //RESTAPI Monitor 열람/수정 권한 추가 필요.

        public bool bRightRecipeView;           // 레시피 열람 권한
        public bool bRightRecipeExecute;        // 레시피 실행 권한 (Download/Upload)
        public bool bRightRecipeModify;         // 레시피 편집 권한 (생성/수정/삭제)
        public bool bRightRecipeApprove;        // 레시피 승인 권한 (Draft→Approved, Obsolete)
        public bool bRightRecipeBatchControl;   // 배치 제어 권한 (Hold/Restart/Abort)


        protected virtual bool LoadLocalItem(string command, CommaBlockString comma)
        {
            return false;
        }

        protected bool LoadFile(out string err_msg, string filename, string username)
        {
            err_msg = "";
            if (!File.Exists(filename))
            {
                if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                {
                    if (NetTools.Tools.IsLangKorean())
                        err_msg = "사용자 이름이 존재하지 않거나 암호가 틀립니다.";
                    else if (NetTools.Tools.IsLangChinese())
                        err_msg = "用户名不存在或密码不正确。";
                    else
                        err_msg = "Invalid Username or Password.";
                }
                else
                {
                    if (Tools.IsLangKorean())
                        err_msg = "사용자나 그룹 파일이 존재하지 않습니다.";
                    else
                        err_msg = "Userfile or GroupFile not exists.";
                }

                return false;
            }

            byte[] buffer = File.ReadAllBytes(filename);

            //7E214023  
            //if (buffer.Length > 102 && buffer[0] == '~' && buffer[1] == '!' && buffer[2] == '@' && buffer[3] == '#')
            if (buffer.Length > 102 && buffer[0] == '7' && buffer[1] == 'E' && buffer[2] == '2' && buffer[3] == '1' &&
                buffer.Length > 102 && buffer[4] == '4' && buffer[5] == '0' && buffer[6] == '2' && buffer[7] == '3')
            {
                sUsername = username;
                return LoadByHashedFile(buffer, filename);
            }

            TextReader reader;

            reader = new StreamReader(filename);

            LoadFromTextReader(reader);

            reader.Close();

            sUsername = username;

            return true;
        }

        bool HashBufferRuntime(byte[] buffer, int index, int size)
        {
            int[] rand_hash = new int[17] { 0x78, 0x31, 0x91, 0x78, 0xD7, 0x63, 0xB7, 0x19, 0x7F, 0xB2, 0xCA, 0x8E, 0x23, 0xA8, 0x6F, 0x78, 0x71 };

            for (int i = 0, pos = index; i < size; i++, pos++)
            {
                buffer[pos] = (byte)(buffer[pos] ^ (i % 256) ^ rand_hash[i % 17]);
            }

            return true;
        }

        // Hash된 파일에서 가져온다.
        bool LoadByHashedFile(byte[] buffer_org, string filename)
        {
            byte[] buffer = new byte[buffer_org.Length / 2];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = HexBuf.ToByte(buffer_org, i * 2);
            }

            if (buffer[6] > 1)  // major version
            {
                string msg;
                msg = String.Format("Binary Modx file Major Version too Higher\nFile Version={0}.{1}\nProgram Version=1.0", buffer[6], buffer[7]);
                MessageBox.Show(msg, filename);
                return false;
            }

            HashBufferRuntime(buffer, 8, buffer.Length - 8); // Hash 는 한번만 한다.

            ushort file_crc = (ushort)(buffer[buffer.Length - 2] + buffer[buffer.Length - 1] * 256);
            ushort crc = NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length - 2);

            if (file_crc != crc)
            {
                MessageBox.Show("Binary Modx CRC mismatched.", filename);
                return false;
            }

            uint size = (uint)(buffer[8] + (buffer[9] << 8) + (buffer[10] << 16) + (buffer[11] << 24));

            if (size != buffer.Length)
            {
                MessageBox.Show("Size mismatched.", filename);
                return false;
            }

            byte[] buffer2 = new byte[buffer.Length - 102];

            Array.Copy(buffer, 100, buffer2, 0, buffer.Length - 102);

            //HashBufferRuntime(buffer2, 0, buffer2.Length);

            MemoryStream stream = new MemoryStream(buffer2);

            TextReader reader = new StreamReader(stream);

            LoadFromTextReader(reader);

            stream.Close();

            return true;
        }

        protected bool LoadFromTextReader(TextReader reader)
        {
            string one_line;
            string command = "";
            CommaBlockString comma = new CommaBlockString();

            //sUsername = Path.GetFileNameWithoutExtension(filename);
            //sPassCode = ZipPassword(sUsername, sUsername).ToString();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref command);

                if (LoadLocalItem(command, comma))
                {

                }
                else if (command == "sDescription")
                {
                    comma.GetStringTotalRemain(ref sDescription);
                }
                else if (command == "Tag")
                {
                    string tag = "";
                    comma.GetStringTotalRemain(ref tag);
                    arrayTag.Add(tag);
                }
                else if (command == "bRightProgrammEnd")
                {
                    comma.GetBool(ref bRightProgrammEnd);
                }
                else if (command == "bRightTagChange")
                {
                    comma.GetBool(ref bRightTagChange);
                }
                else if (command == "bRightConfigAlarm")
                {
                    comma.GetBool(ref bRightConfigAlarm);
                }
                else if (command == "bRightConfigEtc")
                {
                    comma.GetBool(ref bRightConfigEtc);
                }
                else if (command == "bRightTagMemberAlarmLevel")
                {
                    comma.GetBool(ref bRightTagMemberAlarmLevel);
                }
                else if (command == "bRightTagMemberDataSave")
                {
                    comma.GetBool(ref bRightTagMemberDataSave);
                }
                else if (command == "bRightAlarmConfirm")
                {
                    comma.GetBool(ref bRightAlarmEventConfirm);
                }
                else if (command == "bRightAlarmEventDelete")
                {
                    comma.GetBool(ref bRightAlarmEventDelete);
                }

                else if (command == "bRightTagMemberViewRange")
                    comma.GetBool(ref bRightTagMemberViewRange);
                else if (command == "bRightConfigData")
                    comma.GetBool(ref bRightConfigData);
                else if (command == "bRightScript")
                    comma.GetBool(ref bRightScript);
                else if (command == "bRightPlcScanEdit")
                    comma.GetBool(ref bRightPlcScanEdit);
                else if (command == "bRightTagMemberAlarmActive")
                    comma.GetBool(ref bRightTagMemberAlarmActive);

                else if (command == "bRightHandInput")
                    comma.GetBool(ref bRightHandInput);
                else if (command == "bRightDeleteAlarmFile")
                    comma.GetBool(ref bRightDeleteAlarmFile);
                else if (command == "bRightDeleteLogFile")
                    comma.GetBool(ref bRightDeleteLogFile);
                else if (command == "bRightScheduleSetup")
                    comma.GetBool(ref bRightScheduleSetup);
                else if (command == "bRightRunStudio")
                    comma.GetBool(ref bRightRunStudio);

                else if (command == "bRightRecipeView")
                    comma.GetBool(ref bRightRecipeView);
                else if (command == "bRightRecipeExecute")
                    comma.GetBool(ref bRightRecipeExecute);
                else if (command == "bRightRecipeModify")
                    comma.GetBool(ref bRightRecipeModify);
                else if (command == "bRightRecipeApprove")
                    comma.GetBool(ref bRightRecipeApprove);
                else if (command == "bRightRecipeBatchControl")
                    comma.GetBool(ref bRightRecipeBatchControl);

                else { }
            }

            return true;
        }

        /*
        protected bool LoadFile(out string err_msg, string filename)
        {
            err_msg = "";
            if (!File.Exists(filename))
            {
                if (Tools.IsLangKorean())
                    err_msg = "사용자나 그룹 파일이 존재하지 않습니다.";
                else
                    err_msg = "Userfile or GroupFile not exists.";

                return false;
            }

            string one_line;
            string command = "";
            Stream fs = File.OpenRead(filename);
            TextReader reader = new StreamReader(fs);
            CommaBlockString comma = new CommaBlockString();

            sUsername = Path.GetFileNameWithoutExtension(filename);
            //sPassCode = ZipPassword(sUsername, sUsername).ToString();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref command);

                if (LoadLocalItem(command, comma))
                {

                }
                else if (command == "sDescription")
                {
                    comma.GetStringTotalRemain(ref sDescription);
                }
                else if (command == "Tag")
                {
                    string tag = "";
                    comma.GetStringTotalRemain(ref tag);
                    arrayTag.Add(tag);
                }
                else if (command == "bRightProgrammEnd")
                {
                    comma.GetBool(ref bRightProgrammEnd);
                }
                else if (command == "bRightTagChange")
                {
                    comma.GetBool(ref bRightTagChange);
                }
                else if (command == "bRightConfigAlarm")
                {
                    comma.GetBool(ref bRightConfigAlarm);
                }
                else if (command == "bRightConfigEtc")
                {
                    comma.GetBool(ref bRightConfigEtc);
                }
                else if (command == "bRightTagMemberAlarmLevel")
                {
                    comma.GetBool(ref bRightTagMemberAlarmLevel);
                }
                else if (command == "bRightTagMemberDataSave")
                {
                    comma.GetBool(ref bRightTagMemberDataSave);
                }
                else if (command == "bRightAlarmConfirm")
                {
                    comma.GetBool(ref bRightAlarmEventConfirm);
                }
                else if (command == "bRightAlarmEventDelete")
                {
                    comma.GetBool(ref bRightAlarmEventDelete);
                }

                else if (command == "bRightTagMemberViewRange")
                    comma.GetBool(ref bRightTagMemberViewRange);
                else if (command == "bRightConfigData")
                    comma.GetBool(ref bRightConfigData);
                else if (command == "bRightScript")
                    comma.GetBool(ref bRightScript);
                else if (command == "bRightPlcScanEdit")
                    comma.GetBool(ref bRightPlcScanEdit);
                else if (command == "bRightTagMemberAlarmActive")
                    comma.GetBool(ref bRightTagMemberAlarmActive);

                else if (command == "bRightHandInput")
                    comma.GetBool(ref bRightHandInput);
                else if (command == "bRightDeleteAlarmFile")
                    comma.GetBool(ref bRightDeleteAlarmFile);
                else if (command == "bRightDeleteLogFile")
                    comma.GetBool(ref bRightDeleteLogFile);
                else if (command == "bRightScheduleSetup")
                    comma.GetBool(ref bRightScheduleSetup);
                else if (command == "bRightRunStudio")
                    comma.GetBool(ref bRightRunStudio);

                else if (command == "bRightRecipeView")
                    comma.GetBool(ref bRightRecipeView);
                else if (command == "bRightRecipeExecute")
                    comma.GetBool(ref bRightRecipeExecute);
                else if (command == "bRightRecipeModify")
                    comma.GetBool(ref bRightRecipeModify);
                else if (command == "bRightRecipeApprove")
                    comma.GetBool(ref bRightRecipeApprove);
                else if (command == "bRightRecipeBatchControl")
                    comma.GetBool(ref bRightRecipeBatchControl);

                else { }
            }

            reader.Close();

            return true;
        }*/

        protected virtual void SaveLocalItem(TextWriter writer)
        {

        }

        void SaveFileHash(string filename)
        {
            MemoryStream stream = new MemoryStream();

            TextWriter writer = new StreamWriter(stream);

            SaveFilePublic(writer);

            writer.Flush(); // TextWriter를 끝낸 후 Flush를 호출하지 않으니까 파일이 13512 크기까지만 저장되고 뒤로 잘림 현상이 있음 이부분을 추가하니 이런현상이 없어졌다. 아마 상속 받은 후 연결 고리를 끊는 듯 하다.
            // 부모가 MemoryStream인 경우만 발생하는 듯 하다.

            byte[] buffer = stream.ToArray();

            writer.Close();

            //HashBufferRuntime(buffer, 0, buffer.Length);

            int total_length = 100 + buffer.Length + 2;

            byte[] buffer2 = new byte[total_length];

            buffer2[0] = (byte)'~';
            buffer2[1] = (byte)'!';
            buffer2[2] = (byte)'@';
            buffer2[3] = (byte)'#';
            buffer2[4] = (byte)0x0D;
            buffer2[5] = (byte)0x0A;
            buffer2[6] = (byte)1;   // major version
            buffer2[7] = (byte)0;   // minor version

            buffer2[8] = (byte)((total_length >> 0) & 0xFF);
            buffer2[9] = (byte)((total_length >> 8) & 0xFF);
            buffer2[10] = (byte)((total_length >> 16) & 0xFF);
            buffer2[11] = (byte)((total_length >> 24) & 0xFF);

            for (int i = 12; i < 100; i++)
            {
                buffer2[i] = 0;
            }

            Array.Copy(buffer, 0, buffer2, 100, buffer.Length);

            ushort crc = NetTools.GetCRC.SumWORD(buffer2, 0, total_length - 2);

            buffer2[total_length - 2] = (byte)((crc >> 0) & 0xFF);
            buffer2[total_length - 1] = (byte)((crc >> 8) & 0xFF);

            HashBufferRuntime(buffer2, 8, total_length - 8);

            byte[] buffer3 = new byte[total_length * 2];
            string imsi;

            for (int i = 0; i < total_length; i++)
            {
                imsi = buffer2[i].ToString("X02");
                buffer3[i * 2] = (byte)(imsi[0]);
                buffer3[i * 2 + 1] = (byte)(imsi[1]);
            }

            File.WriteAllBytes(filename, buffer3);
        }

        protected void SaveFile(string username, bool group_flag)
        {
            string work_dir = TotalConfig.sDirWorkProject;
            string dir;
            string filename;

            dir = String.Format("{0}\\Users", work_dir);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!Directory.Exists(dir))	// 폴더를 만들지 못했다.
                return;

            if (group_flag)
                filename = String.Format("{0}\\Users\\{1}.Group", work_dir, username);
            else
            {
                filename = String.Format("{0}\\Users\\{1}.user", work_dir, UserInfoStruct.EncodeUserFilename(username));
            }

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                SaveFileHash(filename);
            }
            else
            {
                Stream fs = File.Open(filename, FileMode.Create);
                TextWriter writer = new StreamWriter(fs);
                SaveFilePublic(writer);
                writer.Close();
            }
        }

        void SaveFilePublic(TextWriter writer)
        {
            SaveLocalItem(writer);

            //if (sPassCode.Length == 0) sPassCode = ZipPassword(sUsername, "").ToString();			// 암호가 없을때는 비운다. (한글사용자일때는 한글암호를 기본으로 하기는 곤란하다.)

            //writer.WriteLine("sPassCode," + sPassCode);
            writer.WriteLine("sDescription," + sDescription);
            //writer.WriteLine("nWebValueUpdateTime," + nWebValueUpdateTime.ToString());
            //writer.WriteLine("sStartPage," + sStartPage);
            //writer.WriteLine("bAutoOpenStartPage," + bAutoOpenStartPage);

            writer.WriteLine("bRightProgrammEnd,{0}", bRightProgrammEnd);
            writer.WriteLine("bRightTagChange,{0}", bRightTagChange);
            writer.WriteLine("bRightConfigAlarm,{0}", bRightConfigAlarm);
            writer.WriteLine("bRightConfigEtc,{0}", bRightConfigEtc);
            writer.WriteLine("bRightTagMemberAlarmLevel,{0}", bRightTagMemberAlarmLevel);
            writer.WriteLine("bRightTagMemberDataSave,{0}", bRightTagMemberDataSave);
            writer.WriteLine("bRightAlarmConfirm,{0}", bRightAlarmEventConfirm);
            writer.WriteLine("bRightAlarmEventDelete,{0}", bRightAlarmEventDelete);

            writer.WriteLine("bRightTagMemberViewRange,{0}", bRightTagMemberViewRange);
            writer.WriteLine("bRightConfigData,{0}", bRightConfigData);
            writer.WriteLine("bRightScript,{0}", bRightScript);
            writer.WriteLine("bRightPlcScanEdit,{0}", bRightPlcScanEdit);
            writer.WriteLine("bRightTagMemberAlarmActive,{0}", bRightTagMemberAlarmActive);

            writer.WriteLine("bRightHandInput,{0}", bRightHandInput);
            writer.WriteLine("bRightDeleteAlarmFile,{0}", bRightDeleteAlarmFile);
            writer.WriteLine("bRightDeleteLogFile,{0}", bRightDeleteLogFile);

            writer.WriteLine("bRightScheduleSetup,{0}", bRightScheduleSetup);

            writer.WriteLine("bRightRunStudio,{0}", bRightRunStudio);

            writer.WriteLine("bRightRecipeView,{0}", bRightRecipeView);
            writer.WriteLine("bRightRecipeExecute,{0}", bRightRecipeExecute);
            writer.WriteLine("bRightRecipeModify,{0}", bRightRecipeModify);
            writer.WriteLine("bRightRecipeApprove,{0}", bRightRecipeApprove);
            writer.WriteLine("bRightRecipeBatchControl,{0}", bRightRecipeBatchControl);

            //writer.WriteLine("bSmartPhoneUseGraphic,{0}", bSmartPhoneUseGraphic);
            //writer.WriteLine("sSmartPhoneStartupPage,{0}", sSmartPhoneStartupPage);

            for (int i = 0; i < arrayTag.Count; i++)
            {
                writer.WriteLine("Tag," + ((string)arrayTag[i]));
            }
        }

        /*
        protected void SaveFile(string username, bool group_flag)
        {
            string work_dir = TotalConfig.sDirWorkProject;
            string dir;
            string filename;

            dir = String.Format("{0}\\Users", work_dir);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!Directory.Exists(dir))	// 폴더를 만들지 못했다.
                return;

            if(group_flag)
                filename = String.Format("{0}\\Users\\{1}.Group", work_dir, username);
            else
                filename = String.Format("{0}\\Users\\{1}.user", work_dir, username);

            Stream fs = File.Open(filename, FileMode.Create);
            TextWriter writer = new StreamWriter(fs);

            SaveLocalItem(writer);

            //if (sPassCode.Length == 0) sPassCode = ZipPassword(sUsername, "").ToString();			// 암호가 없을때는 비운다. (한글사용자일때는 한글암호를 기본으로 하기는 곤란하다.)

            //writer.WriteLine("sPassCode," + sPassCode);
            writer.WriteLine("sDescription," + sDescription);
            //writer.WriteLine("nWebValueUpdateTime," + nWebValueUpdateTime.ToString());
            //writer.WriteLine("sStartPage," + sStartPage);
            //writer.WriteLine("bAutoOpenStartPage," + bAutoOpenStartPage);

            writer.WriteLine("bRightProgrammEnd,{0}", bRightProgrammEnd);
            writer.WriteLine("bRightTagChange,{0}", bRightTagChange);
            writer.WriteLine("bRightConfigAlarm,{0}", bRightConfigAlarm);
            writer.WriteLine("bRightConfigEtc,{0}", bRightConfigEtc);
            writer.WriteLine("bRightTagMemberAlarmLevel,{0}", bRightTagMemberAlarmLevel);
            writer.WriteLine("bRightTagMemberDataSave,{0}", bRightTagMemberDataSave);
            writer.WriteLine("bRightAlarmConfirm,{0}", bRightAlarmEventConfirm);
            writer.WriteLine("bRightAlarmEventDelete,{0}", bRightAlarmEventDelete);

            writer.WriteLine("bRightTagMemberViewRange,{0}", bRightTagMemberViewRange);
            writer.WriteLine("bRightConfigData,{0}", bRightConfigData);
            writer.WriteLine("bRightScript,{0}", bRightScript);
            writer.WriteLine("bRightPlcScanEdit,{0}", bRightPlcScanEdit);
            writer.WriteLine("bRightTagMemberAlarmActive,{0}", bRightTagMemberAlarmActive);

            writer.WriteLine("bRightHandInput,{0}", bRightHandInput);
            writer.WriteLine("bRightDeleteAlarmFile,{0}", bRightDeleteAlarmFile);
            writer.WriteLine("bRightDeleteLogFile,{0}", bRightDeleteLogFile);

            writer.WriteLine("bRightScheduleSetup,{0}", bRightScheduleSetup);

            writer.WriteLine("bRightRunStudio,{0}", bRightRunStudio);

            //writer.WriteLine("bSmartPhoneUseGraphic,{0}", bSmartPhoneUseGraphic);
            //writer.WriteLine("sSmartPhoneStartupPage,{0}", sSmartPhoneStartupPage);

            for (int i = 0; i < arrayTag.Count; i++)
            {
                writer.WriteLine("Tag," + ((string)arrayTag[i]));
            }

            writer.Close();
        }*/

        public static string GetPathUserCommonProperty()
        {
            string path = String.Format("{0}\\Users\\UserCommonProperty.cfg", TotalConfig.sDirWorkProject);

            return path;
        }

        public virtual bool IsHaveAllRights()
        {
            return false;
        }

        public virtual bool IsHaveTagRight(string tag)
        {
            if (IsHaveAllRights()) return true;

            for (int i = 0; i < arrayTag.Count; i++)
            {
                if ((string)arrayTag[i] == tag) return true;
            }

            return false;
        }

        public virtual bool IsHaveRight(EnumUserRights id)
        {
            if (IsHaveAllRights()) return true;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                if (String.Compare(sUsername, "_PUBLIC_", true) != 0 &&
                    String.Compare(sUsername, "PUBLIC", true) != 0)
                {

                    // 권한 탭이 없어지면서 기본권한이 주어짐
                    if (id == EnumUserRights.RIGHT_PROGRAMM_END) return true;
                    if (id == EnumUserRights.RIGHT_ALARM_CONFIRM) return true;
                    if (id == EnumUserRights.RIGHT_ALARM_EVENT_DELETE) return true;
                    if (id == EnumUserRights.RIGHT_SCRIPT) return true;
                    if (id == EnumUserRights.RIGHT_DELETE_ALARM_FILE) return true;
                    if (id == EnumUserRights.RIGHT_DELETE_LOG_FILE) return true;

                    if (id == EnumUserRights.RIGHT_SCHEDULE_SETUP) return true;

                    if (id == EnumUserRights.RIGHT_RECIPE_VIEW) return true;
                    if (id == EnumUserRights.RIGHT_RECIPE_EXECUTE) return true;
                    if (id == EnumUserRights.RIGHT_RECIPE_MODIFY) return true;
                    if (id == EnumUserRights.RIGHT_RECIPE_APPROVE) return true;
                    if (id == EnumUserRights.RIGHT_RECIPE_BATCH_CONTROL) return true;
                }
            }

            if (id == EnumUserRights.RIGHT_ALARM_CONFIRM) return this.bRightAlarmEventConfirm;
            if (id == EnumUserRights.RIGHT_ALARM_EVENT_DELETE) return this.bRightAlarmEventDelete;
            if (id == EnumUserRights.RIGHT_CONFIG_ALARM) return this.bRightConfigAlarm;
            if (id == EnumUserRights.RIGHT_CONFIG_DATA) return this.bRightConfigData;
            if (id == EnumUserRights.RIGHT_CONFIG_ETC) return this.bRightConfigEtc;
            if (id == EnumUserRights.RIGHT_HAND_INPUT) return this.bRightHandInput;
            if (id == EnumUserRights.RIGHT_PLCSCAN_EDIT) return this.bRightPlcScanEdit;
            if (id == EnumUserRights.RIGHT_PROGRAMM_END) return this.bRightProgrammEnd;
            if (id == EnumUserRights.RIGHT_SCRIPT) return this.bRightScript;
            if (id == EnumUserRights.RIGHT_TAG_CHANGE) return this.bRightTagChange;
            if (id == EnumUserRights.RIGHT_DELETE_ALARM_FILE) return this.bRightDeleteAlarmFile;
            if (id == EnumUserRights.RIGHT_DELETE_LOG_FILE) return this.bRightDeleteLogFile;
            if (id == EnumUserRights.RIGHT_SCHEDULE_SETUP) return this.bRightScheduleSetup;
            if (id == EnumUserRights.RIGHT_RUN_STUDIO) return this.bRightRunStudio;

            if (id == EnumUserRights.RIGHT_RECIPE_VIEW) return this.bRightRecipeView;
            if (id == EnumUserRights.RIGHT_RECIPE_EXECUTE) return this.bRightRecipeExecute;
            if (id == EnumUserRights.RIGHT_RECIPE_MODIFY) return this.bRightRecipeModify;
            if (id == EnumUserRights.RIGHT_RECIPE_APPROVE) return this.bRightRecipeApprove;
            if (id == EnumUserRights.RIGHT_RECIPE_BATCH_CONTROL) return this.bRightRecipeBatchControl;

            if (id == EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE) return this.bRightTagMemberAlarmActive;
            if (id == EnumUserRights.RIGHT_TAG_MEMBER_ALARM_LEVEL) return this.bRightTagMemberAlarmLevel;
            if (id == EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE) return this.bRightTagMemberDataSave;
            if (id == EnumUserRights.RIGHT_TAG_MEMBER_VIEW_RANGE) return this.bRightTagMemberViewRange;

            return false;
        }
    }

    public class GroupInfoStruct : UserGroupInfoPublic
    {
        public static List<GroupInfoStruct> arrayGroups;        // 프로그램 시작 시 읽어와서 끝날때 까지 계속 사용한다.

        public List<string> arrayMembers = new List<string>();

        static GroupInfoStruct()
        {
            arrayGroups = new List<GroupInfoStruct>();

            LoadGroupLists();
        }

        static void LoadGroupLists()
        {
            string dir = TotalConfig.sDirWorkProject;

            dir = String.Format("{0}\\Users", dir);

            if (!Directory.Exists(dir)) return;

            DirectoryInfo info = new DirectoryInfo(dir);

            string err_msg;

            string group_name;

            foreach (FileInfo fi in info.GetFiles("*.group"))
            {
                group_name = Path.GetFileNameWithoutExtension(fi.FullName);

                GroupInfoStruct gi = new GroupInfoStruct();
                if (gi.LoadGroup(out err_msg, fi.FullName, group_name))
                    arrayGroups.Add(gi);
            }
        }

        public bool LoadGroup(out string err_msg, string filename, string username)
        {
            return LoadFile(out err_msg, filename, username);
        }

        public void SaveGroup(string username)
        {
            SaveFile(username, true);
        }

        protected override bool LoadLocalItem(string command, CommaBlockString comma)
        {
            if (command == "sMembers")
                arrayMembers.Add(comma.GetString());
            else
            {
                return false;
            }

            return true;
        }

        public bool IsUserHaveTagRight(string username, string tag)
        {
            for (int i = 0; i < arrayMembers.Count; i++)
            {
                if (String.Compare(username, arrayMembers[i], true) == 0)
                {
                    return IsHaveTagRight(tag);
                }
            }

            return false;
        }

        public bool IsUserHaveRight(string username, EnumUserRights id)
        {
            for (int i = 0; i < arrayMembers.Count; i++)
            {
                if (String.Compare(username, arrayMembers[i], true) == 0)
                {
                    return IsHaveRight(id);
                }
            }

            return false;
        }

        protected override void SaveLocalItem(TextWriter writer)
        {
            for (int i = 0; i < arrayMembers.Count; i++)
            {
                writer.WriteLine("sMembers,{0}", arrayMembers[i]);
            }
        }
    }

	public class UserInfoStruct : UserGroupInfoPublic
	{
		//public string sUsername = "public";
		public string sPassCode = "";
        public string sHashCode256 = "";        // 10.3.1.6 부터 지원
		
		public int nWebValueUpdateTime = 10;
		public string sStartPage = "";
		public bool bAutoOpenStartPage = false;
		/*public ArrayList arrayTag = new ArrayList();

		public bool bRightProgrammEnd;			// programm end right
		public bool bRightTagChange;			// Tag 수정 권한
		public bool bRightConfigAlarm;			// 환경설정 수정 권한.
		public bool bRightConfigEtc;			// 기타환경 수정 권한.
		public bool bRightTagMemberAlarmLevel;
		public bool bRightTagMemberDataSave;
		public bool bRightAlarmEventConfirm;
        public bool bRightAlarmEventDelete;     // 9.3.8 부터 추가
		public bool bRightHandInput;
		public bool bRightDeleteAlarmFile;		// 8.5.2, 9.0.2 부터 추가
		public bool bRightDeleteLogFile;		// 8.5.2, 9.0.2 부터 추가

		public bool bRightTagMemberViewRange;	// View-Full View-Base
		public bool bRightConfigData;			// 
		public bool bRightScript;				// 
		public bool bRightPlcScanEdit;			// 
		public bool bRightTagMemberAlarmActive;	// 태그의 경보 여부
        public bool bRightScheduleSetup;	    // 스케쥴 설정 9.3.9 부터 추가*/

        public bool bSmartPhoneUseGraphic;	    // 10.2.4 부터 추가
        public string sSmartPhoneStartupPage;	// 10.2.4 부터 추가

        // 10.2.4.2 부터 추가
        public bool bUseAutoLockOnPasswordMismatched;
        public bool bUsePasswordCharactorSecurity;
        public bool bUsePasswordExpiryDays;

        public DateTime tPasswordChange = DateTime.Now;        // Password를 변경한 최종 날짜

        public int nPasswordMismatchedCount = 0;    // 연속해서 암호가 틀린 횟수

		public UserInfoStruct()
		{
	        
		}

        public bool LoadUser(out string err_msg, string filename, string username)
        {
            return LoadFile(out err_msg, filename, username);
        }

        public void SaveUser(string username)
        {
            // 웹서버에서는 권한이 충분하지 못해서 쓰기 오류가 난다. IIS_IUSERS 사용자에 쓰기 권한을 부여하면 패스워드 횟수 체크를 할 수 있다.
            try
            {
                SaveFile(username, false);
            }
            catch
            {

            }
        }

        /*
		public bool LoadFile(out string err_msg, string filename)
		{
			err_msg = "";
			if(!File.Exists(filename)) 
			{
				if(Tools.IsLangKorean())
					err_msg = "사용자 파일이 존재하지 않습니다.";
				else
					err_msg = "Userfile not exist.";

				return false;
			}

			string one_line;
			string command="";
			Stream fs = File.OpenRead(filename);
			TextReader reader = new StreamReader(fs);
			CommaBlockString comma = new CommaBlockString();

			sUsername = Path.GetFileNameWithoutExtension(filename);
			sPassCode = ZipPassword(sUsername, sUsername).ToString();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "sDescription") 
				{
					comma.GetStringTotalRemain(ref sDescription);
				}
				else if(command == "sPassCode") 
				{
					comma.GetStringTotalRemain(ref sPassCode);
				}
				else if(command == "nWebValueUpdateTime")
				{
					comma.GetInt(ref nWebValueUpdateTime);
				}
				else if(command == "sStartPage")
				{
					comma.GetString(ref sStartPage);
				}
				else if(command == "bAutoOpenStartPage")
				{
					comma.GetBool(ref bAutoOpenStartPage);
				}
				else if(command == "Tag") 
				{
					string tag="";
					comma.GetStringTotalRemain(ref tag);
					arrayTag.Add(tag);
				}
				else if(command == "bRightProgrammEnd") 
				{
					comma.GetBool(ref bRightProgrammEnd);
				}
				else if(command == "bRightTagChange") 
				{
					comma.GetBool(ref bRightTagChange);
				}
				else if(command == "bRightConfigAlarm") 
				{
					comma.GetBool(ref bRightConfigAlarm);
				}
				else if(command == "bRightConfigEtc") 
				{
					comma.GetBool(ref bRightConfigEtc);
				}
				else if(command == "bRightTagMemberAlarmLevel") 
				{
					comma.GetBool(ref bRightTagMemberAlarmLevel);
				}
				else if(command == "bRightTagMemberDataSave") 
				{
					comma.GetBool(ref bRightTagMemberDataSave);
				}
				else if(command == "bRightAlarmConfirm") 
				{
					comma.GetBool(ref bRightAlarmEventConfirm);
				}
                else if (command == "bRightAlarmEventDelete")
                {
                    comma.GetBool(ref bRightAlarmEventDelete);
                }

				else if(command == "bRightTagMemberViewRange") 
					comma.GetBool(ref bRightTagMemberViewRange);
				else if(command == "bRightConfigData") 
					comma.GetBool(ref bRightConfigData);
				else if(command == "bRightScript") 
					comma.GetBool(ref bRightScript);
				else if(command == "bRightPlcScanEdit") 
					comma.GetBool(ref bRightPlcScanEdit);
				else if(command == "bRightTagMemberAlarmActive") 
					comma.GetBool(ref bRightTagMemberAlarmActive);

				else if(command == "bRightHandInput") 
					comma.GetBool(ref bRightHandInput);
				else if(command == "bRightDeleteAlarmFile") 
					comma.GetBool(ref bRightDeleteAlarmFile);
				else if(command == "bRightDeleteLogFile") 
					comma.GetBool(ref bRightDeleteLogFile);
                else if (command == "bRightScheduleSetup")
                    comma.GetBool(ref bRightScheduleSetup);

                else if (command == "bSmartPhoneUseGraphic")
                    comma.GetBool(ref bSmartPhoneUseGraphic);
                else if (command == "sSmartPhoneStartupPage")
                    comma.GetString(ref sSmartPhoneStartupPage);

				else {}
			}

			reader.Close();

			return true;
		}
        */

        protected override bool LoadLocalItem(string command, CommaBlockString comma)
        {
            /*
            if (command == "sDescription")
            {
                comma.GetStringTotalRemain(ref sDescription);
            }*/
            if (command == "sPassCode")
            {
                comma.GetStringTotalRemain(ref sPassCode);
            }
            else if (command == "sHashCode2")
            {
                comma.GetStringTotalRemain(ref sHashCode256);
            }
            else if (command == "nWebValueUpdateTime")
            {
                comma.GetInt(ref nWebValueUpdateTime);
            }
            else if (command == "sStartPage")
            {
                comma.GetString(ref sStartPage);
            }
            else if (command == "bAutoOpenStartPage")
            {
                comma.GetBool(ref bAutoOpenStartPage);
            }
            /*
            else if (command == "Tag")
            {
                string tag = "";
                comma.GetStringTotalRemain(ref tag);
                arrayTag.Add(tag);
            }
            else if (command == "bRightProgrammEnd")
            {
                comma.GetBool(ref bRightProgrammEnd);
            }
            else if (command == "bRightTagChange")
            {
                comma.GetBool(ref bRightTagChange);
            }
            else if (command == "bRightConfigAlarm")
            {
                comma.GetBool(ref bRightConfigAlarm);
            }
            else if (command == "bRightConfigEtc")
            {
                comma.GetBool(ref bRightConfigEtc);
            }
            else if (command == "bRightTagMemberAlarmLevel")
            {
                comma.GetBool(ref bRightTagMemberAlarmLevel);
            }
            else if (command == "bRightTagMemberDataSave")
            {
                comma.GetBool(ref bRightTagMemberDataSave);
            }
            else if (command == "bRightAlarmConfirm")
            {
                comma.GetBool(ref bRightAlarmEventConfirm);
            }
            else if (command == "bRightAlarmEventDelete")
            {
                comma.GetBool(ref bRightAlarmEventDelete);
            }

            else if (command == "bRightTagMemberViewRange")
                comma.GetBool(ref bRightTagMemberViewRange);
            else if (command == "bRightConfigData")
                comma.GetBool(ref bRightConfigData);
            else if (command == "bRightScript")
                comma.GetBool(ref bRightScript);
            else if (command == "bRightPlcScanEdit")
                comma.GetBool(ref bRightPlcScanEdit);
            else if (command == "bRightTagMemberAlarmActive")
                comma.GetBool(ref bRightTagMemberAlarmActive);

            else if (command == "bRightHandInput")
                comma.GetBool(ref bRightHandInput);
            else if (command == "bRightDeleteAlarmFile")
                comma.GetBool(ref bRightDeleteAlarmFile);
            else if (command == "bRightDeleteLogFile")
                comma.GetBool(ref bRightDeleteLogFile);
            else if (command == "bRightScheduleSetup")
                comma.GetBool(ref bRightScheduleSetup);*/

            else if (command == "bSmartPhoneUseGraphic")
                comma.GetBool(ref bSmartPhoneUseGraphic);
            else if (command == "sSmartPhoneStartupPage")
                comma.GetString(ref sSmartPhoneStartupPage);

            else if (command == "bUseAutoLockOnPasswordMismatched")
                comma.GetBool(ref bUseAutoLockOnPasswordMismatched);
            else if (command == "bUsePasswordCharactorSecurity")
                comma.GetBool(ref bUsePasswordCharactorSecurity);
            else if (command == "bUsePasswordExpiryDays")
                comma.GetBool(ref bUsePasswordExpiryDays);

            else if (command == "nPasswordMismatchedCount")
                comma.GetInt(ref nPasswordMismatchedCount);

            else if (command == "tPasswordChange")
                tPasswordChange = comma.GetDateTime();

            else {
                return false;
            }

            return true;
        }

        protected override void SaveLocalItem(TextWriter writer)
        {
            if (sPassCode.Length == 0) 
                sPassCode = ZipPassword(sUsername, "");			// 암호가 없을때는 비운다. (한글사용자일때는 한글암호를 기본으로 하기는 곤란하다.)

            if (sHashCode256.Length == 0) 
                sHashCode256 = ZipPassword256(sUsername, "");

            writer.WriteLine("sPassCode," + sPassCode);
            writer.WriteLine("sHashCode2," + sHashCode256);
            //writer.WriteLine("sDescription," + sDescription);
            writer.WriteLine("nWebValueUpdateTime," + nWebValueUpdateTime.ToString());
            writer.WriteLine("sStartPage," + sStartPage);
            writer.WriteLine("bAutoOpenStartPage," + bAutoOpenStartPage);

            /*
            writer.WriteLine("bRightProgrammEnd,{0}", bRightProgrammEnd);
            writer.WriteLine("bRightTagChange,{0}", bRightTagChange);
            writer.WriteLine("bRightConfigAlarm,{0}", bRightConfigAlarm);
            writer.WriteLine("bRightConfigEtc,{0}", bRightConfigEtc);
            writer.WriteLine("bRightTagMemberAlarmLevel,{0}", bRightTagMemberAlarmLevel);
            writer.WriteLine("bRightTagMemberDataSave,{0}", bRightTagMemberDataSave);
            writer.WriteLine("bRightAlarmConfirm,{0}", bRightAlarmEventConfirm);
            writer.WriteLine("bRightAlarmEventDelete,{0}", bRightAlarmEventDelete);

            writer.WriteLine("bRightTagMemberViewRange,{0}", bRightTagMemberViewRange);
            writer.WriteLine("bRightConfigData,{0}", bRightConfigData);
            writer.WriteLine("bRightScript,{0}", bRightScript);
            writer.WriteLine("bRightPlcScanEdit,{0}", bRightPlcScanEdit);
            writer.WriteLine("bRightTagMemberAlarmActive,{0}", bRightTagMemberAlarmActive);

            writer.WriteLine("bRightHandInput,{0}", bRightHandInput);
            writer.WriteLine("bRightDeleteAlarmFile,{0}", bRightDeleteAlarmFile);
            writer.WriteLine("bRightDeleteLogFile,{0}", bRightDeleteLogFile);

            writer.WriteLine("bRightScheduleSetup,{0}", bRightScheduleSetup);*/

            writer.WriteLine("bSmartPhoneUseGraphic,{0}", bSmartPhoneUseGraphic);
            writer.WriteLine("sSmartPhoneStartupPage,{0}", sSmartPhoneStartupPage);

            writer.WriteLine("bUseAutoLockOnPasswordMismatched,{0}", bUseAutoLockOnPasswordMismatched);
            writer.WriteLine("bUsePasswordCharactorSecurity,{0}", bUsePasswordCharactorSecurity);
            writer.WriteLine("bUsePasswordExpiryDays,{0}", bUsePasswordExpiryDays);

            writer.WriteLine("nPasswordMismatchedCount,{0}", nPasswordMismatchedCount);

            writer.WriteLine("tPasswordChange,{0}-{1}-{2}", tPasswordChange.Year, tPasswordChange.Month, tPasswordChange.Day);
        }

		/// <summary>
		/// 로컬메인에서 사용하는 함수. 권한을 레지스트리에 저장하여 다른 프로그램에서 권한을 사용할 수 있도록 한다.
		/// 예) 통신프로그램의 통신 편집 권한
		/// </summary>
		public void SaveRightsToRegistry()
		{
			if(!TotalConfig.bLocalMain)	return;	// 로컬메인에서 로그인 했을때만 레지스트리에 기록한다.

			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_IS_ADMIN).ToString(), IsAdmin());

			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_PROGRAMM_END).ToString(), this.bRightProgrammEnd);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_TAG_CHANGE).ToString(), this.bRightTagChange);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_CONFIG_ALARM).ToString(), this.bRightConfigAlarm);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_CONFIG_ETC).ToString(), this.bRightConfigEtc);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_ALARM_CONFIRM).ToString(), this.bRightAlarmEventConfirm);
            TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_ALARM_EVENT_DELETE).ToString(), this.bRightAlarmEventDelete);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_CONFIG_DATA).ToString(), this.bRightConfigData);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_SCRIPT).ToString(), this.bRightScript);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_PLCSCAN_EDIT).ToString(), this.bRightPlcScanEdit);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_HAND_INPUT).ToString(), this.bRightHandInput);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_DELETE_ALARM_FILE).ToString(), this.bRightDeleteAlarmFile);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_DELETE_LOG_FILE).ToString(), this.bRightDeleteLogFile);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_TAG_MEMBER_ALARM_LEVEL).ToString(), this.bRightTagMemberAlarmLevel);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE).ToString(), this.bRightTagMemberDataSave);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_TAG_MEMBER_VIEW_RANGE).ToString(), this.bRightTagMemberViewRange);
			TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE).ToString(), this.bRightTagMemberAlarmActive);
            TotalConfig.SaveRegAutoBaseConfig("Protect", "CurrentRights", ((int)EnumUserRights.RIGHT_SCHEDULE_SETUP).ToString(), this.bRightScheduleSetup);
		}

        /*
		public void SaveFile(string username)
		{
			string work_dir = TotalConfig.sDirWorkProject;
			string dir;
			string filename;

			dir = String.Format("{0}\\Users", work_dir);

			if(!Directory.Exists(dir)) 
			{
				Directory.CreateDirectory(dir);
			}

			if(!Directory.Exists(dir))	// 폴더를 만들지 못했다.
				return;

			filename = String.Format("{0}\\Users\\{1}.user", work_dir, username);

			Stream fs = File.Open(filename, FileMode.Create);
			TextWriter writer = new StreamWriter(fs);

			//if(sPassCode.Length == 0)	sPassCode = ZipPassword(sUsername, sUsername).ToString(); // 암호가 없을때는 사용자 이름을 암호로 한다.
			if(sPassCode.Length == 0)	sPassCode = ZipPassword(sUsername, "").ToString();			// 암호가 없을때는 비운다. (한글사용자일때는 한글암호를 기본으로 하기는 곤란하다.)

			writer.WriteLine("sPassCode,"+sPassCode);
			writer.WriteLine("sDescription,"+sDescription);
			writer.WriteLine("nWebValueUpdateTime,"+nWebValueUpdateTime.ToString());
			writer.WriteLine("sStartPage,"+sStartPage);
			writer.WriteLine("bAutoOpenStartPage,"+bAutoOpenStartPage);

			writer.WriteLine("bRightProgrammEnd,{0}", bRightProgrammEnd);
			writer.WriteLine("bRightTagChange,{0}", bRightTagChange);
			writer.WriteLine("bRightConfigAlarm,{0}", bRightConfigAlarm);
			writer.WriteLine("bRightConfigEtc,{0}", bRightConfigEtc);
			writer.WriteLine("bRightTagMemberAlarmLevel,{0}", bRightTagMemberAlarmLevel);
			writer.WriteLine("bRightTagMemberDataSave,{0}", bRightTagMemberDataSave);
			writer.WriteLine("bRightAlarmConfirm,{0}", bRightAlarmEventConfirm);
            writer.WriteLine("bRightAlarmEventDelete,{0}", bRightAlarmEventDelete);

			writer.WriteLine("bRightTagMemberViewRange,{0}", bRightTagMemberViewRange);
			writer.WriteLine("bRightConfigData,{0}", bRightConfigData);
			writer.WriteLine("bRightScript,{0}", bRightScript);
			writer.WriteLine("bRightPlcScanEdit,{0}", bRightPlcScanEdit);
			writer.WriteLine("bRightTagMemberAlarmActive,{0}", bRightTagMemberAlarmActive);

			writer.WriteLine("bRightHandInput,{0}", bRightHandInput);
			writer.WriteLine("bRightDeleteAlarmFile,{0}", bRightDeleteAlarmFile);
			writer.WriteLine("bRightDeleteLogFile,{0}", bRightDeleteLogFile);

            writer.WriteLine("bRightScheduleSetup,{0}", bRightScheduleSetup);

            writer.WriteLine("bSmartPhoneUseGraphic,{0}", bSmartPhoneUseGraphic);
            writer.WriteLine("sSmartPhoneStartupPage,{0}", sSmartPhoneStartupPage);

			for(int i = 0; i < arrayTag.Count; i++) 
			{
				writer.WriteLine("Tag,"+((string)arrayTag[i]));
			}

			writer.Close();
		}*/

		//------------------------------------------------------------------------------
		//	Password를 알아볼 수 없게 부호화 한다.
        //  이전에는 uint를 return 했는데 10.3.0 부터는 문자열을 return한다. 2015-12-16
		//------------------------------------------------------------------------------

        static public string ZipPassword(string username, string password)
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                return ZipPassword512(username, password);
            }

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                return ZipPassword256(username, password);
            }

            uint zip = 0;
            int i;

            ushort hi_word = 0, lo_word = 0;

            password = password.ToUpper();

            for (i = 0; i < password.Length; i++)
            {
                hi_word = (ushort)(hi_word ^ password[i]);
                lo_word = (ushort)(lo_word + password[i]);
            }

            zip = (uint)(hi_word * 0x10000 + lo_word);

            return zip.ToString();
        }

        /*
		static public uint ZipPasswordOld(string username, string password)
		{
			uint zip = 0;
			int i;

			ushort hi_word = 0, lo_word = 0;

			password = password.ToUpper();

			for(i = 0; i < password.Length; i++)
			{
				hi_word = (ushort)(hi_word ^ password[i]);
				lo_word = (ushort)(lo_word + password[i]);
			}

			zip = (uint)(hi_word*0x10000+lo_word);

			return zip;
		}*/

        public static string ZipPassword256(string username, string password)
        {
            if (!LibrarySecurity.bOK)
            {
                return "Library not initialized.";
            }

            byte[] source_bytes = HashTool.StringToBytes(username.ToUpper() + password + "~!@#$");

            // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
            // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
            SHA256 sha256 = new SHA256Managed(); 
            byte[] result256 = sha256.ComputeHash(source_bytes);

            return Convert.ToBase64String(result256);
        }

        //------------------------------------------------------------------------------
        //	SHA512로 해시한다.
        // XP는 이 해시를 지원안해서 OEM SBAS만 일단 지원한다.
        //------------------------------------------------------------------------------

        static string ZipPassword512(string username, string password)
        {
            if (!LibrarySecurity.bOK)
            {
                return "Library not initialized.";
            }

            byte[] source_bytes = HashTool.StringToBytes(username.ToUpper()+password+"~1209hG");
            
            // SHA1보다 약3.2배 시간이 더 걸린다.
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            byte[] result512 = sha512.ComputeHash(source_bytes);

            return HashTool.ConvertHexaString(result512);
        }

		public bool IsAdmin()
		{
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                string admin_name = TotalConfig.AutoBaseIniGetOemSupervisorName();

                if (String.Compare(this.sUsername, admin_name, true) == 0) return true;
            }
            else
            {
                if (String.Compare(this.sUsername, "admin", true) == 0) return true;
            }

            return false;
		}

        public override bool IsHaveAllRights()
        {
            return IsAdmin();
        }

        public override bool IsHaveTagRight(string tag)
		{
            if (base.IsHaveTagRight(tag)) return true;

            for(int i = 0; i < GroupInfoStruct.arrayGroups.Count; i++) {
                if (GroupInfoStruct.arrayGroups[i].IsUserHaveTagRight(sUsername, tag))
                {
                    return true;        
                }
            }

			return false;
		}

		public bool HaveRightsHandOperation(string tag)
		{
    		return IsHaveTagRight(tag);
		}

        public override bool IsHaveRight(EnumUserRights id)
		{
            if (base.IsHaveRight(id)) return true;

            for (int i = 0; i < GroupInfoStruct.arrayGroups.Count; i++)
            {
                if (GroupInfoStruct.arrayGroups[i].IsUserHaveRight(sUsername, id))
                {
                    return true;
                }
            }

			return false;
		}

		void MsgNoHaveRightsOperationTag(string tag, string des)
		{
			string msg;

			if(Tools.IsLangKorean()) 
			{
				msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자로\n로그인 하시기 바랍니다.\n\n태그={0}\n설명={1}", tag, des);
				MessageBox.Show(msg, "작동 권한 없음");
			}
			else if(Tools.IsLangChinese()) 
			{
				msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}\n标记描述={1}", tag, des);
				MessageBox.Show(msg, "没有启动权限");
			}
			else 
			{
				msg = String.Format("You have not the right to control this tag.\nTag={0}\nDescription={1}", tag, des);
				MessageBox.Show(msg, "Protect Error");
			}
		}

		void MsgScreenNoHaveRightsOperationTag(string tag, string des)
		{
			string msg;

			if(Tools.IsLangKorean()) 
			{
				msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}\n설명={1}", tag, des);
				MessageDisplay.Show(msg);
			}
			else if(Tools.IsLangChinese()) 
			{
				msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}\n标记描述={1}", tag, des);
				MessageDisplay.Show(msg, "没有启动权限"); //250807 PSU MessageBox => MessageDisplay로 변경
            }
			else 
			{
				msg = String.Format("You have not the right to control this tag.\nTag={0}\nDescription={1}", tag, des);
				MessageDisplay.Show(msg);
			}
		}

		bool HaveRightsHandOperation(TagPublicClass tp)
		{
			if(IsHaveAllRights())	return true;

			if(tp.cTagLinkType == 3)	// 간접태그
			{	
				if(tp.assign != null && tp.assign.pos[0] != TagLib.TAG_NOT_FOUND)
				{	// assign tag가 있을 때
					TagPublicClass tp2 = TagLib.GetStructPublic(tp.assign.tag, ref tp.assign.pos);
					return HaveRightsHandOperation(tp2);
				}
			}		
	
			return IsHaveTagRight(tp.tag);
		}

		public bool HaveRightsHandOperationAndMsg(TagAiClass ai)
		{
			if(HaveRightsHandOperation(ai))		return true;

			MsgNoHaveRightsOperationTag(ai.tag, ai.description);

			return false;
		}

		public bool HaveRightsHandOperationAndMsg(TagAoClass ao)
		{
			if(HaveRightsHandOperation(ao))		return true;

			MsgNoHaveRightsOperationTag(ao.tag, ao.description);

			return false;
		}

		public bool HaveRightsHandOperationAndMsg(TagDiClass di)
		{
			if(HaveRightsHandOperation(di))		return true;

			MsgNoHaveRightsOperationTag(di.tag, di.description);

			return false;
		}

		public bool HaveRightsHandOperationAndMsg(TagDoClass dout)
		{
			if(HaveRightsHandOperation(dout))		return true;

			MsgNoHaveRightsOperationTag(dout.tag, dout.description);

			return false;
		}

		public bool HaveRightsHandOperationAndMsg(TagStClass st)
		{
			if(HaveRightsHandOperation(st))		return true;

			MsgNoHaveRightsOperationTag(st.tag, st.description);

			return false;
		}

		public bool HaveRightsHandOperationAndMsgAtScript(TagPublicClass tp)
		{
			if(HaveRightsHandOperation(tp))		return true;

			MsgScreenNoHaveRightsOperationTag(tp.tag, tp.description);

			return false;
		}

        public static string EncodeUserFilename(string source)
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                source = source.ToUpper();

                NetTools.Hash.HashString hash = new NetTools.Hash.HashString();

                hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);

                string target = hash.Encode(source, "UserNameGroupName");

                return target;
            }
            else
            {
                return source;
            }
        }

        public static string DecodeUserFilename(string source)
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                NetTools.Hash.HashString hash = new NetTools.Hash.HashString();

                hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);

                string target = hash.Decode(source, "UserNameGroupName");

                return target;
            }
            else
            {
                return source;
            }
        }
	}

    public class UserProtectConfig
    {
        public static int AutoLockMismatchedCount
        {
            get
            {
                ProfileHash profile = new ProfileHash(UserGroupInfoPublic.GetPathUserCommonProperty());
                return profile.GetInt("Password", "AutoLockMismatchedCount", 0);
            }
        }

        public static bool DisableShortcutOnLoginDialogBox
        {
            get
            {
                ProfileHash profile = new ProfileHash(UserGroupInfoPublic.GetPathUserCommonProperty());
                return profile.GetBool("Password", "DisableShortcutOnLoginDialogBox", false);
            }
        }

        public static bool DoNotAllowSamePassword
        {
            get
            {
                ProfileHash profile = new ProfileHash(UserGroupInfoPublic.GetPathUserCommonProperty());
                return profile.GetBool("Password", "DoNotAllowSamePassword", false);
            }
        }
    }


}
