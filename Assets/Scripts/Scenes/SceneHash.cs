using Common.Global;
using Common.Scene;
using Common.Utils;
using System.Security.Cryptography;
using System.Text;
using UI.Menu;
using UnityEngine;

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
}
