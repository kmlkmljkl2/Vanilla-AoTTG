using UnityEngine;

public class BTN_LOGIN : MonoBehaviour
{
    public GameObject username;

    public GameObject password;

    public GameObject output;

    public GameObject logincomponent;

    public void Awake()
    {
        //RIP Login Panel
        var Objects = FindObjectsOfType<GameObject>();
        foreach (var obj in Objects)
        {
            if (obj.name == "LOGIN")
                Destroy(obj);
        }
    }

    private void OnClick()
    {
        logincomponent.GetComponent<LoginFengKAI>().login(username.GetComponent<UIInput>().text, password.GetComponent<UIInput>().text);
        output.GetComponent<UILabel>().text = "please wait...";
    }
}