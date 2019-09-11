using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.Pdf;
using PdfGeneration.Data.Entities;
using PdfGeneration.Core.Extensions;

namespace PdfGeneration.Data.Extensions
{
    public static class PdfExtensions
    {

        public static string GeneratePdfFile(this Person person, string basePath, string name)
        {
            return $@"{basePath}{person.LastName}_{person.FirstName}_{name}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";
        }

        //format for PDF Field names -> {Entity}.{OptionalSpecialType}.Property.{Optional[CheckBox] or Optional[(int)SingleCharacter]}
        // Example: Z.LastName  /   Z.AreaCode.HomePhone   /   Z.SSN.1   /   Z.Gender.Male.CheckBox
        public static Task GeneratePdf(this Object entity, string file, string template) => Task.Run(() =>
        {
            using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
            {
                processor.LoadDocument(template);

                var data = processor.GetFormData();
                var fields = processor.GetFormFieldNames();
                var isMultiple = fields.Any(x => x.ToLower().StartsWith("z.") || x.ToLower().StartsWith("y."));

                foreach (var field in fields)
                {
                    if (field.ToLower().StartsWith("x.") || field.ToLower().StartsWith("y.") || field.ToLower().StartsWith("z."))
                    {
                        entity = isMultiple ? entity.GetEntitySub(field.Split('.')[0]) : entity;

                        if (entity != null)
                        {
                            if (int.TryParse(field.Split('.').Last(), out int res))
                            {
                                data.SetSingleField(entity, field);
                            }
                            else
                            {
                                data.SetValue(entity, field);
                            }
                        }
                    }
                }

                processor.ApplyFormData(data);
                file.EnsureFileDirectoryExists();
                processor.SaveDocument(file);
            }
        });

        static void SetSingleField(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');
            if (entity.IsMatch(splitField[splitField.Length - 2]))
            {
                var entityData = entity
                    .GetEntityValue(splitField[splitField.Length - 2])
                    .UrlEncode("[^a-zA-Z0-9]");

                data[field].Value = entityData[Int32.Parse(splitField[splitField.Length - 1]) - 1].ToString();
            }
        }

        static void SetValue(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');
            if (splitField.Last().IsSpecialType())
            {
                data.SetSpecialValue(entity, field, splitField);
            }
            else
            {
                if (entity.IsMatch(splitField[splitField.Length - 1]))
                {
                    var entityData = entity.GetEntityValue(splitField[splitField.Length - 1]);

                    if (!string.IsNullOrEmpty(entityData))
                    {
                        switch (Type.GetTypeCode(data[field].GetType()))
                        {
                            case TypeCode.DateTime:
                                data[field].Value = Convert.ToDateTime(entityData);
                                break;
                            case TypeCode.Int32:
                                data[field].Value = int.Parse(entityData);
                                break;
                            default:
                                data[field].Value = entityData.ToString();
                                break;
                        }
                    }
                }
            }
        }

        static bool IsSpecialType(this string type)
        {
            if (type.ToLower().Contains("phone"))
            {
                return true;
            }
            else
            {
                switch (type.ToLower())
                {
                    case "checkbox":
                    case "height":
                    case "date":
                    case "name":
                    case "ssn":
                        return true;
                    default:
                        return false;
                }
            }
        }

        static void SetSpecialValue(this PdfFormData data, object entity, string field, string[] splitField)
        {
            if (splitField.Last().ToLower().Contains("phone"))
            {
                data.SetPhoneNumber(field, entity);
            }
            else if (splitField.Last().ToLower() == "date" && splitField.Length == 4)
            {
                data.SetDate(field, entity);
            }
            else
            {
                switch (splitField.Last().ToLower())
                {
                    case "checkbox":
                        data.SetCheckbox(entity, field);
                        break;
                    case "height":
                        data.SetHeight(field, entity);
                        break;
                    case "name":
                        data.SetName(field, entity);
                        break;
                    case "ssn":
                        data.SetSsn(field, entity);
                        break;
                }
            }
        }

        static void SetCheckbox(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');

            if (entity.IsMatch(splitField[splitField.Length - 3]))
            {
                var entityData = entity.GetEntityValue(splitField[splitField.Length - 3]);

                if (entityData.ToLower() == splitField[splitField.Length - 2].ToLower())
                {
                    data[field].Value = "On";
                }
            }
        }

        public static void SetHeight(this PdfFormData data, string field, object entity)
        {
            if (entity.IsMatch("Height"))
            {
                var check = entity.GetEntityValue("Height");
                int height;

                if (!string.IsNullOrEmpty(check) && int.TryParse(check, out height))
                {
                    int feet = height / 12;
                    int inches = height % 12;

                    if (field.ToLower().Contains("feet"))
                        data[field].Value = $"{feet.ToString()}'";
                    else if (field.ToLower().Contains("inches"))
                        data[field].Value = $"{inches.ToString()}\"";
                    else
                        data[field].Value = height;
                }
            }
        }

        public static void SetDate(this PdfFormData data, string field, object entity)
        {
            var splitField = field.Split('.');
            var dateType = splitField[1];
            var format = splitField[2];

            if (entity.IsMatch(dateType) && entity.HasValue(dateType))
            {
                var date = (DateTime)entity.GetValue(dateType);
                data[field].Value = date.ToString(format);
            }
        }

        public static void SetName(this PdfFormData data, string field, object entity)
        {
            switch (field.Split('.')[1].ToLower())
            {
                case "last":
                    data[field].Value = GetLast(entity);
                    break;
                case "first":
                    data[field].Value = GetFirst(entity);
                    break;
                case "m":
                    data[field].Value = GetM(entity);
                    break;
                case "middle":
                    data[field].Value = GetMiddle(entity);
                    break;
                case "firstlast":
                    data[field].Value = $"{GetFirst(entity)} {GetLast(entity)}";
                    break;
                case "lastfirst":
                    data[field].Value = $"{GetLast(entity)}, {GetFirst(entity)}";
                    break;
                case "firstmlast":
                    data[field].Value = $"{GetFirst(entity)} {GetM(entity)}. {GetLast(entity)}";
                    break;
                case "firstmiddlelast":
                    data[field].Value = $"{GetFirst(entity)} {GetMiddle(entity)} {GetLast(entity)}";
                    break;
                case "lastfirstm":
                    data[field].Value = $"{GetLast(entity)}, {GetFirst(entity)} {GetM(entity)}.";
                    break;
                case "lastfirstmiddle":
                    data[field].Value = $"{GetLast(entity)}, {GetFirst(entity)} {GetMiddle(entity)}";
                    break;
            }
        }

        public static void SetSsn(this PdfFormData data, string field, object entity)
        {
            if (entity.IsMatch("Ssn"))
            {
                var ssn = entity.GetEntityValue("Ssn");

                if (!string.IsNullOrEmpty(ssn) && ssn.Contains("-"))
                {
                    var splitSsn = ssn.Split("-");

                    if (field.ToLower().Contains("area"))
                        data[field].Value = splitSsn[0];
                    else if (field.ToLower().Contains("group"))
                        data[field].Value = splitSsn[1];
                    else if (field.ToLower().Contains("series"))
                        data[field].Value = splitSsn[2];
                    else
                        data[field].Value = ssn;
                }
            }
        }

        // z.AreaCode.{Some}Phone
        // z.Number.{Some}Phone
        // z.Prefix.{Some}Phone
        // z.Suffix.{Some}Phone
        // z.{Some}Phone
        public static void SetPhoneNumber(this PdfFormData data, string field, object entity)
        {
            var splitField = field.Split('.');
            var phoneType = splitField[splitField.Length - 1];
            var isSubset = splitField.Length == 3;

            if (entity.IsMatch(phoneType))
            {
                data[field].Value = entity.GetPhoneValue(phoneType, splitField, isSubset);
            }
        }

        public static string GetPhoneValue(this object entity, string phoneType, string[] splitField, bool isSubset)
        {
            var phoneNumber = entity.GetEntityValue(phoneType);

            if (isSubset)
            {
                if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Contains('-'))
                {
                    var splitPhone = phoneNumber.Split('-');

                    switch (splitField[1].ToLower())
                    {
                        case "areacode":
                            return splitPhone[0];
                        case "number":
                            return $"{splitPhone[1]}-{splitPhone[2]}";
                        case "prefix":
                            return splitPhone[1];
                        case "suffix":
                            return splitPhone[2];
                    }
                }
            }

            return phoneNumber;
        }

        static object GetEntitySub(this object entity, string context) => context.ToLower() == "z" ?
            entity.GetType()
                .GetProperty("Associate")
                .GetValue(entity) :
            entity.GetType()
                .GetProperty("Person")
                .GetValue(entity);

        static object GetValue(this object entity, string prop) => entity.GetType()
            .GetProperty(prop)
            .GetValue(entity);

        static bool HasValue(this object entity, string prop) => entity.GetValue(prop) != null;

        static string GetEntityValue(this object entity, string prop) => entity.HasValue(prop) ? entity.GetValue(prop).ToString() : string.Empty;

        static string GetLast(this object entity) => entity.IsMatch("LastName") ? entity.GetEntityValue("LastName") : string.Empty;
        static string GetFirst(this object entity) => entity.IsMatch("FirstName") ? entity.GetEntityValue("FirstName") : string.Empty;
        static string GetMiddle(this object entity) => entity.IsMatch("MiddleName") ? entity.GetEntityValue("MiddleName") : string.Empty;
        static string GetM(this object entity)
        {
            var mid = entity.GetMiddle();

            if (!(string.IsNullOrEmpty(mid)))
            {
                return mid[0].ToString().ToUpper();
            }

            return mid;
        }

        //Method to match PDF Form fields to Database Entity Properties
        static bool IsMatch(this object entity, string field) => entity.GetType()
            .GetProperties()
            .Select(x => x.Name)
            .FirstOrDefault(x => x == field) != null;

        // TestReflection demo done in repl.it
        // https://repl.it/@JaimeStill/ReflectedExtensionsMethod
        public static void TestReflection(this object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                Console.WriteLine($"{prop}: {prop.GetValue(obj)}");
            }
        }

        public static void Usage(this Person person)
        {
            (person as object).TestReflection();
        }
    }
}