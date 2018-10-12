using dnlib.DotNet;
using DotNetPatcher.DotNetPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger logger = new Logger();
            if (args.Length < 1)
            {
                logger.Error("Please select your file first.");
                Console.ReadKey();
            }
            if (args[0] != null)
            {
                string path = args[0];
                Deobfuscate deobf = new Deobfuscate(ModuleDefMD.Load(path));
                logger.Info("File loaded successfully.");
                deobf.Excute();
                deobf.Save();
                logger.Info("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
