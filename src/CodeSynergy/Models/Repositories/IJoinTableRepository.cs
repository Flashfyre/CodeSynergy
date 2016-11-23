using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public interface IJoinTableRepository<T, in TPkA, in TPkB> where T : class
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        T Find(TPkA idA, TPkB idB);
        bool Remove(TPkA idA, TPkB idB);
        bool Remove(T item);
        void Update(T item);
    }
}