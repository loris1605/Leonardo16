using Models.Entity;
using Models.Interfaces;

namespace Models.Repository
{
    public interface IOperatoreR : IDisposable, IGroupQ<OperatoreMap>
    {
        int Add(OperatoreMap map);
        bool Del(OperatoreMap map);
        OperatoreMap First(int id);
        bool IfExistBadge(int badge, int codiceoperatore);
        bool IfExistBadge(OperatoreMap dT);
        bool IfExistNickname(OperatoreMap dT);
        bool IfExistNickname(string nomeoperatore, int codiceoperatore);
        bool IfExistPermessi(OperatoreMap dT);
        bool Upd(OperatoreMap map);
    }
}