using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace AirOut.Web.Extensions
{
    public static class CreateRefDynamicClass
    {
        public static Assembly CreateAssembly(string ClassName,Dictionary<string,string> dictProperty)
        {
    //        //创建编译器实例。  
    //        var provider = new CSharpCodeProvider();
    //        //设置编译参数。  
    //        var paras = new CompilerParameters();

    ////        ReferencedAssemblies：获取当前项目所引用的程序集。Add方法为程序集添加引用。
    ////GenerateExecutable：获取或设置一个值，该值指示是否生成可执行文件。若此属性为false，则生成DLL，默认是false。
    ////GenerateInMemory：获取或设置一个值，该值指示是否在内存中生成输出。

    //        //设置生成的程序集并驻入内存
    //        paras.GenerateExecutable = false;
    //        //在内存中就无须输出目录
    //        paras.GenerateInMemory = true;
    //        //创建动态代码。  
    //        StringBuilder classSource = new StringBuilder();
    //        classSource.Append("public class " + ClassName + " n");
    //        classSource.Append("{n");
    //        //创建属性。  
    //        classSource.Append(propertyString("aaa"));
    //        classSource.Append(propertyString("bbb"));
    //        classSource.Append(propertyString("ccc"));
    //        classSource.Append("}");
    //        System.Diagnostics.Debug.WriteLine(classSource.ToString());
    //        //编译代码。  
    //        CompilerResults result = provider.CompileAssemblyFromSource(paras, classSource.ToString());
    //        //获取编译后的程序集。  
    //        Assembly assembly = result.CompiledAssembly;
    //        return assembly;

            // 1.CSharpCodePrivoder
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.ICodeComplier
            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            // 3.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            // 4.CompilerResults
            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, GenerateCode(ClassName, dictProperty));

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine(err.ErrorText);
                }

                return null;
            }
            else
            {
                // 通过反射，调用HelloWorld的实例
                Assembly objAssembly = cr.CompiledAssembly;

                return objAssembly;

                //object objHelloWorld = objAssembly.CreateInstance("DynamicCodeGenerate.HelloWorld");
                //MethodInfo objMI = objHelloWorld.GetType().GetMethod("OutPut");

                //Console.WriteLine(objMI.Invoke(objHelloWorld, null));
            }
        }

        static string GenerateCode( string ClassName,Dictionary<string,string> dictProperty)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;");
            sb.Append(Environment.NewLine);
            sb.Append("namespace AirOut.Web");
            sb.Append(Environment.NewLine);
            sb.Append("{");
            sb.Append(Environment.NewLine); 
            sb.Append("    public class " + ClassName + "");
            sb.Append(Environment.NewLine);
            sb.Append("    {");
            sb.Append(Environment.NewLine);
            sb.Append("    public " + ClassName + " ()");
            sb.Append(Environment.NewLine);
            sb.Append("    {");
            sb.Append(Environment.NewLine);
            sb.Append("    }");
            sb.Append(Environment.NewLine);

            foreach (var item in dictProperty)
            {
                sb.Append("        public " + item.Value + " " + item.Key);
                sb.Append(Environment.NewLine);
                sb.Append("        {");
                sb.Append(Environment.NewLine);
                sb.Append("             get;set;");
                sb.Append(Environment.NewLine);
                sb.Append("        }");
                sb.Append(Environment.NewLine);
            }

            sb.Append("        public string OutPut()");
            sb.Append(Environment.NewLine);
            sb.Append("        {");
            sb.Append(Environment.NewLine);
            sb.Append("             return \"funcOutPut\";");
            sb.Append(Environment.NewLine);
            sb.Append("        }");
            sb.Append(Environment.NewLine);

            sb.Append("    }");
            sb.Append(Environment.NewLine);
            sb.Append("}");

            string code = sb.ToString();
            Console.WriteLine(code);
            Console.WriteLine();

            return code;
        }

        /// <summary>
        /// propertyString方法就是用来拼写字符串的
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string propertyInt(string propertyName)
        {
            StringBuilder sbProperty = new StringBuilder();
            sbProperty.Append(" private   int   _" + propertyName + "   =   0;n");
            sbProperty.Append(" public   int   " + "" + propertyName + "n");
            sbProperty.Append(" {n");
            sbProperty.Append(" get{   return   _" + propertyName + ";}   n");
            sbProperty.Append(" set{   _" + propertyName + "   =   value;   }n");
            sbProperty.Append(" }");
            return sbProperty.ToString();
        }

        /// <summary>
        /// propertyString方法就是用来拼写字符串的
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string propertyString(string propertyName)
        {
            StringBuilder sbProperty = new StringBuilder();
            sbProperty.Append(" private   string   _" + propertyName + "   =   null;n");
            sbProperty.Append(" public   string   " + "" + propertyName + "n");
            sbProperty.Append(" {n");
            sbProperty.Append(" get{   return   _" + propertyName + ";}   n");
            sbProperty.Append(" set{   _" + propertyName + "   =   value;   }n");
            sbProperty.Append(" }");
            return sbProperty.ToString();
        }

        /// <summary>
        /// propertyString方法就是用来拼写字符串的
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string propertyBool(string propertyName)
        {
            StringBuilder sbProperty = new StringBuilder();
            sbProperty.Append(" private   bool   _" + propertyName + "   =   false;n");
            sbProperty.Append(" public   bool   " + "" + propertyName + "n");
            sbProperty.Append(" {n");
            sbProperty.Append(" get{   return   _" + propertyName + ";}   n");
            sbProperty.Append(" set{   _" + propertyName + "   =   value;   }n");
            sbProperty.Append(" }");
            return sbProperty.ToString();
        }
        
//整个代码比较简单，主要步骤就是：1、拼写类的字符串  2、调用CSharpCodeProvider类进行编译得到程序集(assembly)
//接下来就可以利用之前反射的方法来动态调用这个类中的属性了：

            //Assembly assembly = NewAssembly();
            //object Class1 = assembly.CreateInstance("DynamicClass");
            //ReflectionSetProperty(Class1, "aaa", 10);
            //ReflectionGetProperty(Class1, "aaa");
            //object Class2 = assembly.CreateInstance("DynamicClass");
            //ReflectionSetProperty(Class1, "bbb", 20);
            //ReflectionGetProperty(Class1, "bbb");

         
        /// <summary>
        /// 给属性赋值
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private static bool ReflectionSetProperty(object objClass, string propertyName, object value)
        {
            bool retbool =false;
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == propertyName && info.CanWrite)
                {
                    info.SetValue(objClass, value, null);
                    retbool = true;
                }
            }
            return retbool;
        }

        /// <summary>
        /// 取得属性的值
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="propertyName"></param>
        private static object ReflectionGetProperty(object objClass, string propertyName)
        {
            object retval = null;
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == propertyName && info.CanRead)
                {
                    retval = info.GetValue(objClass, null);
                }
            }

            return retval;
        }
    }
}