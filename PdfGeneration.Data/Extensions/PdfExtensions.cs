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
            return $@"{basePath}{person.LastName}_{person.FirstName}_{name}_{DateTime.Now.ToString("yyyyMMdd_HH:mm:ss")}.pdf";
        }

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

            if (entity.IsMatch(splitField[splitField.Length - 2]))
            {
                var entityData = entity.GetType()
                    .GetProperty(splitField[splitField.Length - 2])
                    .GetValue(entity);

                var fieldData = entityData
                    .ToString()
                    .UrlEncode("[^a-zA-Z0-9]");

                data[field].Value = fieldData[splitField.Length - 1];
            }
        }

        static void SetCheckbox(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');

            if (entity.IsMatch(splitField[splitField.Length - 3]))
            {
                var value = entity.GetType()
                    .GetProperty(splitField[splitField.Length - 3])
                    .GetValue(entity)
                    .ToString();

                if (value.ToLower() == splitField[splitField.Length - 2])
                {
                    data[field].Value = "On";
                }
            }
        }

        static void SetValue(this PdfFormData data, object entity, string field)
        {
            var splitField = field.Split('.');

            var entityData = entity.GetType()
                .GetProperty(splitField[splitField.Length - 1])
                .GetValue(entity)
                .ToString();
            

            if (entity.IsMatch(splitField.Last()))
            {
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
                            data[field].Value = data;
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
            switch (splitField[1])
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

        }

        public static void SetHeight(this PdfFormData data, string field, string entityData)
        {

        }

        public static void SetDob(this PdfFormData data, string field, string entityData)
        {

        }

        public static void SetSsn(this PdfFormData data, string field, string entityData)
        {

        }

        //Method to match PDF Form fields to Database Entity Properties
        static bool IsMatch(this object entity, string field) => entity.GetType()
            .GetProperties()
            .Select(x => x.Name)
            .FirstOrDefault(x => x == field) != null;

        //Method to set Height based on conditions specified in the notation of the PDF Form field
        // public static void SetHeight(this PdfFormData formData, string formField, string data, int i, int length)
        // {
        //     int feet = Int32.Parse(data);
        //     feet = feet / 12;
        //     int inches = Int32.Parse(data);
        //     inches = inches % 12;
        //     for (int j = i; j < length; j++)
        //     {
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Feet")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = $"{feet.ToString()}" + " " + formData[formField].Value;
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{feet.ToString()}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Inches")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = $"{inches.ToString()}" + " " + formData[formField].Value;
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{inches.ToString()}";
        //             }
        //         }
        //         if ((length - 1) == j && formData.GetFormFieldPropertyType(formField, j) == "Height")
        //         {
        //             formData[formField].Value = data;
        //         }
        //     }
        // }

        //Method to set the Date of Birth Field based on the notation set on the PDF Form Field
        // public static void SetDOBDate(this PdfFormData formData, string formField, string data, int i, int length)
        // {
        //     var date = data.Split('/');
        //     for (int j = i; j < length; j++)
        //     {
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "MM")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{date[0]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{date[0]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "dd")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{date[1]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{date[1]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "yyyy")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 date = date[2].Split(" ");
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{date[0]}";
        //             }
        //             else
        //             {
        //                 date = date[2].Split(" ");
        //                 formData[formField].Value = $"{date[0]}";
        //             }

        //         }
        //         if ((length - 1) == j && formData.GetFormFieldPropertyType(formField, j) == "Date")
        //         {
        //             formData[formField].Value = data;
        //         }
        //     }
        // }

        //Method to Set the SSN based on the notation set on the PDF Form field
        // public static void SetSSN(this PdfFormData formData, string formField, string data, int i, int length)
        // {
        //     var ssn = data.Split('-');
        //     for (int j = i; j < length; j++)
        //     {
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Area")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{ssn[0]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{ssn[0]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Group")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{ssn[1]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{ssn[1]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Series")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{ssn[2]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{ssn[2]}";
        //             }
        //         }
        //         if ((length - 1) == j && formData.GetFormFieldPropertyType(formField, j) == "Ssn")
        //         {
        //             formData[formField].Value = data;
        //         }
        //     }
        // }

        //Method to set the Phone Number based on the notation of the PDF Form Field
        // public static void SetPhoneNumber(this PdfFormData formData, string formField, string data, int i, int length)
        // {
        //     var phone = data.Split('-');
        //     for (int j = i; j < length; j++)
        //     {
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "CountyCode")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{phone[0]}";

        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{phone[0]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "AreaCode")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{phone[1]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{phone[1]}";
        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "Prefix")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{phone[2]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{phone[2]}";

        //             }
        //         }
        //         if (length > j && formData.GetFormFieldPropertyType(formField, j) == "LineNumber")
        //         {
        //             if (string.IsNullOrEmpty(formData[formField].Value.ToString()) == false)
        //             {
        //                 formData[formField].Value = formData[formField].Value + "-" + $"{phone[3]}";
        //             }
        //             else
        //             {
        //                 formData[formField].Value = $"{phone[3]}";

        //             }
        //         }
        //         if ((length - 1) == j && formData.GetFormFieldPropertyType(formField, j) == "HomePhone")
        //         {
        //             formData[formField].Value = data;
        //         }
        //     }
        // }

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