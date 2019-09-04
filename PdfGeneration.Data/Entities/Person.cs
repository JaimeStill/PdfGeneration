using System;
using System.Collections.Generic;

namespace PdfGeneration.Data.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string Suffix { get; set; }
        public string Nickname { get; set; }
        public string HomePhone { get; set; }
        public string DutyPhone { get; set; }
        public string OtherPhone { get; set; }
        public DateTime? Dob { get; set; }
        public string StateOfBirth { get; set; }
        public string CityOfBirth { get; set; }
        public string MothersMaidenName { get; set; }
        public string Religion { get; set; }
        public string Race { get; set; }
        public string FingerPrints { get; set; }
        public string Bdi { get; set; }
        public string Unit { get; set; }
        public string HairColor { get; set; }
        public string SectionAssigned { get; set; }
        public string EyeColor { get; set; }
        public string AttachedSection { get; set; }
        public string BloodType { get; set; }
        public string MosRate { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Gender { get; set; }
        public string Mpc { get; set; }
        public string Rank { get; set; }
        public string Basd { get; set; }
        public string Ets { get; set; }
        public string Dor { get; set; }
        public string Pebd { get; set; }
        public string Branch { get; set; }
        public string Edipi { get; set; }
        public string Allergies { get; set; }
        public string Remarks { get; set; }
        public string PicUrl { get; set; }
        public string PicPath { get; set; }
        public string PicFile { get; set; }
        public string PicName { get; set; }
        public bool IsDeleted { get; set; }

        public List<PersonAssociate> Associates { get; set; }
        public List<PersonAssociate> People { get; set; }
    }
}