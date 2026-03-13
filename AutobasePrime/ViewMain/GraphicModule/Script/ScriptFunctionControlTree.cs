using System;
using NetTools;
using AutoLibLocal;
using AutoLib;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace GraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionTag.
    /// </summary>
    public class ScriptFunctionControlTree
    {
        static async Task<(int, object val)> Run_Common(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                // object[] args이지만 param object[] args를 호출하면 자동으로 param object[] 로 배열된다.
                val = await scriptClass.ExecuteClassName(ObjectControlTreeView.arrayClassList, (string)args[0], method_name, args);
            }

            return (1, val);
        }

        static int ProcTreeNodeNew(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string text;
            int imageIndex;
            int selectedImageIndex;

            try
            {
                text = (string)args[0];
                imageIndex = (int)args[1];
                selectedImageIndex = (int)args[2];
            }
            catch (Exception exception)
            {
                scriptClass.ErrorMessage(exception.Message);
                val = 0;
                return -1;
            }

            val = new TreeNode(text, imageIndex, selectedImageIndex);
            return 1;
        }

        static int ProcTreeNodeAdd(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            TreeNode parent;
            TreeNode child;

            val = 0;

            try
            {
                parent = (TreeNode)args[0];
                child = (TreeNode)args[1];
            }
            catch (Exception exception)
            {
                scriptClass.ErrorMessage(exception.Message);
                return -1;
            }

            parent.Nodes.Add(child);

            val = 1;

            return 1;
        }

        static int ProcTreeNodeGetString(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            TreeNode node;

            val = "";

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                try
                {
                    node = (TreeNode)args[0];
                }
                catch (Exception exception)
                {
                    scriptClass.ErrorMessage(exception.Message);
                    return -1;
                }

                if (method_name == "TreeNodeGetText")
                    val = node.Text;
                else if (method_name == "TreeNodeGetFullPath")
                    val = node.FullPath;
                else
                    val = "";
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Tree";

            prepare.AddMethod(prename, "TreeNodeAdd", "void", new ScriptExternalRun.DeleMethod(ProcTreeNodeAdd), "in:object:node", "in:object:child");
            prepare.AddMethod(prename, "TreeNodeNew", "object", new ScriptExternalRun.DeleMethod(ProcTreeNodeNew), "in:string:text", "in:int:imageIndex", "in:int:selectedImageIndex");
            prepare.AddMethod(prename, "TreeNodeGetFullPath", "string", new ScriptExternalRun.DeleMethod(ProcTreeNodeGetString), "in:object:node");
            prepare.AddMethod(prename, "TreeNodeGetText", "string", new ScriptExternalRun.DeleMethod(ProcTreeNodeGetString), "in:object:node");

            prepare.AddMethod(prename, "TreeViewExpandAll", "void", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname");
            prepare.AddMethod(prename, "TreeViewGetSelectedNode", "object", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname");
            prepare.AddMethod(prename, "TreeViewNodeAdd", "void", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname", "in:object:child");
            prepare.AddMethod(prename, "TreeViewNodeClear", "void", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname");
            prepare.AddMethod(prename, "TreeViewSeekNode", "object", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname", "in:string:fullpath");
            prepare.AddMethod(prename, "TreeViewSetImageList", "void", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname", "in:object:imageList");
            prepare.AddMethod(prename, "TreeViewSetSelectedNode", "void", new ScriptExternalRun.AsyncDeleMethod(Run_Common), "in:string:classname", "in:object:child");

            /*
            prepare.AddMethod(prename, "TabControlAddPage", "void", new ScriptExternalRun.DeleMethod(Run_Common), "in:string:classname", "in:string:page_title", "in:string:module_name");
            prepare.AddMethod(prename, "TabControlInsertPage", "void", new ScriptExternalRun.DeleMethod(Run_Common), "in:string:classname", "in:int:position", "in:string:page_title", "in:string:module_name");
            prepare.AddMethod(prename, "TabControlDeletePage", "void", new ScriptExternalRun.DeleMethod(Run_Common), "in:string:classname", "in:int:position");
             
            prepare.AddMethod(prename, "ContextMenuNew", "object", new ScriptExternalRun.DeleMethod(ProcContextMenuNew));
            prepare.AddMethod(prename, "ContextMenuNewItem", "object", new ScriptExternalRun.DeleMethod(ProcContextMenuNewItem), "in:string:title", "in:string:script");
            prepare.AddMethod(prename, "ContextMenuAddItem", "void", new ScriptExternalRun.DeleMethod(ProcContextMenuAddItem), "in:object:context", "in:object:menuitem");
            prepare.AddMethod(prename, "ContextMenuAddSubItem", "void", new ScriptExternalRun.DeleMethod(ProcContextMenuAddSubItem), "in:object:menuitem", "in:object:subitem");
            prepare.AddMethod(prename, "ContextMenuShow", "void", new ScriptExternalRun.DeleMethod(ProcContextMenuShow), "in:object:context", "in:int:x", "in:int:y");
             
             */

        }
    }
}


