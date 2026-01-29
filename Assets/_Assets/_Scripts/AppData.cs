using System;
using System.Collections.Generic;

[Serializable]
public class AppData
{
    public Page1Data page1;
    public Page2Data page2;
    public Page3Data page3;
}

[Serializable]
public class Page1Data
{
    public List<TextItem> text;
}

[Serializable]
public class Page2Data
{
    public List<TextItem> buttons;
}

[Serializable]
public class Page3Data
{
    public List<TextItem> animation;
}

[Serializable]
public class TextItem
{
    // JSON keys vary (page1 uses "title"/"button", others use "text")
    public string text;
    public string title;
    public string button;
}
