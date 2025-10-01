using System;
using System.Collections.Generic;

namespace IT_HelpDesk
{
    public class Ticket
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OriginId { get; set; }
        public string CategoryId { get; set; }
        public string SubcategoryId { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<string> Attachments { get; set; } = new List<string>();
    }

}




