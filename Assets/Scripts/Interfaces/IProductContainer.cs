using System.Collections.Generic;

namespace Interfaces
{
    public interface IProductContainer
    {
        List<Product> GetProducts();
        void EmptyContainer();
    }
}
