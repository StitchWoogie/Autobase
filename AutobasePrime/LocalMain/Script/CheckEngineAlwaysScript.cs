using System;
using System.Collections;
using GraphicModule;
using AutoLib;
using NetTools;
using System.IO;
using AutoLibLocal;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain
{
    #region thread 방식
    //   class PCL_STRUCT
    //{
    //	public string filename;		    // filename
    //	public bool	  flag;			    // programm active ?
    //	public ScriptClass pcl;		    // control class point
    //	public DateTime tOld;
    //	public int   currSec;		    // 몇초가 흘렀는지를 검사한다.

    //       public Thread threadScript = null;

    //       public bool bInitialized = false;   // 초기화 되었다.

    //       public void Init()
    //       {
    //           if (!pcl.bUseThread) return;

    //           bEnd = false;
    //           threadScript = new Thread(new ThreadStart(ThreadLoopWriteCheck));
    //           threadScript.Start();

    //           bInitialized = true;
    //       }

    //       public void UnInit()
    //       {
    //           bEnd = true;

    //           if (threadScript != null)   // 초기 로그인에서 실패시 걸림
    //           {
    //               threadScript.Join(5000);
    //           }
    //       }

    //       bool bEnd = false;

    //      void ThreadLoopWriteCheck()
    //       {
    //           DateTime t;

    //           while (!bEnd)
    //           {
    //               Thread.Sleep(1);

    //               if (!flag) continue;

    //               if (ConfigRunMain.bRunScriptIfError || !pcl.IsError())
    //               {
    //                   pcl.ClearError();
    //                   t = DateTime.Now;

    //                   if (t.Second != tOld.Second)
    //                   {
    //                       tOld = t;
    //                       currSec++;
    //                   }

    //                   if (currSec >= pcl.GetScanTime())
    //                   {
    //                       pcl.Run(FormLocalMain.formMain, null);
    //                       /*
    //                       if (pcl.IsError())
    //                       {
    //                           string message;
    //                           message = pcl.pcl.GetError();
    //                           MessageDisplay.Show("Script Error: {0}\n{1}", pcl.filename, message);
    //                       }*/
    //                       currSec = 0;
    //                   }
    //               }
    //           }
    //       }
    //}
    #endregion

    class PCL_STRUCT
    {
        public string filename;
        public bool flag;
        public ScriptClass pcl;
        public DateTime tOld;
        public int currSec;
        private Task taskScript = null;
        private CancellationTokenSource cancellationTokenSource = null;
        public bool bInitialized = false;

        public void Init()
        {
            if (!pcl.bUseThread) return;

            cancellationTokenSource = new CancellationTokenSource();
            taskScript = Task.Run(() => ThreadLoopWriteCheck(cancellationTokenSource.Token));
            bInitialized = true;
        }

        public void UnInit()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();

                if (taskScript != null)
                {
                    try
                    {
                        taskScript.Wait(5000);
                    }
                    catch (AggregateException)
                    {
                        // 취소로 인한 예외는 무시
                    }
                }

                cancellationTokenSource.Dispose();
            }
        }

        async Task ThreadLoopWriteCheck(CancellationToken cancellationToken)
        {
            DateTime t;
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1, cancellationToken);

                if (!flag) continue;

                if (ConfigRunMain.bRunScriptIfError || !pcl.IsError())
                {
                    pcl.ClearError();
                    t = DateTime.Now;
                    if (t.Second != tOld.Second)
                    {
                        tOld = t;
                        currSec++;
                    }
                    if (currSec >= pcl.GetScanTime())
                    {
                        await pcl.RunAsync(FormLocalMain.formMain, null);
                        currSec = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Summary description for CheckEngineAlwaysScript.
    /// </summary>
    public class CheckEngineAlwaysScript
	{
		public static ArrayList blockPCL = new ArrayList();

		static CheckEngineAlwaysScript()
		{
			//
			// TODO: Add constructor logic here
			//

			PclProgrammStart();
		}

        public static object ProcScriptCallBack(string method_name, object[] args)
        {
            if (method_name == "ScriptGetActiveAll")
            {
                return CheckEngineAlwaysScript.bActiveScript;
            }
            else if (method_name == "ScriptGetActiveFile")
            {
                PCL_STRUCT pcl;
                for (int i = 0; i < blockPCL.Count; i++)
                {
                    pcl = (PCL_STRUCT)blockPCL[i];
                    if (String.Compare(pcl.filename, (string)args[0], true) == 0)
                    {
                        return pcl.flag ? 1 : 0;
                    }
                }

                return 0;
            }
            else if (method_name == "ScriptSetActiveAll")
            {
                CheckEngineAlwaysScript.bActiveScript = ConvertTool.ToBoolean(args[0]);
                FormAlwaysScript.SetTitle();

                return 1;
            }
            else if (method_name == "ScriptSetActiveFile")
            {
                PCL_STRUCT pcl;
                for (int i = 0; i < blockPCL.Count; i++)
                {
                    pcl = (PCL_STRUCT)blockPCL[i];
                    if (String.Compare(pcl.filename, (string)args[0], true) == 0)
                    {
                        pcl.flag = ((int)args[1] == 1);
                        return 1;
                    }
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static bool bActiveScript = true;

		public static async Task PclProgrammStatus(System.Windows.Forms.Form form)
		{
            if (bActiveScript == false) return;
			if(blockPCL.Count == 0)	return;	// 검사할 필요가 없다.

			int l;

			for(l = 0; l < 5 && l < blockPCL.Count; l++) 
			{
				await PclProgrammStatusOne(form).ConfigureAwait(false);
			}
		}

		static int lPosAlwaysScriptStatus;

		static async Task PclProgrammStatusOne(System.Windows.Forms.Form form)
		{
			if(blockPCL.Count == 0)	return;	// 검사할 필요가 없다.

			lPosAlwaysScriptStatus ++;
			if(lPosAlwaysScriptStatus >= blockPCL.Count) 
			{
				lPosAlwaysScriptStatus = 0;
			}

			DateTime t = DateTime.Now;

			PCL_STRUCT pcl;

			pcl = (PCL_STRUCT)blockPCL[lPosAlwaysScriptStatus];

            // 스레드이면 한번만 초기화 하면 스레드내에서 RUN이 된다.
            if (pcl.flag == true && pcl.pcl.bUseThread)
            {
                if (!pcl.bInitialized)
                {
                    pcl.Init();
                }

                /*
                if (pcl.pcl.IsError())
                {
                    string message;
                    message = pcl.pcl.GetError();
                    MessageDisplay.Show("Script Error: {0}\n{1}", pcl.filename, message);
                }*/
            }
			else if(pcl.flag == true && pcl.pcl != null) 
			{
				if(ConfigRunMain.bRunScriptIfError || !pcl.pcl.IsError()) 
				{
					pcl.pcl.ClearError();
					if(t.Second != pcl.tOld.Second) 
					{
						pcl.tOld = t;
						pcl.currSec++;
					}

					if(pcl.currSec >= pcl.pcl.GetScanTime()) 
					{
                        DebugSpeed elapsed = new DebugSpeed();
                        elapsed.Start();
						await pcl.pcl.RunAsync(form, null).ConfigureAwait(false);
                        elapsed.Stop(pcl.filename);

						if(pcl.pcl.IsError()) 
						{
							string message;
							message = pcl.pcl.GetError();
							MessageDisplay.Show("Script Error: {0}\n{1}", pcl.filename, message);
						}
						pcl.currSec = 0;
					}
				}
			}
		}

		static void PclProgrammStart()
		{
			string filename;
			PCL_STRUCT pcl;
			int count = 0;

			filename = String.Format("{0}\\control\\always", TotalConfig.sDirWorkProject);

			if(!Directory.Exists(filename))	return;

			DirectoryInfo info = new DirectoryInfo(filename);

			foreach(FileInfo fi in info.GetFiles("*.ctl?")) 
			{
				pcl = new PCL_STRUCT();
				pcl.pcl = new ScriptClass();
				LoadOneControl(fi.Name, pcl);
				//pcl.pcl.idControl = count;		// class에 유일한 고유 id를 부여한다.
				blockPCL.Add(pcl);
				count ++;
			}

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {

            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    filename = String.Format("콘트롤 프로그램 {0}개 읽어서 운전 준비됨", blockPCL.Count);
                }
                else
                {
                    filename = String.Format("Control programm {0} count loaded. Ready auto operation.", blockPCL.Count);
                }
                SmLog.LogInfo(LogCategory.SYSTEM, filename);
            }

			LoadConfig();
		}

        public static void PclProgramEnd()
        {
            PCL_STRUCT pcl;

            for (int i = 0; i < blockPCL.Count; i++)
            {
                pcl = (PCL_STRUCT)blockPCL[i];

                pcl.UnInit();
            }
        }

		//----------------------------------------------------------------------------------------------
		// CONTROL 폴더에 있는 *.CTL 파일을 메모리로 불러들인다. 프로그램 시작할 때 한번만 불러준다.
		// 그리고 프로그램 환경을 저장한다.
		//----------------------------------------------------------------------------------------------

		static void LoadOneControl(string fileext, PCL_STRUCT pcl)
		{
			string filename;

			pcl.filename = fileext;

			filename = String.Format("{0}\\control\\always\\{1}", TotalConfig.sDirWorkProject, fileext);

			pcl.pcl.LoadFromFile(filename);
		}

		static void LoadOldConfig()
		{
			PCL_STRUCT pcl;

			string inifile = String.Format("{0}\\control\\always\\control.ini", TotalConfig.sDirWorkProject);

			for(int i = 0; i < blockPCL.Count; i++) 
			{
				pcl = (PCL_STRUCT)blockPCL[i];
				pcl.flag = Profile.GetPrivateProfileIntA(pcl.filename, "Active", 0, inifile) == 1 ? true : false;
			}
		}

		static void LoadConfig()
		{
			string inifile = String.Format("{0}\\control\\always\\control.inix", TotalConfig.sDirWorkProject);

			if(!File.Exists(inifile))
			{
				LoadOldConfig();
				return;
			}

			Profile profile = new Profile();
			profile.LoadByEncodingUtf8(inifile);

			PCL_STRUCT pcl;

			for(int i = 0; i < blockPCL.Count; i++) 
			{
				pcl = (PCL_STRUCT)blockPCL[i];
				pcl.flag = profile.GetIntFromReadyMemory(pcl.filename, "Active", 0) == 1 ? true : false;
			}
		}

		public static void SaveConfig()
		{
			string inifile;
			
			inifile = String.Format("{0}\\control\\always", TotalConfig.sDirWorkProject);

			Directory.CreateDirectory(inifile);

			inifile = String.Format("{0}\\control\\always\\control.inix", TotalConfig.sDirWorkProject);

			TextWriter writer = new StreamWriter(inifile);

			if(writer == null)	return;

			PCL_STRUCT pcl;

			for(int i = 0; i < blockPCL.Count; i++) 
			{
				pcl = (PCL_STRUCT)blockPCL[i];
				writer.WriteLine("[{0}]", pcl.filename);
				writer.WriteLine("Active={0}", pcl.flag ? 1 : 0);
			}

			writer.Close();
		}
	}
}

