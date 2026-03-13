using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.Security.Cryptography;

namespace AutoLibLocal
{
    // 해킹하는 사람들이 해시인지 모르도록 Seed, Pass 같은 용어를 사용하지 않도록 한다.
    // GetGuid, SetGuid만을 사용하지만 여러 함수들을 몇개 더 넣어서 헷갈리도록 한다.
    public class _LibraryInit
    {
        static string sSeed = Guid.NewGuid().ToString();

        public static string GetGuid()
        {
            return sSeed;
        }

        public static bool SetGuid(byte[] guid)
        {
            bool retn = HashTool.CompareHash(sSeed + "AutoLibLocal.dll", guid);

            LibrarySecurity.bOK = retn;

            return retn;
        }

        //-------------------------------------------
        // 아래에 있는 코드는 해커가 더버깅 시 헷갈리게 만들 코드이다.
        //-------------------------------------------

        public static bool Init(string initial_code)
        {
            initial_code += "!2DDD334f9g756D3g$0__=-+=";
            byte[] s = HashTool.StringToBytes(initial_code);
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result1 = sha.ComputeHash(s);

            return result1[0] == 1;
        }

        public static bool Prepare(string pass_code)
        {
            pass_code += "!2D@DD33M4f9g7j:..<3g$?0__=-+=";
            byte[] s = HashTool.StringToBytes(pass_code);
            // SHA1보다 약3.2배 더 걸린다.
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            byte[] result512 = sha512.ComputeHash(s);

            return result512[0] == 1;
        }

        public static void Uninit()
        {

        }

        public static void UnPrepare()
        {

        }
    }

    // 이 클래스는 public으로 하면 안된다. 밖에서 알수 없도록 숨긴다.
    class LibrarySecurity
    {
        public static bool bOK = false;
    }
}
