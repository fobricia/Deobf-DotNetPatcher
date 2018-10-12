using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.Antis
{
    public class AntiTamper : iDeobf
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "AntiTamper";
        public override int Excute(MethodDef method)
        {
            MethodDef antitamperMethod = null;
            if (method.DeclaringType != method.Module.GlobalType) return 0;
            if (method.Name != ".cctor") return 0;
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (!(instr.Operand is MethodDef)) continue;
                MethodDef mDef = instr.Operand as MethodDef;
                if (!mDef.HasBody) continue;
                if (mDef.Body.Variables.Count < 4) continue;
                if (mDef.Body.Variables[0].Type.FullName == (typeof(System.IO.Stream)).FullName)
                {
                    if (mDef.Body.Variables[4].Type.FullName == (typeof(System.IO.BinaryReader)).FullName)
                    {
                        instr.OpCode = OpCodes.Nop;
                        antitamperMethod = mDef;
                    }
                }
            }

            if (antitamperMethod != null)
            {
                antitamperMethod.DeclaringType.Remove(antitamperMethod);
                return 1;
            }
            return 0;
        }
    }
}
