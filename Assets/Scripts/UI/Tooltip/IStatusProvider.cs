using System;
using System.Collections.Generic;

public interface IStatusProvider {
    event Action<IStatusProvider> OnStatusChanged;

    IEnumerable<(string Name, string Value)> GetStatus();
}