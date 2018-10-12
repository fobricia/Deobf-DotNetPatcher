using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DotNetPatcher.DotNetPatcher.Protections.ControlFlow
{
    public class ControlFlow : iDeobf
    {
        public override string Version => "DotNetPatcher v4.5.9.0";
        public override string Name => "Control Flow";
        public override int Excute(MethodDef method)
        {
            int fixedcflow = 0;
            if (!method.HasBody) return fixedcflow;
            if (method.Body.HasExceptionHandlers) return fixedcflow;
            if (checkCflow(method))
            {
                method.Body.SimplifyBranches();
                Blocks blocks = FixBlocks(method);
                method.Body.Instructions.Clear();
                foreach (var bl in blocks.blocks)
                {
                    foreach (var instr in bl.instructions)
                    {
                        method.Body.Instructions.Add(instr);
                    }
                }
                method.Body.OptimizeBranches();
                fixedcflow++;
            }
            return fixedcflow;
        }
        private bool checkCflow(MethodDef method)
        {
            for(int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if ((method.Body.Instructions[i].OpCode == OpCodes.Brfalse || method.Body.Instructions[i].OpCode == OpCodes.Brfalse_S)
                    && method.Body.Instructions[i - 1].OpCode == OpCodes.Ceq
                    && method.Body.Instructions[i - 2].IsLdcI4()) return true;
            }
            return false;
        }
        private Blocks FixBlocks(MethodDef method)
        {
            Blocks blocks = GetBlocks(method);
            Blocks newBlocks = new Blocks();
            foreach (Block block in blocks.blocks)
            {
                if (block == blocks.getBlock(blocks.blocks.Count - 1)) continue;
                foreach (Instruction instr in block.instructions)
                {
                    if (block.instructions[block.instructions.Count - 1].IsStloc()
                        && block.instructions[block.instructions.Count - 2].IsLdcI4())
                    {
                        Block next = blocks.getBlock(block.ID - 1);
                        if (next.instructions[next.instructions.Count - 1].OpCode == OpCodes.Brfalse_S || next.instructions[next.instructions.Count - 1].OpCode == OpCodes.Brfalse
                            && next.instructions[next.instructions.Count - 2].OpCode == OpCodes.Ceq
                            && next.instructions[next.instructions.Count - 3].IsLdcI4()) continue;
                            if (newBlocks.ids.Contains(block.instructions[block.instructions.Count - 2].GetLdcI4Value())) continue;
                        Block newBlock = new Block();
                        newBlock.ID = block.instructions[block.instructions.Count - 2].GetLdcI4Value();
                        newBlocks.ids.Add(newBlock.ID);
                        newBlock.instructions = next.instructions;
                        newBlocks.blocks.Add(newBlock);
                    }
                }
            }

            newBlocks.blocks.Add(new Block()
            {
                instructions = blocks.getBlock(blocks.blocks.Count - 1).instructions, ID = blocks.blocks.Count
            });
            return sorting(newBlocks, blocks.blocks.Count);
        }
        private Blocks sorting(Blocks blocks, int max)
        {
            Blocks Nblocks = new Blocks();
            for(int i = 0; i <= max; i++)
            {
                foreach(Block block in blocks.blocks)
                {
                    if (block.ID == i)
                    {
                        Nblocks.blocks.Add(new Block() { instructions = block.instructions, ID = i });
                    }
                }
            }
            return Nblocks;
        }
        private Blocks GetBlocks(MethodDef method)
        {
            Blocks blocks = new Blocks();
            Block block = new Block();
            int Id = 0;
            int usage = 0;
            block.ID = Id;
            Id++;
            block.nextBlock = block.ID + 1;
            block.instructions.Add(Instruction.Create(OpCodes.Nop));
            blocks.blocks.Add(block);
            block = new Block();
            foreach (Instruction instruction in method.Body.Instructions)
            {
                int pops = 0;
                int stacks;
                instruction.CalculateStackUsage(out stacks, out pops);
                block.instructions.Add(instruction);
                usage += stacks - pops;
                if (stacks == 0)
                {
                    if (instruction.OpCode != OpCodes.Nop)
                    {
                        if (usage == 0 || instruction.OpCode == OpCodes.Ret)
                        {
                            block.ID = Id;
                            Id++;
                            block.nextBlock = block.ID + 1;
                            blocks.blocks.Add(block);
                            block = new Block();
                        }
                    }
                }
            }
            return blocks;
        }
    }
}
