using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPatcher.DotNetPatcher.Protections.ControlFlow
{
    public class Blocks
    {
        public List<Block> blocks = new List<Block>();
        public List<int> ids = new List<int>();
        public Block getBlock(int id)
        {
            return blocks.Single(block => block.ID == id);
        }
    }
}
