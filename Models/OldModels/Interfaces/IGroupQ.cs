namespace Models.Interfaces
{
    public interface IGroupQ<T> : IDisposable where T : class
    {
        List<T> Load(int index = 0);
        List<T> LoadByModel(object model);
        

    }
}
