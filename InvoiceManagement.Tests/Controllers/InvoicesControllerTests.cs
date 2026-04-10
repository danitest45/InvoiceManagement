using FluentAssertions;
using InvoiceManagement.Api.Controllers;
using InvoiceManagement.Application.DTOs.Invoices;
using InvoiceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvoiceManagement.Tests.Controllers;

public class InvoicesControllerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly InvoiceController _controller;

    public InvoicesControllerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _controller = new InvoiceController(_serviceMock.Object);
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction()
    {
        // Arrange
        var response = new InvoiceResponse
        {
            Id = Guid.NewGuid(),
            CustomerName = "Daniel"
        };

        _serviceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateInvoiceRequest>()))
            .ReturnsAsync(response);

        var request = new CreateInvoiceRequest
        {
            CustomerName = "Daniel"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task AddItem_Should_Return_NoContent()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        var request = new AddInvoiceItemRequest
        {
            Description = "Mouse",
            Quantity = 1,
            UnitPrice = 100
        };

        _serviceMock
            .Setup(x => x.AddItemAsync(invoiceId, request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddItem(invoiceId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Close_Should_Return_NoContent()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _serviceMock
            .Setup(x => x.CloseAsync(invoiceId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Close(invoiceId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Invoice_Is_Null()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _serviceMock
            .Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync((InvoiceResponse?)null);

        // Act
        var result = await _controller.GetById(invoiceId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Invoice_Exists()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        var response = new InvoiceResponse
        {
            Id = invoiceId,
            CustomerName = "Daniel"
        };

        _serviceMock
            .Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(invoiceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Invoices()
    {
        // Arrange
        var response = new List<InvoiceResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CustomerName = "Daniel"
            }
        };

        _serviceMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<string?>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll(
            "Daniel",
            null,
            null,
            null);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }
}