using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPatcher.DotNetPatcher.Protections
{
    public class DeobfTypes
    {
        public static List<TypeDef> typeDefs;
        public static List<MethodDef> methodDefs;
        public static void Add(TypeDef type)
        {
            if (typeDefs == null)
                typeDefs = new List<TypeDef>();
            if (!typeDefs.Contains(type))
                typeDefs.Add(type);
        }
        public static void Add(MethodDef method)
        {
            if (methodDefs == null)
                methodDefs = new List<MethodDef>();
            if (!methodDefs.Contains(method))
                methodDefs.Add(method);
        }
    }
}
