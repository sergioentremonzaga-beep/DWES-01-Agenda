namespace DWES_01_Agenda.Mappers;

public interface IMapper<T, TEntity> where T : class
{
    public T ToModel(TEntity entity);
    public TEntity ToEntity(T model);
}