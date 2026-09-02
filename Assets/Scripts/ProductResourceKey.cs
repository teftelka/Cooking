using System;

[Serializable]
public struct ProductResourceKey : IEquatable<ProductResourceKey>
{
    public ProductSO productSO;
    public int range;

    public ProductResourceKey(ProductSO productSO, int range)
    {
        this.productSO = productSO;
        this.range = range;
    }

    public bool Equals(ProductResourceKey other)
    {
        return productSO == other.productSO && range == other.range;
    }

    public override bool Equals(object obj)
    {
        return obj is ProductResourceKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(productSO, range);
    }
}