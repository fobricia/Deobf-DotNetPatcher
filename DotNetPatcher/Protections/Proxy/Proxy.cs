using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Proxy
{
    public class Proxy : iDeobfL
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "Proxy";
        public override int Excute(ModuleDef module)
        {
            //Console.WriteLine(method.Name + " : " + method.DeclaringType.Name);
            int fixedProxy = 0;
            foreach(var tDef in module.Types)
            {
                foreach(MethodDef method in tDef.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {

                        if (method.Body.Instructions[i].Operand is MethodDef && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            MethodDef opMethod = (MethodDef)method.Body.Instructions[i].Operand;

                            if (opMethod.HasBody)
                            {
                                if (noNop(opMethod).Count == 2 || noNop(opMethod).Count == 4)
                                {
                                    if (opMethod.ReturnType != opMethod.Module.CorLibTypes.Void)
                                    {
                                        Instruction newInstr = noNop(opMethod)[0];
                                        if (newInstr.OpCode == OpCodes.Newobj || newInstr.IsLdcI4() || newInstr.OpCode == OpCodes.Ldstr || newInstr.OpCode == OpCodes.Ldc_I8 || newInstr.OpCode == OpCodes.Ldc_R4)
                                        {
                                            method.Body.Instructions[i].OpCode = newInstr.OpCode;
                                            method.Body.Instructions[i].Operand = newInstr.Operand;
                                            fixedProxy++;
                                            DeobfTypes.Add(opMethod);
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
            }
            return fixedProxy;
        }
        private List<Instruction> noNop(MethodDef method)
        {
            List<Instruction> np = new List<Instruction>();
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode != OpCodes.Nop)
                    np.Add(instr);
            }
            return np;
        }
     
    }
}
