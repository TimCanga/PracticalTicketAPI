using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalTicketAPI.Controllers;
using PracticalTicketAPI.Data;
using PracticalTicketAPI.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PracticalTicketAPI.Tests
{
    public class TicketsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetTickets_ReturnsAllTickets()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Tickets.Add(new Ticket { Title = "Ticket 1", Description = "First ticket", CreatedAt = DateTime.UtcNow });
            context.Tickets.Add(new Ticket { Title = "Ticket 2", Description = "Second ticket", CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();
            var repo = new TicketRepository(context);
            var controller = new TicketsController(repo);

            // Act
            var result = await controller.GetTickets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tickets = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Ticket>>(okResult.Value);
            Assert.NotEmpty(tickets);
        }
    }
}
