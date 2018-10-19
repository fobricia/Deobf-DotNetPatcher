using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPatcher.DotNetPatcher
{
    public class Deobfuscate
    {
        private ModuleDef module;
        private List<iDeobf> firstPart = new List<iDeobf>()
        {
            new Protections.NumericsEncoding.NumericsEncoding(),
            new Protections.ControlFlow.ControlFlow(),
            new Protections.Antis.AntiTamper(),
            new Protections.Antis.AntiDump(),
            new Protections.Antis.AntiDebug()
        };
        private List<iDeobfL> secondPart = new List<iDeobfL>()
        {
            new Protections.Proxy.Proxy(),
            new Protections.Proxy.Bool(),
            new Protections.Proxy.Bool2(),
            new Protections.StringEncryption.StringEncryption(),
            new Protections.IntegerEncryption.IntegerEncryption(),
            new Protections.Proxy.Proxy()
        };
        public Deobfuscate(ModuleDef module)
        {
            this.module = module;
        }
        public void Excute()
        {
            Dictionary<string, int> info = new Dictionary<string, int>();
            new Logger().Line();
            new Logger().mark();
            foreach (TypeDef tDef in module.Types)
            {
                foreach (MethodDef mDef in tDef.Methods.ToArray())
                {
                    if (!mDef.HasBody) continue;
                    foreach (iDeobf task in firstPart)
                    {
                        try
                        {
                            if (!CheckMarker(task.Version))
                            {
                                new Logger().Error("Can't Deobfuscate (" + task.Name + "), Because the file not protected by DotNetPatcher v4.5.9.0");
                                continue;
                            }
                            if (!info.ContainsKey(task.Name)) info.Add(task.Name, 0);
                            info[task.Name] += task.Excute(mDef);
                        }
                        catch
                        {
                            new Logger().Error("An error with " + task.Name + ".");
                        }

                    }
                }
            }
            foreach (iDeobfL task in secondPart)
            {
                try
                {
                    if (!CheckMarker(task.Version))
                    {
                        new Logger().Error("Can't Deobfuscate (" + task.Name + "), Because the file not protected by DotNetPatcher v4.5.9.0");
                        continue;
                    }
                    if (!info.ContainsKey(task.Name)) info.Add(task.Name, 0);
                    info[task.Name] += task.Excute(module);
                }
                catch
                {
                    new Logger().Error("An error with " + task.Name + ".");
                }

            }
            foreach (iDeobf task in firstPart)
                new Logger().Fixed(info[task.Name] + " " + task.Name);
            foreach (iDeobfL task in secondPart)
                new Logger().Fixed(info[task.Name] + " " + task.Name);
            if (Protections.DeobfTypes.methodDefs != null)
                foreach (var mDef in Protections.DeobfTypes.methodDefs.ToArray())
                    mDef.DeclaringType.Methods.Remove(mDef);
            new Logger().Line();
        }
        public void Save()
        {
            Logger logger = new Logger();
            string newPath = Path.GetDirectoryName(module.Location) + "\\" + Path.GetFileNameWithoutExtension(module.Location) + "-cleaned" + Path.GetExtension(module.Location);
            logger.Info("new Path: " + newPath);
            logger.Success("Writing...");
            try
            {
                module.Write(newPath);
                logger.Success("Saved Successfully.");
            }
            catch (Exception e)
            {
                logger.Info("There's an error at writing, Don't worry we will try to re-write it again.");
                module.Write(newPath, new dnlib.DotNet.Writer.ModuleWriterOptions(module) { Logger = DummyLogger.NoThrowInstance });
                logger.Success("Saved Successfully.");
            }
        }
        private bool CheckMarker(string version)
        {
            foreach(var t in module.Assembly.CustomAttributes)
            {
                if (t.TypeFullName == "DotNetPatcherObfuscatorAttribute")
                    if (version == t.ConstructorArguments[0].Value.ToString())
                        return true;
            }
            return false;
        }
    }
}
