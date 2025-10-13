namespace project.Services;

public interface IService<TResponse, TDto>
{
    Task<TResponse> Save(TDto dto);
    Task<IEnumerable<TResponse>> FindAll(int page, int pageSize);
    Task<TResponse> FindById(int id);
    Task<TResponse> Update(int id, TDto dto);
    Task<TResponse> Delete(int id);
}