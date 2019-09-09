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

                foreach (var field in fields)
                {
                    if (int.TryParse(field.Split('.').Last(), out int res))
                    {
                        data.SetSingleField(entity, field);
                    }
                    else if (field.Split('.').Last().ToLower() == "checkbox")
                    {
                        data.SetCheckbox(entity, field);
                    }
                    else
                    {
                        data.SetValue(entity, field);
                    }
                }

                processor.ApplyFormData(data);
                processor.SaveDocument(file);
            }
        });

        static void SetSingleField(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');
            try{
            if (entity.IsMatch(splitField[splitField.Length - 2]))
            {
                var entityData = entity.GetType()
                    .GetProperty(splitField[splitField.Length - 2])
                    .GetValue(entity);

                var fieldData = entityData
                    .ToString()
                    .UrlEncode("[^a-zA-Z0-9]");
                data[field].Value = fieldData[Int32.Parse(splitField[splitField.Length-1])-1].ToString();
            }
            }catch{}
        }

        static void SetCheckbox(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');

            if (entity.IsMatch(splitField[splitField.Length - 3]))
            {
                var entityData = entity.GetType()
                    .GetProperty(splitField[splitField.Length - 3])
                    .GetValue(entity)
                    .ToString();
                if (entityData.ToLower() == splitField[splitField.Length - 2].ToLower())
                {
                    data[field].Value = "On";
                }
            }
        }

        static void SetValue(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');
            if (entity.IsMatch(splitField.Last()))
            {
                var entityData = entity.GetType()
                .GetProperty(splitField[splitField.Length - 1])
                .GetValue(entity)
                .ToString();
                if (splitField.Last().IsSpecialType())
                {
                    data.SetSpecialValue(entity, entityData, field, splitField);
                }
                else
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

        public static bool IsSpecialType(this string type)
        {
            switch (type.ToLower())
            {
                case "homephone":
                case "height":
                case "dob":
                case "ssn":
                    return true;
                default:
                    return false;
            }
        }

        public static void SetSpecialValue(this PdfFormData data, object entity, string entityData, string field, string[] splitField)
        {
            switch (splitField.Last().ToLower())
            {
                case "homephone":
                    data.SetPhoneNumber(field, entityData);
                    break;
                case "height":
                    data.SetHeight(field, entityData);
                    break;
                case "dob":
                    data.SetDob(field, entityData);
                    break;
                case "ssn":
                    data.SetSsn(field, entityData);
                    break;
            }
        }

        public static void SetPhoneNumber(this PdfFormData data, string field, string entityData)
        {
            var splitField = entityData.Split("-");
            Console.WriteLine("Split Field Length"+splitField.Length);
            Console.WriteLine("Split Field: "+splitField[0]+splitField[1]);
            if(field.ToLower().Contains("phonenumber"))
                data[field].Value = $"{splitField[1]}-{splitField[2]}";
            else if(field.ToLower().Contains("areacode"))
                data[field].Value = splitField[0];
            else
                data[field].Value = entityData;
            
        }

        public static void SetHeight(this PdfFormData data, string field, string entityData)
        {

            int feet = (Int32.Parse(entityData))/12;
            int inches = (Int32.Parse(entityData))%12;

            if(field.ToLower().Contains("feet"))
                data[field].Value = $"{feet.ToString()}'";
            else if(field.ToLower().Contains("inches"))
                data[field].Value = $"{inches.ToString()}\"";
            else
                data[field].Value = entityData;

        }

        public static void SetDob(this PdfFormData data, string field, string entityData)
        {
            var splitField = entityData.Split("/");

            if(field.ToLower().Contains("mm"))
                data[field].Value = splitField.First();
            else if(field.ToLower().Contains("dd"))
                data[field].Value = splitField[1];
            else if(field.ToLower().Contains("yyyy"))
                data[field].Value = splitField.Last().Split(" ").First();
            else
                data[field].Value = entityData;
        }

        public static void SetSsn(this PdfFormData data, string field, string entityData)
        {
            var splitField = entityData.Split("-");

            if(field.ToLower().Contains("area"))
                data[field].Value = splitField[0];
            else if (field.ToLower().Contains("group"))
                data[field].Value = splitField[1];
            else if (field.ToLower().Contains("series"))
                data[field].Value = splitField[2];
            else
                data[field].Value = entityData;
            
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