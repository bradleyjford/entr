using System.Data;
using System.Net.Mail;
using Entr.Net.Smtp;
using FluentValidation;
using SampleApi.Data;

namespace SampleApi.Products;

public class CreateProductCommand : IAsyncCommand<Product>
{
    public string Name { get; set; } = default!;

    public class Validator : AbstractValidator<CreateProductCommand>
    {
        public Validator()
        {
            this.RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}

public class CreateProductCommandHandler : IAsyncCommandHandler<CreateProductCommand, Product>
{
    private readonly SampleApiDbContext _dbContext;
    private readonly EmailQueue _emailQueue;

    public CreateProductCommandHandler(
        SampleApiDbContext dbContext,
        EmailQueue emailQueue)
    {
        _dbContext = dbContext;
        _emailQueue = emailQueue;
    }

    public async Task<Product> Handle(CreateProductCommand request)
    {
        var product = new Product(request.Name);

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
