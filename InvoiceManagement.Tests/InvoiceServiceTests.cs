using FluentAssertions;
using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Services;
using InvoiceManagement.Domain.Enums;
using InvoiceManagement.Domain.Exceptions;
using InvoiceManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace InvoiceManagement.Tests.Services;

public class InvoiceServiceTests
{
    [Fact]
    public async Task Should_Create_Invoice_Successfully()
    {
        var context = DbContextFactory.Create();
        var service = new InvoiceService(
            context,
            NullLogger<InvoiceService>.Instance);

        var request = new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        };

        var result = await service.CreateAsync(request);

        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Daniel");
        result.Status.Should().Be(InvoiceStatus.Open);
        result.TotalAmount.Should().Be(0);
    }

    [Fact]
    public async Task Should_Throw_When_Item_Above_1000_Has_No_Justification()
    {
        var context = DbContextFactory.Create();
        var service = new InvoiceService(
            context,
            NullLogger<InvoiceService>.Instance);

        var invoice = await service.CreateAsync(new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        });

        var request = new AddInvoiceItemRequest
        {
            Description = "Notebook",
            Quantity = 1,
            UnitPrice = 1500
        };

        var action = async () => await service.AddItemAsync(invoice.Id, request);

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*Justification*");
    }

    [Fact]
    public async Task Should_Recalculate_Total_When_Adding_Item()
    {
        var context = DbContextFactory.Create();
        var service = new InvoiceService(
            context,
            NullLogger<InvoiceService>.Instance);

        var invoice = await service.CreateAsync(new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        });

        await service.AddItemAsync(invoice.Id, new AddInvoiceItemRequest
        {
            Description = "Mouse",
            Quantity = 2,
            UnitPrice = 100
        });

        var updated = await service.GetByIdAsync(invoice.Id);

        updated!.TotalAmount.Should().Be(200);
    }

    [Fact]
    public async Task Should_Not_Allow_Add_Item_When_Invoice_Is_Closed()
    {
        var context = DbContextFactory.Create();
        var service = new InvoiceService(
            context,
            NullLogger<InvoiceService>.Instance);

        var invoice = await service.CreateAsync(new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        });

        var request = new AddInvoiceItemRequest
        {
            Description = "Notebook",
            Quantity = 1,
            UnitPrice = 1500,
            Justification = "Needed"
        };
        await service.AddItemAsync(invoice.Id, request);
        await service.CloseAsync(invoice.Id);

        var action = async () => await service.AddItemAsync(invoice.Id,
            new AddInvoiceItemRequest
            {
                Description = "Teclado",
                Quantity = 1,
                UnitPrice = 100,
                Justification = "Needed for work"
            });

        await action.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*Closed*");
    }
}