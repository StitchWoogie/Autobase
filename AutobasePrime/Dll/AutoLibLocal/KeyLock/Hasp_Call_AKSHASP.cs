/*
using System;
using System.Collections.Generic;
using System.Text;
using Aladdin.Hasp;

namespace AutoLibLocal.KeyLock
{
    class Hasp_Call_AKSHASP
    {
        int seed = 300, lptnum = 0;
        int passw1, passw2;

        private bool CheckHasp()
        {
            passw1 = 31551;
            passw2 = 16294;

            int result = 0;
            int status = 0;

            object param1 = (object)result;
            object param3 = (object)status;

            HaspKey.Hasp(HaspService.IsHasp, seed, lptnum, passw1, passw2, param1, null, param3, null);

            result = (int)param1;
            status = (int)param3;

            if (status != 0)
            {
                //string ex = "Hasp Error: " + status;
                return false;
            }

            if (result == 0)
            {
                //string ex = "Hasp No found";
                return false;
            }
            else return true;
        }

        public bool CheckLockHaspLock()
        {
            if (CheckHasp() ) // && IsMemoHasp() ) /* Some Hasp is connected 
            {
                HaspService service = HaspService.ReadBlock;

                object p1, p2, p3, p4;

                p1 = (int)0;
                p2 = (int)24;
                p3 = (int)0;
                p4 = (int)0;

                byte[] buffer = new Byte[(int)p2 * 2];
                try
                {
                    HaspKey.Hasp(service, seed, lptnum, passw1, passw2, p1, p2, p3, buffer);
                }
                catch// (Exception someerror)
                {
                    //MessageBox.Show(someerror.Message, "Hasp Call",
                    //	MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if ((int)p3 == 0)
                {
                    //this.textOperationResult.Text = "Read Block OK.";	
                    //this.textMemoryContents.Text = System.Text.Encoding.ASCII.GetString(buffer);		
                }
                else { }// textOperationResult.Text = "Read Block failed: " + Hasp.GetErrorDescription(p3, 0);
                // Set p1-3 to the result value 

                service = HaspService.HaspID;
                HaspKey.Hasp(service, seed, lptnum, passw1, passw2,
                    p1, p2, p3, p4);

                int dwSerialNumber = 0;

                if ((int)p3 != 0)
                {
                    KeyLock.sSerialNumber = "????????";
                }
                else
                {
                    dwSerialNumber = (((int)p2) << 16) + (int)p1;
                    KeyLock.sSerialNumber = String.Format("{0:X08}", dwSerialNumber);
                }

                KeyLock.KeyLockDecodeData(ref buffer, 48, (uint)dwSerialNumber);

                if (buffer[0] == 'A' &&
                    buffer[1] == 'u' &&
                    buffer[2] == 'T' &&
                    buffer[3] == 'o' &&
                    buffer[4] == 'B' &&
                    buffer[5] == 'a' &&
                    buffer[6] == 'S' &&
                    buffer[7] == 'e')
                {
                    KeyLock.bExistLocalKey = true;
                }

                KeyLock.wTagSize = (ushort)(buffer[16] * 32);
                KeyLock.nKeyLockVersion = buffer[17];
                KeyLock.nKeyLockRunOrDev = buffer[18];
                KeyLock.nKeyLockOem = buffer[19];

                if (buffer[24] == 'A' && buffer[25] == 'W')
                {
                    KeyLock.bExistWebKey = true;
                }

                KeyLock.nWebUserCount = buffer[26] * 256 + buffer[27];

                KeyLock.cLockType = LOCK_TYPE.HASP_LOCK;
                //cLockType = cLockType;	// unused warning을 방지.

                return true;
            }

            return false;
        }
    }


}

*/