using SampleApi.Data;

namespace SampleApi.Products;

public sealed class RenameProductCommand : IAsyncCommand<Unit>
{
    public RenameProductCommand(ProductId id, ProductName newName)
    {
        Id = id;
        NewName = newName;
    }

    public ProductId Id { get; }
    public ProductName NewName { get; }
}

public sealed class RenameProductCommandHandler : IAsyncCommandHandler<RenameProductCommand, Unit>
{
    readonly SampleApiDbContext _dbContext;

    public RenameProductCommandHandler(SampleApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(RenameProductCommand command)
    {
        var product = await _dbContext.Products
            .SingleOrDefaultAsync(p => p.Id == command.Id);

        if (product is null)
        {
            throw new EntityNotFoundException();
        }

        product.Rename(command.NewName);

        return Unit.Value;
    }
}
