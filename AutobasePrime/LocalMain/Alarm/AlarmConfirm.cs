using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using GraphicModule;
using System.Threading;

namespace LocalMain.Alarm
{
	/// <summary>
	/// Summary description for AlarmConfirm.
	/// </summary>
	public class AlarmConfirm
	{
		static AlarmConfirm()  
		{ 
			//
			// TODO: Add constructor logic here
			//
			FormAlarmEvent.procConfirmAlarmSound = new GraphicModule.FormAlarmEvent.DelegateConfirmAlarmSound(ConfirmAlarmSound);
            FormAlarmEvent.procSaveAlarmConfirmList = new FormAlarmEvent.DelegateSaveAlarmConfirmList(SaveAlarmConfirmList);
		}

		public static void AlarmContinueRegister(TagAiClass ai, ALARM_FILE_STRUCT alarm, bool flag)
		{
			if((ConfigAlarm.dwAlarmFilterEvent & Tools.DWORD_MASK[alarm.alarm_type]) == 0)	return;

			sbyte method = AlarmPriority.AlarmPriorityGetOptionScreen(alarm.priority);

			if(method == 0 || method == 1)
			{ 	// 화면에 안 뿌리거나/한번만 뿌린다.
				return;
			}

			ai.bCurrentAlarmStatus = flag;		// 경보 상태를 등록하거나 삭제한다.

			ExecuteAlarmComfirmation(alarm, ai.tag, ai.sAlarmWaveFile, flag, method);
		}

		public static void AlarmContinueRegister(TagDiClass di, ALARM_FILE_STRUCT alarm, bool flag)
		{
			if((ConfigAlarm.dwAlarmFilterEvent & Tools.DWORD_MASK[alarm.alarm_type]) == 0)	return;

			sbyte method = AlarmPriority.AlarmPriorityGetOptionScreen(alarm.priority);

			if(method == 0 || method == 1) 
			{ 	// 화면에 안 뿌리거나/한번만 뿌린다.
				return;
			}

			di.bCurrentAlarmStatus = flag;	// 경보 상태를 등록하거나 삭제한다.

			ExecuteAlarmComfirmation(alarm, di.tag, di.sAlarmWaveFile, flag, method);
		}

		static void ExecuteAlarmComfirmation(ALARM_FILE_STRUCT alarm, string tag_name, string wave_file, bool flag, sbyte method)
		{
			if(flag == true) 
			{
				InsertOneAlarmConfirmation(alarm, tag_name, wave_file, method);
				if(ConfigAlarm.bAlarmAutoMakeConfirmBox) 
				{
					FormPopupAlarmConfirmation.CreatePopupAlarmConfirmation(true);
				}
			}
			else 
			{
				DeleteOneAlarmConfirmation(alarm.tag, alarm.t);
			}
		}

		//------------------------------------------------------------------------------
		// 하나의 계속 경보 태그를 등록한다.
		//------------------------------------------------------------------------------

		static int InsertOneAlarmConfirmation(ALARM_FILE_STRUCT alarm, string tag_name, string wave_file, sbyte screen_method)
		{
			ALARM_CONFIRMATION_STRUCT block = new ALARM_CONFIRMATION_STRUCT();

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {
                if (FormAlarmEvent.blockAlarmConfirmNot.Count >= 1000)
                {	// 확인만 하고 삭제를 하지 않으면 너무 많은 것이 쌓일 수 있다.
                    FormAlarmEvent.blockAlarmConfirmNot.RemoveAt(0);
                }
            }

			block.tag = alarm.tag;
			block.description = alarm.description;
			block.message = alarm.msg;

			//block.tag_type = tag_type;
			//block.tag_pos  = tag_pos;
			SYSTEMTIME.memcpy(block.t, alarm.t);
			block.sAlarmWaveFile = wave_file;
			block.screen_method = screen_method;
			block.bAlarm = 1;
            block.bAlarmTag = 1;
			block.msg_type = alarm.alarm_type;
			block.priority = alarm.priority;
			block.port = alarm.port%500;

            block.id = FormAlarmEvent.MakeUniqueID();

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {
                FormAlarmEvent.blockAlarmConfirmNot.Add(block);
            }

			FormAlarmEvent.AlarmConfirmCountChanged();

			return 1;
		}

		

		//------------------------------------------------------------------------------
		//	하나의 계속 경보 태그를 삭제한다.
		//------------------------------------------------------------------------------

		static void DeleteOneAlarmConfirmation(string tag, SYSTEMTIME tReturn)
		{
			ALARM_CONFIRMATION_STRUCT block;

			int l;

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {
                for (l = 0; l < FormAlarmEvent.blockAlarmConfirmNot.Count; l++)
                {
                    block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[l];
                    if (block.tag == tag)
                    {	
                        block.bAlarmTag = 0;

                        if (block.bAlarm == 1)
                        {
                            if (block.screen_method != 3)	// 사용자 확인 때까지 계속 남아 있음
                                FormAlarmEvent.blockAlarmConfirmNot.RemoveAt(l);
                            else
                            {
                                block.bAlarm = 0;
                                SYSTEMTIME.memcpy(block.tReturn, tReturn);
                            }
                        }

                        //FormAlarmEvent.AlarmConfirmCountChanged();

                        //return;
                    }
                }
                //for문 밖에서 한번만 실행. 20241111 PSU 
                //ViewMain 의 DeleteOneAlarmConfirmatio. >> ViewMain은 호출하지 않고, 리스트내용만 서버와 교환. 수정필요없음.
                FormAlarmEvent.AlarmConfirmCountChanged(); 
            }
		}

		public static void LoadAlarmConfirmList()
		{
			string filename;
			string cfg_dir;

            cfg_dir = Application.UserAppDataPath;

			filename = String.Format("{0}\\AlarmConfirmList.lst", cfg_dir);

			if(!File.Exists(filename))	return;

			Stream s = File.OpenRead(filename);
			IFormatter format = new BinaryFormatter();
	
			try 
			{
                lock (FormAlarmEvent.blockAlarmConfirmNot)
                {
                    FormAlarmEvent.blockAlarmConfirmNot = (ArrayList)format.Deserialize(s);
                }
			}
			catch(Exception exception) 
			{
				ErrorMsg.Show(exception, filename, "Can't load the file");
			}
	
			s.Close();
		}

		public static void SaveAlarmConfirmList()
		{
            if (TotalConfig.threadMain != Thread.CurrentThread) return; // 메인 스레드에서만 저장한다.

			string filename;
			string cfg_dir;

			cfg_dir = Application.UserAppDataPath;

            filename = String.Format("{0}\\AlarmConfirmList.lst", cfg_dir);

			Stream s = File.OpenWrite(filename);
			IFormatter format = new BinaryFormatter();

			try 
			{
                lock (FormAlarmEvent.blockAlarmConfirmNot)
                {
                    format.Serialize(s, FormAlarmEvent.blockAlarmConfirmNot);
                }
			}
			catch(Exception exception) 
			{
				ErrorMsg.Show(exception, filename, "Can't save the file");
			}

			s.Close();

		}

		static int currAlarmWavePosition = 0;

		public static void AlarmConfirmationStatus()
		{
            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {
                if (FormAlarmEvent.blockAlarmConfirmNot.Count == 0) return;

                ALARM_CONFIRMATION_STRUCT block;

                for (int l = 0; l < FormAlarmEvent.blockAlarmConfirmNot.Count; l++)
                {
                    currAlarmWavePosition++;
                    currAlarmWavePosition %= FormAlarmEvent.blockAlarmConfirmNot.Count;

                    block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[currAlarmWavePosition];

                    if (block.bAlarm == 1)	// 현재 알람이 진행 중일때만 경보 소리를 울린다.
                    {
                        int option = AlarmPriority.AlarmPriorityGetOptionSound(block.priority);

                        if ((option == 2 || option == 3) && block.bConfirmedAlarmSound == false)
                        {
                            AlarmSound.Play(block.sAlarmWaveFile, block.priority);
                            return;	// 소리를 울렸으면 돌아간다.
                        }
                    }
                }
            }
		}

		public static void ConfirmAlarmSound()
		{
			ALARM_CONFIRMATION_STRUCT block;

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {
                for (int i = 0; i < FormAlarmEvent.blockAlarmConfirmNot.Count; i++)
                {
                    block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[i];

                    block.bConfirmedAlarmSound = true;
                }
            }
		}


		
	}
}

