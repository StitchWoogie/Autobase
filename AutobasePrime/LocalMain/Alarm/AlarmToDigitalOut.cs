using System;
using AutoLibLocal;
using AutoLib;
using System.Collections;
using System.Threading.Tasks;

namespace LocalMain
{
	public class ALARM_TO_DIGITAL 
	{
		public string		out_tag;
		public int			tag_type;
		public int[]		tag_pos;
		public ArrayList	item;		// list 는 약 4000개 까지 등록 가능하다.
	}

	public class ITEM_STRUCT
	{
		public string	tag;
		public int		tag_type;
		public int[]	tag_pos;
		public sbyte	bOldStatus;
	}

	/// <summary>
	/// Summary description for AlarmToDigitalOut.
	/// </summary>
	public class AlarmToDigitalOut
	{
		public AlarmToDigitalOut()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static ArrayList blockList = new ArrayList();

		public static void CheckAlarmToDigitalOut(EnumAlarmType alarm_type)
		{
			if(ConfigAlarm.sAlarmDigitalOut.Length == 0)		return;

			if(	alarm_type == EnumAlarmType.HIHI ||
				alarm_type == EnumAlarmType.HIGH ||
				alarm_type == EnumAlarmType.LOW ||
				alarm_type == EnumAlarmType.LOLO ||
				alarm_type == EnumAlarmType.DI_ON ||
				alarm_type == EnumAlarmType.DI_OFF) 
			{
			}
			else 
			{
				return;
			}

			int[] pos = null;

			TagDoClass dout = TagLib.GetStructDO(ConfigAlarm.sAlarmDigitalOut, ref pos);
            			
			PlcScan.CommWriteDigitalOutOnOff(dout, 1, false);
		}

		//static int list_pos = 0;

		public static void CheckAlarmStatusToDigitalOutList()
		{
			/*
			if(blockList.Count == 0)	return;

			list_pos++;
			list_pos %= blockList.Count;
	
			int l;
			ALARM_TO_DIGITAL list;
			ITEM_STRUCT item;
			sbyte change_flag = 0;
			sbyte out_value = 0;	// out value는 알람상태중 하나라도 살아있으면 ON이다.

			list = (ALARM_TO_DIGITAL)blockList[list_pos];

			for(l = 0; l < list.item.Count; l++) 
			{
				item = (ITEM_STRUCT)list.item[l];
				if(item.tag_type == 0) 
				{
					TAG_AI_STRUCT *ai = &terminalStruct[0].analogInput[item.tag_pos];
					if(ai->bNeedAlarmConfirm)	out_value = ON;
					if(ai->bNeedAlarmConfirm != item.bOldStatus) 
					{
						change_flag = ON;
						item.bOldStatus = ai->bNeedAlarmConfirm;
						list.item->SetBlock(&item, l);
					}
				}
				else if(item.tag_type == 2) 
				{
					TAG_DI_STRUCT *di = &terminalStruct[0].digitalInput[item.tag_pos];
					if(di->bNeedAlarmConfirm)	out_value = ON;
					if(di->bNeedAlarmConfirm != item.bOldStatus) 
					{
						change_flag = ON;
						item.bOldStatus = di->bNeedAlarmConfirm;
						list.item->SetBlock(&item, l);
					}
				}
			}

			if(change_flag) 
			{
				if(list.tag_type == 3) 
				{	// DO tag
					CommWriteDigitalOutOnOff(0, list.tag_pos, out_value);
				}
			}
			*/
		}
	}
}

/*
void LoadAlarmStatusToDigitalOutList()
{
	FILE *in;
	CommaBlockString comma;
	StackChar buf(50000);
	char filename[MAXPATH];
	ALARM_TO_DIGITAL list;
	ITEM_STRUCT item;
	
	sprintf(filename, "{0}\\alarm\\AlarmOut.lst", sDirWorkProject);

	in = fopen(filename, "rb");
	if(in == NULL)	return;
	while(1) {
		if(!TextGetOneLine(in, buf.data, 50000))	break;
		if(buf.data[0] == 0)						continue;

		memset(&list, 0, sizeof(ALARM_TO_DIGITAL));
		comma.Set(buf.data);
		comma.GetString(list.out_tag, sizeof(list.out_tag));
		GetTagTypeAndPos(0, list.out_tag, list.tag_type, list.tag_pos);
		list.item = new Block(sizeof(ITEM_STRUCT));
		while(1) {
			memset(&item, 0, sizeof(ITEM_STRUCT));
			comma.GetString(item.tag, sizeof(item.tag));
			if(item.tag[0] == 0)	break;
			GetTagTypeAndPos(0, item.tag, item.tag_type, item.tag_pos);
			list.item->AddBlock(&item);
		}
		blockList.AddBlock(&list);
	}
	fclose(in);
}

void CheckAlarmStatusToDigitalOutList()
{
	if(blockList.Count == 0)	return;

	static DWORD list_pos = 0;
	
	list_pos++;
	list_pos %= blockList.Count;
	
	DWORD l;
	ALARM_TO_DIGITAL list;
	ITEM_STRUCT item;
	char change_flag = OFF;
	char out_value = OFF;	// out value는 알람상태중 하나라도 살아있으면 ON이다.

	blockList.GetBlock(&list, list_pos);

	for(l = 0; l < list.item.Count; l++) {
		list.item->GetBlock(&item, l);
		if(item.tag_type == 0) {
			TAG_AI_STRUCT *ai = &terminalStruct[0].analogInput[item.tag_pos];
			if(ai->bNeedAlarmConfirm)	out_value = ON;
			if(ai->bNeedAlarmConfirm != item.bOldStatus) {
				change_flag = ON;
				item.bOldStatus = ai->bNeedAlarmConfirm;
				list.item->SetBlock(&item, l);
			}
		}
		else if(item.tag_type == 2) {
			TAG_DI_STRUCT *di = &terminalStruct[0].digitalInput[item.tag_pos];
			if(di->bNeedAlarmConfirm)	out_value = ON;
			if(di->bNeedAlarmConfirm != item.bOldStatus) {
				change_flag = ON;
				item.bOldStatus = di->bNeedAlarmConfirm;
				list.item->SetBlock(&item, l);
			}
		}
	}

	if(change_flag) {
		if(list.tag_type == 3) {	// DO tag
			CommWriteDigitalOutOnOff(0, list.tag_pos, out_value);
		}
	}
}



*/