using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class ProductGroup : SimpleEntity
{
    private readonly List<Product> _products = [];
    private readonly List<ProductGroup> _childGroups = [];

    protected ProductGroup(TitleValue title, CodeValue code, Guid id) : base(title, code, id)
    {
    }

    public ProductGroup? ParentGroup { get; set; }
    public IReadOnlyCollection<ProductGroup> ChildGroups => _childGroups.AsReadOnly();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public static Operation<ProductGroup, string> Create(TitleValue title, CodeValue code, ProductGroup? parentGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");

        var productGroup = new ProductGroup(title, code, Guid.CreateVersion7());

        if (parentGroup is not null)
            productGroup.SetParent(parentGroup);

        return productGroup;
    }

    public Operation<bool, string> Edit(TitleValue title, ProductGroup? parentGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        Title = title;

        if (parentGroup is not null)
            SetParent(parentGroup);

        return true;
    }

    public void SetParent(ProductGroup? parentGroup)
    {
        ParentGroup = parentGroup;
    }

    public void AddChildGroup(ProductGroup childGroup)
    {
        childGroup.SetParent(this);
        if (_childGroups.Any(x => x.Code == childGroup.Code))
            return;

        _childGroups.Add(childGroup);
    }
}
