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
}
