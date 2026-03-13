using System;
using System.Collections;
using System.Windows.Forms;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for CatWindowRing.
	/// </summary>
	public class CatWindowRing
	{
		class RING_STRUCT 
		{
			public Form hwnd;
		}

		public CatWindowRing()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ArrayList blockPush = new ArrayList();
		
		public void push(Form hwnd)
		{
			RING_STRUCT push;

			push = new RING_STRUCT();
			push.hwnd = hwnd;

			blockPush.Add(push);
		}

		public void pop(Form hwnd)
		{
			RING_STRUCT push;
			int l;

			for(l = 0; l < blockPush.Count; l++) 
			{
				push = (RING_STRUCT)blockPush[l];
				if(push.hwnd == hwnd) 
				{
					blockPush.RemoveAt(l);
					return;
				}
			}	
		}

		public Form GetFirstHWND()
		{
			if(blockPush.Count == 0)	return null;

			RING_STRUCT push;

			push = (RING_STRUCT)blockPush[0];

			return push.hwnd;
		}

		int GetCount() 
		{
			return blockPush.Count; 
		}

		public bool OneMdiCheck()
		{
			if(GetCount() == 0)	return false;

			Form form = GetFirstHWND();

			if(form.WindowState == FormWindowState.Minimized) 
			{
				form.WindowState = FormWindowState.Normal;
			}

			form.Select();

			return true;
		}

		public void OneMdiCreate(Form formRoot, Form form)
		{
			if(formRoot.MdiChildren.Length == 0) 
			{
				form.WindowState = FormWindowState.Maximized;	
			}
			else 
			{
				if(formRoot.MdiChildren[0].WindowState == FormWindowState.Minimized)
					form.WindowState = FormWindowState.Maximized;
				else
					form.WindowState = formRoot.MdiChildren[0].WindowState;
			}

			form.MdiParent = formRoot;
			form.Show();
		}

		public void CreateMdi(Form formRoot, Form form, int limit)
		{
			if(GetCount() >= limit)	
			{
				GetFirstHWND().Close();
			}

			OneMdiCreate(formRoot, form);
		}
	}
}
