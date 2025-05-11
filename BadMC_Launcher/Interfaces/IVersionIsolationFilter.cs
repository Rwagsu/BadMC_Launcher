using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Interfaces;

public interface IVersionIsolationFilter {
    public abstract string Name { get; }

    public abstract bool IsVersionIsolation(MinecraftEntry entry);

    public bool Equals(object? obj) {
        return obj is IVersionIsolationFilter filter && Name == filter.Name;
    }

    public int GetHashCode() {
        return HashCode.Combine(Name);
    }
}
