﻿using System.Linq.Expressions;

namespace NLayer.Core.Services
{
    public interface IService<T> where T : class
    {
        //IGenericRepository methotları kopyaladık ancak proje ilerlediğinde productService veya categoryService oluşturulduğunda dönüş tipleri farklı olacağı için ayırmak gereklidir.
        Task<T> GetByIdAync(int id);
        //GetAll asenkron yapıp tüm datayı çekmesi için Expression ifadesini kaldırdık.
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        //Update ve Remove u niye asenkron yaptık veritabanına değişikliklerin yansıması için SaveChangeAsync kullanacağımız için asenkron tanımladık.
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);

    }
}