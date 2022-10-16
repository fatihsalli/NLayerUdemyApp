using System.Linq.Expressions;

namespace NLayer.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAync(int id);
        //=>GetAll,Where Neden asenkron tanımlamadık aşağıda henüz veritabanına sorgu yapmıyoruz aşağıdaki metot ile veritabanına yapılacak sorguyu oluşturuyoruz. Bu sebeple IQueryable seçtik. ToList dendiğinde veritabanına gidip sorgu yapar.
        //=>productRepository.where(x=> x.id>5).OrderBy.ToList() - ToList dediğim anda sorgu yapar.
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
        IQueryable<T> Where(Expression<Func<T,bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        //=>EF Core da Update ve Delete için asenkron olan yapısı yoktur.Entity'i zaten EF Core takip ediyor, Update metoduna geldiğinde State'ini Modify yapıyor. Yani uzun süren bir işlem olmadığı için asenkronu yoktur.
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
