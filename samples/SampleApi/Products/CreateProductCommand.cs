using System.Net.Mail;
using Entr.Net.Smtp;
using SampleApi.Data;

namespace SampleApi.Products;

public class CreateProductCommand : IAsyncCommand<Product>
{
    public string Name { get; set; } = default!;
}

public class CreateProductCommandHandler : IAsyncCommandHandler<CreateProductCommand, Product>
{
    readonly SampleApiDbContext _dbContext;
    readonly EmailQueue _emailQueue;

    public CreateProductCommandHandler(
        SampleApiDbContext dbContext,
        EmailQueue emailQueue)
    {
        _dbContext = dbContext;
        _emailQueue = emailQueue;
    }

    public async Task<Product> Handle(CreateProductCommand command)
    {
        var product = new Product(new ProductName(command.Name));

        await _dbContext.Products.AddAsync(product);

        EnqueueEmails(product);

        return product;
    }

    void EnqueueEmails(Product product)
    {
        var message = new MailMessage(
            "system@sampleapi.local",
            "admin@sampleapi.local",
            "New Product Added",
            product.Id.ToString());

        _emailQueue.QueueMessage(message);
    }
}
