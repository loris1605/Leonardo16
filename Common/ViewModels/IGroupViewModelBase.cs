namespace Common.ViewModels
{
    public interface IGroupViewModelBase
    {
        bool GroupFocus { get; set; }
        Task CaricaDataSource(int id = 0);
        Task CaricaByModel(object model);

    }
}
