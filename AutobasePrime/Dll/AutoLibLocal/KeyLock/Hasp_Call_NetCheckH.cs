using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AutoLibLocal.KeyLock
{
    class Hasp_Call_NetCheckH
    {
        [DllImport("_NetCheckH.DLL", EntryPoint = "GetBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBuffer32(uint seed, [In, Out] byte[] array);

        [DllImport("_NetCheckH_x64.DLL", EntryPoint = "GetBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBuffer64(uint seed, [In, Out] byte[] array);

        public bool CheckLockHaspLock()
        {
            byte[] buffer = new Byte[200];
            int type = 0;
            DateTime t = DateTime.Now;
            uint seed = (uint)(t.Millisecond + t.Second * 1000 + t.Minute * 1000 * 60);

            try
            {
                if(IntPtr.Size == 8)
                    type = GetBuffer64(seed, buffer);
                else 
                    type = GetBuffer32(seed, buffer);
            }
            catch// (Exception someerror)
            {
                // MessageBox.Show(someerror.Message, "Hasp Call",
                // MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (type == 0) return false;

            uint dwSerialNumber;

            dwSerialNumber = (uint)((buffer[99] << 24) + (buffer[98] << 16) + (buffer[97] << 8) + buffer[96]);
            KeyLock.sSerialNumber = String.Format("{0:X08}", dwSerialNumber);

            KeyLock.KeyLockDecodeData(ref buffer, 96, seed);

            if (    buffer[1] == 'u' &&
                    buffer[2] == 'T' &&
                    buffer[3] == 'o' &&
                    buffer[4] == 'B' &&
                    buffer[5] == 'a' &&
                    buffer[6] == 'S' &&
                    buffer[7] == 'e')
            {

                if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
                {
                    if (buffer[0] == 'B')
                        KeyLock.bExistLocalKey = true;
                }
                else
                {
                    if (buffer[0] == 'A')
                        KeyLock.bExistLocalKey = true;
                }
            }

            KeyLock.nTagSize = (buffer[16] * 32);
            KeyLock.nKeyLockVersion = buffer[17];
            KeyLock.nKeyLockRunOrDev = buffer[18];
            KeyLock.nKeyLockOem = buffer[19];

            if (KeyLock.nKeyLockVersion >= 10)  // 2010년 이상된 키만 32태그 이하 적용 2010-4-29 10.1.1 부터 32태그 이하 지원
            {
                if (KeyLock.nTagSize == 32)
                {
                    if (buffer[20] > 0)
                    {
                        KeyLock.nTagSize = buffer[20] * buffer[16];
                    }
                }
            }

            if (buffer[24] == 'A' && buffer[25] == 'W')
            {
                KeyLock.bExistWebKey = true;
            }

            KeyLock.nWebUserCount = buffer[26] * 256 + buffer[27];

            KeyLock.cLockType = LOCK_TYPE.HASP_LOCK;

            return true;
        }
    }
}
