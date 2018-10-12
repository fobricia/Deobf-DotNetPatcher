using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Antis
{
    public class AntiDebug : iDeobf
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "AntiDebug";
        public override int Excute(MethodDef method)
        {
            MethodDef antiDebug = null;
            if (!method.HasBody) return 0;
            if (method.DeclaringType != method.Module.GlobalType) return 0;
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (!(instr.Operand is MethodDef)) continue;
                MethodDef mDef = instr.Operand as MethodDef;
                if (!mDef.HasBody) continue;
                if (mDef.Body.Variables.Count < 2) continue;
                if (mDef.Body.Variables[0].Type.FullName == (typeof(System.Threading.Thread)).FullName)
                {
                    if (mDef.Body.Variables[1].Type.FullName == (typeof(System.Threading.Thread)).FullName)
                    {
                        instr.OpCode = OpCodes.Nop;
                        antiDebug = mDef;
                    }
                }
            }

            if (antiDebug != null)
            {
                antiDebug.DeclaringType.Remove(antiDebug);
                return 1;
            }
            return 0;
        }
    }
}
