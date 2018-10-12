using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Antis
{
    public class AntiDump : iDeobf
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "AntiDump";
        public override int Excute(MethodDef method)
        {
            MethodDef antidump = null;
            if (!method.HasBody) return 0;
            if (method.DeclaringType != method.Module.GlobalType) return 0;
            if (method.Name != ".cctor") return 0;
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (!(instr.Operand is MethodDef)) continue;
                MethodDef mDef = instr.Operand as MethodDef;
                if (!mDef.HasBody) continue;
                if (mDef.Body.Variables.Count == 43)
                {
                    instr.OpCode = OpCodes.Nop;
                    antidump = mDef;
                }
            }
            if (antidump != null)
            {
                antidump.DeclaringType.Remove(antidump);
                return 1;
            }
            return 0;
        }
    }
}
