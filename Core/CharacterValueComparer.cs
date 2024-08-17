using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Core
{
    public class CharacterValueComparer : IEqualityComparer<CharacterValue>
    {
        public bool Equals(CharacterValue x, CharacterValue y)
        {
            // Сравниваем объекты по Id
            return x.Id == y.Id;
        }

        public int GetHashCode(CharacterValue obj)
        {
            // Возвращаем хэш-код Id
            return obj.Id.GetHashCode();
        }
    }
}
