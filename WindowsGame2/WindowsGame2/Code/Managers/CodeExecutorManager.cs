using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using MiningGame.Code.Blocks;

namespace MiningGame.Code.Managers
{
    public static class CodeExecutorManager
    {

        public static List<Block> BuildBlock(string[] bCode, string[] fileNames)
        {
            string code = @"using MiningGame.Code;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;
using YogUILibrary.Managers;
using YogUILibrary.Structs;
using YogUILibrary.UIComponents;
using YogUILibrary;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Blocks;
using MiningGame.Code.Structs;
using MiningGame.Code.Server;
using MiningGame.Code;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace buildBlock {
	static class blockBuild{
	
		public static void setBlockName(string name){
			returnBlock.SetBlockName(name);
		}
		
		public static void setBlockColor(int r, int g, int b, int a = 255){
			returnBlock.SetBlockColorRGBA(r, g, b, a);
		}

        public static void setBlockColor(Color c){
            returnBlock.SetBlockColor(c);
        }

        public static void setBlockOnTouched(Action<int, int, int, EntityMoveable> onBlockTouched){
            returnBlock.SetBlockOnTouched(onBlockTouched);
        }

        public static void setGetBlockBB(Func<int, int, Rectangle> getBB){
            returnBlock.SetBlockGetBB(getBB);
        }
		
		public static void setBlockLightLevel(int lightLevel){
			returnBlock.SetBlockLightLevel(lightLevel);
		}
		
		public static void setBlockOnPlaced(Action<int, int, bool> onBlockPlaced)
        {
            returnBlock.SetBlockOnPlaced(onBlockPlaced);
        }

        public static void setBlockOnUpdated(Func<int, int, int> onBlockUpdated){
            returnBlock.SetBlockOnUpdated(onBlockUpdated);
        }

        public static void setBlockOnRemoved(Action<int, int> onBlockRemoved)
        {
            returnBlock.SetBlockOnRemoved(onBlockRemoved);
        }

        public static void setBlockOnUsed(Action<int, int> onBlockUsed)
        {
            returnBlock.SetBlockOnUsed(onBlockUsed);
        }

        public static void setBlockGetRender(Func<int, int, SpriteBatch, Texture2D> toRender)
        {
            returnBlock.SetBlockGetRender(toRender);
        }

        public static void setBlockGetDrop(Func<int, int, byte> onBlockGetDrop)
        {
            returnBlock.SetBlockGetDrop(onBlockGetDrop);
        }

        public static void setBlockGetDropNum(Func<int, int, int> onBlockGetDropNum)
        {
            returnBlock.SetBlockGetDropNum(onBlockGetDropNum);
        }

        public static void setBlockHide(bool hide)
        {
            returnBlock.SetBlockHide(hide);
        }

        public static void setBlockOpaque(bool opaque)
        {
            returnBlock.SetBlockOpaque(opaque);
        }

        public static void setBlockHardness(int hardness)
        {
            returnBlock.SetBlockHardness(hardness);
            
        }

        public static void setBlockRenderSpecial(bool special)
        {
            returnBlock.SetBlockRenderSpecial(special);
        }

        public static void setBlockWalkThrough(bool b)
        {
            returnBlock.SetBlockWalkThrough(b);
        }
        
        public static void setBlockID(byte id){
            returnBlock.SetBlockID(id);
        }    
		
		private static Block returnBlock = new Block();
		public static List<Block> Main(){
            List<Block> ret = new List<Block>();";
            string endCode = @"
			return ret;
		}
	}
}";

            string midCode = "";
            for (int i = 0; i < bCode.Length; i++)
            {
                string s = bCode[i];
                string name = fileNames[i];
                midCode += "try{" + s + @"
                }catch (Exception e){"
                    + "ConsoleManager.Log(\"Error reading block " + name + ": \" + e.Message);"
                + @"}
                ret.Add(returnBlock);
                returnBlock = new Block();
";
            }
            code = code + midCode + endCode;
            return (List<Block>)ExecuteCode(code, "buildBlock", "blockBuild", "Main", true);
        }

        public static object ExecuteCode(string code,
    string namespacename, string classname,
    string functionname, bool isstatic, params object[] args)
        {
            object returnval = null;

            Assembly asm = BuildAssembly(code);
            if (asm == null) return null;
            object instance = null;
            Type type = null;
            if (isstatic)
            {
                type = asm.GetType(namespacename + "." + classname);
            }
            else
            {
                instance = asm.CreateInstance(namespacename + "." + classname);
                type = instance.GetType();
            }
            MethodInfo method = type.GetMethod(functionname);
            returnval = method.Invoke(instance, args);
            return returnval;
        }

        private static Assembly BuildAssembly(string code)
        {
            Microsoft.CSharp.CSharpCodeProvider provider =
               new CSharpCodeProvider();
            string path = System.Windows.Forms.Application.ExecutablePath.Replace("/", "\\");

            string add = path.Replace(System.Windows.Forms.Application.StartupPath, "").Replace("\\", "");

            AssemblyName[] s = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            List<string> assembliesToReference = new List<string>();
            foreach (AssemblyName a in s)
            {
                if (a.Name.Contains("Microsoft.Xna.Framework") || a.Name == "mscorlib" || a.Name.Contains("System") || a.Name.Contains("YogUI"))
                {
                    assembliesToReference.Add(Assembly.ReflectionOnlyLoad(a.FullName).Location);
                }
            }
            assembliesToReference.Add(add);

            CompilerParameters compilerparams = new CompilerParameters(assembliesToReference.ToArray());
            compilerparams.GenerateExecutable = false;

            compilerparams.GenerateInMemory = true;
            CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults results =
               compiler.CompileAssemblyFromSource(compilerparams, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n",
                           error.Line, error.Column, error.ErrorText);
                }
                string errorString = errors.ToString();
                ConsoleManager.Log("Error loading block: " + errorString, Microsoft.Xna.Framework.Color.Red);
                return null;
            }
            else
            {
                return results.CompiledAssembly;
            }
        }
    }
}
