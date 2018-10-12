using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Proxy
{
    public class Bool2 : iDeobfL
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "Bool2Encryption";
        public override int Excute(ModuleDef module)
        {
            int fixedProxy = 0;

            foreach (TypeDef tDef in module.Types)
            {
                foreach (MethodDef method in tDef.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].Operand is MethodDef && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            MethodDef opMethod = method.Body.Instructions[i].Operand as MethodDef;
                            if (opMethod.Parameters.Count == 1)
                            {
                                if (containsRound(opMethod))
                                {
                                    if (method.Body.Instructions[i - 1].IsLdcI4())
                                    {
                                        method.Body.Instructions[i].OpCode = round((int)method.Body.Instructions[i - 1].Operand) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
                                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                        fixedProxy++;
                                        DeobfTypes.Add(opMethod);
                                    }

                                }
                            }
                        }
                    }

                }
            }


            return fixedProxy;
        }
        private bool containsRound(MethodDef method)
        {
            foreach(Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Call && instr.Operand.ToString().ToLower().Contains("round") && ((MemberRef)instr.Operand).DeclaringType.Name == typeof(Math).Name)
                {
                    return true;
                }
            }
            return false;
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
        #region utils
        private bool round(int numberInteg)
        {
            bool result = true;
            checked
            {
                int num = (int)Math.Round((double)numberInteg / 2.0);
                int num2 = 2;
                int num3 = num;
                for (int i = num2; i <= num3; i++)
                {
                    if (numberInteg % i == 0)
                    {
                        result = false;
                    }
                }
                return result;
            }
        }
        #endregion
    }
}
