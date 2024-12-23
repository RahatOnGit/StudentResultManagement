using ClosedXML.Excel;
using StudentResultManagement.Models;

namespace StudentResultManagement.Data
{
    public class ExcelFileHandling
    {
        public List<Students> ParseExcelFile(Stream stream)
        {
            var students = new List<Students>();

            //Create a workbook instance
            //Opens an existing workbook from a stream.
            using (var workbook = new XLWorkbook(stream))
            {
                //Lets assume the First Worksheet contains the data
                var worksheet = workbook.Worksheet(1);

                //Lets assume first row contains the header, so skip the first row
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                //Loop Through all the Rows except the first row which contains the header data
                foreach (var row in rows)
                {
                    //Create an Instance of Employee object and populate it with the Excel Data Row
                    var student = new Students
                    {
                        Roll = row.Cell(1).GetValue<string>(),
                        Name = row.Cell(2).GetValue<string>(),
                        PhoneNo = row.Cell(3).GetValue<string>(),
                        Email = row.Cell(4).GetValue<string>(),
                    };

                    //Add the Employee to the List of Employees
                    students.Add(student);
                }
            }

            //Finally return the List of Employees
            return students;
        }

        public List<MarksFromExcelview> ParseMarksExcelFile(Stream stream)
        {
            var students = new List<MarksFromExcelview>();

            //Create a workbook instance
            //Opens an existing workbook from a stream.
            using (var workbook = new XLWorkbook(stream))
            {
                //Lets assume the First Worksheet contains the data
                var worksheet = workbook.Worksheet(1);

                //Lets assume first row contains the header, so skip the first row
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                //Loop Through all the Rows except the first row which contains the header data
                foreach (var row in rows)
                {
                    var student = new MarksFromExcelview
                    {
                        Roll = row.Cell(1).GetValue<string>(),
                        CT1 = Double.Parse(row.Cell(2).GetValue<string>()),
                        CT2 = Double.Parse(row.Cell(3).GetValue<string>()),
                        CT3 = Double.Parse(row.Cell(4).GetValue<string>()),
                        CT4 = Double.Parse(row.Cell(5).GetValue<string>()),
                        Attendance = Double.Parse(row.Cell(6).GetValue<string>()),
                        Final = Double.Parse(row.Cell(7).GetValue<string>())

                    };

                    
                    students.Add(student);
                }
            }

            //Finally return the List of Employees
            return students;
        }

    }
}
