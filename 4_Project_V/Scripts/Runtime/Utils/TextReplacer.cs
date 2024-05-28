using System.Collections.Generic;

public static class TextReplacer
{
    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeywordDic)
    {
        if (textByKeywordDic != null)
        {
            foreach (var pair in textByKeywordDic) 
                text = text.Replace($"${{{pair.Key}}}", pair.Value);
        }

        return text;
    }

    public static string Replace(string text, string prefixKeyword, IReadOnlyDictionary<string, string> textByKeywordDic)
    {
        if (textByKeywordDic != null)
        {
            foreach (var pair in textByKeywordDic) 
                text = text.Replace($"${{{prefixKeyword}.{pair.Key}}}", pair.Value);
        }

        return text;
    }

    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeywordDic, string suffixKeyword)
    {
        if (textByKeywordDic != null)
        {
            foreach (var pair in textByKeywordDic) 
                text = text.Replace($"${{{pair.Key}.{suffixKeyword}}}", pair.Value);
        }

        return text;
    }

    public static string Replace(string text, string prefixKeyword, IReadOnlyDictionary<string, string> textByKeywordDic, string suffixKeyword)
    {
        if (textByKeywordDic != null)
        {
            foreach (var pair in textByKeywordDic) 
                text = text.Replace($"${{{prefixKeyword}.{pair.Key}.{suffixKeyword}}}", pair.Value);
        }

        return text;
    }
}