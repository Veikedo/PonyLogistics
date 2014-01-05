using System;

namespace GUI.Model
{
  public interface IDataService
  {
    void GetData(Action<DataItem, Exception> callback);
  }
}