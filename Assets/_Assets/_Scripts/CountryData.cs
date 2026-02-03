using System;
using System.Collections.Generic;

[Serializable]
public class Country
{
    public string name;
    public string code;
}

[Serializable]
public class CountryList
{
    public List<Country> countries;
}