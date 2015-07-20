using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace QPP.Validation
{
    /// <summary>
    /// 动态计算表达式的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationExpression<T>
    {
        object instance;
        MethodInfo method;

        public ValidationExpression(string expression)
        {
            Type tp = typeof(T);

            //if (expression.IndexOf("return") < 0)
            //    expression = "return " + expression + ";";
            string className = "Expression";
            string methodName = "Validate";
            CompilerParameters p = new CompilerParameters();
            //String exeName = String.Format(@"{0}\exp.dll",
            //System.Environment.CurrentDirectory);
            //p.OutputAssembly = exeName;
            p.GenerateInMemory = true;

            foreach (var a in Assembly.GetCallingAssembly().GetReferencedAssemblies())
                p.ReferencedAssemblies.Add(Assembly.Load(a).Location);
            foreach (var a in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                p.ReferencedAssemblies.Add(Assembly.Load(a).Location);
            CompilerResults cr = new CSharpCodeProvider().CompileAssemblyFromSource(
                p, string.Format(@"
using System;
using QPP;
using QPP.Validation;
sealed class {0}
{{
        public void {1}({3} model, bool isNew)
        {{
            {2};
        }}
}}",
                className, methodName, expression, tp.ToString()));
            if (cr.Errors.Count > 0)
            {
                string msg = "Expression(\"" + expression + "\"): \n";
                foreach (CompilerError err in cr.Errors) msg += err.ToString() + "\n";
                throw new Exception(msg);
            }
            instance = cr.CompiledAssembly.CreateInstance(className);
            method = instance.GetType().GetMethod(methodName);
        }

        public void Validate(T model, bool isNew)
        {
            method.Invoke(instance, new object[] { model, isNew });
        }
    }
}
