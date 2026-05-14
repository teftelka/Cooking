using System.Collections.Generic;

public interface IProductContainer
{
    List<Product> GetProducts();
    void EmptyContainer();
}
