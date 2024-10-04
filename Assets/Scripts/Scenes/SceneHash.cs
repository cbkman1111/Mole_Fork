using Common.Global;
using Common.Scene;
using Common.Utils;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UI.Menu;

public class SceneHash : SceneBase
{
    private UIMenuHash menu;

    public override bool Init(JSONObject param)
    {
        menu = UIManager.Instance.OpenMenu<UIMenuHash>();
        if (menu != null)
        {
            menu.InitMenu(ConvertSHA, ConvertMD5, ConvertBase64);
        }

        Hashtable table = new Hashtable(7);
        table.Add(1, "One");
        table.Add(2, "Two");
        table.Add(3, "Three");
        table.Add(4, "Four");
        table.Add(5, "Five");
        table.Add(6, "Six");
        table.Add(7, "Seven");
        table.Add(8, "Eight");
        table.Add(9, "Nine");
        table.Add(10, "Ten");
        table.Add(11, "Eleven");
        table.Add(12, "Twelve");

        foreach (DictionaryEntry entry in table)
        {
            string msg = $"{entry.Key.GetHashCode()} => {entry.Key.GetHashCode() % table.Count} / {entry.Value}";
            GiantDebug.Log(msg);
        }

        return true;
    }

    private void ConvertSHA(string str)
    {
        var hash = SHA256Hash(str);
        GiantDebug.Log(hash);

        menu.SetResult(hash);
    }

    private void ConvertMD5(string str)
    {
        var hash = MD5Hash(str);
        GiantDebug.Log(hash);

        menu.SetResult(hash);
    }

    private void ConvertBase64(string str)
    {
        var base64 = Base64(str);
        GiantDebug.Log(base64);
        menu.SetResult(base64);
    }

    public string SHA256Hash(string data)
    {
        SHA256 sha = new SHA256Managed();
        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));

        StringBuilder stringBuilder = new StringBuilder();

        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }

    public string MD5Hash(string data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(data));

        byte[] result = md5.Hash;

        StringBuilder stringBuilder = new StringBuilder();
        for(int i = 0; i < result.Length; i++)
        {
            stringBuilder.Append(result[i].ToString("x2"));
        }

        return stringBuilder.ToString();
    }

    public string Base64(string str)
    {
        byte[] bytes = ASCIIEncoding.ASCII.GetBytes(str);
        return System.Convert.ToBase64String(bytes);
    }

    bool FindPrime(int number)
    {
        if (number == 1) 
            return false;

        if (number == 2) 
            return true;

        if (number % 2 == 0) 
            return false;

        // 제곱근의 정수부분까지만 검사
        var squareRoot = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= squareRoot; i += 2)
        {
            if (number % i == 0) 
                return false;
        }

        return true;
    }
}
