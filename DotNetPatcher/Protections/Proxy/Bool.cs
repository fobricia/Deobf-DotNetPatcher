using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Proxy
{
    public class Bool : iDeobfL
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "BoolEncryption";
        public override int Excute(ModuleDef module)
        {
            int fixedProxy = 0;

            foreach (TypeDef tDef in module.Types)
            {
                foreach(MethodDef method in tDef.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].Operand is MethodDef && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            MethodDef opMethod = method.Body.Instructions[i].Operand as MethodDef;
                            if (opMethod.Parameters.Count == 0)
                            {
                                
                                List<Instruction> fixedMethod = noNop(opMethod);
                                if (fixedMethod[0].IsLdcI4() && fixedMethod[1].Operand is MethodDef && fixedMethod[1].OpCode == OpCodes.Call)
                                {
                                    
                                    MethodDef methodDef = fixedMethod[1].Operand as MethodDef;
                                    List<Instruction> fixedMethod_ = noNop(methodDef);
                                    
                                    if (fixedMethod_.Count == 8)
                                    {
                                        if (fixedMethod[0].IsLdcI4())
                                        {
                                            //Console.Write(fixedMethod_.Count + " ");
                                            method.Body.Instructions[i].OpCode = perc((int)fixedMethod[0].Operand) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
                                            //method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                            fixedProxy++;
                                            DeobfTypes.Add(methodDef);
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
        private bool perc(int numInteg)
        {
            return numInteg % 2 != 0;
        }
        #endregion
    }
}
