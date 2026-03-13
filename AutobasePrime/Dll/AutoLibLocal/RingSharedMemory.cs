using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AutoLibLocal
{
    public class RingSharedMemory
    {
        public delegate IntPtr DelegateRingCreate(string name, int ring_count, int ring_size);
        public delegate bool DelegateRingGetItem(IntPtr handle, byte[] buf, int size);
        public delegate int DelegateRingAddItem(IntPtr handle, byte[] buf, int size);
        public delegate void DelegateRingClose(IntPtr handle);

        public static DelegateRingCreate RingCreate = null;
        public static DelegateRingGetItem RingGetItem = null;
        public static DelegateRingAddItem RingAddItem = null;
        public static DelegateRingClose RingClose = null;

        static RingSharedMemory()
		{
			//
			// TODO: Add constructor logic here
			//
            if (IntPtr.Size == 8)
            {
                RingCreate = RingSharedMemory64.RingCreate;
                RingGetItem = RingSharedMemory64.RingGetItem;
                RingAddItem = RingSharedMemory64.RingAddItem;
                RingClose = RingSharedMemory64.RingClose;
            }
            else
            {
                RingCreate = RingSharedMemory32.RingCreate;
                RingGetItem = RingSharedMemory32.RingGetItem;
                RingAddItem = RingSharedMemory32.RingAddItem;
                RingClose = RingSharedMemory32.RingClose;
            }
		}

        /*
        public static bool OpenAndAddItem(string name, object source)
        {
            MemoryStream s = new MemoryStream();

            BinaryFormatter fomat = new BinaryFormatter();
            fomat.Serialize(s, source);
            s.Seek(0, SeekOrigin.Begin);
            byte[] b = s.ToArray();

            int retn = RingOpenAndAddItem(name, b, b.Length);

            if (retn < 0)
            {
                if (retn == -1)
                {
                    MessageDisplay.Show("[{0}] AddItem Timeouted.", name);
                }
                else if (retn == -2)
                {
                    string msg = String.Format("[{0}] AddItem block size too small.", name);
                    MessageDisplay.Show(msg);
                    System.Windows.Forms.MessageBox.Show(msg, "Error");
                }
                else
                {
                    MessageDisplay.Show("[{0}] AddItem error. [errorcode = {2}]", name, retn);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
        */

        IntPtr pHandle = IntPtr.Zero;
        int nRingSize = 0;
        string sName;

        public void Create(string name, int ring_count, int ring_size)
        {
            pHandle = RingCreate(name, ring_count, ring_size);
            nRingSize = ring_size;
            sName = name;
        }

        public void Close()
        {
            RingClose(pHandle);
        }

        public object GetItem()
        {
            byte[] buf = new byte[nRingSize];

            if (RingGetItem(pHandle, buf, nRingSize) == false)
            {
                return null;
            }

            MemoryStream s = new MemoryStream();
            BinaryFormatter fomat = new BinaryFormatter();
            s.Write(buf, 0, nRingSize);
            s.Seek(0, SeekOrigin.Begin);
            object obj = fomat.Deserialize(s);

            return obj;
        }

        public bool AddItem(object source)
        {
            MemoryStream s = new MemoryStream();

            BinaryFormatter fomat = new BinaryFormatter();
            fomat.Serialize(s, source);
            s.Seek(0, SeekOrigin.Begin);
            byte[] b = s.ToArray();

            if (b.Length > nRingSize)
            {
                string msg = String.Format("[{0}] AddItem block size too small. b.Length={1}, nRingSize={2}", sName, b.Length, nRingSize);
                MessageDisplay.Show(msg);
                return false;

            }

            int retn = RingAddItem(pHandle, b, b.Length);

            if (retn < 0)
            {
                if (retn == -1)
                {
                    MessageDisplay.Show("[{0}] AddItem Timeouted.", sName);
                }
                else if (retn == -2)
                {
                    string msg = String.Format("[{0}] AddItem block size too small.", sName);
                    MessageDisplay.Show(msg);
                    //System.Windows.Forms.MessageBox.Show(msg, "Error");
                }
                else
                {
                    MessageDisplay.Show("[{0}] AddItem error. [errorcode = {2}]", sName, retn);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
