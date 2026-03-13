using System;

namespace ExportLib
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ClassExportLib2
	{
		public ClassExportLib2()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public virtual string ProtocolGetName()
		{
			return "Protocol name not defined"; 
		}

		public virtual EnumCodeMode ProtocolGetCodeMode()
		{
			return EnumCodeMode.HEX;	// Hex Mode
		}

		public virtual void ProtocolInit(string option)
		{

		}

		public virtual void ProtocolUnInit()
		{

		}

		public virtual void ProtocolRecvBytes(byte[] buf, int size)
		{
		
		}

		public virtual int ProtocolOption(ref string option)
		{
			return 0;
		}

		public delegate void DelegateSendBytes(byte[] buf, int size);
		DelegateSendBytes procSendBytes = null;

		public void SetFunctionSend(DelegateSendBytes obj)
		{
			procSendBytes = obj;
		}

		public void SendBytes(byte[] buf, int size)
		{
			if(procSendBytes == null)	return;

			procSendBytes(buf, size);
		}

		public delegate object DelegateGetTagValue(int address);
		DelegateGetTagValue procGetTagValue = null;

		public void SetFunctionGetTagValue(DelegateGetTagValue obj)
		{
			procGetTagValue = obj;
		}

		protected object GetTagValue(int address)
		{
			if(procGetTagValue == null)	return 0;

			return procGetTagValue(address);
		}

		public double GetTagValueDouble(int address)
		{
			object obj = GetTagValue(address);

			if(obj.GetType() == typeof(double))
				return (double)obj;
			else if(obj.GetType() == typeof(float)) 
				return (float)obj;
			else if(obj.GetType() == typeof(int)) 
				return (int)obj;
			else if(obj.GetType() == typeof(long)) 
				return (long)obj;
			else if(obj.GetType() == typeof(string)) 
				return ConvertTool.ToDouble((string)obj);
			else
				return 0;
		}

		public delegate bool DelegateSetTagValue(int address, object val);
		DelegateSetTagValue procSetTagValue = null;

		public void SetFunctionSetTagValue(DelegateSetTagValue obj)
		{
			procSetTagValue = obj;
		}

		public bool SetTagValue(int address, object val)
		{
			if(procSetTagValue == null)	return false;

			return procSetTagValue(address, val);
		}


		public delegate object DelegateGetTagValueByName(string tag);
		DelegateGetTagValueByName procGetTagValueByName = null;

		public void SetFunctionGetTagValueByName(DelegateGetTagValueByName obj)
		{
			procGetTagValueByName = obj;
		}

		public object GetTagValueByName(string tag)
		{
			if(procGetTagValueByName == null)	return 0;

			return procGetTagValueByName(tag);
		}

		public delegate bool DelegateSetTagValueByName(string tag, object val);
		DelegateSetTagValueByName procSetTagValueByName = null;

		public void SetFunctionSetTagValueByName(DelegateSetTagValueByName obj)
		{
			procSetTagValueByName = obj;
		}

		public bool SetTagValueByName(string tag, object val)
		{
			if(procSetTagValueByName == null)	return false;

			return procSetTagValueByName(tag, val);
		}

	}

	public enum EnumCodeMode
	{
		ASCII,
		HEX,
		DECIMAL,
	}
}
