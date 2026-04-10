using FluentAssertions;
using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Services;
using InvoiceManagement.Domain.Exceptions;
using InvoiceManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace InvoiceManagement.Tests.Services;

public class InvoiceServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static InvoiceService CreateService(AppDbContext context)
    {
        return new InvoiceService(
            context,
            NullLogger<InvoiceService>.Instance);
    }

    [Fact]
    public async Task Should_Create_Invoice_Successfully()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var request = new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        };

        var result = await service.CreateAsync(request);

        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Daniel");
        result.Status.Should().Be("Open");
        result.TotalAmount.Should().Be(0);
    }

    [Fact]
    public async Task Should_Throw_When_Customer_Name_Is_Empty()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var action = async () =>
            await service.CreateAsync(new CreateInvoiceRequest
            {
                CustomerName = ""
            });

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Customer name is required.");
    }

    [Fact]
    public async Task Should_Add_Item_And_Recalculate_Total()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        await service.AddItemAsync(invoice.Id,
            new AddInvoiceItemRequest
            {
                Description = "Mouse",
                Quantity = 2,
                UnitPrice = 100
            });

        var result = await service.GetByIdAsync(invoice.Id);

        result.Should().NotBeNull();
        result!.TotalAmount.Should().Be(200);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_Throw_When_Item_Above_1000_Has_No_Justification()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        var action = async () =>
            await service.AddItemAsync(invoice.Id,
                new AddInvoiceItemRequest
                {
                    Description = "Notebook",
                    Quantity = 1,
                    UnitPrice = 1500
                });

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Justification is required for items above R$ 1,000.");
    }

    [Fact]
    public async Task Should_Throw_When_Description_Is_Invalid()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        var action = async () =>
            await service.AddItemAsync(invoice.Id,
                new AddInvoiceItemRequest
                {
                    Description = "A",
                    Quantity = 1,
                    UnitPrice = 100
                });

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Description must have at least 3 characters.");
    }

    [Fact]
    public async Task Should_Close_Invoice_Successfully()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        await service.AddItemAsync(invoice.Id,
            new AddInvoiceItemRequest
            {
                Description = "Mouse",
                Quantity = 1,
                UnitPrice = 100
            });

        await service.CloseAsync(invoice.Id);

        var result = await service.GetByIdAsync(invoice.Id);

        result!.Status.Should().Be("Closed");
    }

    [Fact]
    public async Task Should_Not_Close_Invoice_Without_Items()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        var action = async () =>
            await service.CloseAsync(invoice.Id);

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Cannot close an invoice without items.");
    }

    [Fact]
    public async Task Should_Not_Allow_Add_Item_When_Invoice_Is_Closed()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        await service.AddItemAsync(invoice.Id,
            new AddInvoiceItemRequest
            {
                Description = "Mouse",
                Quantity = 1,
                UnitPrice = 100
            });

        await service.CloseAsync(invoice.Id);

        var action = async () =>
            await service.AddItemAsync(invoice.Id,
                new AddInvoiceItemRequest
                {
                    Description = "Keyboard",
                    Quantity = 1,
                    UnitPrice = 100
                });

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Closed invoice cannot be changed.");
    }

    [Fact]
    public async Task Should_Filter_By_Status()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var invoice = await service.CreateAsync(
            new CreateInvoiceRequest
            {
                CustomerName = "Daniel"
            });

        await service.AddItemAsync(invoice.Id,
            new AddInvoiceItemRequest
            {
                Description = "Mouse",
                Quantity = 1,
                UnitPrice = 100
            });

        await service.CloseAsync(invoice.Id);

        var result = await service.GetAllAsync(
            null,
            null,
            null,
            "Closed");

        result.Should().HaveCount(1);
        result.First().Status.Should().Be("Closed");
    }

    [Fact]
    public async Task Should_Return_Null_When_Invoice_Not_Found()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}