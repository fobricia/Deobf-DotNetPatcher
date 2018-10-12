using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.IntegerEncryption
{
    public class IntegerEncryption : iDeobfL
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "IntegerEncryption";
        public override int Excute(ModuleDef module)
        {
            int fixedInteger = 0;
            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].Operand is MethodDef && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            try
                            {
                                MethodDef opMethod = method.Body.Instructions[i].Operand as MethodDef;
                                if (opMethod.ReturnType != module.CorLibTypes.Int32) continue;
                                Tuple<Instruction, Instruction> tuple = getValues(opMethod);
                                if (tuple == null) continue;
                                method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                                method.Body.Instructions[i].Operand = getFixedValue(opMethod);
                                //method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                fixedInteger++;
                                DeobfTypes.Add(opMethod);
                            }
                            catch
                            {

                            }

                        }
                    }
                }
            }
            return fixedInteger;
        }
        private int getFixedValue(MethodDef method)
        {
            Tuple<Instruction, Instruction> tuple = getValues(method);
            //Console.WriteLine(tuple.Item1.Operand.ToString());
            if (tuple.Item2 == null)
            {
                return iw(gv(tuple.Item1.Operand.ToString()));
            }
            else
            {
                //Console.WriteLine(method.Name + " : " + tuple.Item1.ToString() + " : " + tuple.Item2.GetLdcI4Value());
                return fixit(tuple.Item1.ToString(), tuple.Item2.GetLdcI4Value());
            }
        }
        private Tuple<Instruction, Instruction> getValues(MethodDef method)
        {
            Instruction str = null;
            Instruction i = null;
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.IsLdcI4())
                    i = instr;
                else if (instr.OpCode == OpCodes.Ldstr)
                    str = instr;
            }
            if (str == null) return null;
            return new Tuple<Instruction, Instruction>(str, i);
        }


        #region helper
        private int fixit(string dString, int integ)
        {
            string[] array = dString.Split(new char[]
            {
                Convert.ToChar(integ)
            });
            checked
            {
                int[] array2 = new int[array.Length - 1 - 1 + 1];
                int num = 0;
                int num2 = array2.Length - 1;
                for (int i = num; i <= num2; i++)
                {
                    //Console.WriteLine(array[i]);
                    int.TryParse(array[i], out array2[i]);
                }
                return array2[array2[array2.Length - 1]];
            }
        }
        private string[] gv(string expressionStr)
        {
            return expressionStr.ToLower().Split(new char[]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries);
        }
        private int iw(string[] operands)
        {
            Stack<int> stack = new Stack<int>();
            checked
            {
                foreach (string text in operands)
                {
                    string left = text;
                    if (string.Compare(left, "+", false) == 0)
                    {
                        stack.Push(stack.Pop() + stack.Pop());
                    }
                    else if (string.Compare(left, "-", false) == 0)
                    {
                        stack.Push(0 - stack.Pop() + stack.Pop());
                    }
                    else if (string.Compare(left, "*", false) == 0)
                    {
                        stack.Push(stack.Pop() * stack.Pop());
                    }
                    else if (string.Compare(left, "/", false) == 0)
                    {
                        int num = stack.Pop();
                        stack.Push((int)Math.Round((double)stack.Pop() / (double)num));
                    }
                    else if (string.Compare(left, "sqrt", false) == 0)
                    {
                        stack.Push((int)Math.Round(Math.Sqrt((double)stack.Pop())));
                    }
                    else
                    {
                        stack.Push(int.Parse(text));
                    }
                }
                return stack.Pop();
            }
        }
        #endregion
    }
}
