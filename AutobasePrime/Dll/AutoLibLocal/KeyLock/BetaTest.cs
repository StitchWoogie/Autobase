using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLibLocal.KeyLock
{
    public class BetaTest
    {
        public static bool IsNextVersionBetaTester()
        {
            KeyLock keylock = new KeyLock();
            bool bKeyLock = keylock.CheckKeyLockLocal();

            if (KeyLock.nKeyLockVersion < 13)
            {
                System.Windows.Forms.MessageBox.Show("오토베이스 차세대 버전 테스트용 키락이 있어야 합니다. Need Version =13");
                return false;
            }

            return true;
        }
    }
}
