using System;
using System.Collections.Generic;

namespace IT_HelpDesk
{
    public interface ITicketRepository
    {
        void AddTicket(Ticket ticket);
        List<Ticket> GetAllTickets();
        Ticket GetTicketById(Guid id);
        void AddComment(Guid ticketId, string comment);
    }
}

