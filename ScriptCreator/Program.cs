using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Assembly assembly = Assembly.LoadFile(@"C:\Users\1\source\repos\TestApp\obj\Release\netcoreapp3.0\TestApp.dll");
            Random random = new Random();
            Assembly assembly = Assembly.LoadFile(Console.ReadLine());
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("CREATE Database dbGalym");
            stringBuilder.Append($"{random.Next(int.MaxValue)};");
            var types = new Dictionary<string, string>();
            types.Add("System.String", "nvarchar(max)");
            types.Add("System.Guid", "uniqueidentifier");
            types.Add("System.DateTime", "date");
            types.Add("System.Boolean", "BIT");
            types.Add("System.Int64", "bigint");
            types.Add("System.Byte", "byte");
            types.Add("System.Byte[]", "bynary");
            types.Add("System.DateTimeOffset", "datetimeoffset");
            types.Add("System.Decimal", "decimal");
            types.Add("System.Double", "float");
            types.Add("System.Int32", "int");
            types.Add("System.Int16", "smallint");

            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "Program")
                {
                    continue;
                }
                stringBuilder.Append($"\nCREATE Table {type.Name}\n(");
                foreach (var memberInfo in type.GetMembers())
                {
                    if (memberInfo is PropertyInfo)
                    {
                        stringBuilder.Append($"\n  {memberInfo.Name} ");
                        var propertyInfo = memberInfo as PropertyInfo;
                        if (!propertyInfo.PropertyType.ToString().Contains("Nullable"))
                        {
                            if (types[propertyInfo.PropertyType.ToString()].Contains("uniqueidentifier"))
                            {
                                stringBuilder.Append("uniqueidentifier primary key, ");
                            }
                            else
                            {
                                stringBuilder.Append($"{types[propertyInfo.PropertyType.ToString()]} not null,");
                            }
                        }
                        else
                        {
                            stringBuilder.Append($"{types[DeleteNullable(propertyInfo.PropertyType.ToString())]}");
                        }
                    }
                }
                stringBuilder.Append("\n)");
            }
            var script = stringBuilder.ToString();

            Console.WriteLine(script);


            Console.ReadKey();
        }

        public static string DeleteNullable(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var beginIndex = text.LastIndexOf('[');
            var endIndex = text.LastIndexOf(']') - 1;
            while (beginIndex < endIndex)
            {
                beginIndex++;
                stringBuilder.Append(text[beginIndex]);
            }
            var txt = stringBuilder.ToString();
            return txt;
        }
    }
}

