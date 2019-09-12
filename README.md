# PDF Generation

* [PDF Field Naming Conventions](#pdf-field-naming-conventions)
    * [Standard Fields](#standard-fields)
    * [Phone Fields](#phone-fields)
    * [Checkbox Fields](#checkbox-fields)
    * [Height Fields](#height-fields)
    * [Date Fields](#date-fields)
    * [Name Fields](#name-fields)
    * [Ssn Fields](#ssn-fields)

## PDF Field Naming Conventions
[Back to Top](#pdf-generation)  

X = Person  
Y = Multi.Person  
Z = Multi.Associate  

The **`.`** character is only allowed for separating segments of the field name.

### Standard Fields
[Back to Top](#pdf-generation)  

Sets the property as is with no additional formatting

**Convention**  
```cs
x.{Prop}
```

**Example**  
```cs
x.Suffix
```  

### Phone Fields  
[Back to Top](#pdf-generation)  

Multiple phone number values can be requested in a single form, and the information required by one or more fields for a given phone number can vary. With this in mind, phone number can be accessed in one of three ways:
* The number in its entirety
* The area code and the number
* The area code, the prefix, and the suffix all individually

**Conventions**  
```cs
x.{Prop}Phone
x.AreaCode.{Prop}Phone
x.Number.{Prop}Phone
x.Prefix.{Prop}Phone
x.Suffix.{Prop}Phone
```  

Given data with the following phone numbers specified:

```js
{
    HomePhone: "555-123-4567",
    MobilePhone: "444-987-6543"
}
```  

Field Name | Value
-----------|------
`x.HomePhone` | 555-123-4567
`x.AreaCode.HomePhone` | 555
`x.Number.HomePhone` | 123-4567
`x.Prefix.HomePhone` | 123
`x.Suffix.HomePhone` | 4567
`x.MobilePhone` | 444-987-6543
`x.AreaCode.MobilePhone` | 444
`x.Number.MobilePhone` | 987-6543
`x.Prefix.MobilePhone` | 987
`x.Suffix.MobilePhone` | 6543

### Checkbox Fields  
[Back to Top](#pdf-generation)  

Reads the property associated with the value of `{Label}` and determines if the value matches `{Value}`. If so, the Checkbox is checked.

**Convention**
```cs
x.{Label}.{Value}.Checkbox
```  

**Example**  
```cs
x.Gender.Male.Checkbox
```  

### Height Fields
[Back to Top](#pdf-generation)  

Can be written in two ways:
* Specifying feet and inches as separate fields
* Specifying the height in inches

**Conventions**  
```cs
// Height all in inches
x.Height

// Height separated by feet and inches
x.Feet.Height
x.Inches.Height
```  

### Date Fields  
[Back to Top](#pdf-generation)  

The `Format` segment of the field name indicates how the `DateTime` value of `Prop` should be displayed. It can contain any portion of the date and time value.

**Convention**  
```cs
x.{Prop}.{Format}.Date
```  

**Example Date Time**: 05 September 2004 14:07:08

> The below tables use the above example `DateTime` in their outputs  

**`DateTime` Format Specifications**  

Format | Description | Output
-------|-------------|---------
`d` | The day of the month, from 1 - 31 | 5
`dd` | The day of the month, from 01 - 31 | 05
`ddd` | The abbreviated name of the day of the week | Thu
`dddd` | The full name of the day of the week | Thursday
`h` | The hour, using a 12-hour clock from 1 to 12 | 2
`hh` | The hour, using a 12-hour clock from 01 to 12 | 02
`H` | The hour, using a 24-hour clock from 0 to 23 | 14
`HH` | The hour, using a 24-hour clock from 00 to 23 | 14
`%K` | Time zone information | Depends on `DateTimeKind` (`UTC` vs `Local`)
`m` | The minute, from 0 through 59 | 7
`mm` | The minute, from 00 through 59 | 07
`M` | The month, from 1 through 12 | 9
`MM` | The month, from 01 through 12 | 09
`MMM` | The abbreviated name of the month | Sep
`MMMM` | The full name of the month | September
`s` | The second, from 0 through 59 | 8
`ss` | The second, from 00 through 59 | 08
`t` | The first character of the AM/PM designator | P
`tt` | The AM/PM designator | PM
`y` | The year, from 0 to 99 | 4
`yy` | The year, from 00 to 99 | 04
`yyy` | The year, with a minimum of three digits | 2004 (0900 would be 900)
`yyyy` | The year as a four-digit number | 2004
`yyyyy` | The year as a five-digit number | 02004
`z` | Hours offset from UTC, with no leading zeros | -4 (Eastern)
`zz` | Hours offset from UTC, with a leading zero for a single-digit value | -04
`zzz` | Hours and minutes offset from UTC | -04:00  

**Examples**  

Example | Output
--------|-------
`x.DateOfBirth.hh:mm:ss.Date` | 14:07:08
`x.DateOfBirth.dd-MM-yyyy.Date` | 05-09-2004
`x.DateOfBirth.dd.Date` | 05
`x.DateOfBirth.MM.Date` | 09
`x.DateOfBirth.yyyy.Date` | 2004  

### Name Fields
[Back to Top](#pdf-generation)  

Renders a name based on the provided format and ending with the `.Name` indicator

Convention | Output | Example
-----------|--------|--------
`x.Last.Name` | Last | Smith
`x.First.Name` | First | John
`x.M.Name` | M. | D.
`x.Middle.Name` | Middle | David
`x.FirstLast.Name` | First Last | John Smith
`x.LastFirst.Name` | Last, First | Smith, John
`x.FirstMLast.Name` | First M. Last | John D. Smith
`x.FirstMiddleLast.Name` | First Middle Last | John David Smith
`x.LastFirstM.Name` | Last, First M. | Smith, John D.
`x.LastFirstMiddle.Name` | Last, First Middle | Smith, John David

### Ssn Fields  
[Back to Top](#pdf-generation)  

The full SSN can be accessed, or each individual component, broken up as `{area}-{group}-{series}`. For example, given `012-34-5678`:

Specifier | Value
----------|------
`area` | 012
`group` | 34
`series` | 5678

**Conventions**  
``` cs
// Entire SSN
x.Ssn

// Parts of SSN retrieved individually
x.Area.Ssn
x.Group.Ssn
x.Series.Ssn
```  