using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class Person
{
    public Person(int iD, string firstName, string lastName, string birthday, List<string> otherBirthdays, string nickname, string city, List<string> cityAliases, string country, string petsName, string petsBirtday, string petType, string petBreed)
    {
        ID = iD;
        FirstName = firstName;
        LastName = lastName;
        Birthday = birthday;
        OtherBirthdays = otherBirthdays;
        Nickname = nickname;
        City = city;
        CityAliases = cityAliases;
        Country = country;
        PetsName = petsName;
        PetsBirtday = petsBirtday;
        PetType = petType;
        PetBreed = petBreed;
    }

    public Person()
    {
        FirstName = "Hendrik";
        LastName = "Sauer";
        Birthday = "02.10.2007";
        OtherBirthdays = new List<string> { "01.01.2000", "02.02.2002" };
        Nickname = "hend";
        City = "Mellrichstadt";
        CityAliases = new List<string> { "MET", "met"};
        Country = "Germany";
        PetsName = "Perino";
        PetsBirtday = "01.01.2017";
        PetType = "dog";
        PetBreed = "mischling";
    }


    //person
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthday { get; set; }
    public List<string> OtherBirthdays { get; set; }
    public string Nickname { get; set; }
    public string City { get; set; }
    public List<string> CityAliases { get; set; }
    public string Country { get; set; }

    //pet
    public string PetsName { get; set; }
    public string PetsBirtday { get; set; }
    public string PetType { get; set; }
    public string PetBreed { get; set; }

    public string formatInfos()
    {
        var cultureInfo = new CultureInfo("de-DE");

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("ID: : " + ID.ToString());
        stringBuilder.AppendLine("First name: " + FirstName);
        stringBuilder.AppendLine("Last name: " + LastName);
        stringBuilder.AppendLine("Birthday: " + Birthday);
        stringBuilder.AppendLine("Other birthdays: " + String.Join("; ", OtherBirthdays));
        stringBuilder.AppendLine("nickname: " + Nickname);
        stringBuilder.AppendLine("City: " + City);
        stringBuilder.AppendLine("City aliases: " + String.Join("; ", CityAliases));
        stringBuilder.AppendLine("Country: " + Country);
        stringBuilder.AppendLine("Pets name: " + PetsName);
        stringBuilder.AppendLine("Pets birtday: " + PetsBirtday);
        stringBuilder.AppendLine("Pet type: " + PetType);
        stringBuilder.AppendLine("Pet breed: " + PetBreed);


        return stringBuilder.ToString();
    }
    public string shortInfos()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("[" + ID + "]\t" + FirstName + " " + LastName);

        return stringBuilder.ToString();
    }
}
