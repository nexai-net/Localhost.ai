using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.Corpus
{
    public class Cv : EntityBase
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNumber { get; set; }
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string AddressCountry { get; set; }
        public string Phone { get; set; }
        public string LinkedInProfile { get; set; }
        public string Summary { get; set; }
        public List<Education> Educations { get; set; }
        public List<Experience> Experiences { get; set; }   
        public List<string> Skills { get; set; }
    }

    public class Experience
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        
    }


    public class Creation
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public class Education
    {
        public string Institution { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
    }
}
