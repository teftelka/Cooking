using System;
using System.Collections.Generic;

namespace Interfaces
{
    // A plate or cookware: holds products and stays in place when its contents move elsewhere
    public interface IProductContainer
    {
        // Raised after any change of what is inside
        event EventHandler OnContentsChanged;

        IReadOnlyList<Product> GetProducts();

        // Forgets the contents: they have moved into something else
        void EmptyContainer();

        // Destroys the contents (recycle bin); the container itself stays
        void DestroyContents();
    }
}
