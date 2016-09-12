using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public interface IKeyValueRepository<T, in TPk> where T : class
    {
        void Add(TPk key, T val);
        IEnumerable<T> GetAll();
        T Find(TPk key);
        bool Remove(TPk key);
        void Update(T val);
    }
}
