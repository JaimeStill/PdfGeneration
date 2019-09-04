using System;
using System.Collections.Generic;

namespace PdfGeneration.Data.Entities
{
    public class PersonAssociate
    {
        public int Id { get; set; }
        public int AssociateId { get; set; }
        public int PersonId { get; set; }

        public Person Associate { get; set; }
        public Person Person { get; set; }
    }
}