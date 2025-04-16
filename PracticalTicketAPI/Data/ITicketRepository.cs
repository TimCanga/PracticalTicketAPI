using PracticalTicketAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticalTicketAPI.Data
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(int id);
    }
}
