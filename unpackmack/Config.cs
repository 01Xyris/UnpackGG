using dnlib.DotNet;
using System.Collections.Generic;

public abstract class Config
{
    public abstract string Name { get; }
    public abstract List<string> Patterns { get; }

    public abstract void Unpack(ModuleDefMD module);

    public virtual void Settings(ModuleDefMD module)
    {
        Work.Search(module, this);
    }
}
