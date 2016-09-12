using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public interface IRepository<T, in TPk> where T : class
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        T Find(TPk id);
        bool Remove(TPk id);
        bool Remove(T item);
        void Update(T item);
    }
}
