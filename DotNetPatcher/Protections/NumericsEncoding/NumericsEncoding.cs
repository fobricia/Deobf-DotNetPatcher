using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPatcher.DotNetPatcher.Protections.NumericsEncoding
{
    public class NumericsEncoding : iDeobf
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "NumericsEncoding";
        public override int Excute(MethodDef method)
        {
            int done = 0;
            if (!method.HasBody) return 0;
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Add)
                {
                    if (method.Body.Instructions[i - 1].OpCode == OpCodes.Sizeof && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        ITypeDefOrRef type = method.Body.Instructions[i - 1].Operand as ITypeDefOrRef;
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), type, MathType.Add);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }
                    else if (method.Body.Instructions[i - 1].IsLdcI4() && method.Body.Instructions[i - 2].IsLdcI4())
                    {

                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), method.Body.Instructions[i - 1].GetLdcI4Value(), MathType.Add);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, (result));
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }

                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Sub)
                {
                    if (method.Body.Instructions[i - 1].OpCode == OpCodes.Sizeof && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        ITypeDefOrRef type = method.Body.Instructions[i - 1].Operand as ITypeDefOrRef;
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), type, MathType.Min);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, (result));
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }
                    else if (method.Body.Instructions[i - 1].IsLdcI4() && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), method.Body.Instructions[i - 1].GetLdcI4Value(), MathType.Min);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }

                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Mul)
                {
                    if (method.Body.Instructions[i - 1].OpCode == OpCodes.Sizeof && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        ITypeDefOrRef type = method.Body.Instructions[i - 1].Operand as ITypeDefOrRef;
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), type, MathType.Mul);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }
                    else if (method.Body.Instructions[i - 1].IsLdcI4() && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), method.Body.Instructions[i - 1].GetLdcI4Value(), MathType.Mul);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }

                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Div)
                {
                    if (method.Body.Instructions[i - 1].OpCode == OpCodes.Sizeof && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        ITypeDefOrRef type = method.Body.Instructions[i - 1].Operand as ITypeDefOrRef;
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), type, MathType.Div);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }
                    else if (method.Body.Instructions[i - 1].IsLdcI4() && method.Body.Instructions[i - 2].IsLdcI4())
                    {
                        int result = Emulate(method.Body.Instructions[i - 2].GetLdcI4Value(), method.Body.Instructions[i - 1].GetLdcI4Value(), MathType.Div);
                        method.Body.Instructions[i] = new Instruction(OpCodes.Ldc_I4, result);
                        method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                        method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                        done++;
                    }

                }
            }

            return done;
        }
        #region Utils
        private int Emulate(int ldc, ITypeDefOrRef type, MathType mathType)
        {
            return Emulate(ldc, getLdc(type), mathType);
        }
        private int Emulate(int ldc, int ldc_, MathType mathType)
        {
            switch (mathType)
            {
                case MathType.Add: return (ldc + ldc_);
                case MathType.Min: return (ldc - ldc_);
                case MathType.Mul: return (ldc * ldc_);
                case MathType.Div: return (ldc / ldc_);
                default: return 0;
            }
        }
        private int getLdc(ITypeDefOrRef type)
        {
            if (type.FullName == typeof(Int32).FullName) return 4;
            if (type.FullName == typeof(SByte).FullName) return 1;
            if (type.FullName == typeof(Byte).FullName) return 1;
            if (type.FullName == typeof(Boolean).FullName) return 1;
            if (type.FullName == typeof(Decimal).FullName) return 16;
            if (type.FullName == typeof(Int16).FullName) return 2;
            if (type.FullName == typeof(Int64).FullName) return 8;
            return 1;
        }
        private System.Reflection.Emit.OpCode getOpCode(MathType mathType)
        {
            switch (mathType)
            {
                case MathType.Add: return System.Reflection.Emit.OpCodes.Add;
                case MathType.Min: return System.Reflection.Emit.OpCodes.Sub;
                case MathType.Mul: return System.Reflection.Emit.OpCodes.Mul;
                case MathType.Div: return System.Reflection.Emit.OpCodes.Div;
                default: return System.Reflection.Emit.OpCodes.Add;
            }
        }
        #endregion
    }
}
