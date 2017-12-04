using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL
{
    public abstract class Equatable<T> where T : IEquatable<T>
    {
        // Overrides System.Object GetHashCode method
        public abstract override int GetHashCode();

        // Overrides System.Object Equals method
        public override bool Equals(object obj)
        {
            if (obj is T)
                return ((IEquatable<T>)this).Equals((T)obj);
            else
                return false;
        }
    }
}
