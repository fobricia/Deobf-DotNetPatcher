using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.StringEncryption
{
    public class StringEncryption : iDeobfL
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "StringEncryption";
        public override int Excute(ModuleDef module)
        {
            int fixedString = 0;
            foreach(TypeDef type in module.Types)
            {
                foreach(MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].Operand is MethodDef && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            MethodDef opMethod = method.Body.Instructions[i].Operand as MethodDef;
                            if (opMethod.Parameters.Count != 1) continue;
                            Tuple<string, int> tuple = getValues(opMethod);
                            if (tuple == null) continue;
                            if (method.Body.Instructions[i - 1].IsLdcI4())
                            {
                                bool b = (method.Body.Instructions[i - 1].GetLdcI4Value() == 1 ? true : false);
                                method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                                method.Body.Instructions[i].Operand = fixit(b, tuple.Item1, tuple.Item2);
                                method.Body.Instructions[i-1].OpCode = OpCodes.Nop;
                                fixedString++;
                                DeobfTypes.Add(opMethod);
                            }
                        }
                    }
                }
            }
            return fixedString;
        }
        private Tuple<string, int> getValues(MethodDef method)
        {
            string str = null;
            int i = 0;
            foreach(Instruction instr in method.Body.Instructions)
            {
                if (instr.IsLdcI4())
                    i = instr.GetLdcI4Value();
                else if (instr.OpCode == OpCodes.Ldstr)
                    str = instr.Operand.ToString();
            }
            if (str == null) return null;
            return new Tuple<string, int>(str, i);
        }


        #region helper

        private string fixit(bool A_0, string tString, int numInteg)
        {
            string stringStr = Yzs(tString, numInteg);
            byte[] strByt = Rqp(stringStr, A_0);
            return ZVv(strByt, A_0);
        }
        private string Yzs(string tString, int numInteg)
        {
            string text = string.Empty;
            checked
            {
                int num = tString.Length - 1;
                for (int i = 0; i <= num; i++)
                {
                    int utf = Convert.ToInt32(tString[i]) ^ numInteg;
                    text += char.ConvertFromUtf32(utf);
                }
                return text;
            }
        }
        private byte[] Rqp(string stringStr, bool defEnc)
        {
            checked
            {
                byte[] result;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] bytes;
                    if (defEnc)
                    {
                        bytes = Encoding.Default.GetBytes(stringStr);
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(stringStr);
                    }
                    using (FromBase64Transform fromBase64Transform = new FromBase64Transform())
                    {
                        byte[] array = new byte[fromBase64Transform.OutputBlockSize - 1 + 1];
                        int num = 0;
                        while (bytes.Length - num > 4)
                        {
                            fromBase64Transform.TransformBlock(bytes, num, 4, array, 0);
                            num += 4;
                            memoryStream.Write(array, 0, fromBase64Transform.OutputBlockSize);
                        }
                        array = fromBase64Transform.TransformFinalBlock(bytes, num, bytes.Length - num);
                        memoryStream.Write(array, 0, array.Length);
                        fromBase64Transform.Clear();
                    }
                    memoryStream.Position = 0L;
                    int num2;
                    if (memoryStream.Length > 2147483647L)
                    {
                        num2 = int.MaxValue;
                    }
                    else
                    {
                        num2 = Convert.ToInt32(memoryStream.Length);
                    }
                    byte[] array2 = new byte[num2 - 1 + 1];
                    memoryStream.Read(array2, 0, num2);
                    memoryStream.Close();
                    result = array2;
                }
                return result;
            }
        }
        private string ZVv(byte[] strByt, bool defEnc)
        {
            int num;
            do
            {
                num = 4;
                if (num == 1)
                {
                    num = 2;
                }
                if (num == 2)
                {
                    if (!defEnc)
                    {
                        break;
                    }
                    num = 3;
                }
                if (num == 0)
                {
                    num = 1;
                }
            }
            while (num != 4);
            return Encoding.UTF8.GetString(strByt);
        }
        #endregion
    }
}
