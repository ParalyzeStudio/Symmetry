using UnityEngine;

public class LanguageUtils
{
    private static string GetTranslationFilenameForLanguage(SystemLanguage language)
    {
        if (language == SystemLanguage.French)
            return "Strings/strings_fr";
        else
            return "Strings/strings_en";
    }

    public static string GetTranslationForTag(string tag)
    {
        string stringsFilename = GetTranslationFilenameForLanguage(Application.systemLanguage);
        Object stringsObjectFile = Resources.Load(stringsFilename);
        if (stringsObjectFile == null)
            return null;

        TextAsset stringsFile = (TextAsset)stringsObjectFile;

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(stringsFile.text);

        XMLNodeList stringsNodeList = rootNode.GetNodeList("strings>0>string");
        foreach (XMLNode stringsNode in stringsNodeList)
        {
            string nodeTag = stringsNode.GetValue("@tag"); //get the node with the relevant tag
            if (nodeTag.Equals(tag))
                return stringsNode.GetValue("@value");
        }

        return null;
    }

    public static string GetLatinNumberForNumber(int number)
    {
        if (number < 1)
            throw new System.Exception("number has to be > 0");
        if (number > 15)
            throw new System.Exception("number is too big");

        switch (number)
        {
            case 1:
                return "I";
            case 2:
                return "II";
            case 3:
                return "III";
            case 4:
                return "IV";
            case 5:
                return "V";
            case 6:
                return "VI";
            case 7:
                return "VII";
            case 8:
                return "VIII";
            case 9:
                return "IX";
            case 10:
                return "X";
            case 11:
                return "XI";
            case 12:
                return "XII";
            case 13:
                return "XIII";
            case 14:
                return "XIV";
            case 15:
                return "XV";
        }

        return null;
    }
}
