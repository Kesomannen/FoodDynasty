﻿namespace Dynasty.Library.Data {

/// <summary>
/// Simple interface for providing data.
/// </summary>
public interface IDataProvider<out T> {
    T Data { get; }
}

}