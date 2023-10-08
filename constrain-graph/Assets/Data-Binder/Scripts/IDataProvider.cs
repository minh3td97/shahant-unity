
public interface IDataProvider 
{
    event System.Action<IDataProvider, int, object> DataChanged;
    System.Type GetDataType();
    object GetData();

    string[] BindingFilters { get; }
}

public static class DataProviderExtension
{
    public static int GetFilters(this IDataProvider provider, string filters)
    {
        int value = 0x00;
        int index;

        if ((index = System.Array.IndexOf(provider.BindingFilters, filters)) >= 0)
        {
            value |= 1 << index;
        }

        return value;
    }
}